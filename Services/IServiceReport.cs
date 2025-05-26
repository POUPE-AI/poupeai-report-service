using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;

namespace poupeai_report_service.Services;

internal interface IServiceReport
{
    Task<IResult> GenerateReport(PeriodFilters filters, IAIService aiService, AIModel model = AIModel.Gemini);
}