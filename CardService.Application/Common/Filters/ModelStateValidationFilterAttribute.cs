using CardService.Application.Common.Enums;
using CardService.Application.Common.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CardService.Application.Common.Filters
{
    public class ModelStateValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Dictionary<string, string[]> Errors = new Dictionary<string, string[]>();
            var validationMessages = new List<string>();
            if (!context.ModelState.IsValid)
            {
                string err = "";

                foreach (var keyModelStatePair in context.ModelState)
                {
                    var key = keyModelStatePair.Key;
                    var errors = keyModelStatePair.Value.Errors;

                    if (errors != null && errors.Count > 0)
                    {
                        if (errors.Count == 1)
                        {
                            var errormessage = GetErrorMessage(errors[0]);
                            Errors.Add(key, new[] { errormessage });
                            validationMessages.Add(errormessage);
                            err = errormessage;
                        }
                        else
                        {
                            var errormessages = new string[errors.Count];
                            for (int i = 0; i < errors.Count; i++)
                            {
                                errormessages[i] = GetErrorMessage(errors[i]);
                                validationMessages.Add(errormessages[i]);
                            }
                            err = errormessages[0];
                        }
                    }
                }

                var apiError = new ResponseModel<string>();
                apiError.ResponseCode = ResponseCodes.BAD_REQUEST;
                apiError.Message = err;
                apiError.ValidationMessages = validationMessages;
                context.HttpContext.Response.StatusCode = 400;

                context.Result = new BadRequestObjectResult(apiError);
            }
        }

        private string GetErrorMessage(ModelError modelError)
        {
            return string.IsNullOrEmpty(modelError.ErrorMessage) ? "The input was not valid " : modelError.ErrorMessage;
        }
    }
}
