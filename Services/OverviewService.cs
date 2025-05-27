using System.Text.Json;
using MongoDB.Driver;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Models;
using Serilog;
using System.IO;
using poupeai_report_service.Utils;
using MongoDB.Bson;

namespace poupeai_report_service.Services;

internal class OverviewService(IMongoDatabase database) : IServiceReport
{
    private readonly Serilog.ILogger _logger = Log.ForContext("Service", nameof(OverviewService));

    private readonly IMongoCollection<OverviewReportModel> _reportsCollection = database.GetCollection<OverviewReportModel>("overviewreports");

    private readonly string output = File.ReadAllText(
        Path.Combine(AppContext.BaseDirectory, "Schemas", "OverviewOutputSchema.json"));

    public async Task<IResult> GenerateReport(IAIService aiService, AIModel model = AIModel.Gemini)
    {
        // Mock data object
        var mockData = new
        {
            AccountId = 124,
            StartDate = "2023-01-01",
            EndDate = "2023-12-31",
            Receitas = new[]
            {
            new { Id = 1, Nome = "Salário", Valor = 5000.00 },
            new { Id = 4, Nome = "Freelance", Valor = 1200.00 }
            },
            Despesas = new[]
            {
            new { Id = 2, Nome = "Aluguel", Valor = 1500.00 },
            new { Id = 5, Nome = "Supermercado", Valor = 800.00 },
            new { Id = 7, Nome = "Transporte", Valor = 300.00 },
            new { Id = 9, Nome = "Lazer", Valor = 400.00 }
            },
            Categorias = new[]
            {
            new { Nome = "Moradia", Saldo = -1500.00 },
            new { Nome = "Alimentação", Saldo = -800.00 },
            new { Nome = "Transporte", Saldo = -300.00 },
            new { Nome = "Lazer", Saldo = -400.00 },
            new { Nome = "Renda", Saldo = 6200.00 }
            }
        };

        try
        {
            var receitaIds = string.Join("", mockData.Receitas.Select(r => r.Id));
            var despesaIds = string.Join("", mockData.Despesas.Select(d => d.Id));
            var transactionHash = $"{mockData.AccountId}-{mockData.StartDate}-{mockData.EndDate}-[{receitaIds}]-[{despesaIds}]";
            var hash = Hash.GenerateFromString(transactionHash);

            var existingReport = await _reportsCollection.Find(r => r.Hash == hash).FirstOrDefaultAsync();
            if (existingReport != null)
            {
                _logger.Debug("Overview report already exists for hash: {Hash}", hash);
                return Results.Ok(existingReport);
            }

            var mockDataJson = JsonSerializer.Serialize<object>(mockData);

            var prompt = $@"
            Gere um relatório geral de finanças.
            Utilize os seguintes dados mockados de exemplo para gerar o relatório (em formato JSON):

            {mockDataJson}

            Gere a análise textual, sugestão, saldo, total de receitas, total de despesas e categorias conforme o schema de output.
            ";

            var result = await aiService.GenerateReportAsync(prompt, output, model);

            if (string.IsNullOrEmpty(result))
            {
                _logger.Error("AI service returned an empty result for overview report generation.");
                return Results.Problem("Failed to generate the report due to empty response from AI service.");
            }

            var deserializedResult = JsonSerializer.Deserialize<OverviewReportModel>(result);

            if (deserializedResult == null)
            {
                _logger.Error("Failed to deserialize AI service response for overview report.");
                return Results.Problem("Failed to generate the report due to deserialization error.");
            }

            deserializedResult.Hash = hash;
            deserializedResult.AccountId = mockData.AccountId;
            deserializedResult.StartDate = DateOnly.Parse(mockData.StartDate);
            deserializedResult.EndDate = DateOnly.Parse(mockData.EndDate);
            deserializedResult.UpdatedAt = DateTime.UtcNow;

            // Save the report to the database
            await _reportsCollection.InsertOneAsync(deserializedResult);

            _logger.Information("Overview report generated and saved successfully with hash: {Hash}", hash);
            return Results.Ok(deserializedResult);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating overview report");
            return Results.Problem("An error occurred while generating the report.");
        }
    }
}