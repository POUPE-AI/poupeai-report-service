using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;

namespace poupeai_report_service.Interfaces;

internal interface IServiceReport
{
    Task<IResult> GenerateReport(TransactionsData transactionsData, IAIService aiService, AIModel model = AIModel.Gemini);
}