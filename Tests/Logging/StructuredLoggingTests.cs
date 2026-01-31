using FluentAssertions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using Serilog.Context;

namespace poupeai_report_service.Tests.Logging;

/// <summary>
/// Testes para verificar o comportamento do logging estruturado com Serilog
/// Caso de Teste: RS-UT-102
/// </summary>
public class StructuredLoggingTests : IDisposable
{
    public StructuredLoggingTests()
    {
        // Configurar Serilog para testes
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.TestCorrelator()
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_WithStructuredParameters_ShouldCaptureProperties()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange
            var userId = "user-123";
            var reportType = "overview";

            // Act
            Log.Information("Report {ReportType} generated for user {UserId}", reportType, userId);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(1);

            var logEvent = logEvents.First();
            logEvent.Level.Should().Be(LogEventLevel.Information);
            logEvent.Properties.Should().ContainKey("ReportType");
            logEvent.Properties.Should().ContainKey("UserId");
            logEvent.Properties["ReportType"].ToString().Should().Contain("overview");
            logEvent.Properties["UserId"].ToString().Should().Contain("user-123");
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_WithException_ShouldCaptureExceptionDetails()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception message");

            // Act
            Log.Error(exception, "Error occurred while processing report");

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(1);

            var logEvent = logEvents.First();
            logEvent.Level.Should().Be(LogEventLevel.Error);
            logEvent.Exception.Should().NotBeNull();
            logEvent.Exception.Should().BeOfType<InvalidOperationException>();
            logEvent.Exception!.Message.Should().Be("Test exception message");
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_WithLogContext_ShouldEnrichWithContextProperties()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange
            var correlationId = "test-correlation-id";
            var userId = "user-456";

            // Act
            using (LogContext.PushProperty("trace.correlation_id", correlationId))
            using (LogContext.PushProperty("user.id", userId))
            {
                Log.Information("Processing request");
            }

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(1);

            var logEvent = logEvents.First();
            logEvent.Properties.Should().ContainKey("trace.correlation_id");
            logEvent.Properties.Should().ContainKey("user.id");
            logEvent.Properties["trace.correlation_id"].ToString().Should().Contain(correlationId);
            logEvent.Properties["user.id"].ToString().Should().Contain(userId);
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_MultipleEventsInContext_ShouldAllHaveContextProperties()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange
            var correlationId = "correlation-123";

            // Act
            using (LogContext.PushProperty("trace.correlation_id", correlationId))
            {
                Log.Information("First log entry");
                Log.Information("Second log entry");
                Log.Warning("Third log entry");
            }

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(3);

            foreach (var logEvent in logEvents)
            {
                logEvent.Properties.Should().ContainKey("trace.correlation_id");
                logEvent.Properties["trace.correlation_id"].ToString().Should().Contain(correlationId);
            }
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_WithDestructuring_ShouldCaptureObjectStructure()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange
            var reportData = new { Type = "expense", Period = "monthly", Amount = 1500.50 };

            // Act
            Log.Information("Report generated: {@Report}", reportData);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(1);

            var logEvent = logEvents.First();
            logEvent.Properties.Should().ContainKey("Report");
            var reportProperty = logEvent.Properties["Report"].ToString();
            reportProperty.Should().Contain("Type");
            reportProperty.Should().Contain("expense");
            reportProperty.Should().Contain("Period");
            reportProperty.Should().Contain("monthly");
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_DifferentLevels_ShouldRespectMinimumLevel()
    {
        using (TestCorrelator.CreateContext())
        {
            // Act
            Log.Debug("Debug message");
            Log.Information("Info message");
            Log.Warning("Warning message");
            Log.Error("Error message");

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(4);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Debug);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Information);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Warning);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Error);
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_WithNestedContext_ShouldMaintainOuterContext()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange & Act
            using (LogContext.PushProperty("outer", "outer-value"))
            {
                Log.Information("Outer log");

                using (LogContext.PushProperty("inner", "inner-value"))
                {
                    Log.Information("Inner log");
                }

                Log.Information("Back to outer");
            }

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(3);

            // First log: only outer
            logEvents[0].Properties.Should().ContainKey("outer");
            logEvents[0].Properties.Should().NotContainKey("inner");

            // Second log: both outer and inner
            logEvents[1].Properties.Should().ContainKey("outer");
            logEvents[1].Properties.Should().ContainKey("inner");

            // Third log: only outer again
            logEvents[2].Properties.Should().ContainKey("outer");
            logEvents[2].Properties.Should().NotContainKey("inner");
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_WithHttpRequestException_ShouldCaptureExceptionType()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange
            var httpException = new HttpRequestException("Connection timeout");

            // Act
            Log.Error(httpException, "HTTP request failed for service {ServiceName}", "Django Finance Service");

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(1);

            var logEvent = logEvents.First();
            logEvent.Exception.Should().BeOfType<HttpRequestException>();
            logEvent.Properties.Should().ContainKey("ServiceName");
            logEvent.Properties["ServiceName"].ToString().Should().Contain("Django Finance Service");
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_WithNumericValues_ShouldPreserveTypes()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange
            var totalExpense = 2500.75m;
            var transactionCount = 42;

            // Act
            Log.Information("Processed {TransactionCount} transactions with total {TotalExpense}",
                transactionCount, totalExpense);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(1);

            var logEvent = logEvents.First();
            logEvent.Properties.Should().ContainKey("TransactionCount");
            logEvent.Properties.Should().ContainKey("TotalExpense");
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-102")]
    public void Log_WithDateTime_ShouldCaptureTimestamp()
    {
        using (TestCorrelator.CreateContext())
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 1, 31);

            // Act
            Log.Information("Report period: {StartDate} to {EndDate}", startDate, endDate);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(1);

            var logEvent = logEvents.First();
            logEvent.Properties.Should().ContainKey("StartDate");
            logEvent.Properties.Should().ContainKey("EndDate");
            logEvent.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }
    }

    public void Dispose()
    {
        Log.CloseAndFlush();
    }
}
