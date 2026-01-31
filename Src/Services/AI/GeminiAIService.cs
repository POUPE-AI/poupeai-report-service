using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Utils;

namespace poupeai_report_service.Services.AI;

internal class GeminiAIService(IConfiguration configuration, ILogger<GeminiAIService> logger) : IAIService
{
    private readonly string _apiKey = configuration["GeminiAI:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "API Key is not configured.");
    private readonly ILogger<GeminiAIService> _logger = logger;
    private const string MODEL_NAME = "gemini-3-flash-preview";

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

                var client = new Client(apiKey: _apiKey);

                var config = new GenerateContentConfig
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = Schema.FromJson(output)
                };

                var response = await client.Models.GenerateContentAsync(
                    model: MODEL_NAME,
                    contents: prompt,
                    config: config
                );

                if (response?.Candidates == null || response.Candidates.Count == 0)
                {
                    throw new Exception("No candidates returned from Gemini AI.");
                }

                var candidate = response.Candidates[0];

                if (candidate.Content?.Parts == null || candidate.Content.Parts.Count == 0)
                {
                    throw new Exception("No content parts returned from Gemini AI.");
                }

                var text = candidate.Content.Parts[0].Text;

                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new Exception("Empty text returned from Gemini AI.");
                }

                return text;
            }
            catch (Exception ex) when (
                (ex.Message.Contains("429") ||
                 ex.Message.Contains("TooManyRequests") ||
                 ex.Message.Contains("Quota exceeded") ||
                 ex.Message.Contains("quota") ||
                 ex.Message.Contains("503") ||
                 ex.Message.Contains("ServiceUnavailable")) &&
                 attempt < maxRetries)
            {
                _logger.LogWarning(ex, "Rate limit or service error on attempt {Attempt}: {Message}", attempt, ex.Message);

                var delay = ex.Message.Contains("Quota") || ex.Message.Contains("quota")
                    ? Math.Max(20000, baseDelayMs * (int)Math.Pow(2, attempt))
                    : baseDelayMs * (int)Math.Pow(2, attempt - 1);

                _logger.LogInformation("Retrying in {Delay}ms...", delay);
                await Task.Delay(delay);
                continue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on generating report: {Message}", ex.Message);

                if (attempt < maxRetries)
                {
                    var delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                    _logger.LogInformation("Retrying in {Delay}ms...", delay);
                    await Task.Delay(delay);
                    continue;
                }

                return Tools.CreateErrorResponse($"Error generating report: {ex.Message}");
            }
        }

        return Tools.CreateErrorResponse("Maximum retry attempts exceeded");
    }
}