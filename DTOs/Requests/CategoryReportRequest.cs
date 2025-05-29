using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Requests;

internal record CategoryReportRequest : TransactionsData
{
    /// <summary>
    /// A categoria para a qual o relatório será gerado.
    /// Esta propriedade é usada para filtrar as transações por categoria antes de gerar o relatório.
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
}