using FiscalCore.Api;
using FiscalCore.Application;
using FiscalCore.Infrastructure;
using FiscalCore.Infrastructure.Security.Encryption;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "FiscalCore API v1", Version = "v1" }));

// Dependency Injection
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddWeb();

// Configuration validation at startup
builder.Services
    .AddOptions<EncryptionOptions>()
    .Bind(builder.Configuration.GetSection(EncryptionOptions.SectionName))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.AesKey) &&
        !string.IsNullOrWhiteSpace(options.AesIv),
        "Encryption configuration is invalid")
    .ValidateOnStart();




var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

