namespace poupeai_report_service.DTOs.Requests;

internal record Transaction
{
    public long Id { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Category { get; set; }
    public string? Type { get; set; }
}