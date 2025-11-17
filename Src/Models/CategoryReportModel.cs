using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using poupeai_report_service.DTOs.Responses.Content;

namespace poupeai_report_service.Models;

internal class CategoryReportModel : BaseReportModel
{
    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("total")]
    public double Total { get; set; }

    [BsonElement("average")]
    public float Average { get; set; }

    [BsonElement("trend")]
    [BsonIgnoreIfNull]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Trend { get; set; }

    [BsonElement("peak_days")]
    [BsonIgnoreIfNull]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? PeakDays { get; set; }

    [BsonElement("main_transactions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TransactionModel>? Transactions { get; set; }

    /// <summary>
    /// Criar uma instância de CategoryReportModel a partir de um CategoryReportResponse.
    /// </summary>
    public static CategoryReportModel CreateFromDTO(CategoryReportResponse? response)
    {
        if (response == null)
            return new CategoryReportModel();

        return new CategoryReportModel
        {
            TextAnalysis = response.TextAnalysis,
            Suggestion = response.Suggestion,
            Category = response.Category,
            Total = response.Total,
            Average = response.Average,
            Trend = response.Trend,
            PeakDays = response.PeakDays,
            Transactions = [.. (response.Transactions ?? [])
            .Select(t => new TransactionModel
            {
                Description = t.Description,
                Amount = t.Amount,
                Date = t.Date,
            })]
        };
    }
}