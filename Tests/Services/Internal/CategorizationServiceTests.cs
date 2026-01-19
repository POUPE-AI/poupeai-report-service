using FluentAssertions;
using Moq;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.DTOs.Responses.Content;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Services.Internal;
using System.Text.Json;

namespace poupeai_report_service.Tests.Services.Internal;

/// <summary>
/// Testes unitários para o CategorizationService
/// Caso de Teste: RS-UT-009
/// </summary>
public class CategorizationServiceTests
{
    private readonly Mock<IAIService> _mockAIService;
    private readonly CategorizationService _categorizationService;

    public CategorizationServiceTests()
    {
        _mockAIService = new Mock<IAIService>();
        _categorizationService = new CategorizationService();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    Description = "Salário mensal",
                    Amount = 5000m,
                    Type = "Receita",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" },
                new() { Id = "cat-2", Name = "Salário", Description = "Receitas de trabalho" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 200, Message = "Sucesso" },
            Content = new CategorizationResponse
            {
                Categorizations = new Dictionary<string, string>
                {
                    { "Compra no supermercado", "cat-1" },
                    { "Salário mensal", "cat-2" }
                }
            }
        };

        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Gemini))
            .ReturnsAsync(JsonSerializer.Serialize(aiResponse));

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();

        // Verificar usando reflexão que é um OK
        var resultType = result.GetType();
        resultType.Name.Should().Contain("Ok");

        // Obter o valor
        var valueProp = resultType.GetProperty("Value");
        valueProp.Should().NotBeNull();

        var resultValue = valueProp!.GetValue(result);
        resultValue.Should().NotBeNull();

        var headerProp = resultValue!.GetType().GetProperty("Header");
        var contentProp = resultValue.GetType().GetProperty("Content");

        headerProp.Should().NotBeNull();
        contentProp.Should().NotBeNull();

        var header = headerProp!.GetValue(resultValue) as Header;
        var content = contentProp!.GetValue(resultValue) as CategorizationResponse;

        header.Should().NotBeNull();
        header!.Status.Should().Be(200);

        content.Should().NotBeNull();
        content!.Categorizations.Should().HaveCount(2);
        content.Categorizations.Should().ContainKey("Compra no supermercado");
        content.Categorizations.Should().ContainKey("Salário mensal");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithEmptyTransactions_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>(),
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();

        // Verificar usando reflexão que é um BadRequest
        var resultType = result.GetType();
        resultType.Name.Should().Contain("BadRequest");

        // Tentar obter o valor
        var valueProp = resultType.GetProperty("Value");
        if (valueProp != null)
        {
            var resultValue = valueProp.GetValue(result);
            resultValue.Should().NotBeNull();

            var headerProp = resultValue!.GetType().GetProperty("Header");
            if (headerProp != null)
            {
                var header = headerProp.GetValue(resultValue);
                var statusProp = header!.GetType().GetProperty("Status");
                var messageProp = header.GetType().GetProperty("Message");

                var status = (int)statusProp!.GetValue(header)!;
                var message = messageProp!.GetValue(header) as string;

                status.Should().Be(400);
                message.Should().Contain("transação");
            }
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithNullTransactions_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = null!,
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();

        // Verificar usando reflexão que é um BadRequest
        var resultType = result.GetType();
        resultType.Name.Should().Contain("BadRequest");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithEmptyCategories_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>()
        };

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();

        // Verificar usando reflexão que é um BadRequest
        var resultType = result.GetType();
        resultType.Name.Should().Contain("BadRequest");

        // Tentar obter o valor
        var valueProp = resultType.GetProperty("Value");
        if (valueProp != null)
        {
            var resultValue = valueProp.GetValue(result);
            resultValue.Should().NotBeNull();

            var headerProp = resultValue!.GetType().GetProperty("Header");
            if (headerProp != null)
            {
                var header = headerProp.GetValue(resultValue);
                var messageProp = header!.GetType().GetProperty("Message");
                var message = messageProp!.GetValue(header) as string;
                message.Should().Contain("categoria");
            }
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithNullCategories_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = null!
        };

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();

        // Verificar usando reflexão que é um BadRequest
        var resultType = result.GetType();
        resultType.Name.Should().Contain("BadRequest");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithEmptyAIResponse_ShouldReturnProblem()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Gemini))
            .ReturnsAsync(string.Empty);

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();
        var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
        problemResult.Should().NotBeNull();
        problemResult!.ProblemDetails.Detail.Should().Contain("vazia");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithInvalidJSON_ShouldReturnProblem()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Gemini))
            .ReturnsAsync("{ json inválido }");

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();
        var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
        problemResult.Should().NotBeNull();
        problemResult!.ProblemDetails.Detail.Should().Contain("desserialização");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithNon200Status_ShouldReturnAppropriateError()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 400, Message = "Dados insuficientes" },
            Content = null
        };

        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Gemini))
            .ReturnsAsync(JsonSerializer.Serialize(aiResponse));

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();
        // Verificar que não é um resultado OK
        var resultType = result.GetType();
        resultType.Name.Should().NotContain("Ok");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithNullContent_ShouldReturnProblem()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 200, Message = "Sucesso" },
            Content = null
        };

        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Gemini))
            .ReturnsAsync(JsonSerializer.Serialize(aiResponse));

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();
        var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
        problemResult.Should().NotBeNull();
        problemResult!.ProblemDetails.Detail.Should().Contain("nulo");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithNullCategorizations_ShouldReturnProblem()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 200, Message = "Sucesso" },
            Content = new CategorizationResponse
            {
                Categorizations = null!
            }
        };

        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Gemini))
            .ReturnsAsync(JsonSerializer.Serialize(aiResponse));

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();
        var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
        problemResult.Should().NotBeNull();
        problemResult!.ProblemDetails.Detail.Should().Contain("nulo");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithException_ShouldReturnProblem()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Gemini))
            .ThrowsAsync(new Exception("Erro de conexão"));

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        result.Should().NotBeNull();
        var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
        problemResult.Should().NotBeNull();
        problemResult!.ProblemDetails.Detail.Should().Contain("erro");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithDeepseekModel_ShouldCallAIServiceWithCorrectModel()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra no supermercado",
                    Amount = -150.50m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 200, Message = "Sucesso" },
            Content = new CategorizationResponse
            {
                Categorizations = new Dictionary<string, string>
                {
                    { "Compra no supermercado", "cat-1" }
                }
            }
        };

        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Deepseek))
            .ReturnsAsync(JsonSerializer.Serialize(aiResponse));

        // Act
        var result = await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Deepseek
        );

        // Assert
        result.Should().NotBeNull();
        _mockAIService.Verify(
            x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Deepseek),
            Times.Once
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public void BuildPrompt_ShouldContainRequiredInstructions()
    {
        // Arrange
        var dataJson = JsonSerializer.Serialize(new
        {
            transactions = new[] { new { description = "Test", amount = 100, type = "Despesa" } },
            available_categories = new[] { new { id = "cat-1", name = "Test", description = "Test" } }
        });

        // Act
        var method = typeof(CategorizationService).GetMethod("BuildPrompt",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var prompt = (string)method!.Invoke(_categorizationService, new object[] { dataJson })!;

        // Assert
        prompt.Should().NotBeNullOrEmpty();
        prompt.Should().Contain(dataJson);
        prompt.Should().Contain("ESTRITAMENTE");
        prompt.Should().Contain("categorizar");
        prompt.Should().Contain("categoria");
        prompt.Should().Contain("NUNCA invente");
        prompt.Should().Contain("descricao_transacao");
        prompt.Should().Contain("id_categoria");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_ShouldSerializeTransactionsCorrectly()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = 1,
                    Description = "Compra específica",
                    Amount = -250.75m,
                    Type = "Despesa",
                    Date = DateTime.UtcNow
                }
            },
            AvailableCategories = new List<AvailableCategory>
            {
                new() { Id = "cat-1", Name = "Alimentação", Description = "Gastos com comida" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 200, Message = "Sucesso" },
            Content = new CategorizationResponse
            {
                Categorizations = new Dictionary<string, string>
                {
                    { "Compra específica", "cat-1" }
                }
            }
        };

        string? capturedPrompt = null;
        _mockAIService
            .Setup(x => x.GenerateReportAsync(It.IsAny<string>(), It.IsAny<string>(), AIModel.Gemini))
            .Callback<string, string, AIModel>((prompt, schema, model) => capturedPrompt = prompt)
            .ReturnsAsync(JsonSerializer.Serialize(aiResponse));

        // Act
        await _categorizationService.CategorizeTransactionsAsync(
            request,
            _mockAIService.Object,
            AIModel.Gemini
        );

        // Assert
        capturedPrompt.Should().NotBeNull();
        capturedPrompt.Should().Contain("Compra espec");
        capturedPrompt.Should().Contain("-250.75");
        capturedPrompt.Should().Contain("Despesa");
        capturedPrompt.Should().Contain("Alimenta");
    }
}

