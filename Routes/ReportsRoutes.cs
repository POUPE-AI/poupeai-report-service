using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using poupeai_report_service.Documentation;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Services;

namespace poupeai_report_service.Routes
{
    public static class ReportsRoutes
    {
        public static void MapReportsRoutes(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/reports").WithTags("Reports");

            group.MapPost("/api/v1/overview", OverviewReportOperation)
                .WithOpenApi(ReportsDocumentation.GetReportsOverviewOperation());

            // TODO: Implement expense report generation logic
            group.MapGet("/expense", () => "Expense report")
                .WithOpenApi(ReportsDocumentation.GetReportsExpenseOperation());

            // TODO: Implement income report generation logic
            group.MapGet("/income", () => "Income report")
                .WithOpenApi(ReportsDocumentation.GetReportsIncomeOperation());

            // TODO: Implement category report generation logic
            group.MapGet("/category/{categoryId}", (int categoryId) => $"Category report for category {categoryId}")
                .WithOpenApi(ReportsDocumentation.GetReportsCategoryOperation());

            // TODO: Implement insights report generation logic
            group.MapPost("/insights", ([FromBody] InsightRequest insight) => "Insights report")
                .WithOpenApi(ReportsDocumentation.GetReportsInsightsOperation());
        }

        private static async Task<IResult> OverviewReportOperation(
                    [FromServices] IServiceReport overviewService,
                    [FromServices] IAIService aiService,
                    [FromQuery] AIModel model)
                {
                    return await overviewService.GenerateReport(aiService, model);
                }
        }
    }
