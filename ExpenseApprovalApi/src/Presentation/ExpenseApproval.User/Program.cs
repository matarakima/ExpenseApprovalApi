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
if (!string.IsNullOrEmpty(vaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(vaultUrl),
        new DefaultAzureCredential());
}

// ---------- Configuration overrides from environment variables ----------
// (fallback for local development or Docker without Key Vault)
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (!string.IsNullOrEmpty(connectionString))
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

var auth0Domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN");
if (!string.IsNullOrEmpty(auth0Domain))
    builder.Configuration["Auth0:Domain"] = auth0Domain;

var auth0Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");
if (!string.IsNullOrEmpty(auth0Audience))
    builder.Configuration["Auth0:Audience"] = auth0Audience;

var auth0ClientId = Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID");
if (!string.IsNullOrEmpty(auth0ClientId))
    builder.Configuration["Auth0:ClientId"] = auth0ClientId;

var auth0ClientSecret = Environment.GetEnvironmentVariable("AUTH0_CLIENT_SECRET");
if (!string.IsNullOrEmpty(auth0ClientSecret))
    builder.Configuration["Auth0:ClientSecret"] = auth0ClientSecret;

// If SQL SA password comes from Key Vault, inject it into the connection string
var sqlSaPassword = builder.Configuration["Sql:SaPassword"];
if (!string.IsNullOrEmpty(sqlSaPassword))
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection")!;
    var connBuilder = new SqlConnectionStringBuilder(connStr) { Password = sqlSaPassword };
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connBuilder.ConnectionString;
}

// ---------- Services ----------
builder.Services.AddInfrastructure(builder.Configuration);
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

// Auth0 JWT Authentication
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
