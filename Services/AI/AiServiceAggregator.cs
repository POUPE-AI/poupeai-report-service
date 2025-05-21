using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;

namespace poupeai_report_service.Services.AI;

internal class AIServiceAggregator(GeminiAIService geminiAIService, DeepseekAIService deepseekAIService) : IAIService
{
    private readonly GeminiAIService _geminiAIService = geminiAIService;
    private readonly DeepseekAIService _deepseekAIService = deepseekAIService;

    public async Task<string> GenerateReportAsync(string prompt, AIModel model = AIModel.Gemini)
    {
        return model switch
        {
            AIModel.Gemini => await _geminiAIService.GenerateReportAsync(prompt),
            AIModel.Deepseek => await _deepseekAIService.GenerateReportAsync(prompt),
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Invalid AI model specified.")
        };
    }
}