using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Models;
using Serilog;

namespace poupeai_report_service.Services;

internal class OverviewService(IMongoDatabase database) : IServiceReport
{
    private readonly Serilog.ILogger _logger = Log.ForContext("Service", nameof(OverviewService));

    private readonly IMongoCollection<OverviewReport> _reportsCollection = database.GetCollection<OverviewReport>("overviewreports");

    public async Task<IResult> GenerateReport(PeriodFilters filters, IAIService aiService, AIModel model = AIModel.Gemini)
    {
        var prompt = $"Gere um relatório geral de finanças para o período de {filters.StartDate} a {filters.EndDate}";
        //_logger.Information("Generating overview report with prompt", prompt);

        var report = new OverviewReport
        {
            UserId = new Guid(),
            Hash = "aslaskhdalskjdlaksdj"
        };
        await _reportsCollection.InsertOneAsync(report);

        var result = await aiService.GenerateReportAsync(prompt, model);
        return Results.Ok(result);
    }
}