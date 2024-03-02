using CardService.API.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardService.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender _mediator;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();

        protected JwtUser JwtUser
        {
            get
            {
                return new JwtUser(User);
            }
        }
    }
}
