using System.Net.Http.Headers;
using System.Net.Http.Json;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;

namespace poupeai_report_service.Services.AI;

internal class GeminiAIService(IConfiguration configuration, HttpClient httpClient) : IAIService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = configuration["GeminiAI:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "API Key is not configured.");
    private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public async Task<string> GenerateReportAsync(string prompt, AIModel model = AIModel.Gemini)
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
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            Console.WriteLine($"Response status code: {response.StatusCode}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            return result?.candidates?[0]?.content?.parts?[0]?.text ?? throw new Exception("No content returned from Gemini AI.");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return "An error occurred while communicating with the AI service.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on generating report: {ex.Message}");
            return $"Error on generating report: {ex.Message}";
        }
    }
}