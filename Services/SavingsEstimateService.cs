using System.Text.Json;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using Serilog;

namespace poupeai_report_service.Services;

internal class SavingsEstimateService
{
    private readonly Serilog.ILogger _logger = Log.ForContext<SavingsEstimateService>();

    /// <summary>
    /// Calcula a economia estimada baseada nas transações, comparando períodos automaticamente.
    /// </summary>
    public async Task<IResult> CalculateEstimatedSavingsAsync(
        TransactionsData transactionsData,
        IAIService aiService,
        AIModel model = AIModel.Gemini,
        string comparisonType = "monthly")
    {
        try
        {
            if (transactionsData?.Transactions == null || !transactionsData.Transactions.Any())
            {
                var emptyDataResponse = new SavingsEstimateResponse
                {
                    EstimatedSavings = 0,
                    SavingsPercentage = 0,
                    Message = "Não é possível calcular economia sem dados de transações.",
                    ComparisonPeriod = comparisonType
                };
                return Results.Ok(emptyDataResponse);
            }

            // Separar as transações por período baseado nas datas
            var (currentPeriodTransactions, previousPeriodTransactions) = SeparateTransactionsByPeriod(
                transactionsData.Transactions, comparisonType);

            if (!previousPeriodTransactions.Any())
            {
                var insufficientDataResponse = new SavingsEstimateResponse
                {
                    EstimatedSavings = 0,
                    SavingsPercentage = 0,
                    Message = $"Dados insuficientes para comparação {comparisonType}. São necessárias transações de pelo menos dois períodos.",
                    ComparisonPeriod = comparisonType
                };
                return Results.Ok(insufficientDataResponse);
            }

            if (!currentPeriodTransactions.Any())
            {
                var noCurrentDataResponse = new SavingsEstimateResponse
                {
                    EstimatedSavings = 0,
                    SavingsPercentage = 0,
                    Message = $"Não há transações no período atual para realizar a comparação {comparisonType}.",
                    ComparisonPeriod = comparisonType
                };
                return Results.Ok(noCurrentDataResponse);
            }

            var analysisData = new
            {
                comparisonType = comparisonType,
                currentPeriod = new
                {
                    transactions = currentPeriodTransactions,
                    totalExpenses = currentPeriodTransactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount)),
                    totalIncome = currentPeriodTransactions.Where(t => t.Amount > 0).Sum(t => t.Amount)
                },
                previousPeriod = new
                {
                    transactions = previousPeriodTransactions,
                    totalExpenses = previousPeriodTransactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount)),
                    totalIncome = previousPeriodTransactions.Where(t => t.Amount > 0).Sum(t => t.Amount)
                }
            };

            var prompt = BuildPrompt(analysisData, comparisonType);
            var dataJson = JsonSerializer.Serialize(analysisData);

            var result = await aiService.GenerateReportAsync(prompt, GetOutputSchema(), model);

            if (string.IsNullOrEmpty(result))
            {
                _logger.Error("AI service returned empty result for savings estimate");
                return Results.Problem("Failed to calculate savings estimate due to empty AI response.");
            }

            try
            {
                var response = JsonSerializer.Deserialize<SavingsEstimateResponse>(result);
                if (response == null)
                {
                    _logger.Error("Failed to deserialize savings estimate response");
                    return Results.Problem("Failed to process savings estimate response.");
                }

                _logger.Information("Savings estimate calculated successfully for {ComparisonType} comparison", comparisonType);
                return Results.Ok(response);
            }
            catch (JsonException ex)
            {
                _logger.Error(ex, "Error deserializing savings estimate response: {Response}", result);
                return Results.Problem("Failed to process savings estimate response.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error calculating savings estimate");
            return Results.Problem("An error occurred while calculating the savings estimate.");
        }
    }

    private static (List<Transaction> current, List<Transaction> previous) SeparateTransactionsByPeriod(
        List<Transaction> transactions, string comparisonType)
    {
        var sortedTransactions = transactions.OrderByDescending(t => t.Date).ToList();
        var latestDate = sortedTransactions.First().Date;

        var (currentStart, currentEnd, previousStart, previousEnd) = comparisonType.ToLower() switch
        {
            "weekly" => GetWeeklyPeriods(latestDate),
            "monthly" => GetMonthlyPeriods(latestDate),
            "yearly" => GetYearlyPeriods(latestDate),
            _ => GetMonthlyPeriods(latestDate) // default to monthly
        };

        var currentPeriod = transactions
            .Where(t => t.Date >= currentStart && t.Date <= currentEnd)
            .ToList();

        var previousPeriod = transactions
            .Where(t => t.Date >= previousStart && t.Date <= previousEnd)
            .ToList();

        return (currentPeriod, previousPeriod);
    }

    private static (DateTime currentStart, DateTime currentEnd, DateTime previousStart, DateTime previousEnd) GetMonthlyPeriods(DateTime referenceDate)
    {
        var currentStart = new DateTime(referenceDate.Year, referenceDate.Month, 1);
        var currentEnd = currentStart.AddMonths(1).AddDays(-1);

        var previousStart = currentStart.AddMonths(-1);
        var previousEnd = currentStart.AddDays(-1);

        return (currentStart, currentEnd, previousStart, previousEnd);
    }

    private static (DateTime currentStart, DateTime currentEnd, DateTime previousStart, DateTime previousEnd) GetWeeklyPeriods(DateTime referenceDate)
    {
        var daysFromMonday = (int)referenceDate.DayOfWeek - (int)DayOfWeek.Monday;
        if (daysFromMonday < 0) daysFromMonday += 7;

        var currentStart = referenceDate.AddDays(-daysFromMonday).Date;
        var currentEnd = currentStart.AddDays(6);

        var previousStart = currentStart.AddDays(-7);
        var previousEnd = currentStart.AddDays(-1);

        return (currentStart, currentEnd, previousStart, previousEnd);
    }

    private static (DateTime currentStart, DateTime currentEnd, DateTime previousStart, DateTime previousEnd) GetYearlyPeriods(DateTime referenceDate)
    {
        var currentStart = new DateTime(referenceDate.Year, 1, 1);
        var currentEnd = new DateTime(referenceDate.Year, 12, 31);

        var previousStart = new DateTime(referenceDate.Year - 1, 1, 1);
        var previousEnd = new DateTime(referenceDate.Year - 1, 12, 31);

        return (currentStart, currentEnd, previousStart, previousEnd);
    }

    private static string BuildPrompt(object analysisData, string comparisonType)
    {
        var dataJson = JsonSerializer.Serialize(analysisData);

        return $@"
            Analise os dados financeiros de dois períodos e calcule uma economia estimada simples.
            
            Dados para análise (formato JSON):
            {dataJson}

            Tipo de comparação: {comparisonType}

            Instruções:
            1. Compare o total de despesas entre o período atual e o anterior
            2. Calcule a diferença (economia se positiva, aumento de gastos se negativa)
            3. Calcule o percentual de mudança
            4. Forneça uma mensagem clara e objetiva sobre o resultado

            Critérios:
            - Use apenas despesas (valores negativos) para o cálculo
            - Ignore receitas neste cálculo
            - Valor positivo = economia (gastou menos que o período anterior)
            - Valor negativo = aumento de gastos (gastou mais que o período anterior)
            - Seja preciso com os cálculos matemáticos
            - Mensagem deve ser clara, concisa e em português

            Retorne apenas o JSON seguindo o schema de output fornecido.
        ";
    }

    private static string GetOutputSchema()
    {
        return JsonSerializer.Serialize(new
        {
            type = "object",
            properties = new
            {
                estimated_savings = new { type = "number" },
                savings_percentage = new { type = "number" },
                message = new { type = "string" },
                comparison_period = new { type = "string" }
            },
            required = new[] { "estimated_savings", "savings_percentage", "message", "comparison_period" }
        });
    }
}
