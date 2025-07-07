using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

internal record OverviewReportResponse : BaseReportResponse
{
    /// <summary>
    /// Saldo atual (receitas - despesas).
    /// </summary>
    [JsonPropertyName("balance")]
    public double Balance { get; init; }

    /// <summary>
    /// Total de receitas.
    /// </summary>
    [JsonPropertyName("total_income")]
    public double TotalIncome { get; init; }

    /// <summary>
    /// Total de despesas.
    /// </summary>
    [JsonPropertyName("total_expense")]
    public double TotalExpense { get; init; }

    /// <summary>
    /// Lista de categorias com seus respectivos valores.
    /// </summary>
    [JsonPropertyName("categories")]
    public List<CategoryResponse> Categories { get; init; } = [];
}
