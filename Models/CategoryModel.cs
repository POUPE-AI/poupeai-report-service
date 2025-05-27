using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace poupeai_report_service.Models;

internal class CategoryModel
{
    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("balance")]
    [JsonPropertyName("balance")]
    public double Balance { get; set; }
}