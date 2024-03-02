using CardService.Application.Common.Enums;
using CardService.Application.Common.Extensions;
using FluentValidation.Results;

namespace CardService.Application.Common.Exceptions
{
    public class CustomException : Exception
    {
        public int ErrorCode { get; private set; }
        public new string? Message { get; set; }
        public ResponseCodes ResponseCode { get; set; }
        public IDictionary<string, string[]>? ValidationMessages { get; }

        public CustomException()
            : base()
        {
        }

        public CustomException(string message)
            : base(message)
        {
            Message = message;
        }

        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public CustomException(string name, object key)
            : base($"Entity \"{name}\" ({key}) resulted into an error.")
        {
        }

        public CustomException(ResponseCodes code, string message) : base(message)
        {
            ErrorCode = code.GetStatusCode();
            Message = message;
        }

        public CustomException(List<ValidationFailure> failures)
            : this(ResponseCodes.BAD_REQUEST, "One or more validation failures have occurred.")
        {
            ValidationMessages = new Dictionary<string, string[]>();
            IEnumerable<string> propertyNames = failures
                .Select(e => e.PropertyName)
                .Distinct();

            foreach (string propertyName in propertyNames)
            {
                string[] propertyFailures = failures
                    .Where(e => e.PropertyName == propertyName)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                ValidationMessages.Add(propertyName, propertyFailures);
            }
        }
    }
}
