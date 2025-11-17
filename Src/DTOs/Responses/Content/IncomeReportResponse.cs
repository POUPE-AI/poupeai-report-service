using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

internal record IncomeReportResponse : BaseReportResponse
{
    /// <summary>
    /// Total de receitas.
    /// </summary>
    [JsonPropertyName("total_income")]
    public double TotalIncome { get; init; }

    /// <summary>
    /// Lista de categorias com seus respectivos valores.
    /// </summary>
    [JsonPropertyName("categories")]
    public List<CategoryResponse> Categories { get; init; } = [];

    /// <summary>
    /// Lista das principais receitas.
    /// </summary>
    [JsonPropertyName("main_incomes")]
    public List<MainIncomeResponse> MainIncomes { get; init; } = [];
}

internal record MainIncomeResponse
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
