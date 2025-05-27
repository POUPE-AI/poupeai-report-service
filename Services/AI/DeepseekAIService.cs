using System.Net.Http.Headers;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;

namespace poupeai_report_service.Services.AI;

internal class DeepseekAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BASE_URL = "https://api.deepseek.com/v1/chat/completions";

    public DeepseekAIService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["DeepseekAI:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "API Key is not configured.");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> GenerateReportAsync(string prompt, string output = "", AIModel model = AIModel.Deepseek)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));

            if (model != AIModel.Deepseek)
                throw new ArgumentOutOfRangeException(nameof(model), model, "Invalid AI model specified.");

            if (!string.IsNullOrEmpty(output))
            {
                prompt = $"{prompt}\n\nA respota deve ser formatada como um relatório em json, extamente no formato:\n{output}\n\n";
            }


            var requestBody = new
            {
                model = "deepseek-reasoner",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Você é um assistente de IA que ajuda a gerar relatórios financeiros com base em dados fornecidos."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.7,
                max_tokens = 2000,
            };

            var response = await _httpClient.PostAsJsonAsync(BASE_URL, requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DeepSeekResponse>();
            return result?.Choices?[0].Message?.Content ?? throw new Exception("Empty response from Deepseek AI.");
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