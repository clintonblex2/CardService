using CardService.Application.Common.Enums;
using CardService.Application.Common.Exceptions;
using CardService.Application.Common.Extensions;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CardService.Application.Common.Filters
{
    public class ServiceExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ServiceExceptionFilter> _logger;

        public ServiceExceptionFilter(ILogger<ServiceExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            ResponseCodes statusCode = ResponseCodes.EXCEPTION;

            var response = new ResponseModel(statusCode, context.Exception?.Message ?? "Something went wrong");

            if (context.Exception is CustomException ex)
            {
                if (ex.ValidationMessages != null)
                {
                    response = new ValidationResponseModel(ex.ValidationMessages);
                }
            }

            _logger.LogError(context.Exception, "{@Message}", response);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Result = new JsonResult(response);
            var logKey = Utils.GenerateCode(15);
            response.Message = response.Message;
            var errorDetails = context?.Exception?.FormatException();
            Serilog.Log.Error($"ErrorID={logKey} {Environment.NewLine} {errorDetails}");
        }
    }
}
