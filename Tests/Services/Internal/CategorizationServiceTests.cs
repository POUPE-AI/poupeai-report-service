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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM",
                "IFOOD *PAGAMENTO"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" },
                new() { Id = "uuid-2222", Name = "Alimentação" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 200, Message = "Sucesso" },
            Content = new CategorizationResponse
            {
                Categorizations = new List<Categorization>
                {
                    new() { Description = "UBER *VIAGEM", CategoryId = "uuid-1111" },
                    new() { Description = "IFOOD *PAGAMENTO", CategoryId = "uuid-2222" }
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
        content.Categorizations[0].Description.Should().Be("UBER *VIAGEM");
        content.Categorizations[0].CategoryId.Should().Be("uuid-1111");
        content.Categorizations[1].Description.Should().Be("IFOOD *PAGAMENTO");
        content.Categorizations[1].CategoryId.Should().Be("uuid-2222");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithEmptyDescriptions_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Descriptions = new List<string>(),
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
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
                message.Should().Contain("descrição");
            }
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_WithNullDescriptions_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Descriptions = null!,
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = new List<UserCategory>()
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = null!
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
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
            Descriptions = new List<string>
            {
                "UBER *VIAGEM"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-1111", Name = "Transporte" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 200, Message = "Sucesso" },
            Content = new CategorizationResponse
            {
                Categorizations = new List<Categorization>
                {
                    new() { Description = "UBER *VIAGEM", CategoryId = "uuid-1111" }
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
            descriptions = new[] { "UBER *VIAGEM", "IFOOD *PAGAMENTO" },
            user_categories = new[] { new { id = "uuid-1111", name = "Transporte" } }
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
        prompt.Should().Contain("description");
        prompt.Should().Contain("category_id");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-009")]
    public async Task CategorizeTransactionsAsync_ShouldSerializeDescriptionsCorrectly()
    {
        // Arrange
        var request = new CategorizationRequest
        {
            Descriptions = new List<string>
            {
                "POSTO IPIRANGA",
                "NETFLIX.COM"
            },
            UserCategories = new List<UserCategory>
            {
                new() { Id = "uuid-3333", Name = "Lazer" },
                new() { Id = "uuid-5555", Name = "Combustível" }
            }
        };

        var aiResponse = new AIResponse<CategorizationResponse>
        {
            Header = new Header { Status = 200, Message = "Sucesso" },
            Content = new CategorizationResponse
            {
                Categorizations = new List<Categorization>
                {
                    new() { Description = "POSTO IPIRANGA", CategoryId = "uuid-5555" },
                    new() { Description = "NETFLIX.COM", CategoryId = "uuid-3333" }
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
        capturedPrompt.Should().Contain("POSTO IPIRANGA");
        capturedPrompt.Should().Contain("NETFLIX.COM");
        capturedPrompt.Should().Contain("Lazer");
        capturedPrompt.Should().Contain("Combust");
    }
}

