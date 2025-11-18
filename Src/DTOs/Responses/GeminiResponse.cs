namespace poupeai_report_service.DTOs.Responses;

internal class GeminiResponse
{
#pragma warning disable IDE1006 // Naming Styles
    public Candidate[]? candidates { get; set; }


    public class Candidate
    {
        public Content? content { get; set; }
    }

    public class Content
    {
        public Part[]? parts { get; set; }
    }

    public class Part
    {
        public string? text { get; set; }
    }
#pragma warning restore IDE1006
}