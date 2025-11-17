using System.Security.Cryptography;
using System.Text;
using poupeai_report_service.DTOs.Requests;
using poupeai_report_service.Enums;

namespace poupeai_report_service.Utils;

internal static class Hash
{
    private static string Generate(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    public static string GenerateFromTransaction(TransactionsData transactionsData, AIModel model = AIModel.Gemini)
    {
        var transactionHash = $"account_id: {transactionsData.AccountId}";
        transactionHash += $"start_date: {transactionsData.StartDate}";
        transactionHash += $"end_date: {transactionsData.EndDate}";
        transactionHash += $"transacions_ids: [{string.Join(",", transactionsData.Transactions.Select(r => r.Id))}]";
        transactionHash += $"model: {Tools.ModelToString(model)}";

        return Generate(transactionHash);
    }
}