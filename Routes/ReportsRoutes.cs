using Microsoft.AspNetCore.Mvc;
using poupeai_report_service.Documentation;
using poupeai_report_service.DTOs.Requests;

namespace poupeai_report_service.Routes
{
    public static class ReportsRoutes
    {
        public static void MapReportsRoutes(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/reports").WithTags("Reports");

            // TODO: Implement overview report generation logic
            group.MapGet("/overview", ([AsParameters] PeriodFilters filters) => "Overview of reports")
                .WithOpenApi(ReportsDocumentation.GetReportsOverviewOperation());

            // TODO: Implement expense report generation logic
            group.MapGet("/expense", ([AsParameters] PeriodFilters filters) => "Expense report")
                .WithOpenApi(ReportsDocumentation.GetReportsExpenseOperation());

            // TODO: Implement income report generation logic
            group.MapGet("/income", ([AsParameters] PeriodFilters filters) => "Income report")
                .WithOpenApi(ReportsDocumentation.GetReportsIncomeOperation());

            // TODO: Implement category report generation logic
            group.MapGet("/category/{categoryId}", (int categoryId, [AsParameters] PeriodFilters filters) => $"Category report for category {categoryId}")
                .WithOpenApi(ReportsDocumentation.GetReportsCategoryOperation());
                
            // TODO: Implement insights report generation logic
            group.MapPost("/insights", ([AsParameters] PeriodFilters filters, [FromBody] InsightRequest insight) => "Insights report")
                .WithOpenApi(ReportsDocumentation.GetReportsInsightsOperation());
        }
    }
}
