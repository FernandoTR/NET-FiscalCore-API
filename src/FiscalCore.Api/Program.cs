using Asp.Versioning;
using FiscalCore.Api;
using FiscalCore.Application;
using FiscalCore.Infrastructure;
using FiscalCore.Infrastructure.Security.Encryption;
using FiscalCore.Infrastructure.Security.Jwt;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddMemoryCache();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "FiscalCore API v1", Version = "v1" }));

// Dependency Injection
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddWeb();


#region // Configuration validation at startup
builder.Services
    .AddOptions<EncryptionOptions>()
    .Bind(builder.Configuration.GetSection(EncryptionOptions.SectionName))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.AesKey) &&
        !string.IsNullOrWhiteSpace(options.AesIv),
        "Encryption configuration is invalid")
    .ValidateOnStart();

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.SigningKey) &&
        !string.IsNullOrWhiteSpace(options.ExpirationMinutes.ToString()),
        "Jwt configuration is invalid")
    .ValidateOnStart();

#endregion

// Versioning 
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


// Authentication / Authorization
builder.Services
    .AddAuthentication(options =>
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

            ValidIssuer = "FiscalFlow.API",
            ValidAudience = "FiscalFlow.Client",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("CAMBIA_ESTA_CLAVE_LARGA_Y_SEGURA")
            )
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
}

app.UseHangfireDashboard("/hangfire");


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

