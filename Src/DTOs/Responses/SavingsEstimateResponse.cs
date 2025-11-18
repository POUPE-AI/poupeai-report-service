using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses;

/// <summary>
/// Resposta simples para cálculo de economia estimada.
/// </summary>
internal record SavingsEstimateResponse
{
    /// <summary>
    /// Valor da economia estimada em comparação com o período anterior.
    /// Valor positivo indica economia, negativo indica aumento de gastos.
    /// </summary>
    [JsonPropertyName("estimated_savings")]
    public double EstimatedSavings { get; init; }

    /// <summary>
    /// Percentual de economia em relação ao período anterior.
    /// </summary>
    [JsonPropertyName("savings_percentage")]
    public double SavingsPercentage { get; init; }

    /// <summary>
    /// Mensagem explicativa sobre a economia/gasto.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Período de comparação utilizado (ex: "monthly", "weekly").
    /// </summary>
    [JsonPropertyName("comparison_period")]
    public string ComparisonPeriod { get; init; } = string.Empty;
}
