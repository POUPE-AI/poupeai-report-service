using System.Text.Json;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;

namespace poupeai_report_service.Utils;

internal static class Tools
{
    private static string GetSchemaPath(string schemaName)
    {
        return Path.Combine(AppContext.BaseDirectory, "Schemas", $"{schemaName}.json");
    }

    public static string ReadSchema(string schemaName)
    {
        var path = GetSchemaPath(schemaName);
        return File.ReadAllText(path);
    }

    public static string ModelToString(AIModel model)
    {
        return model switch
        {
            AIModel.Gemini => "gemini",
            AIModel.Deepseek => "deepseek",
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Invalid AI model specified.")
        };
    }

    public static AIModel StringToModel(string model)
    {
        return model.ToLowerInvariant() switch
        {
            "gemini" => AIModel.Gemini,
            "deepseek" => AIModel.Deepseek,
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Invalid AI model specified.")
        };
    }

    public static AIResponse<T>? DeserializeJson<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<AIResponse<T>>(json);;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to deserialize JSON: {ex.Message}", ex);
        }
    }

    public static IResult BuildResultFromHeader(dynamic header, int status)
    {
        return status switch
        {
            200 => Results.Ok(new { Header = header }),
            204 => Results.NoContent(),
            400 => Results.BadRequest(new { Header = header }),
            401 => Results.Unauthorized(),
            403 => Results.Forbid(),
            404 => Results.NotFound(new { Header = header }),
            500 => Results.Problem(header.Message, statusCode: 500),
            _ => Results.Problem("An unexpected error occurred.", statusCode: status)
        };
    }
}