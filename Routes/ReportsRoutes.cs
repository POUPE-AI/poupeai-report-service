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

            group.MapPost("/overview", OverviewReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsOverviewOperation());

            group.MapPost("/expense", ExpenseReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsExpenseOperation());

            group.MapPost("/income", IncomeReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsIncomeOperation());

            group.MapPost("/category", CategoryReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsCategoryOperation());

            group.MapPost("/insights", InsightReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsInsightsOperation());
        }

        private static async Task<IResult> OverviewReportOperation(
            [FromBody] TransactionsData transactionsData,
            [FromServices] OverviewService overviewService,
            [FromServices] IAIService aiService,
            [FromQuery] string model = "gemini")
        {
            try
            {
                if (transactionsData == null || transactionsData.Transactions == null || transactionsData.Transactions.Count == 0)
                {
                    return Results.BadRequest("No transactions data provided.");
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
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating expense report");
                return Results.Problem($"An error occurred while generating the expense report: {ex.Message}");
            }
        }

        private static async Task<IResult> ExpenseReportOperation(
            [FromBody] TransactionsData transactionsData,
            [FromServices] ExpenseService expenseService,
            [FromServices] IAIService aiService,
            [FromQuery] string model = "gemini"
        )
        {
            try
            {
                if (transactionsData == null || transactionsData.Transactions == null || transactionsData.Transactions.Count == 0)
                {
                    return Results.BadRequest("No transactions data provided.");
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
            [FromBody] TransactionsData transactionsData,
            [FromServices] IncomeService incomeService,
            [FromServices] IAIService aiService,
            [FromQuery] string model = "gemini"
        )
        {
            try
            {
                if (transactionsData == null || transactionsData.Transactions == null || transactionsData.Transactions.Count == 0)
                {
                    return Results.BadRequest("No transactions data provided.");
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
            [FromBody] CategoryReportRequest categoryReportRequest,
            [FromServices] CategoryService categoryService,
            [FromServices] IAIService aiService,
            [FromQuery] string model = "gemini"
        )
        {
            try
            {
                var aiModel = Tools.StringToModel(model);

                return await categoryService.GenerateReportAsync(
                                categoryReportRequest,
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
