using System.Text.Json;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;
using Serilog;

namespace poupeai_report_service.Utils;

public static class Tools
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
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidOperationException("JSON string is null or empty");
            }

            // Log first 100 characters for debugging
            var preview = json.Length > 100 ? json.Substring(0, 100) + "..." : json;
            Log.Information($"Attempting to deserialize JSON: {preview}");

            return JsonSerializer.Deserialize<AIResponse<T>>(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize JSON: {ex.Message}. JSON content preview: {(json?.Length > 200 ? json.Substring(0, 200) + "..." : json)}", ex);
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

    public static string CreateErrorResponse(string errorMessage)
    {
        return JsonSerializer.Serialize(new
        {
            header = new
            {
                status = 500,
                message = errorMessage
            },
            content = new
            {
                text_analysis = "Não foi possível gerar a análise devido a um erro no serviço.",
                suggestion = "Tente novamente mais tarde.",
                insight_response = errorMessage,
                total_expense = 0.0,
                total_income = 0.0,
                balance = 0.0,
                categories = Array.Empty<object>(),
                main_expenses = Array.Empty<object>(),
                main_incomes = Array.Empty<object>(),
                category = "Erro",
                total = 0.0,
                average = 0.0,
                trend = "Não disponível",
                main_transactions = Array.Empty<object>(),
                peak_days = Array.Empty<string>()
            }
        });
    }
}