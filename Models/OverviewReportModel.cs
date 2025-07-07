using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using poupeai_report_service.DTOs.Responses.Content;

namespace poupeai_report_service.Models;

internal class OverviewReportModel : BaseReportModel
{
    [BsonElement("balance")]
    [JsonPropertyName("balance")]
    public float Balance { get; set; }

    [BsonElement("total_income")]
    [JsonPropertyName("totalIncome")]
    public float TotalIncome { get; set; }

    [BsonElement("total_expense")]
    [JsonPropertyName("totalExpense")]
    public float TotalExpense { get; set; }

    [BsonElement("categories")]
    [JsonPropertyName("categories")]
    public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();

    /// <summary>
    /// Criar uma instância de OverviewReportModel a partir de um OverviewReportResponse.
    /// </summary>
    public static OverviewReportModel CreateFromDTO(OverviewReportResponse? response)
    {
        if (response == null)
            return new OverviewReportModel();

        return new OverviewReportModel
        {
            TextAnalysis = response.TextAnalysis,
            Suggestion = response.Suggestion,
            Balance = (float)response.Balance,
            TotalIncome = (float)response.TotalIncome,
            TotalExpense = (float)response.TotalExpense,
            Categories = [.. (response.Categories ?? [])
                .Select(c => new CategoryModel
                {
                    Name = c.Name,
                    Balance = c.Balance
                })]
        };
    }
}