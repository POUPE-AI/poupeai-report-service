using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;

namespace poupeai_report_service.Services.AI;

internal class AIServiceAggregator(GeminiAIService geminiAIService, DeepseekAIService deepseekAIService) : IAIService
{
    private readonly GeminiAIService _geminiAIService = geminiAIService;
    private readonly DeepseekAIService _deepseekAIService = deepseekAIService;

    private const string RESPONSE_RULES = @"
        Use as regras abaixo para gerar o relatório:
        1. As repostas devem ser em português.
        2. Caso não não tenha informações suficientes, 
           retorne uma mensagem informando que não é possível gerar o relatório.
        3. Sempre retorne o relatório no formato JSON.
        4. Nas listas, retorne no máximo 5, porém pode retornar menos se não houver informações suficientes.
        5. Se não houver dados suficientes, responda com um status equivalente, e uma mensagem em português, siga os padrões do HTTP response para os status, exemplo: 200.
        6. Nunca retorne o relatório com dados fictícios caso não tenha informações suficientes, 
           retorne uma mensagem informando que não é possível gerar o relatório.
        7. O content não é obrigatório caso não tenha informações suficientes, 
           mas se houver, retorne o conteúdo em português.
        ";

    public async Task<string> GenerateReportAsync(string prompt, string output = "", AIModel model = AIModel.Gemini)
    {
        prompt = $"{prompt}\n\n{RESPONSE_RULES}";
        
        return model switch
        {
            AIModel.Gemini => await _geminiAIService.GenerateReportAsync(prompt, output),
            AIModel.Deepseek => await _deepseekAIService.GenerateReportAsync(prompt, output),
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Invalid AI model specified.")
        };
    }
}