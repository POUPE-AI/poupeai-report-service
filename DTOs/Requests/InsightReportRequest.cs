using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace poupeai_report_service.DTOs.Requests
{
    /// <summary>
    /// Representa a mensagem de insight que será enviada para o serviço de relatórios.
    /// </summary>
    /// <param name="InsightText"></param>
    internal record InsightReportRequest : TransactionsData
    {
        /// <summary>
        /// Texto do insight a ser enviado.
        /// </summary>
        [Description("Texto do insight a ser enviado.")]
        [JsonPropertyName("insight_text")]
        public required string InsightText { get; init; }
    }

}
