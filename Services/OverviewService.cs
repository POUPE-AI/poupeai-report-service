using System.Text.Json;
using MongoDB.Driver;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Models;
using Serilog;
using poupeai_report_service.Utils;

namespace poupeai_report_service.Services;

internal class OverviewService(IMongoDatabase database) : IServiceReport
{
    private readonly Serilog.ILogger _logger = Log.ForContext("Service", nameof(OverviewService));

    private readonly IMongoCollection<OverviewReportModel> _reportsCollection = database.GetCollection<OverviewReportModel>("overviewreports");

    private readonly string output = File.ReadAllText(
        Path.Combine(AppContext.BaseDirectory, "Schemas", "OverviewOutputSchema.json"));

    public async Task<IResult> GenerateReport(TransactionsData transactionsData, IAIService aiService, AIModel model = AIModel.Gemini)
    {
        try
        {
            // Gerar hash dos dados a ser analisados
            var hash = Hash.GenerateFromTransaction(transactionsData, model);

            var existingReport = await _reportsCollection.Find(r => r.Hash == hash).FirstOrDefaultAsync();
            if (existingReport != null)
            {
                _logger.Debug("Overview report already exists for hash: {Hash}", hash);
                return Results.Ok(existingReport);
            }

            var dataJson = JsonSerializer.Serialize(transactionsData);

            var prompt = $@"
            Gere um relatório geral de finanças.
            Utilize os seguintes dados mockados de exemplo para gerar o relatório (em formato JSON):

            {dataJson}

            Gere a análise textual, sugestão, saldo, total de receitas, total de despesas e categorias conforme o schema de output.
            ";

            var result = await aiService.GenerateReportAsync(prompt, output, model);

            if (string.IsNullOrEmpty(result))
            {
                _logger.Error("AI service returned an empty result for overview report generation.");
                return Results.Problem("Failed to generate the report due to empty response from AI service.");
            }

            try
            {
                var deserializedResult = JsonSerializer.Deserialize<OverviewReportModel>(result);

                if (deserializedResult == null)
                {
                    _logger.Error("Failed to deserialize AI service response for overview report.");
                    return Results.Problem("Failed to generate the report due to deserialization error.");
                }

                deserializedResult.Hash = hash;
                deserializedResult.AccountId = transactionsData.AccountId;
                deserializedResult.StartDate = transactionsData.StartDate;
                deserializedResult.EndDate = transactionsData.EndDate;
                deserializedResult.UpdatedAt = DateTime.UtcNow;

                // Save the report to the database
                await _reportsCollection.InsertOneAsync(deserializedResult);

                _logger.Information("Overview report generated and saved successfully with hash: {Hash}", hash);
                return Results.Ok(deserializedResult);
            }
            catch
            {
                return Results.Problem("Failed to generate the report from AI service response. Please check the AI service.");
            }
        }

        catch (ArgumentNullException argEx)
        {
            _logger.Error(argEx, "Argument null error generating overview report");
            return Results.Problem("An error occurred while generating the report: missing required data.");
        }
        catch (InvalidOperationException invOpEx)
        {
            _logger.Error(invOpEx, "Invalid operation error generating overview report");
            return Results.Problem("An error occurred while generating the report: invalid operation.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating overview report");
            return Results.Problem("An error occurred while generating the report.");
        }
    }
}