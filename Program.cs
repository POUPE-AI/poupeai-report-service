using poupeai_report_service.Interfaces;
using poupeai_report_service.Routes;
using poupeai_report_service.Services.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddScoped<DeepseekAIService>();
builder.Services.AddScoped<GeminiAIService>();
builder.Services.AddScoped<IAIService, AIServiceAggregator>();

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
