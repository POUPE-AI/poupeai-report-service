
using Microsoft.AspNetCore.Mvc;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;

namespace poupeai_report_service.Services;

internal static class OverviewService
{
    internal static async Task<string> GenerateOverviewReport([AsParameters] PeriodFilters filters, IAIService aiService, [FromQuery] AIModel model = AIModel.Gemini)
    {
        var prompt = $"Gere um relatório geral de finanças para o período de {filters.StartDate} a {filters.EndDate}";

        return await aiService.GenerateReportAsync(prompt, model);
    }
}