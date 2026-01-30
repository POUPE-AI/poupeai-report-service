using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Moq;
using Moq.Protected;
using poupeai_report_service.Services;

namespace poupeai_report_service.Tests.Services;

public class FinancesServiceTests
{
    [Fact]
    public async Task GetTransactionsAsync_ParsesNewCoreResponse()
    {
        var sample = @"{
        ""content"": [
            {
            ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
            ""description"": ""string"",
            ""amount"": 0,
            ""type"": ""INCOME"",
            ""transactionDate"": ""2026-01-30"",
            ""bankAccountId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
            ""creditCardId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
            ""category"": {
                    ""id"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
                ""name"": ""string"",
                ""colorHex"": ""string""
            },
            ""invoiceId"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
            ""attachmentKey"": ""string"",
            ""attachmentUrl"": ""string"",
            ""isInstallment"": true,
            ""installmentNumber"": 0,
            ""totalInstallments"": 0,
            ""purchaseGroupUuid"": ""3fa85f64-5717-4562-b3fc-2c963f66afa6"",
            ""originalStatementId"": ""string"",
            ""originalStatementDescription"": ""string"",
            ""createdAt"": ""2026-01-30T20:35:45.748Z"",
            ""updatedAt"": ""2026-01-30T20:35:45.748Z""
            }
        ],
        ""page"": 0,
        ""size"": 0,
        ""totalElements"": 0,
        ""totalPages"": 0
        }";

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(sample, Encoding.UTF8, "application/json")
                    })
                    .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://core-service:8000")
        };

        var inMemorySettings = new Dictionary<string, string?> {
            { "FinancesService:BaseUrl", "http://core-service:8000" },
            { "FinancesService:TransactionsEndpoint", "/api/v1/transactions" },
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer dummy";

        // Provide IAuthenticationService implementation so HttpContext.GetTokenAsync() works
        var services = new ServiceCollection();
        var mockAuth = new Mock<IAuthenticationService>();
        mockAuth.Setup(m => m.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync((HttpContext ctx, string scheme) =>
            {
                var props = new AuthenticationProperties();
                props.StoreTokens(new[] { new AuthenticationToken { Name = "access_token", Value = "dummy" } });
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(), props, scheme ?? "TestScheme");
                return AuthenticateResult.Success(ticket);
            });
        services.AddSingleton<IAuthenticationService>(mockAuth.Object);
        httpContext.RequestServices = services.BuildServiceProvider();

        // Use a real HttpContextAccessor so the service can read the HttpContext
        var accessor = new Microsoft.AspNetCore.Http.HttpContextAccessor { HttpContext = httpContext };

        var finances = new FinancesService(httpClient, configuration, accessor);



        var start = DateOnly.Parse("2026-01-30");
        var end = DateOnly.Parse("2026-01-30");

        var result = await finances.GetTransactionsAsync(start, end);

        result.Should().HaveCount(1);
        var t = result[0];
        t.Id.Should().Be("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        t.Category.Should().Be("string");
        t.Date.Should().Be(new DateTime(2026, 1, 30));

        handlerMock.Protected().Verify("SendAsync", Times.AtLeastOnce(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }
}