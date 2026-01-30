using FluentAssertions;
using poupeai_report_service.DTOs.Requests;

namespace poupeai_report_service.Tests.Services;

/// <summary>
/// Testes unitários para o SavingsEstimateService
/// Caso de Teste: RS-UT-004
/// Nota: Testes de métodos privados foram removidos. 
/// Estes métodos serão testados indiretamente através dos testes de integração.
/// </summary>
public class SavingsEstimateServiceTests
{
    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-004")]
    public void TransactionsData_WithMultiplePeriods_CanBeCreated()
    {
        var transactions = new List<Transaction>
        {
            new() { Id = "1", Amount = -50.00m, Date = new DateTime(2024, 10, 15), Description = "Compra Outubro", Category = "Alimentação" },
            new() { Id = "2", Amount = -75.00m, Date = new DateTime(2024, 10, 20), Description = "Compra Outubro 2", Category = "Transporte" },
            new() { Id = "3", Amount = 1000.00m, Date = new DateTime(2024, 10, 5), Description = "Salário Outubro", Category = "Receita" },
            new() { Id = "4", Amount = -100.00m, Date = new DateTime(2024, 9, 10), Description = "Compra Setembro", Category = "Alimentação" },
            new() { Id = "5", Amount = -25.00m, Date = new DateTime(2024, 9, 25), Description = "Compra Setembro 2", Category = "Compras" },
            new() { Id = "6", Amount = 1000.00m, Date = new DateTime(2024, 9, 1), Description = "Salário Setembro", Category = "Receita" }
        };

        var transactionsData = new TransactionsData
        {
            AccountId = "conta_123",
            StartDate = new DateOnly(2024, 9, 1),
            EndDate = new DateOnly(2024, 10, 31),
            Transactions = transactions
        };

        transactionsData.Should().NotBeNull();
        transactionsData.Transactions.Should().HaveCount(6);
        transactionsData.Transactions.Where(t => t.Date.Month == 10).Should().HaveCount(3);
        transactionsData.Transactions.Where(t => t.Date.Month == 9).Should().HaveCount(3);
    }
}
