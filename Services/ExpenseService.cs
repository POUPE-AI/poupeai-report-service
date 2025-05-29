using System.Text.Json;
using MongoDB.Driver;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Models;
using poupeai_report_service.Utils;
using Serilog;

internal class ExpenseService(IMongoDatabase database) : IServiceReport
{
    private readonly Serilog.ILogger _logger = Log.ForContext("Service", nameof(ExpenseService));

    private readonly IMongoCollection<ExpenseReportModel> _reportsCollection = database.GetCollection<ExpenseReportModel>("expensereports");

    private readonly string Output = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Schemas", "ExpenseOutputSchema.json"));

    public async Task<IResult> GenerateReport(TransactionsData transactionsData, IAIService aiService, AIModel model = AIModel.Gemini)
    {
        try
        {
            // Gerar hash dos dados a ser analisados
            var hash = Hash.GenerateFromTransaction(transactionsData, model);

            // Buscar no banco de dados se o relatório já foi criado
            var existingReport = await _reportsCollection.Find(r => r.Hash == hash).FirstOrDefaultAsync();
            if (existingReport != null)
            {
                _logger.Debug("Expense report already exists for hash: {Hash}", hash);
                return Results.Ok(existingReport);
            }

            // Montar prompt para enviar para IA
            var dataJson = JsonSerializer.Serialize(transactionsData);

            var prompt = $@"
                Gere um relatório financeiro para as despesas.
                Utilize os seguintes dados abaixo para gerar o relatório (em formato JSON):

                {dataJson}

                Gere a análise textual, sugestão, total de despesas, até 5 categorias e 
                até 5 transações princiapais conforme o schema de output.";

            // Fazer requisição ao serviço de IA
            var result = await aiService.GenerateReportAsync(prompt, Output, model);

            _logger.Debug("Resposta bruta da IA: {Result}", result);

            if (string.IsNullOrEmpty(result))
            {
                _logger.Error("AI service returned an empty result for expense report generation.");
                return Results.Problem("Failed to generate the report due to empty response from AI service.");
            }

            try
            {
                var deserializedResult = JsonSerializer.Deserialize<ExpenseReportModel>(result);

                if (deserializedResult == null)
                {
                    _logger.Error("Failed to deserialize AI service response for expense report.");
                    return Results.Problem("Failed to generate the report due to deserialization error.");
                }

                deserializedResult.Hash = hash;
                deserializedResult.AccountId = transactionsData.AccountId;
                deserializedResult.StartDate = transactionsData.StartDate;
                deserializedResult.EndDate = transactionsData.EndDate;
                deserializedResult.UpdatedAt = DateTime.UtcNow;

                // Salvar relatório no banco de dados
                await _reportsCollection.InsertOneAsync(deserializedResult);

                _logger.Information("Expense report generated and saved successfully with hash: {Hash}", hash);
                return Results.Ok(deserializedResult);
            }
            catch
            {
                return Results.Problem("Failed to generate the report from AI service response. Please check the AI service.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating Expense report");
            return Results.Problem("An error occurred while generating the report.");
        }
    }
}