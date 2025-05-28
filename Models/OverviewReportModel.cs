using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
}