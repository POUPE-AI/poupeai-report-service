using FluentAssertions;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Utils;

namespace poupeai_report_service.Tests.Utils;

/// <summary>
/// Testes unitários para a classe Hash
/// Caso de Teste: RS-UT-001
/// </summary>
public class HashTests
{
    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-001")]
    public void GenerateFromTransaction_WithIdenticalObjects_ShouldGenerateIdenticalHashes()
    {
        var transaction1 = new Transaction
        {
            Id = "1",
            Amount = 100.50m,
            Date = new DateTime(2024, 10, 15),
            Description = "Compra no Supermercado",
            Category = "Alimentação"
        };

        var transaction2 = new Transaction
        {
            Id = "1",
            Amount = 100.50m,
            Date = new DateTime(2024, 10, 15),
            Description = "Compra no Supermercado",
            Category = "Alimentação"
        };

        var transactionsData1 = new TransactionsData
        {
            AccountId = "conta_123",
            StartDate = new DateOnly(2024, 10, 1),
            EndDate = new DateOnly(2024, 10, 31),
            Transactions = new List<Transaction> { transaction1 }
        };

        var transactionsData2 = new TransactionsData
        {
            AccountId = "conta_123",
            StartDate = new DateOnly(2024, 10, 1),
            EndDate = new DateOnly(2024, 10, 31),
            Transactions = new List<Transaction> { transaction2 }
        };

        var hash1 = Hash.GenerateFromTransaction(transactionsData1, AIModel.Gemini);
        var hash2 = Hash.GenerateFromTransaction(transactionsData2, AIModel.Gemini);

        hash1.Should().Be(hash2);
        hash1.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-001")]
    public void GenerateFromTransaction_WithDifferentTransactionIds_ShouldGenerateDifferentHashes()
    {
        var transactionsData1 = new TransactionsData
        {
            AccountId = "conta_123",
            StartDate = new DateOnly(2024, 10, 1),
            EndDate = new DateOnly(2024, 10, 31),
            Transactions = new List<Transaction>
            {
                new() { Id = "1", Amount = 100.50m, Date = DateTime.Now, Description = "Transação 1", Category = "Alimentação" }
            }
        };

        var transactionsData2 = new TransactionsData
        {
            AccountId = "conta_123",
            StartDate = new DateOnly(2024, 10, 1),
            EndDate = new DateOnly(2024, 10, 31),
            Transactions = new List<Transaction>
            {
                new() { Id = "2", Amount = 100.50m, Date = DateTime.Now, Description = "Transação 2", Category = "Alimentação" }
            }
        };

        var hash1 = Hash.GenerateFromTransaction(transactionsData1);
        var hash2 = Hash.GenerateFromTransaction(transactionsData2);

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-001")]
    public void GenerateFromTransaction_WithDifferentModels_ShouldGenerateDifferentHashes()
    {
        var transactionsData = new TransactionsData
        {
            AccountId = "conta_123",
            StartDate = new DateOnly(2024, 10, 1),
            EndDate = new DateOnly(2024, 10, 31),
            Transactions = new List<Transaction>
            {
                new() { Id = "1", Amount = 100.50m, Date = DateTime.Now, Description = "Transação 1", Category = "Alimentação" }
            }
        };

        var hashGemini = Hash.GenerateFromTransaction(transactionsData, AIModel.Gemini);
        var hashDeepseek = Hash.GenerateFromTransaction(transactionsData, AIModel.Deepseek);

        hashGemini.Should().NotBe(hashDeepseek);
    }
}
