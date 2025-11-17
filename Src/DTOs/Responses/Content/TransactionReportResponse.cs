using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

internal record TransactionReportResponse
{
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;

    [JsonPropertyName("amount")]
    public double Amount { get; init; }

    [JsonPropertyName("date")]
    public DateTime Date { get; init; }

    [JsonPropertyName("category")]
    public string? Category { get; init; }
}