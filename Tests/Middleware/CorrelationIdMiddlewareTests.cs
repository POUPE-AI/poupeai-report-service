using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using poupeai_report_service.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using System.Security.Claims;

namespace poupeai_report_service.Tests.Middleware;

/// <summary>
/// Testes unitários para CorrelationIdMiddleware
/// Caso de Teste: RS-UT-101
/// </summary>
public class CorrelationIdMiddlewareTests
{
    private readonly DefaultHttpContext _httpContext;
    private readonly Mock<RequestDelegate> _mockNext;

    public CorrelationIdMiddlewareTests()
    {
        _httpContext = new DefaultHttpContext();
        _mockNext = new Mock<RequestDelegate>();
        _mockNext.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-101")]
    public async Task InvokeAsync_WithCorrelationIdInHeader_ShouldUseExistingId()
    {
        // Arrange
        var expectedCorrelationId = "test-correlation-id-123";
        _httpContext.Request.Headers["X-Correlation-ID"] = expectedCorrelationId;

        var middleware = new CorrelationIdMiddleware(_mockNext.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.Headers["X-Correlation-ID"].ToString().Should().Be(expectedCorrelationId);
        _mockNext.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-101")]
    public async Task InvokeAsync_WithoutCorrelationIdInHeader_ShouldGenerateNewId()
    {
        // Arrange
        var middleware = new CorrelationIdMiddleware(_mockNext.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        var responseHeader = _httpContext.Response.Headers["X-Correlation-ID"].ToString();
        responseHeader.Should().NotBeNullOrEmpty();
        Guid.TryParse(responseHeader, out _).Should().BeTrue("Correlation ID should be a valid GUID");
        _mockNext.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-101")]
    public async Task InvokeAsync_WithUserAuthenticated_ShouldAddUserIdToLogContext()
    {
        // Arrange
        var expectedUserId = "user-uuid-12345";
        var claims = new List<Claim>
        {
            new Claim("sub", expectedUserId),
            new Claim("name", "Test User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _httpContext.User = claimsPrincipal;

        using (TestCorrelator.CreateContext())
        {
            var middleware = new CorrelationIdMiddleware(_mockNext.Object);

            // Act
            await middleware.InvokeAsync(_httpContext);

            // Assert
            _mockNext.Verify(next => next(_httpContext), Times.Once);
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-101")]
    public async Task InvokeAsync_WithPreferredUsernameOnly_ShouldUsePreferredUsername()
    {
        // Arrange
        var expectedUsername = "testuser@example.com";
        var claims = new List<Claim>
        {
            new Claim("preferred_username", expectedUsername)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _httpContext.User = claimsPrincipal;

        var middleware = new CorrelationIdMiddleware(_mockNext.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-101")]
    public async Task InvokeAsync_WithoutUser_ShouldStillWork()
    {
        // Arrange
        _httpContext.User = new ClaimsPrincipal();

        var middleware = new CorrelationIdMiddleware(_mockNext.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        var responseHeader = _httpContext.Response.Headers["X-Correlation-ID"].ToString();
        responseHeader.Should().NotBeNullOrEmpty();
        _mockNext.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-101")]
    public async Task InvokeAsync_WithEmptyCorrelationId_ShouldGenerateNewId()
    {
        // Arrange
        _httpContext.Request.Headers["X-Correlation-ID"] = string.Empty;

        var middleware = new CorrelationIdMiddleware(_mockNext.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        var responseHeader = _httpContext.Response.Headers["X-Correlation-ID"].ToString();
        responseHeader.Should().NotBeNullOrEmpty();
        Guid.TryParse(responseHeader, out _).Should().BeTrue();
        _mockNext.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-101")]
    public async Task InvokeAsync_ShouldCallNextMiddleware()
    {
        // Arrange
        var middleware = new CorrelationIdMiddleware(_mockNext.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-101")]
    public async Task InvokeAsync_WithBothSubAndPreferredUsername_ShouldPreferSub()
    {
        // Arrange
        var expectedUserId = "sub-user-id";
        var claims = new List<Claim>
        {
            new Claim("sub", expectedUserId),
            new Claim("preferred_username", "other-username")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _httpContext.User = claimsPrincipal;

        var middleware = new CorrelationIdMiddleware(_mockNext.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _mockNext.Verify(next => next(_httpContext), Times.Once);
    }
}
