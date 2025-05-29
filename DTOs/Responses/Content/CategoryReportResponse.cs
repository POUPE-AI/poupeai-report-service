using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

internal record CategoryReportResponse : BaseReportResponse
{
    [JsonPropertyName("category")]
    public string Category { get; init; } = null!;

    [JsonPropertyName("total")]
    public double Total { get; init; }

    [JsonPropertyName("average")]
    public float Average { get; init; }

    [JsonPropertyName("trend")]
    public string? Trend { get; init; }

    [JsonPropertyName("peak_days")]
    public string[]? PeakDays { get; init; }

    [JsonPropertyName("main_transactions")]
    public List<TransactionReportResponse> Transactions { get; init; } = null!;
}