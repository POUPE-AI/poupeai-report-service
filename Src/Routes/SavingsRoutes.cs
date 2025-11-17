using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Services;
using poupeai_report_service.Utils;
using Serilog;

namespace poupeai_report_service.Routes;

/// <summary>
/// Rotas para funcionalidades relacionadas a economia e comparação de gastos.
/// </summary>
public static class SavingsRoutes
{
    public static void MapSavingsRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/savings").WithTags("Savings");

        group.MapPost("/estimate", SavingsEstimateOperation)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Calcular economia estimada",
                Description = "Calcula a economia estimada comparando o período atual com o período anterior com base nas datas das transações. " +
                     "Suporta comparações mensais, semanais e anuais. " +
                     "Parâmetros de consulta: model (gemini|deepseek, padrão: gemini), comparisonType (monthly|weekly|yearly, padrão: monthly)",
            });
    }

    private static async Task<IResult> SavingsEstimateOperation(
        [FromBody] TransactionsData transactionsData,
        [FromServices] SavingsEstimateService savingsService,
        [FromServices] IAIService aiService,
        [FromQuery] string model = "gemini",
        [FromQuery] string comparisonType = "monthly"
    )
    {
        try
        {
            if (transactionsData?.Transactions == null || !transactionsData.Transactions.Any())
            {
                return Results.BadRequest(new
                {
                    error = "Transaction data is required",
                    message = "Please provide a list of transactions with dates from multiple periods for comparison"
                });
            }

            var aiModel = Tools.StringToModel(model);

            return await savingsService.CalculateEstimatedSavingsAsync(
                transactionsData,
                aiService,
                aiModel,
                comparisonType);
        }
        catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "model")
        {
            Log.Error(ex, "Invalid AI model specified: {Model}", model);
            return Results.BadRequest(new
            {
                error = "Invalid AI model",
                message = $"Model '{model}' is not supported. Use 'gemini' or 'deepseek'."
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error calculating savings estimate");
            return Results.Problem($"An error occurred while calculating the savings estimate: {ex.Message}");
        }
    }
}
