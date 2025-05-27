using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace poupeai_report_service.Models;

internal abstract class BaseReportModel
{
    [BsonId]
    [BsonElement("hash")]
    public string Hash { get; set; } = string.Empty;

    [BsonElement("account_id")]
    public long AccountId { get; set; }

    [BsonElement("user_id")]
    public DateOnly StartDate { get; set; }

    [BsonElement("end_date")]
    public DateOnly EndDate { get; set; }

    [BsonElement("text_analysis")]
    [JsonPropertyName("textAnalysis")]
    public string TextAnalysis { get; set; } = string.Empty;

    [BsonElement("suggestion")]
    [JsonPropertyName("suggestion")]
    public string Suggestion { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}