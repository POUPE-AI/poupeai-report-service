namespace poupeai_report_service.DTOs.Responses
{
    internal class DeepSeekResponse
    {
        public Choice[]? Choices { get; set; }

        public class Choice
        {
            public Message? Message { get; set; }
        }

        public class Message
        {
            public string? Content { get; set; }
        }
    }
}