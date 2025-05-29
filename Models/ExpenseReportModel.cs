using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace poupeai_report_service.Models;

internal class ExpenseReportModel : BaseReportModel
{
    [BsonElement("total_expense")]
    [JsonPropertyName("totalExpense")]
    public double TotalExpense { get; set; }

    [BsonElement("categories")]
    [JsonPropertyName("categories")]
    public List<CategoryModel> Categories { get; set; } = [];

    [BsonElement("main_expenses")]
    [JsonPropertyName("mainExpenses")]
    public List<TransactionModel> MainExpenses { get; set; } = [];
}