using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses;

/// <summary>
/// Representa o cabeçalho de uma resposta HTTP.
/// Contém informações sobre o status da resposta e uma mensagem opcional.
/// </summary>
public record Header {
    /// <summary>
    /// O status da resposta HTTP.
    /// </summary>
    [JsonPropertyName("status")]
    public ushort Status { get; init; } = 200;

    /// <summary>
    /// A mensagem de status da resposta HTTP.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}

/// <summary>
/// Representa uma resposta de IA genérica.
/// Esta classe é usada para encapsular a resposta de um serviço de IA, incluindo o modelo utilizado,
/// </summary>
/// <typeparam name="TContent">Tipo do conteúdo da resposta.</typeparam>
public record AIResponse<TContent>
{
    /// <summary>
    /// O cabeçalho da resposta, contendo informações sobre o status e mensagem.
    /// </summary>
    [JsonPropertyName("header")]
    public Header Header { get; init; } = null!;

    /// <summary>
    /// O conteúdo da resposta, que pode ser de qualquer tipo especificado por TContent.
    /// Este campo é opcional e pode ser nulo se não houver conteúdo a ser retornado.
    /// </summary>
    [JsonPropertyName("content")]
    public TContent? Content { get; init; }
}