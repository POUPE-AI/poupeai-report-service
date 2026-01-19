using System.ComponentModel;

namespace poupeai_report_service.DTOs.Requests;

/// <summary>
/// Representa uma categoria disponível para classificação.
/// </summary>
internal record AvailableCategory
{
    [Description("ID da categoria.")]
    public string Id { get; set; } = null!;

    [Description("Nome da categoria.")]
    public string Name { get; set; } = null!;

    [Description("Descrição da categoria.")]
    public string? Description { get; set; }
}

/// <summary>
/// Representa os dados necessários para realizar a categorização de transações.
/// </summary>
internal record CategorizationRequest
{
    [Description("Lista de transações a serem categorizadas.")]
    public List<Transaction> Transactions { get; set; } = [];

    [Description("Lista de categorias disponíveis para classificação.")]
    public List<AvailableCategory> AvailableCategories { get; set; } = [];
}
