using System.Text.Json;
using MongoDB.Driver;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using Serilog;

internal class ExpenseService(IMongoDatabase database) : IServiceReport
{
    private readonly Serilog.ILogger _logger = Log.ForContext("Service", nameof(ExpenseService));

    //private readonly IMongoCollection<ExpenseReport> _reportsCollection = database.GetCollection<ExpenseReport>("expensereports");

    public async Task<IResult> GenerateReport(IAIService aiService, AIModel model = AIModel.Gemini)
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

        // TODO: Define the output schema for the expense report
        var output = @"
                    {
                        ""type"": ""object"",
                        ""properties"": {
                            ""textAnalysis"": { ""type"": ""string"" },
                            ""suggestion"": { ""type"": ""string"" },
                            ""balance"": { ""type"": ""number"" },
                            ""totalExpense"": { ""type"": ""number"" },
                            ""categories"": {
                                ""type"": ""array"",
                                ""items"": {
                                    ""type"": ""object"",
                                    ""properties"": {
                                        ""name"": { ""type"": ""string"" },
                                        ""balance"": { ""type"": ""number"" }
                                    },
                                    ""required"": [ ""name"", ""balance"" ]
                                }
                            }
                        },
                        ""required"": [
                            ""textAnalysis"",
                            ""suggestion"",
                            ""balance"",
                            ""totalExpense"",
                            ""categories""
                        ]
                    }
                    ";

        try
        {
            var result = await aiService.GenerateReportAsync(prompt, output, model);
            return Results.Ok(JsonSerializer.Deserialize<object>(result));
        }catch (Exception ex)
        {
            _logger.Error(ex, "Error generating overview report");
            return Results.Problem("An error occurred while generating the report.");
        }
    }
}