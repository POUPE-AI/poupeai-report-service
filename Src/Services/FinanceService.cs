using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.DTOs.Responses.FinancesServiceResponses;
using Serilog;

namespace poupeai_report_service.Services
{
    internal class FinancesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string _transactionsEndPoint;

        public FinancesService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri(_configuration["FinancesService:BaseUrl"] ?? throw new ArgumentNullException("FinancesService:BaseUrl is not configured."));
            _transactionsEndPoint = _configuration["FinancesService:TransactionsEndpoint"]?.TrimEnd('/') ?? throw new ArgumentNullException("FinancesService:TransactionsEndpoint is not configured.");
        }

        public async Task<List<Transaction>> GetTransactionsAsync(DateOnly startDate, DateOnly endDate)
        {
            // Try to read bearer token from Authorization header (preferred for forwarded requests and tests)
            string? accessToken = null;
            var authHeader = _httpContextAccessor.HttpContext?.Request?.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                accessToken = authHeader["Bearer ".Length..].Trim();
            }

            // Fallback to authentication abstraction if header not present
            if (string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    accessToken = _httpContextAccessor.HttpContext?.GetTokenAsync("access_token").GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Log.Debug(ex, "Could not get token via GetTokenAsync; ensure Authorization header is present in HttpContext.");
                }
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                Log.Warning("Access token not found in HttpContext. User might not be authenticated.");
                throw new UnauthorizedAccessException("Access token not available for Finances API call.");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // New core-service pagination/filtering parameters. Use page/size and include date filters as transactionDateStart/End
            var query = $"page=0&size=1000&sortBy=transactionDate&sortDirection=DESC&transactionDateStart={startDate:yyyy-MM-dd}&transactionDateEnd={endDate:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync($"{_transactionsEndPoint}?{query}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactionsResponse = JsonSerializer.Deserialize<TransactionsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var transactions = new List<Transaction>();

            // Support new API (Content) and fallback to older Results
            var items = transactionsResponse?.Content ?? transactionsResponse?.Results ?? new List<TransactionResponse>();

            foreach (var item in items)
            {
                // Category name is provided in transaction payload (new core-service). Prefer category.name if present; else fallback to string value.
                string categoryName = string.Empty;
                try
                {
                    if (item.Category.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        if (item.Category.TryGetProperty("name", out var nameProp))
                        {
                            categoryName = nameProp.GetString() ?? string.Empty;
                        }
                        else if (item.Category.TryGetProperty("id", out var idProp))
                        {
                            // use id as fallback (stringify)
                            categoryName = idProp.ToString().Trim('"');
                        }
                    }
                    else if (item.Category.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        categoryName = item.Category.GetString() ?? string.Empty;
                    }
                    else if (item.Category.ValueKind == System.Text.Json.JsonValueKind.Number)
                    {
                        categoryName = item.Category.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to parse category value from transaction payload");
                }

                // Parse date: prefer transactionDate, fallback to issue_date
                DateTime date;
                if (!string.IsNullOrEmpty(item.TransactionDate) && DateTime.TryParse(item.TransactionDate, out var parsed))
                {
                    date = parsed;
                }
                else if (item.IssueDate.HasValue)
                {
                    date = item.IssueDate.Value.ToDateTime(TimeOnly.MinValue);
                }
                else
                {
                    date = DateTime.MinValue;
                }

                transactions.Add(new Transaction
                {
                    Id = item.Id,
                    Description = item.Description,
                    Amount = item.Amount,
                    Date = date,
                    Category = string.IsNullOrEmpty(categoryName) ? item.PurchaseGroupUuid ?? string.Empty : categoryName,
                    Type = item.Type
                });
            }

            return transactions;
        }
    }
}