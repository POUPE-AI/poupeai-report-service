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
        private readonly string _categoryEndPoint;

        public FinancesService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri(_configuration["FinancesService:BaseUrl"] ?? throw new ArgumentNullException("FinancesService:BaseUrl is not configured."));
            _transactionsEndPoint = _configuration["FinancesService:TransactionsEndpoint"] ?? throw new ArgumentNullException("FinancesService:TransactionsEndpoint is not configured.");
            _categoryEndPoint = _configuration["FinancesService:CategoryEndpoint"] ?? throw new ArgumentNullException("FinancesService:CategoryEndpoint is not configured.");
        }

        public async Task<List<Transaction>> GetTransactionsAsync(DateOnly startDate, DateOnly endDate)
        {
            // Obter o token JWT do usuário autenticado
            // Acessamos o HttpContext da requisição atual através de IHttpContextAccessor
            var accessToken = _httpContextAccessor.HttpContext?.GetTokenAsync("access_token").Result;

            if (string.IsNullOrEmpty(accessToken))
            {
                Log.Warning("Access token not found in HttpContext for Django Finance Service. User might not be authenticated.");
                // Dependendo da sua lógica, você pode lançar uma exceção ou retornar uma lista vazia.
                throw new UnauthorizedAccessException("Access token not available for Django API call.");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var query = $"issue_date_start={startDate:yyyy-MM-dd}&issue_date_end={endDate:yyyy-MM-dd}&page_size=1000";

            var response = await _httpClient.GetAsync($"{_transactionsEndPoint}?{query}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var djangoResponse = JsonSerializer.Deserialize<TransactionsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var transactions = new List<Transaction>();
            foreach (var djangoTran in djangoResponse?.Results ?? new List<TransactionResponse>())
            {
                // Buscar nome da categoria
                string categoryName = await GetCategoryNameAsync(djangoTran.Category);
                transactions.Add(new Transaction
                {
                    Id = djangoTran.Id,
                    Description = djangoTran.Description,
                    Amount = djangoTran.Amount,
                    Date = djangoTran.IssueDate.ToDateTime(TimeOnly.MinValue),
                    Category = categoryName,
                    Type = djangoTran.Type
                });
            }

            return transactions;
        }

        private async Task<string> GetCategoryNameAsync(long categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_categoryEndPoint}{categoryId}/");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("name", out var nameProp))
                    {
                        return nameProp.GetString() ?? categoryId.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to fetch category name for category id {CategoryId}", categoryId);
            }
            return categoryId.ToString();
        }
    }
}