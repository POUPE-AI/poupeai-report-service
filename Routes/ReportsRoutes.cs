using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using poupeai_report_service.Documentation;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.DTOs.Responses.Content;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Models;
using poupeai_report_service.Services;
using poupeai_report_service.Utils;
using Serilog;

namespace poupeai_report_service.Routes
{
    public static class ReportsRoutes
    {
        public static void MapReportsRoutes(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/reports").WithTags("Reports").RequireAuthorization();

            group.MapGet("/overview", OverviewReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsOverviewOperation());

            group.MapGet("/expense", ExpenseReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsExpenseOperation());

            group.MapGet("/income", IncomeReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsIncomeOperation());

            group.MapGet("/category", CategoryReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsCategoryOperation());

            group.MapGet("/insights", InsightReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsInsightsOperation());
        }

        private static async Task<IResult> OverviewReportOperation(
            ClaimsPrincipal user,
            [FromServices] FinancesService financeService,
            [FromServices] OverviewService overviewService,
            [FromServices] IAIService aiService,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate,
            [FromQuery] string model = "gemini")
        {
            try
            {
                // Validar datas
                var sDate = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
                var eDate = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

                // Obter transações do Django
                var transactions = await financeService.GetTransactionsAsync(sDate, eDate);

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    Log.Warning("User ID not found in claims for overview report generation.");
                    return Results.Unauthorized();
                }

                // Mapear para TransactionsData DTO se necessário, ou ajustar GenerateReport para receber List<Transaction>
                var transactionsData = new TransactionsData
                {
                    Transactions = transactions,
                    StartDate = sDate,
                    EndDate = eDate,
                    AccountId = userId
                };


                if (transactionsData.Transactions == null || transactionsData.Transactions.Count == 0)
                {
                    return Results.NotFound("No transactions found for the specified period.");
                }

                var aiModel = Tools.StringToModel(model);

                return await overviewService.GenerateReportAsync(
                                transactionsData,
                                aiService,
                                aiModel,
                                Tools.DeserializeJson<OverviewReportResponse>,
                                response => OverviewReportModel.CreateFromDTO(response.Content)
                            );
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Warning(ex, "Unauthorized access attempt to Django Finance Service.");
                return Results.Unauthorized(); // Retorna 401 se não tiver token para chamar o Django
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "Error calling Finance Service.");
                return Results.Problem($"Failed to retrieve transactions from finance service: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating overview report");
                return Results.Problem($"An error occurred while generating the overview report: {ex.Message}");
            }
        }

        private static async Task<IResult> ExpenseReportOperation(
            ClaimsPrincipal user,
            [FromServices] FinancesService financeService,
            [FromServices] ExpenseService expenseService,
            [FromServices] IAIService aiService,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate,
            [FromQuery] string model = "gemini"
        )
        {
            try
            {
                var sDate = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
                var eDate = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

                var transactions = await financeService.GetTransactionsAsync(sDate, eDate);

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    Log.Warning("User ID not found in claims for expense report generation.");
                    return Results.Unauthorized();
                }

                var transactionsData = new TransactionsData
                {
                    Transactions = transactions,
                    StartDate = sDate,
                    EndDate = eDate,
                    AccountId = userId
                };

                if (transactionsData.Transactions == null || transactionsData.Transactions.Count == 0)
                {
                    return Results.NotFound("No transactions found for the specified period.");
                }

                var aiModel = Tools.StringToModel(model);

                return await expenseService.GenerateReportAsync(
                                transactionsData,
                                aiService,
                                aiModel,
                                Tools.DeserializeJson<ExpenseReportResponse>,
                                response => ExpenseReportModel.CreateFromDTO(response.Content)
                            );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating expense report");
                return Results.Problem($"An error occurred while generating the expense report: {ex.Message}");
            }
        }

        private static async Task<IResult> IncomeReportOperation(
            ClaimsPrincipal user,
            [FromServices] FinancesService financeService,
            [FromServices] IncomeService incomeService,
            [FromServices] IAIService aiService,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate,
            [FromQuery] string model = "gemini"
        )
        {
            try
            {
                var sDate = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
                var eDate = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

                var transactions = await financeService.GetTransactionsAsync(sDate, eDate);

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    Log.Warning("User ID not found in claims for income report generation.");
                    return Results.Unauthorized();
                }

                var transactionsData = new TransactionsData
                {
                    Transactions = transactions,
                    StartDate = sDate,
                    EndDate = eDate,
                    AccountId = userId
                };

                if (transactionsData.Transactions == null || transactionsData.Transactions.Count == 0)
                {
                    return Results.NotFound("No transactions found for the specified period.");
                }

                var aiModel = Tools.StringToModel(model);

                return await incomeService.GenerateReportAsync(
                                transactionsData,
                                aiService,
                                aiModel,
                                Tools.DeserializeJson<IncomeReportResponse>,
                                response => IncomeReportModel.CreateFromDTO(response.Content)
                            );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating income report");
                return Results.Problem($"An error occurred while generating the income report: {ex.Message}");
            }
        }


        private static async Task<IResult> CategoryReportOperation(
            ClaimsPrincipal user,
            [FromServices] FinancesService financeService,
            [FromServices] CategoryService categoryService,
            [FromServices] IAIService aiService,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate,
            [FromQuery] string model = "gemini"
        )
        {
            try
            {
                var sDate = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
                var eDate = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

                var transactions = await financeService.GetTransactionsAsync(sDate, eDate);

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    Log.Warning("User ID not found in claims for category report generation.");
                    return Results.Unauthorized();
                }

                var transactionsData = new TransactionsData
                {
                    Transactions = transactions,
                    StartDate = sDate,
                    EndDate = eDate,
                    AccountId = userId
                };

                if (transactionsData.Transactions == null || transactionsData.Transactions.Count == 0)
                {
                    return Results.NotFound("No transactions found for the specified period.");
                }

                var aiModel = Tools.StringToModel(model);

                return await categoryService.GenerateReportAsync(
                                transactionsData,
                                aiService,
                                aiModel,
                                Tools.DeserializeJson<CategoryReportResponse>,
                                response => CategoryReportModel.CreateFromDTO(response.Content)
                            );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating category report");
                return Results.Problem($"An error occurred while generating the category report: {ex.Message}");
            }

        }


        private static async Task<IResult> InsightReportOperation(
            [FromBody] InsightReportRequest insightReportRequest,
            [FromServices] InsightService insightService,
            [FromServices] IAIService aiService,
            [FromQuery] string model = "gemini"
            )
        {
            try
            {
                var aiModel = Tools.StringToModel(model);

                return await insightService.GenerateReportAsync(
                                insightReportRequest,
                                aiService,
                                aiModel,
                                Tools.DeserializeJson<InsightReportResponse>,
                                response => InsightReportModel.CreateFromDTO(response.Content)
                            );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating insight report");
                return Results.Problem($"An error occurred while generating the insight report: {ex.Message}");
            }
        }
    }
}
