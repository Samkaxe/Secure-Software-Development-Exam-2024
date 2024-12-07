using Application.Interfaces;
using Application.Services;
using Infrastructure;
using Infrastructure.DataAccessInterfaces;
using Infrastructure.DataAccessServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Register services
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<ITokenService>(provider =>
    new TokenService(
        jwtSecret: builder.Configuration["JwtSettings:Secret"], 
        jwtExpirationMinutes: int.Parse(builder.Configuration["JwtSettings:ExpirationMinutes"]) // Get expiration time
    ));

// Register EncryptionHelper with a key from configuration
builder.Services.AddSingleton(provider => 
    new EncryptionHelper(builder.Configuration["EncryptionKey"]));
// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger and API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger setup for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();