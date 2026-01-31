using MongoDB.Driver;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.DTOs.Responses.Content;
using poupeai_report_service.Models;
using poupeai_report_service.Services.Bases;

namespace poupeai_report_service.Services;

internal class OverviewService(IMongoDatabase database)
    : BaseReportService<OverviewReportModel, AIResponse<OverviewReportResponse>>(
        database,
        "overviewreports",
        "OverviewOutputSchema",
        nameof(OverviewService)
    )
{
    protected override string BuildPrompt(string dataJson)
    {
        return $@"
            Gere um relatório geral de finanças.
            Utilize os seguintes dados abaixo para gerar o relatório (em formato JSON):

            {dataJson}

            Gere a análise textual, sugestão, saldo, total de receitas, total de despesas e 
            categorias conforme o schema de output.
            ";
    }
}