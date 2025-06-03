using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

internal record InsightReportResponse : BaseReportResponse
{
    /// <summary>
    /// Texto do insight enviado.
    /// </summary>
    [JsonPropertyName("insight_response")]
    public string InsightResponse { get; init; } = null!;
}