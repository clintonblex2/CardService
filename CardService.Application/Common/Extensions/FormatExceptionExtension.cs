using CardService.Application.Common.Models.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Text;

namespace CardService.Application.Common.Extensions
{
    public static class FormatExceptionExtension
    {
        public static string? FormatException(this Exception e)
        {
            if (e == null)
            {
                return default;
            }

            var ex = e;
            var sb = new StringBuilder();

            while (ex != null)
            {
                sb.AppendLine($"Exception: {ex.Message} \nStackTrace: {ex.StackTrace} {Environment.NewLine}");
                ex = ex.InnerException;
            }

            sb.AppendLine("================================================================================");

            return sb.ToString();
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var statusCode = context.Response.StatusCode;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error(contextFeature.Error, "An Error Occured");
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(ResponseModel.Failure("Unable to process action..")));
                    }
                });
            });
        }
    }
}
