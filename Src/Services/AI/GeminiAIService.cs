using System.Text.Json;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Utils;

namespace poupeai_report_service.Services.AI;

internal class GeminiAIService(IConfiguration configuration, HttpClient httpClient, ILogger<GeminiAIService> logger) : IAIService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = configuration["GeminiAI:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "API Key is not configured.");
    private readonly ILogger<GeminiAIService> _logger = logger;
    private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public async Task<string> GenerateReportAsync(string prompt, string output, AIModel model = AIModel.Gemini)
    {
        const int maxRetries = 3;
        const int baseDelayMs = 1000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prompt))
                    throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));

                if (model != AIModel.Gemini)
                    throw new ArgumentOutOfRangeException(nameof(model), model, "Invalid AI model specified.");

                var url = $"{BASE_URL}?key={_apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        responseMimeType = "application/json",
                        responseSchema = JsonSerializer.Deserialize<object>(output)
                    },
                };

                var response = await _httpClient.PostAsJsonAsync(url, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Request error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    
                    if ((response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || 
                         response.StatusCode == System.Net.HttpStatusCode.TooManyRequests) && 
                         attempt < maxRetries)
                    {
                        var delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                        _logger.LogInformation("Retrying in {Delay}ms...", delay);
                        await Task.Delay(delay);
                        continue;
                    }
                    
                    return Tools.CreateErrorResponse($"API returned {response.StatusCode}: {response.ReasonPhrase}");
                }

                var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                return result?.candidates?[0]?.content?.parts?[0]?.text ?? throw new Exception("No content returned from Gemini AI.");
            }
            catch (HttpRequestException ex) when (attempt < maxRetries)
            {
                _logger.LogWarning(ex, "HTTP request error on attempt {Attempt}: {Message}", attempt, ex.Message);
                var delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delay);
                continue;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Request error: {Message}", ex.Message);
                return Tools.CreateErrorResponse($"Communication error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on generating report: {Message}", ex.Message);
                return Tools.CreateErrorResponse($"Error generating report: {ex.Message}");
            }
        }

        // This should never be reached, but just in case
        return Tools.CreateErrorResponse("Maximum retry attempts exceeded");
    }
}