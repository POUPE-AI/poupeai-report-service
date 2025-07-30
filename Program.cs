using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using poupeai_report_service.Database;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Routes;
using poupeai_report_service.Services;
using poupeai_report_service.Services.AI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.MetadataAddress = $"{options.Authority}/.well-known/openid-configuration";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Error("Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Information("Token validated successfully for user: {User}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PoupeAI Report Service API", Version = "v1" });
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Token de Autenticação",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Insira seu token JWT Bearer no campo abaixo.",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
}
);

builder.Services.AddHttpClient();
builder.Services.AddScoped<DeepseekAIService>();
builder.Services.AddScoped<GeminiAIService>();
builder.Services.AddScoped<IAIService, AIServiceAggregator>();

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["Database:ConnectionString"];
    var databaseName = config["Database:DatabaseName"];
    var client = new MongoClient(connectionString);
    return client.GetDatabase(databaseName);
});

builder.Services.AddScoped<OverviewService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<IncomeService>();
builder.Services.AddScoped<SavingsEstimateService>();

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<InsightService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PoupeAI Report Service API V1");
        c.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

var api_group = app.MapGroup("/api/v1").WithTags("API");
api_group.MapGet("", () => Results.Ok(new
{
    app_name = "PoupeAI Report Services",
    version = "1.0.0",
}));

app.MapReportsRoutes();
app.MapSavingsRoutes();

app.Run();
