using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace poupeai_report_service.Models;

internal class TransactionModel
{
    [BsonElement("description")]
    [BsonRepresentation(BsonType.String)]
    public string Description { get; set; } = string.Empty;

    [BsonElement("category_name")]
    [BsonRepresentation(BsonType.String)]
    public string CategoryName { get; set; } = string.Empty;

    [BsonElement("transaction_date")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime Date { get; set; }

    [BsonElement("value")]
    [BsonRepresentation(BsonType.Double)]
    public double Value { get; set; }
}