using Microsoft.AspNetCore.Mvc;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Services.Internal;
using poupeai_report_service.Utils;

namespace poupeai_report_service.Routes;

public static class CategorizationRoutes
{
    public static void MapCategorizationRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/internal/categorization").WithTags("Categorization");

        group.MapPost("/predict", PredictCategoriesOperation)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Categoriza descrições usando IA";
                operation.Description = "Recebe uma lista de descrições e categorias, e retorna a categoria mais apropriada para cada descrição.";
                return operation;
            });
    }

    private static async Task<IResult> PredictCategoriesOperation(
        [FromBody] CategorizationRequest request,
        [FromServices] CategorizationService categorizationService,
        [FromServices] IAIService aiService,
        [FromQuery] string model = "gemini")
    {
        var aiModel = Tools.StringToModel(model);
        return await categorizationService.CategorizeTransactionsAsync(request, aiService, aiModel);
    }
}
