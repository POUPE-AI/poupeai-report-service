using System.Text.Json;
using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.FinancesServiceResponses
{
    internal record TransactionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!; // accepts UUID or numeric ids

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        // New core-service returns "transactionDate" (e.g. "2026-01-30"). Keep old "issue_date" for backward compatibility.
        [JsonPropertyName("transactionDate")]
        public string? TransactionDate { get; init; }

        [JsonPropertyName("issue_date")]
        public DateOnly? IssueDate { get; init; }

        [JsonPropertyName("type")]
        public string? Type { get; init; }

        [JsonPropertyName("bankAccountId")]
        public string? BankAccountId { get; init; }

        [JsonPropertyName("creditCardId")]
        public string? CreditCardId { get; init; }

        // The "category" field can be an object (new API) or a numeric id (old API).
        [JsonPropertyName("category")]
        public JsonElement Category { get; init; }

        [JsonPropertyName("purchaseGroupUuid")]
        public string? PurchaseGroupUuid { get; init; }

        [JsonPropertyName("isInstallment")]
        public bool? IsInstallment { get; init; }

        [JsonPropertyName("installmentNumber")]
        public int? InstallmentNumber { get; init; }

        [JsonPropertyName("totalInstallments")]
        public int? TotalInstallments { get; init; }

        [JsonPropertyName("createdAt")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("updatedAt")]
        public DateTimeOffset? UpdatedAt { get; init; }
    }

    internal record TransactionsResponse
    {
        [JsonPropertyName("content")]
        public List<TransactionResponse> Content { get; init; } = new();

        [JsonPropertyName("page")]
        public int Page { get; init; }

        [JsonPropertyName("size")]
        public int Size { get; init; }

        [JsonPropertyName("totalElements")]
        public int TotalElements { get; init; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; init; }

        // Backwards compatibility with older Django-like APIs
        [JsonPropertyName("results")]
        public List<TransactionResponse>? Results { get; init; }
    }
}