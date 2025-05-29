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
}