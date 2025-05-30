using System.ComponentModel;

namespace poupeai_report_service.DTOs.Requests;

internal record Transaction
{
    [Description("ID da transação.")]
    public long Id { get; set; }

    [Description("Descrição da transação.")]
    public string? Description { get; set; }

    [Description("Valor da transação.")]
    public decimal Amount { get; set; }

    [Description("Data da transação.")]
    public DateTime Date { get; set; }

    [Description("Nome da categoria da transação.")]
    public string? Category { get; set; }

    [Description("Tipo da transação ('Receita', 'Despesa').")]
    public string? Type { get; set; }
}