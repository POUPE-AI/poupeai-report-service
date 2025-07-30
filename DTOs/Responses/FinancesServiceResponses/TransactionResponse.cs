using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.FinancesServiceResponses
{
    internal record TransactionResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("issue_date")]
        public DateOnly IssueDate { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("source_type")]
        public string? SourceType { get; set; }

        [JsonPropertyName("category")]
        public long Category { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("bank_account")]
        public long? BankAccount { get; set; }

        [JsonPropertyName("credit_card")]
        public long? CreditCard { get; set; }
    }

    internal record TransactionsResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("previous")]
        public string? Previous { get; set; }

        [JsonPropertyName("results")]
        public List<TransactionResponse> Results { get; set; } = [];
    }
}