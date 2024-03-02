using CardService.Application.Common.Enums;

namespace CardService.Application.Common.Models.Responses
{
    public class ResponseModel
    {
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
        public ResponseCodes ResponseCode { get; set; }
        public List<string> ValidationMessages { get; set; } = new List<string>();

        public ResponseModel()
        {
        }
        public ResponseModel(ResponseCodes responseCode, string message)
        {
            ResponseCode = responseCode;
            Message = message;
        }

        public static ResponseModel Success(string? message = null)
        {
            return new ResponseModel()
            {
                IsSuccessful = true,
                Message = message ?? "Request was successful",
                ResponseCode = ResponseCodes.SUCCESS
            };
        }

        public static ResponseModel Failure(string? message = null)
        {
            return new ResponseModel()
            {
                Message = message ?? "Request was not completed",
                ResponseCode = ResponseCodes.BAD_REQUEST
            };

        }
    }

    public class ResponseModel<T> : ResponseModel
    {
        public T? Result { get; set; }

        public static ResponseModel<T> Success(T result, string? message = null)
        {
            return new ResponseModel<T>()
            {
                IsSuccessful = true,
                Message = message ?? "Request was successful",
                Result = result,
                ResponseCode = ResponseCodes.SUCCESS
            };
        }

        public static new ResponseModel<T> Failure(string? message = null)
        {
            return new ResponseModel<T>()
            {
                IsSuccessful = false,
                Message = message ?? "Request failed",
                ResponseCode = ResponseCodes.BAD_REQUEST
            };
        }
    }
}
