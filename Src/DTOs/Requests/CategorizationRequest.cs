using System.ComponentModel;
using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Requests;

/// <summary>
/// Representa uma categoria disponível para classificação.
/// </summary>
internal record UserCategory
{
    [Description("ID da categoria.")]
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [Description("Nome da categoria.")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}

/// <summary>
/// Representa os dados necessários para realizar a categorização de transações.
/// </summary>
internal record CategorizationRequest
{
    [Description("Lista de descrições a serem categorizadas.")]
    [JsonPropertyName("descriptions")]
    public List<string> Descriptions { get; set; } = [];

    [Description("Lista de categorias disponíveis para classificação.")]
    [JsonPropertyName("userCategories")]
    public List<UserCategory> UserCategories { get; set; } = [];
}
