using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using poupeai_report_service.DTOs.Responses.Content;

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

    /// <summary>
    /// Criar uma instância de ExpenseReportModel a partir de um ExpenseReportResponse.
    /// </summary>
    public static ExpenseReportModel CreateFromDTO(ExpenseReportResponse? response)
    {
        if (response == null)
            return new ExpenseReportModel();

        return new ExpenseReportModel
        {
            TextAnalysis = response.TextAnalysis,
            Suggestion = response.Suggestion,
            TotalExpense = response.TotalExpense,
            Categories = [.. (response.Categories ?? [])
                .Select(c => new CategoryModel
                {
                    Name = c.Name,
                    Balance = c.Balance
                })],
            MainExpenses = [.. (response.MainExpenses ?? [])
                .Select(e => new TransactionModel
                {
                    Description = e.Description,
                    Amount = e.Value,
                    Date = DateTime.TryParse(e.TransactionDate, out var date) ? date : DateTime.MinValue,
                    CategoryName = e.CategoryName
                })]
        };
    }
}