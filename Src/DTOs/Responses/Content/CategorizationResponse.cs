using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.Content;

/// <summary>
/// Representa a resposta de categorização de transações.
/// </summary>
internal record CategorizationResponse
{
    /// <summary>
    /// Mapeamento de descrição da transação para ID da categoria.
    /// Chave: Descrição da transação
    /// Valor: ID da categoria atribuída
    /// </summary>
    [JsonPropertyName("categorizations")]
    public Dictionary<string, string> Categorizations { get; init; } = new();
}
