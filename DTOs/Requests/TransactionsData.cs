using System.ComponentModel;
using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Requests;

internal record TransactionsData
{
    [Description("ID da conta para a qual as transações serão buscadas.")]
    public long AccountId { get; set; }

    [Description("Data de início para filtrar as transações.")]
    public DateOnly StartDate { get; set; }

    [Description("Data de fim para filtrar as transações.")]
    public DateOnly EndDate { get; set; }

    [Description("Lista de transações a serem processadas.")]
    public List<Transaction> Transactions { get; set; } = [];
}