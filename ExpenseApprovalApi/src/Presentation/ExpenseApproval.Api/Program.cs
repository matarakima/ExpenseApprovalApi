using Azure.Identity;
using ExpenseApproval.Api.Middleware;
using ExpenseApproval.Infrastructure;
using ExpenseApproval.Infrastructure.Data;
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

var connectionString = builder.Configuration["ConnectionString"];
var auth0Domain = builder.Configuration["AuthDomain"];
var auth0Audience = builder.Configuration["AuthAudience"];
var auth0ClientId = builder.Configuration["AutClientId"];
var auth0ClientSecret = builder.Configuration["AuthClientSecret"];


// ---------- Services ----------
builder.Services.AddInfrastructure(builder.Configuration, connectionString);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ExpenseApproval.Application.Features.Expenses.Queries.GetAllExpensesQuery).Assembly));
builder.Services.AddHttpClient();
builder.Services.AddControllers();


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

// Authorization policies (one per claim)
builder.Services.AddSingleton<IAuthorizationHandler, ClaimRequirementHandler>();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("expenses:list", p => p.Requirements.Add(new ClaimRequirement("expenses:list")))
    .AddPolicy("expenses:create", p => p.Requirements.Add(new ClaimRequirement("expenses:create")))
    .AddPolicy("expenses:read", p => p.Requirements.Add(new ClaimRequirement("expenses:read")))
    .AddPolicy("expenses:edit", p => p.Requirements.Add(new ClaimRequirement("expenses:edit")))
    .AddPolicy("expenses:approve", p => p.Requirements.Add(new ClaimRequirement("expenses:approve")))
    .AddPolicy("expenses:reject", p => p.Requirements.Add(new ClaimRequirement("expenses:reject")))
    .AddPolicy("expenses:filter", p => p.Requirements.Add(new ClaimRequirement("expenses:filter")))
    .AddPolicy("expenses:metrics", p => p.Requirements.Add(new ClaimRequirement("expenses:metrics")))
    .AddPolicy("users:list", p => p.Requirements.Add(new ClaimRequirement("users:list")))
    .AddPolicy("users:create", p => p.Requirements.Add(new ClaimRequirement("users:create")))
    .AddPolicy("users:read", p => p.Requirements.Add(new ClaimRequirement("users:read")))
    .AddPolicy("roles:list", p => p.Requirements.Add(new ClaimRequirement("roles:list")))
    .AddPolicy("roles:create", p => p.Requirements.Add(new ClaimRequirement("roles:create")))
    .AddPolicy("roles:read", p => p.Requirements.Add(new ClaimRequirement("roles:read")))
    .AddPolicy("roles:add-claim", p => p.Requirements.Add(new ClaimRequirement("roles:add-claim")))
    .AddPolicy("roles:remove-claim", p => p.Requirements.Add(new ClaimRequirement("roles:remove-claim")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Expense Approval API", Version = "v1" });

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

var app = builder.Build();

// ---------- Database migration & seed on startup ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DatabaseSeeder.SeedAsync(db, logger);
}

// ---------- Middleware pipeline ----------
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseMiddleware<PermissionMiddleware>();
app.UseAuthorization();

// ---------- Map Controllers ----------
app.MapControllers();

app.Run();

// Required for integration tests
public partial class Program { }
