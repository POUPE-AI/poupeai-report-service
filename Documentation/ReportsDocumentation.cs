using Microsoft.OpenApi.Models;

namespace poupeai_report_service.Documentation
{
    internal static class ReportsDocumentation
    {
        private static OpenApiOperation AddBearerSecurity(OpenApiOperation operation)
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }, []
                }
            });
            return operation;
        }

        internal static Func<OpenApiOperation, OpenApiOperation> GetReportsOverviewOperation()
        {
            return operation =>
            {
                operation.Summary = "Gerar relatório financeiro geral consolidado";
                operation.Description = "Esse endpoint gera e retorna um relatório financeiro completo para o usuário autenticado, " +
                                        "baseado em suas receitas e despesas registradas. A resposta inclui um resumo textual, valores " +
                                        "totais, previsões financeiras para os próximos meses e sugestões automáticas fornecidas por uma IA.";
                operation = AddBearerSecurity(operation);

                return operation;
            };
        }

        internal static Func<OpenApiOperation, OpenApiOperation> GetReportsExpenseOperation()
        {
            return operation =>
            {
                operation.Summary = "Gerar relatório de despesa";
                operation.Description = "Esse endpoint gera e retorna um relatório financeiro das despesas do usuário autenticado. " +
                                        "A resposta inclui a soma das despesas, lista de com as 5 categorias que mais têm despesas, " +
                                        "tendência para os próximos meses e sugestões geradas pela IA.";
                operation = AddBearerSecurity(operation);

                return operation;
            };
        }

        internal static Func<OpenApiOperation, OpenApiOperation> GetReportsIncomeOperation()
        {
            return operation =>
            {
                operation.Summary = "Gerar relatório de receita";
                operation.Description = "Esse endpoint gera e retorna um relatório financeiro das receitas do usuário autenticado. " +
                                        "A resposta inclui a soma das receitas, o período do mês que mais tem receita, tendência " +
                                        "para os próximos meses e sugestões geradas pela IA.";
                operation = AddBearerSecurity(operation);

                return operation;
            };
        }

        internal static Func<OpenApiOperation, OpenApiOperation> GetReportsCategoryOperation()
        {
            return operation =>
            {
                operation.Summary = "Gerar relatório financeiro por categoria";
                operation.Description = "Esse endpoint gera e retorna um relatório financeiro por categoria para o usuário autenticado. " +
                                        "A resposta inclui a soma das receitas e despesas, o saldo total, a tendência para os próximos meses " +
                                        "e sugestões geradas pela IA.";
                operation = AddBearerSecurity(operation);

                return operation;
            };
        }

        internal static Func<OpenApiOperation, OpenApiOperation> GetReportsInsightsOperation()
        {
            return operation =>
            {
                operation.Summary = "Gerar relatório com insights personalizadas";
                operation = AddBearerSecurity(operation);
                return operation;
            };
        }
    }
}
