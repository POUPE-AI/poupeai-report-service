using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

internal record ExpenseReportResponse : BaseReportResponse
{
    /// <summary>
    /// Total de despesas.
    /// </summary>
    [JsonPropertyName("total_expense")]
    public double TotalExpense { get; init; }

    /// <summary>
    /// Lista de categorias com seus respectivos balances.
    /// </summary>
    [JsonPropertyName("categories")]
    public List<CategoryResponse> Categories { get; init; } = [];

    /// <summary>
    /// Lista das principais despesas.
    /// </summary>
    [JsonPropertyName("main_expenses")]
    public List<MainExpenseResponse> MainExpenses { get; init; } = [];
}

internal record CategoryResponse
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("balance")]
    public double Balance { get; init; }
}

internal record MainExpenseResponse
{
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;

    [JsonPropertyName("transaction_date")]
    public string TransactionDate { get; init; } = null!;

    [JsonPropertyName("value")]
    public double Value { get; init; }

    [JsonPropertyName("category_name")]
    public string CategoryName { get; init; } = null!;
}
