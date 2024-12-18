using System.Text;
using Application.Interfaces;
using Application.Services;
using Azure.Identity;
using Infrastructure;
using Infrastructure.DataAccessInterfaces;
using Infrastructure.DataAccessServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Access Azure key vault
var keyVaultUri = new Uri(builder.Configuration["SecretsVault:Url"]!);

var azureCredentials = new ClientSecretCredential(
    builder.Configuration["SecretsVault:AzureClientTenantId"],
    builder.Configuration["SecretsVault:AzureClientId"],
    builder.Configuration["SecretsVault:AzureClientSecret"]);
builder.Configuration.AddAzureKeyVault(keyVaultUri, azureCredentials);



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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PatientPolicy", policy => policy.RequireRole("Patient"));
    options.AddPolicy("DoctorPolicy", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("NursePolicy", policy => policy.RequireRole("Nurse"));
    options.AddPolicy("EmergencyResponderPolicy", policy => policy.RequireRole("EmergencyResponder"));
});

// Register EncryptionHelper with a key from configuration
builder.Services.AddSingleton(provider => 
    new EncryptionHelper(builder.Configuration["EncryptionKey"]));
// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Required for Swagger to discover endpoints
builder.Services.AddControllers(); 

// Swagger and API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var app = builder.Build();

// Swagger setup for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // This maps your controller routes

app.Run();