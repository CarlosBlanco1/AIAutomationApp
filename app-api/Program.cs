// using app_api.Models;
using app_api.Models;
using Microsoft.EntityFrameworkCore;
// using app_api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();

builder.Services.AddDbContext<MydbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionString"));
});
builder.Services.AddScoped<IUserRepository, SQLUserRepository>();
builder.Services.AddScoped<IWorkspaceRepository, SQLWorkspaceRepository>();
builder.Services.AddScoped<IDocumentRepository, SQLDocumentRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
