using CardService.Application.UseCases.User.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace CardService.API.Controllers
{
    public class UserController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a bearer token for the authenticated user")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns unauthorized if the user authentication failed")]
        [SwaggerOperation("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserCommand command)
        {
            var result = await Mediator.Send(command);
            return result.IsNullOrEmpty() ? Unauthorized() : Ok(new { Token = result });
        }
    }
}