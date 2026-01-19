using System.Text.Json;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.DTOs.Responses.Content;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Utils;
using Serilog;

namespace poupeai_report_service.Services.Internal;

/// <summary>
/// Serviço responsável por categorizar transações usando IA.
/// Este serviço não persiste dados no banco, apenas retorna o mapeamento de categorizações.
/// </summary>
internal class CategorizationService
{
    private readonly Serilog.ILogger _logger = Log.ForContext<CategorizationService>();
    private readonly string _outputSchema;

    public CategorizationService()
    {
        _outputSchema = Tools.ReadSchema("CategorizationOutputSchema");
    }

    /// <summary>
    /// Categoriza uma lista de transações com base nas categorias disponíveis.
    /// </summary>
    /// <param name="request">Dados de categorização contendo transações e categorias disponíveis.</param>
    /// <param name="aiService">Serviço de IA a ser utilizado.</param>
    /// <param name="model">Modelo de IA a ser utilizado (padrão: Gemini).</param>
    /// <returns>Resultado HTTP contendo o mapeamento de categorizações.</returns>
    public async Task<IResult> CategorizeTransactionsAsync(
        CategorizationRequest request,
        IAIService aiService,
        AIModel model = AIModel.Gemini
    )
    {
        try
        {
            if (request.Descriptions == null || request.Descriptions.Count == 0)
            {
                _logger.Warning("No descriptions provided for categorization.");
                return Results.BadRequest(new
                {
                    Header = new
                    {
                        Status = 400,
                        Message = "Nenhuma descrição fornecida para categorização."
                    }
                });
            }

            if (request.UserCategories == null || request.UserCategories.Count == 0)
            {
                _logger.Warning("No categories provided for categorization.");
                return Results.BadRequest(new
                {
                    Header = new
                    {
                        Status = 400,
                        Message = "Nenhuma categoria disponível para categorização."
                    }
                });
            }

            var dataJson = JsonSerializer.Serialize(new
            {
                descriptions = request.Descriptions,
                user_categories = request.UserCategories.Select(c => new
                {
                    id = c.Id,
                    name = c.Name
                })
            });

            var prompt = BuildPrompt(dataJson);

            _logger.Information("Generating categorization using {Model} model for {DescriptionCount} descriptions and {CategoryCount} categories",
                Tools.ModelToString(model), request.Descriptions.Count, request.UserCategories.Count);

            var result = await aiService.GenerateReportAsync(prompt, _outputSchema, model);

            if (string.IsNullOrEmpty(result))
            {
                _logger.Error("AI service returned an empty result for categorization.");
                return Results.Problem("Falha ao categorizar as transações devido a resposta vazia do serviço de IA.");
            }

            AIResponse<CategorizationResponse>? response;
            try
            {
                response = Tools.DeserializeJson<CategorizationResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to deserialize AI service response. Raw response: {RawResponse}", result);
                return Results.Problem("Falha ao categorizar as transações devido a erro de desserialização.");
            }

            if (response == null)
            {
                _logger.Error("Failed to deserialize AI service response. Response was null.");
                return Results.Problem("Falha ao categorizar as transações devido a erro de desserialização.");
            }

            if (response.Header.Status != 200)
            {
                _logger.Error("AI service returned an error: {Status} - {Message}",
                    response.Header.Status, response.Header.Message);
                return Tools.BuildResultFromHeader(response.Header, response.Header.Status);
            }

            if (response.Content == null || response.Content.Categorizations == null)
            {
                _logger.Error("AI service response content is null.");
                return Results.Problem("Falha ao categorizar as transações devido a conteúdo nulo na resposta da IA.");
            }

            _logger.Information("Categorization completed successfully with {Count} categorizations",
                response.Content.Categorizations.Count);

            return Results.Ok(new
            {
                response.Header,
                Content = response.Content
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error categorizing transactions");
            return Results.Problem($"Ocorreu um erro ao categorizar as transações: {ex.Message}");
        }
    }

    /// <summary>
    /// Constrói o prompt para a IA realizar a categorização.
    /// </summary>
    /// <param name="dataJson">JSON contendo as transações e categorias disponíveis.</param>
    /// <returns>Prompt formatado para a IA.</returns>
    private string BuildPrompt(string dataJson)
    {
        return $@"
            Você é um assistente de IA especializado em classificação financeira.
            
            Sua tarefa é categorizar cada descrição fornecida escolhendo ESTRITAMENTE uma das categorias disponíveis.
            
            Dados fornecidos (em formato JSON):
            {dataJson}
            
            INSTRUÇÕES IMPORTANTES:
            1. Para cada descrição, analise o texto e identifique a categoria mais apropriada.
            2. Escolha a categoria mais apropriada da lista de categorias disponíveis (user_categories).
            3. Você DEVE escolher apenas categorias que estão na lista fornecida - NUNCA invente ou crie novas categorias.
            4. Retorne um array JSON com objetos contendo a descrição original e o ID da categoria escolhida.
            5. O formato deve ser: [{{ ""description"": ""texto_descricao"", ""category_id"": ""id_categoria"" }}]
            6. Se uma descrição não se encaixar perfeitamente em nenhuma categoria, escolha a mais próxima ou genérica.
            7. TODAS as descrições fornecidas devem ter uma categoria atribuída.
            
            Exemplo de resposta esperada:
            {{
              ""categorizations"": [
                {{ ""description"": ""UBER *VIAGEM"", ""category_id"": ""uuid-1111"" }},
                {{ ""description"": ""IFOOD *PAGAMENTO"", ""category_id"": ""uuid-2222"" }},
                {{ ""description"": ""NETFLIX.COM"", ""category_id"": ""uuid-3333"" }}
              ]
            }}
            ";
    }
}
