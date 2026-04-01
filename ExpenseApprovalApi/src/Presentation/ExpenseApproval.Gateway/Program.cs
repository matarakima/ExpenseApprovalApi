using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ExpenseApproval.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------- Azure Key Vault ----------
var vaultUrl = builder.Configuration["KeyVault:VaultUrl"]
    ?? Environment.GetEnvironmentVariable("VAULT_URL");

var tenantId = builder.Configuration["KeyVault:tenantId"]
    ?? Environment.GetEnvironmentVariable("TENANT_ID");

var clientId = builder.Configuration["KeyVault:applicationId"]
    ?? Environment.GetEnvironmentVariable("APPLICATION_ID");

var clientSecret = builder.Configuration["KeyVault:clientSecret"]
    ?? Environment.GetEnvironmentVariable("CLIENT_SECRET");
if (!string.IsNullOrEmpty(vaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(vaultUrl),
        new ClientSecretCredential(
            tenantId,
            clientId,
            clientSecret
        ));
}

var auth0Domain = builder.Configuration["AuthDomain"];
var auth0Audience = builder.Configuration["AuthAudience"];

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{auth0Domain}/";
        options.Audience = auth0Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:3000", "http://localhost:4200"])
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ---------- Services ----------
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtValidationDelegatingHandler>();
builder.Services.AddOcelot();

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();

await app.UseOcelot();

app.Run();
