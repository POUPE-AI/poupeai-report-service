using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using poupeai_report_service.DTOs.Responses.Content;

namespace poupeai_report_service.Models;

internal class IncomeReportModel : BaseReportModel
{
    [BsonElement("total_income")]
    [JsonPropertyName("totalIncome")]
    public double TotalIncome { get; set; }

    [BsonElement("categories")]
    [JsonPropertyName("categories")]
    public List<CategoryModel> Categories { get; set; } = [];

    [BsonElement("main_incomes")]
    [JsonPropertyName("mainIncomes")]
    public List<TransactionModel> MainIncomes { get; set; } = [];

    /// <summary>
    /// Criar uma instância de IncomeReportModel a partir de um IncomeReportResponse.
    /// </summary>
    public static IncomeReportModel CreateFromDTO(IncomeReportResponse? response)
    {
        if (response == null)
            return new IncomeReportModel();

        return new IncomeReportModel
        {
            TextAnalysis = response.TextAnalysis,
            Suggestion = response.Suggestion,
            TotalIncome = response.TotalIncome,
            Categories = [.. (response.Categories ?? [])
                .Select(c => new CategoryModel
                {
                    Name = c.Name,
                    Balance = c.Balance
                })],
            MainIncomes = [.. (response.MainIncomes ?? [])
                .Select(i => new TransactionModel
                {
                    Description = i.Description,
                    Amount = i.Value,
                    Date = DateTime.TryParse(i.TransactionDate, out var date) ? date : DateTime.MinValue,
                    CategoryName = i.CategoryName
                })]
        };
    }
}