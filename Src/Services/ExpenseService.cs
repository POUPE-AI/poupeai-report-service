using MongoDB.Driver;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.DTOs.Responses.Content;
using poupeai_report_service.Models;
using poupeai_report_service.Services.Bases;

namespace poupeai_report_service.Services;

internal class ExpenseService(IMongoDatabase database)
    : BaseReportService<ExpenseReportModel, AIResponse<ExpenseReportResponse>>(
        database,
        "expensereports",
        "ExpenseOutputSchema",
        nameof(ExpenseService)
    )
{
    protected override string BuildPrompt(string dataJson)
    {
        return $@"
            Gere um relatório financeiro para as despesas.
            Utilize os seguintes dados abaixo para gerar o relatório (em formato JSON):

            {dataJson}

            Gere a análise textual, sugestão, total de despesas, até 5 categorias e 
            até 5 transações principais conforme o schema de output.
            ";
    }
}