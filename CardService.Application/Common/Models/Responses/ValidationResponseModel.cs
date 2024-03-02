using CardService.Application.Common.Enums;
using CardService.Application.Common.Extensions;

namespace CardService.Application.Common.Models.Responses
{
    public class ValidationResponseModel : ResponseModel
    {
        public ValidationResponseModel(IDictionary<string, string[]> validationErrors)
            : base(ResponseCodes.BAD_REQUEST, ResponseCodes.BAD_REQUEST.GetStatusMessage())
        {
            ValidationMessages = validationErrors.SelectMany(x => x.Value).ToList();
        }
    }
}
