using MongoDB.Driver;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.DTOs.Responses.Content;
using poupeai_report_service.Models;
using poupeai_report_service.Services.Bases;

namespace poupeai_report_service.Services;

internal class IncomeService(IMongoDatabase database)
    : BaseReportService<IncomeReportModel, AIResponse<IncomeReportResponse>>(
        database,
        "incomereports",
        "IncomeOutputSchema",
        nameof(IncomeService)
    )
{
    protected override string BuildPrompt(string dataJson)
    {
        return $@"
            Gere um relatório financeiro para as receitas.
            Utilize os seguintes dados abaixo para gerar o relatório (em formato JSON):

            {dataJson}

            Gere a análise textual, sugestão, total de receitas, até 5 categorias e 
            até 5 transações principais conforme o schema de output.
            ";
    }
}