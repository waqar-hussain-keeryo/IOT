using IOT.Business.Interfaces;
using IOT.Business.Services;
using IOT.Data;
using IOT.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register MongoDB context
builder.Services.AddSingleton<MongoDBContext>();

// Register repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Register services
builder.Services.AddScoped<IGlobalAdminService, GlobalAdminService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add JWT Authentication
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
