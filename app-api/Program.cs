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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var logger = new LoggerConfiguration()
.WriteTo.Console()
.MinimumLevel.Information()
.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();

builder.Services.AddDbContext<MydbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionString"));
});
builder.Services.AddScoped<IUserRepository, SQLUserRepository>();
builder.Services.AddScoped<IWorkspaceRepository, SQLWorkspaceRepository>();
builder.Services.AddScoped<IDocumentRepository, SQLDocumentRepository>();
builder.Services.AddScoped<IAutomationRepository, SQLAutomationRepository>();
builder.Services.AddScoped<IAutomationLogRepository, SQLAutomationLogRepository>();
builder.Services.AddScoped<IFileStorageService, R2StorageService>();
builder.Services.AddScoped<ITextExtractorService, PythonExtractorService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddHttpClient();

var accessKey = builder.Configuration["ACCESS_KEY"];
var secretKey = builder.Configuration["SECRET_KEY"];
var serviceUrl = builder.Configuration["SERVICE_URL"];

var s3Config = new AmazonS3Config
{
    ServiceURL = serviceUrl,
    ForcePathStyle = true,
    RegionEndpoint = RegionEndpoint.USEast1
};

builder.Services.AddSingleton<IAmazonS3>(sp =>
    new AmazonS3Client(accessKey, secretKey, s3Config));

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
.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
});
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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
