using System.Security.Cryptography;
using System.Text;

namespace poupeai_report_service.Utils;

public static class Hash
{
    public static string GenerateFromString(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}