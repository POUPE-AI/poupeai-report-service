using System.Text.Json;
using MongoDB.Driver;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;
using poupeai_report_service.Interfaces;
using poupeai_report_service.Models;
using poupeai_report_service.Utils;
using Serilog;

namespace poupeai_report_service.Services.Bases;

internal abstract class BaseReportService<TModel, TResponse>(IMongoDatabase database, string collectionName, string outputSchemaFile, string loggerName)
    where TModel : BaseReportModel
    where TResponse : class
{
    /// <summary>
    /// Serviço de Logger para registrar informações e erros.
    /// Utiliza o Serilog para registrar logs com contexto específico do serviço.
    /// </summary>
    protected readonly Serilog.ILogger _logger = Log.ForContext("Service", loggerName);

    /// <summary>
    /// Coleção MongoDB onde os modelos são armazenados.
    /// Esta coleção é utilizada para realizar operações de leitura e escrita no banco de dados MongoDB.
    /// </summary>
    protected readonly IMongoCollection<TModel> _collection = database.GetCollection<TModel>(collectionName);

    /// <summary>
    /// Esquema de saída para o relatório.
    /// Este esquema é carregado de um arquivo JSON localizado na pasta "Schemas".
    /// </summary>
    protected readonly string _outputSchema = File.ReadAllText(
        Path.Combine("Schemas",
        outputSchemaFile.EndsWith(".json") ? outputSchemaFile : $"{outputSchemaFile}.json"));

    public async Task<IResult> GenerateReportAsync(
        TransactionsData transactionsData,
        IAIService aiService,
        AIModel model,
        Func<string, TResponse?> deserializeResponse,
        Func<TResponse, TModel> mapToModel
    )
    {
        try
        {
            var hash = Hash.GenerateFromTransaction(transactionsData, model);

            // Tentar buscar do cache (MongoDB) - se falhar, continuar sem cache
            TModel? existingReport = null;
            try
            {
                existingReport = await _collection.Find(r => r.Hash == hash).FirstOrDefaultAsync();
                if (existingReport != null)
                {
                    _logger.Information("Report found in cache for hash: {Hash}", hash);
                    return Results.Ok(new
                    {
                        Header = new
                        {
                            Status = 200,
                        },
                        Content = existingReport
                    });
                }
                _logger.Debug("Report not found in cache, generating new report for hash: {Hash}", hash);
            }
            catch (Exception cacheEx)
            {
                _logger.Warning(cacheEx, "Failed to check cache (MongoDB). Proceeding without cache for hash: {Hash}", hash);
            }

            var dataJson = JsonSerializer.Serialize(transactionsData);
            var prompt = BuildPrompt(dataJson);

            var result = await aiService.GenerateReportAsync(prompt, _outputSchema, model);

            // Verificar resultado da IA
            if (string.IsNullOrEmpty(result))
            {
                _logger.Error("AI service returned an empty result for report generation.");
                return Results.Problem("Failed to generate the report due to empty response from AI service.");
            }

            TResponse? response;
            try
            {
                response = deserializeResponse(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to deserialize AI service response. Raw response: {RawResponse}", result);
                return Results.Problem("Failed to generate the report due to deserialization error.");
            }

            if (response == null)
            {
                _logger.Error("Failed to deserialize AI service response for report. Response was null.");
                return Results.Problem("Failed to generate the report due to deserialization error.");
            }

            if ((response as dynamic).Header.Status != 200)
            {
                _logger.Error("AI service returned an error: {Status} - {Message}", (response as dynamic).Header.Status, (response as dynamic).Header.Message);
                int status = (response as dynamic).Header.Status;
                var header = (response as dynamic).Header;

                return Tools.BuildResultFromHeader(header, status);
            }

            if ((response as dynamic).Content == null)
            {
                _logger.Error("AI service response content is null.");
                return Results.Problem("Failed to generate the report due to null content in AI service response.");
            }

            var reportModel = mapToModel(response);
            reportModel.Hash = hash;
            reportModel.AccountId = transactionsData.AccountId ?? string.Empty; // Garantir que AccountId não seja nulo
            reportModel.StartDate = transactionsData.StartDate;
            reportModel.EndDate = transactionsData.EndDate;
            reportModel.UpdatedAt = DateTime.UtcNow;

            // Tentar salvar no cache (MongoDB) - se falhar, apenas logar e continuar
            try
            {
                await _collection.InsertOneAsync(reportModel);
                _logger.Information("Report generated and saved to cache successfully for hash: {Hash}", hash);
            }
            catch (Exception cacheEx)
            {
                _logger.Warning(cacheEx, "Failed to save report to cache (MongoDB). Report generated but not cached for hash: {Hash}", hash);
            }

            return Results.Created(string.Empty, new
            {
                (response as dynamic).Header,
                Content = reportModel
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating report");
            return Results.Problem($"An error occurred while generating the report: {ex.Message}");
        }
    }

    /// <summary>
    /// Constrói o prompt para enviar à IA com base nos dados fornecidos.
    /// Este método deve ser implementado por classes derivadas para personalizar o prompt de acordo com o tipo de relatório.
    /// </summary>
    /// <param name="dataJson">JSON contendo os dados necessários para gerar o relatório.</param>
    /// <returns>
    /// Uma string representando o prompt formatado para a IA.
    /// </returns>
    protected abstract string BuildPrompt(string dataJson);
}