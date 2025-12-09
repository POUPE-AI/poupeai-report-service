using Serilog.Context;

namespace poupeai_report_service.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Tenta obter o Correlation ID do header da requisição
        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();

        // Se não existir, gera um novo
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Adiciona o Correlation ID ao header de resposta para rastreabilidade
        context.Response.Headers.TryAdd(CorrelationIdHeaderName, correlationId);

        // Adiciona o Correlation ID ao contexto de log do Serilog
        using (LogContext.PushProperty("trace.correlation_id", correlationId))
        {
            // Adiciona também user.id se disponível
            var userId = context.User?.FindFirst("sub")?.Value
                      ?? context.User?.FindFirst("preferred_username")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                using (LogContext.PushProperty("user.id", userId))
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
