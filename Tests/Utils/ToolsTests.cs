using FluentAssertions;
using Microsoft.AspNetCore.Http;
using poupeai_report_service.DTOs.Responses;
using poupeai_report_service.Enums;
using poupeai_report_service.Utils;
using System.Text.Json.Serialization;

namespace poupeai_report_service.Tests.Utils;

/// <summary>
/// Testes unitários para a classe Tools
/// Casos de Teste: RS-UT-002, RS-UT-003
/// </summary>
public class ToolsTests
{
    private class TestClass
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    #region StringToModel Tests

    [Theory]
    [InlineData("gemini", AIModel.Gemini)]
    [InlineData("deepseek", AIModel.Deepseek)]
    [InlineData("GEMINI", AIModel.Gemini)]
    [InlineData("DEEPSEEK", AIModel.Deepseek)]
    public void StringToModel_WithVariousInputs_ReturnsCorrectModel(string input, AIModel expected)
    {
        var result = Tools.StringToModel(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void StringToModel_WithInvalidInput_ThrowsException()
    {
        var invalidInput = "invalid_model";
        Assert.Throws<ArgumentOutOfRangeException>(() => Tools.StringToModel(invalidInput));
    }

    #endregion

    #region ModelToString Tests

    [Theory]
    [InlineData(AIModel.Gemini, "gemini")]
    [InlineData(AIModel.Deepseek, "deepseek")]
    public void ModelToString_WithValidModel_ReturnsCorrectString(AIModel input, string expected)
    {
        var result = Tools.ModelToString(input);
        result.Should().Be(expected);
    }

    [Fact]
    public void ModelToString_WithInvalidModel_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Tools.ModelToString((AIModel)99));
    }

    #endregion

    #region DeserializeJson Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void DeserializeJson_WithNullOrWhitespace_ThrowsInvalidOperationException(string input)
    {
        Assert.Throws<InvalidOperationException>(() => Tools.DeserializeJson<TestClass>(input));
    }

    [Fact]
    public void DeserializeJson_WithValidJson_ReturnsObject()
    {
        var json = @"{""name"":""Relatório"",""value"":123}";

        var result = Tools.DeserializeJson<TestClass>(json);

        result.Should().NotBeNull();
        result.Content?.Name.Should().Be("Relatório");
        result.Content?.Value.Should().Be(123);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-002")]
    public void DeserializeJson_WithValidAIResponseJson_ReturnsObject()
    {
        var json = @"{
            ""header"": {
                ""status"": 200,
                ""message"": ""Sucesso""
            },
            ""content"": {
                ""name"": ""Relatório Mensal"",
                ""value"": 100
            }
        }";

        var result = Tools.DeserializeJson<TestClass>(json);

        result.Should().NotBeNull();
        result.Header.Should().NotBeNull();
        result.Header.Status.Should().Be(200);
        result.Header.Message.Should().Be("Sucesso");
        result.Content.Should().NotBeNull();
        result.Content.Name.Should().Be("Relatório Mensal");
        result.Content.Value.Should().Be(100);
    }

    [Fact]
    public void DeserializeJson_WithInvalidJson_ThrowsException()
    {
        var invalidJson = "{ json invalido }";

        Assert.Throws<InvalidOperationException>(() => Tools.DeserializeJson<TestClass>(invalidJson));
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("TestCase", "RS-UT-003")]
    public void DeserializeJson_WithInvalidJsonStructure_ThrowsExceptionWithDetails()
    {
        var invalidJson = "{ json invalido sem fechamento";

        var exception = Assert.Throws<InvalidOperationException>(() =>
            Tools.DeserializeJson<TestClass>(invalidJson));

        exception.Message.Should().Contain("Failed to deserialize JSON");
    }

    #endregion

    [Theory]
    [InlineData(200, "Ok")]
    [InlineData(204, "NoContent")]
    [InlineData(400, "BadRequest")]
    [InlineData(401, "Unauthorized")]
    [InlineData(403, "Forbid")]
    [InlineData(404, "NotFound")]
    public void BuildResultFromHeader_WithVariousStatus_ReturnsCorrectResultType(int status, string expectedType)
    {
        var header = new { Status = status, Message = "Mensagem de teste" };

        IResult result = Tools.BuildResultFromHeader(header, status);

        result.GetType().Name.Should().Contain(expectedType);
    }
}