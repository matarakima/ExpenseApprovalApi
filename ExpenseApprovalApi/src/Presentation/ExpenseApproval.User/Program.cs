using Azure.Identity;
using ExpenseApproval.Infrastructure;
using ExpenseApproval.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

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
// ---------- Configuration overrides from environment variables ----------
// (fallback for local development or Docker without Key Vault)
var connectionString = builder.Configuration["ConnectionString"];
var auth0Domain = builder.Configuration["AuthDomain"];
var auth0Audience = builder.Configuration["AuthAudience"];
var auth0ClientId = builder.Configuration["AutClientId"];
var auth0ClientSecret = builder.Configuration["AuthClientSecret"];
// ---------- Services ----------
builder.Services.AddInfrastructure(builder.Configuration, connectionString??string.Empty);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ExpenseApproval.Application.Features.Users.Queries.GetAllUsersQuery).Assembly));

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Expense Approval User API", Version = "v1" });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlFilePath);

    var appXmlFilename = "ExpenseApproval.Application.xml";
    var appXmlFilePath = Path.Combine(AppContext.BaseDirectory, appXmlFilename);
    if (File.Exists(appXmlFilePath))
        options.IncludeXmlComments(appXmlFilePath);

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your Auth0 JWT token"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


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

// Authorization policies
builder.Services.AddSingleton<IAuthorizationHandler, ClaimRequirementHandler>();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("users:list", p => p.Requirements.Add(new ClaimRequirement("users:list")))
    .AddPolicy("users:create", p => p.Requirements.Add(new ClaimRequirement("users:create")))
    .AddPolicy("users:read", p => p.Requirements.Add(new ClaimRequirement("users:read")))
    .AddPolicy("roles:list", p => p.Requirements.Add(new ClaimRequirement("roles:list")))
    .AddPolicy("roles:create", p => p.Requirements.Add(new ClaimRequirement("roles:create")))
    .AddPolicy("roles:read", p => p.Requirements.Add(new ClaimRequirement("roles:read")))
    .AddPolicy("roles:add-claim", p => p.Requirements.Add(new ClaimRequirement("roles:add-claim")))
    .AddPolicy("roles:remove-claim", p => p.Requirements.Add(new ClaimRequirement("roles:remove-claim")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
