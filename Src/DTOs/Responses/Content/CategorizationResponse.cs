using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

/// <summary>
/// Representa uma categorização individual.
/// </summary>
internal record Categorization
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; } = null!;
}

/// <summary>
/// Representa a resposta de categorização de transações.
/// </summary>
internal record CategorizationResponse
{
    /// <summary>
    /// Lista de categorizações.
    /// </summary>
    [JsonPropertyName("categorizations")]
    public List<Categorization> Categorizations { get; init; } = [];
}
