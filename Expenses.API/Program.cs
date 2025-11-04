using Expenses.API.Data;
using Expenses.API.Data.Services;
using Expenses.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddCors(opt => opt.AddPolicy("AllowAll", opt => opt.AllowAnyHeader()
//.AllowAnyMethod()
//.AllowAnyOrigin()));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins(
              "https://black-plant-0a3399900.3.azurestaticapps.net",
              "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options=>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer= true,
            ValidateAudience= true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "dotnethow.net",
            ValidAudience = "dotnethow.net",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-very-secure-secret-key-32-chars-long"))
        };
    });
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration
    .GetConnectionString("Default")));
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<PasswordHasher<User>>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
//app.UseCors("AllowFrontend");
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
