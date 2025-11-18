using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;
using poupeai_report_service.Services.AI;
using System.Net;
using System.Text.Json;

namespace poupeai_report_service.Tests.Services.AI;

/// <summary>
/// Testes unitários para GeminiAIService
/// Caso de Teste: RS-UT-008
/// </summary>
public class GeminiAIServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<GeminiAIService>> _mockLogger;

    public GeminiAIServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["GeminiAI:ApiKey"]).Returns("chave-api-teste");
        _mockLogger = new Mock<ILogger<GeminiAIService>>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-008")]
    public async Task GenerateReportAsync_WithRetryOn429_ShouldRetryAndSucceed()
    {
        var handler = new Mock<HttpMessageHandler>();
        var successResponse = new GeminiResponse
        {
            candidates = new[]
            {
                new GeminiResponse.Candidate
                {
                    content = new GeminiResponse.Content
                    {
                        parts = new[]
                        {
                            new GeminiResponse.Part
                            {
                                text = @"{""header"":{""status"":200,""message"":""Sucesso""},""content"":{""summary"":""Relatório gerado""}}"
                            }
                        }
                    }
                }
            }
        };

        var callCount = 0;
        handler.SetupAnyRequest()
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Content = new StringContent("Muitas requisições")
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(successResponse))
                };
            });

        var httpClient = handler.CreateClient();
        var service = new GeminiAIService(_mockConfiguration.Object, httpClient, _mockLogger.Object);

        var prompt = "Gerar relatório financeiro";
        var outputSchema = JsonSerializer.Serialize(new { type = "object" });

        var result = await service.GenerateReportAsync(prompt, outputSchema, AIModel.Gemini);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Relatório gerado");
        callCount.Should().Be(2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-008")]
    public async Task GenerateReportAsync_WithRetryOnServiceUnavailable_ShouldRetryAndSucceed()
    {
        var handler = new Mock<HttpMessageHandler>();
        var successResponse = new GeminiResponse
        {
            candidates = new[]
            {
                new GeminiResponse.Candidate
                {
                    content = new GeminiResponse.Content
                    {
                        parts = new[]
                        {
                            new GeminiResponse.Part
                            {
                                text = @"{""dados"":""teste""}"
                            }
                        }
                    }
                }
            }
        };

        var callCount = 0;
        handler.SetupAnyRequest()
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent("Serviço indisponível")
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(successResponse))
                };
            });

        var httpClient = handler.CreateClient();
        var service = new GeminiAIService(_mockConfiguration.Object, httpClient, _mockLogger.Object);

        var prompt = "Gerar relatório";
        var outputSchema = JsonSerializer.Serialize(new { type = "object" });

        var result = await service.GenerateReportAsync(prompt, outputSchema, AIModel.Gemini);

        result.Should().NotBeNullOrEmpty();
        callCount.Should().Be(2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GenerateReportAsync_WithSuccess_ShouldReturnResponse()
    {
        var handler = new Mock<HttpMessageHandler>();
        var successResponse = new GeminiResponse
        {
            candidates = new[]
            {
                new GeminiResponse.Candidate
                {
                    content = new GeminiResponse.Content
                    {
                        parts = new[]
                        {
                            new GeminiResponse.Part
                            {
                                text = @"{""header"":{""status"":200},""content"":{""summary"":""Relatório gerado com sucesso""}}"
                            }
                        }
                    }
                }
            }
        };

        handler.SetupRequest(HttpMethod.Post, req => req.RequestUri!.ToString().Contains("generativelanguage.googleapis.com"))
            .ReturnsJsonResponse(HttpStatusCode.OK, successResponse);

        var httpClient = handler.CreateClient();
        var service = new GeminiAIService(_mockConfiguration.Object, httpClient, _mockLogger.Object);

        var prompt = "Gerar relatório mensal";
        var outputSchema = JsonSerializer.Serialize(new { type = "object" });

        var result = await service.GenerateReportAsync(prompt, outputSchema, AIModel.Gemini);

        result.Should().Contain("Relatório gerado com sucesso");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GenerateReportAsync_WithMaxRetriesExceeded_ShouldReturnError()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupAnyRequest()
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Muitas requisições")
            });

        var httpClient = handler.CreateClient();
        var service = new GeminiAIService(_mockConfiguration.Object, httpClient, _mockLogger.Object);

        var prompt = "Gerar relatório";
        var outputSchema = JsonSerializer.Serialize(new { type = "object" });

        var result = await service.GenerateReportAsync(prompt, outputSchema, AIModel.Gemini);

        result.Should().Contain("erro");
    }
}