using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
}