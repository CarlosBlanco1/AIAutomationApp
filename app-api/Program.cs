using app_api.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Amazon.S3;
using Amazon;
using Amazon.Runtime;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;
using Hangfire;
using Hangfire.PostgreSql;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var logger = new LoggerConfiguration()
.WriteTo.Console()
.MinimumLevel.Information()
.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSignalR();

builder.Services.AddDbContext<MydbContext>(options =>
{
    options.UseNpgsql(builder.Configuration["ConnectionString"],
    npgsqlOptions =>
    {
        npgsqlOptions.UseVector();
    });
});

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration["ConnectionString"])));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IUserRepository, SQLUserRepository>();
builder.Services.AddScoped<IWorkspaceRepository, SQLWorkspaceRepository>();
builder.Services.AddScoped<IDocumentRepository, SQLDocumentRepository>();
builder.Services.AddScoped<IAutomationRepository, SQLAutomationRepository>();
builder.Services.AddScoped<IAutomationLogRepository, SQLAutomationLogRepository>();
builder.Services.AddScoped<IFileStorageService, R2StorageService>();
builder.Services.AddScoped<ITextExtractorService, PythonExtractorService>();
builder.Services.AddScoped<IChatService, OllamaChatService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IEmailSenderRepository, EmailRepository>();
builder.Services.AddScoped<IChunkRepository, SQLChunkRepository>();
builder.Services.AddScoped<DocumentProcessingJob>();
builder.Services.AddHttpClient("ExtendedTimeoutClient", client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api/confirmationtoken/email/generate", limiter =>
    {
        limiter.PermitLimit = 3;
        limiter.Window = TimeSpan.FromHours(1);
        limiter.QueueLimit = 0;
    });
});
var accessKey = builder.Configuration["ACCESS_KEY"];
var secretKey = builder.Configuration["SECRET_KEY"];
var serviceUrl = builder.Configuration["SERVICE_URL"];

var credentials = new BasicAWSCredentials(accessKey, secretKey);
var s3Config = new AmazonS3Config
{
    ServiceURL = serviceUrl
};

builder.Services.AddSingleton<IAmazonS3>(sp =>
    new AmazonS3Client(credentials, s3Config));

builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfiles),
typeof(DocumentProfiles),
typeof(WorkspaceProfiles),
typeof(AutomationProfiles),
typeof(AutomationLogProfiles));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My Web API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(document =>
    new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
    });
});

builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>("ProviderName")
    .AddEntityFrameworkStores<MydbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer((options) =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/api/chathub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
}

);
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [new AllowAllDashboardAuthorizationFilter()]
    });
}

app.MapControllers();
app.MapGet("/health", () => { return Results.Ok(); });
app.MapHub<ChatHub>("/api/chathub").RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<MydbContext>();

    dbContext.Database.Migrate();
}

app.Run();
