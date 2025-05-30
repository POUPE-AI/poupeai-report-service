using MongoDB.Driver;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.DTOs.Responses.Content;
using poupeai_report_service.Models;
using poupeai_report_service.Services.Bases;

namespace poupeai_report_service.Services;

internal class InsightService(IMongoDatabase database)
    : BaseReportService<InsightReportModel, AIResponse<InsightReportResponse>>(
        database,
        "insightreports",
        "InsightOutputSchema",
        nameof(InsightService)
    )
{
    protected override string BuildPrompt(string dataJson)
    {
        return $@"
            Gere um relatório de insight financeiro personalizado com base nas transações e na mensagem do usuário abaixo (em formato JSON):

            {dataJson}

            Sua resposta deve conter:
            - Uma análise textual geral das transações do usuário, destacando pontos importantes do comportamento financeiro.
            - Uma sugestão prática e objetiva para ajudar o usuário a melhorar sua saúde financeira(Opcional).
            - Uma resposta detalhada e personalizada para a mensagem de insight enviada pelo usuário, levando em conta o contexto das transações.
            ";
    }
}