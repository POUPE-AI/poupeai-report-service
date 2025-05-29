using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace poupeai_report_service.Models;

internal class TransactionModel
{
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("category_name")]
    [BsonIgnoreIfNull]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CategoryName { get; set; }

    [BsonElement("transaction_date")]
    public DateTime Date { get; set; }

    [BsonElement("amount")]
    public double Amount { get; set; }
}