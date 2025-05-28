using System.Text.Json;
using MongoDB.Driver;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Models;
using Serilog;

internal class ExpenseService(IMongoDatabase database) : IServiceReport
{
    private readonly Serilog.ILogger _logger = Log.ForContext("Service", nameof(ExpenseService));

    private readonly IMongoCollection<ExpenseReportModel> _reportsCollection = database.GetCollection<ExpenseReportModel>("expensereports");

    private readonly string Output = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Schemas", "ExpenseOutputSchema.json"));

    public async Task<IResult> GenerateReport(TransactionsData transactionsData, IAIService aiService, AIModel model = AIModel.Gemini)
    {
        // TODO: Implement the logic to get reports from MongoDB

        // TODO: Implement the logic to get expenses from POST

        // TODO: Define the prompt for generating an expense report
        var prompt = $@"
                        Gere um relatório de despesas.
                        Utilize os seguintes dados mockados de exemplo para gerar o relatório:
                        
                        Despesas:
                        - Aluguel: 1500.00
                        - Supermercado: 800.00
                        - Transporte: 300.00
                        - Lazer: 400.00

                        Gere a análise textual, sugestão, saldo, total de despesas e categorias conforme o schema de output.
                        ";

        try
        {
            var result = await aiService.GenerateReportAsync(prompt, Output, model);
            return Results.Ok(JsonSerializer.Deserialize<object>(result));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating overview report");
            return Results.Problem("An error occurred while generating the report.");
        }
    }
}