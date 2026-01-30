using FluentAssertions;
using MongoDB.Driver;
using Moq;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Services;
using System.Text.Json;

namespace poupeai_report_service.Tests.Services;

/// <summary>
/// Testes unitários para o ExpenseService
/// Caso de Teste: RS-UT-005
/// </summary>
public class ExpenseServiceTests
{
    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-005")]
    public void BuildPrompt_WithTransactionsData_ShouldContainDataJsonAndInstructions()
    {
        var mockDatabase = new Mock<IMongoDatabase>();
        var expenseService = new ExpenseService(mockDatabase.Object);

        var transactionsData = new TransactionsData
        {
            AccountId = "conta_123",
            StartDate = new DateOnly(2024, 10, 1),
            EndDate = new DateOnly(2024, 10, 31),
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = "1",
                    Amount = -150.50m,
                    Date = new DateTime(2024, 10, 15),
                    Description = "Supermercado",
                    Category = "Alimentação",
                    Type = "Despesa"
                },
                new()
                {
                    Id = "2",
                    Amount = -50.00m,
                    Date = new DateTime(2024, 10, 20),
                    Description = "Gasolina",
                    Category = "Transporte",
                    Type = "Despesa"
                }
            }
        };

        var dataJson = JsonSerializer.Serialize(transactionsData);

        var method = typeof(ExpenseService).GetMethod("BuildPrompt",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var prompt = (string)method!.Invoke(expenseService, new object[] { dataJson })!;

        prompt.Should().NotBeNullOrEmpty();
        prompt.Should().Contain(dataJson);
        prompt.Should().Contain("despesa");
        prompt.Should().Contain("transações");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-005")]
    public void BuildPrompt_ShouldIncludeExpenseSpecificInstructions()
    {
        var mockDatabase = new Mock<IMongoDatabase>();
        var expenseService = new ExpenseService(mockDatabase.Object);

        var dataJson = "{}";

        var method = typeof(ExpenseService).GetMethod("BuildPrompt",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var prompt = (string)method!.Invoke(expenseService, new object[] { dataJson })!;

        prompt.Should().NotBeNullOrEmpty();
        var lowercasePrompt = prompt.ToLower();
        lowercasePrompt.Should().Contain("desp");
    }
}
