using MongoDB.Driver;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.DTOs.Responses.Content;
using poupeai_report_service.Models;
using poupeai_report_service.Services.Bases;

namespace poupeai_report_service.Services;

internal class CategoryService(IMongoDatabase database)
    : BaseReportService<CategoryReportModel, AIResponse<CategoryReportResponse>>(
        database,
        "categoryreports",
        "CategoryOutputSchema",
        nameof(CategoryService)
    )
{
    protected override string BuildPrompt(string dataJson)
    {
        return $@"
            Gere um relatório financeiro para a categoria.
            Utilize os seguintes dados abaixo para gerar o relatório (em formato JSON):

            {dataJson}
            ";
    }
}