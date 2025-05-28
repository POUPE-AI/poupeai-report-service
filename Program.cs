using MongoDB.Driver;
using poupeai_report_service.Database;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Routes;
using poupeai_report_service.Services;
using poupeai_report_service.Services.AI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddScoped<IServiceReport, OverviewService>();
builder.Services.AddScoped<IServiceReport, ExpenseService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var api_group = app.MapGroup("/api/v1").WithTags("API");
api_group.MapGet("", () => Results.Ok(new {
    app_name = "PoupeAI Report Services",
    version = "1.0.0",
}));

app.MapReportsRoutes();

app.Run();
