using Azure.Identity;
using ExpenseApproval.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------- Azure Key Vault ----------
var vaultUrl = builder.Configuration["KeyVault:VaultUrl"]
    ?? Environment.GetEnvironmentVariable("VAULT_URL");
if (!string.IsNullOrEmpty(vaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(vaultUrl),
        new DefaultAzureCredential());
}

// ---------- Configuration overrides from environment variables ----------
// (fallback for local development or Docker without Key Vault)
var auth0Domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN");
if (!string.IsNullOrEmpty(auth0Domain))
    builder.Configuration["Auth0:Domain"] = auth0Domain;

var auth0Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");
if (!string.IsNullOrEmpty(auth0Audience))
    builder.Configuration["Auth0:Audience"] = auth0Audience;

// ---------- Ocelot configuration ----------
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ---------- Auth0 JWT Authentication ----------
var domain = builder.Configuration["Auth0:Domain"]!;
var audience = builder.Configuration["Auth0:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{domain}/";
        options.Audience = audience;
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
