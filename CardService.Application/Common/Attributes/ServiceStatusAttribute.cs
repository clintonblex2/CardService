namespace CardService.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ServiceStatusAttribute : Attribute
    {
        public int ResponseCode { get; }
        public string? Message { get; }
        public string? UserMessage { get; }

        public ServiceStatusAttribute(int responseCode, string message, string? userMessage = null)
        {
            ResponseCode = responseCode;
            Message = message;
            UserMessage = userMessage;
        }
    }
}
