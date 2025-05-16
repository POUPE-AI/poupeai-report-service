
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace poupeai_report_service.DTOs.Requests
{
    /// <summary>
    /// Representa os filtros de período para as requisições de relatórios.
    /// </summary>
    
    [Description("Representa os filtros de período para as requisições de relatórios.")]
    public record PeriodFilters
    {
        /// <summary>
        /// Data inicial (formato: YYYY-MM-DD)
        /// </summary>
        [Description("Data inicial do relatório (formato: YYYY-MM-DD)")]
        public DateOnly? StartDate { get; init; }

        /// <summary>
        /// Data final no formato YYYY-MM-DD (opcional)
        /// </summary>
        [Description("Data final do relatório (formato: YYYY-MM-DD)")]
        public DateOnly? EndDate { get; init; }
    }
}
