using System.Text.Json;
using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Responses.FinancesServiceResponses
{
    internal record TransactionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("transactionDate")]
        public string? TransactionDate { get; init; }

        [JsonPropertyName("type")]
        public string? Type { get; init; }

        [JsonPropertyName("bankAccountId")]
        public string? BankAccountId { get; init; }

        [JsonPropertyName("creditCardId")]
        public string? CreditCardId { get; init; }

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
    }
}