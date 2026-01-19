using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using poupeai_report_service.Enums;
using poupeai_report_service.Services.AI;

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
        _mockConfiguration.Setup(c => c["GeminiAI:ApiKey"]).Returns("test-api-key-12345");
        _mockLogger = new Mock<ILogger<GeminiAIService>>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-008")]
    public void Constructor_WithValidApiKey_ShouldCreateInstance()
    {
        // Act
        var service = new GeminiAIService(_mockConfiguration.Object, _mockLogger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-008")]
    public void Constructor_WithNullApiKey_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["GeminiAI:ApiKey"]).Returns((string?)null);

        // Act & Assert
        var act = () => new GeminiAIService(mockConfig.Object, _mockLogger.Object);
        act.Should().Throw<ArgumentNullException>();
    }
}
