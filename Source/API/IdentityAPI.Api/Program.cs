using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using IdentityAPI.Api.Configuration;
using IdentityAPI.Api.Data;
using IdentityAPI.Api.Endpoints;
using IdentityAPI.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Bind Options
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

// Retrieve Jwt options immediately for middleware setup
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var secretKey = jwtSection["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured.");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

// 2. Register Repositories and Factories
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 3. Register Cryptography and Authentication Services
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 4. Register Request Validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 5. Register Authentication & Authorization Middleware
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

// 6. Register Swagger / OpenAPI
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var scheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Paste your JWT access token below (without 'Bearer ')."
        };

        document.Components ??= new Microsoft.OpenApi.Models.OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = scheme;
        return Task.CompletedTask;
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Identity API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Use Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// 7. Map Endpoints
app.MapAuthEndpoints();

app.Run();
