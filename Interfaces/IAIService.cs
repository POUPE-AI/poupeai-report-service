using poupeai_report_service.Enums;

namespace poupeai_report_service.Interfaces;

internal interface IAIService
{
    public Task<string> GenerateReportAsync(string prompt, string output = "", AIModel model = AIModel.Gemini);
}