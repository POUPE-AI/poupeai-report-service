using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace poupeai_report_service.Models;

internal class OverviewReport
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string? ReportId { get; init; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("hash")]
    public string? Hash { get; set; }

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; init; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}