using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.OpenApi.Attributes;

namespace poupeai_report_service.Enums;

public enum AIModel : byte
{
    [Description("Deepseek AI")]
    [EnumMember(Value = "deepseek")]
    [Display("Deepseek AI")]
    /// <summary>
    /// Deepseek AI model
    /// </summary>
    Deepseek = 1,

    [Description("Gemini AI")]
    [EnumMember(Value = "gemini")]
    [Display("Gemini AI")]
    Gemini = 2,
}