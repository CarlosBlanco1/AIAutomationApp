// using app_api.Models;
using System.Text;
using app_api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
// using app_api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();

builder.Services.AddDbContext<MydbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionString"));
});
// builder.Services.AddScoped<IUserRepository, SQLUserRepository>();
// builder.Services.AddScoped<IWorkspaceRepository, SQLWorkspaceRepository>();
builder.Services.AddScoped<IDocumentRepository, SQLDocumentRepository>();
builder.Services.AddScoped<IAutomationRepository, SQLAutomationRepository>();
builder.Services.AddScoped<IAutomationLogRepository, SQLAutomationLogRepository>();

builder.Services.AddAutoMapper(cfg => {}, typeof(UserProfiles), 
typeof(DocumentProfiles), 
typeof(WorkspaceProfiles), 
typeof(AutomationProfiles), 
typeof(AutomationLogProfiles));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

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
