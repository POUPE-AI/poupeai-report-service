using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using poupeai_report_service.DTOs.Responses.Content;

namespace poupeai_report_service.Models;

internal class InsightReportModel : BaseReportModel
{
    /// <summary>
    /// Texto do insight enviado.
    /// </summary>
    [BsonElement("insight_response")]
    [JsonPropertyName("insight_response")]
    public string InsightResponse { get; set; } = string.Empty;

    public static InsightReportModel CreateFromDTO(InsightReportResponse? response)
    {
        if (response == null)
            return new InsightReportModel();

        return new InsightReportModel
        {
            TextAnalysis = response.TextAnalysis,
            Suggestion = response.Suggestion,
            InsightResponse = response.InsightResponse,
        };
    }
}