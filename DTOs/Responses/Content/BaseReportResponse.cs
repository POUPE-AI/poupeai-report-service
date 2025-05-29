using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

internal abstract record BaseReportResponse
{
    [JsonPropertyName("text_analysis")]
    public string TextAnalysis { get; init; } = null!;

    [JsonPropertyName("suggestion")]
    public string Suggestion { get; init; } = null!;
}