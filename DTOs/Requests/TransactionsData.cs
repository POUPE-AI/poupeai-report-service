namespace poupeai_report_service.DTOs.Requests;

internal record TransactionsData
{
    public long AccountId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
}