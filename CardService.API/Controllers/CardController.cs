using AutoMapper;
using CardService.Application.Common.Models.Requests;
using CardService.Application.Common.Models.Responses;
using CardService.Application.UseCases.Card.Commands;
using CardService.Application.UseCases.Card.Queries;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CardService.API.Controllers
{
    public class CardController : ApiControllerBase
    {
        private readonly IMapper _mapper;

        public CardController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet("GetSortBy")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ResponseModel<List<string>>), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of sortable columns")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns unauthorized if the user is not authenticated")]
        [SwaggerOperation("GetSortBy")]
        public async Task<IActionResult> GetSortBy()
        {
            var result = await Mediator.Send(new GetSortByQuery());
            return !result.IsSuccessful ? BadRequest(result) : Ok(result);
        }

        [HttpPost("Create")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Ok for successful card creation")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns unauthorized if the user is not authenticated")]
        [SwaggerOperation("CreateCard")]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardCommand command)
        {
            command.UserId = long.Parse(JwtUser.UserId);
            var result = await Mediator.Send(command);
            return !result.IsSuccessful ? BadRequest(result) : Ok(result);
        }

        [HttpPatch("Update")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Ok for successful card update")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns unauthorized if the user is not authenticated")]
        [SwaggerOperation("UpdateCard")]
        public async Task<IActionResult> UpdateCard([FromBody] UpdateCardCommand command)
        {
            command.UserId = long.Parse(JwtUser.UserId);
            var result = await Mediator.Send(command);
            return !result.IsSuccessful ? BadRequest(result) : Ok(result);
        }

        [HttpDelete("Delete")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Ok for successful card deletion")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns unauthorized if the user is not authenticated")]
        [SwaggerOperation("DeleteCard")]
        public async Task<IActionResult> DeleteCard([FromBody] DeleteCardCommand command)
        {
            command.UserId = long.Parse(JwtUser.UserId);
            var result = await Mediator.Send(command);
            return !result.IsSuccessful ? BadRequest(result) : Ok(result);
        }

        [HttpGet("GetSingleCard")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ResponseModel<CardResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Ok for successful card retrieval")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns unauthorized if the user is not authenticated")]
        [SwaggerOperation("GetSingleCard")]
        public async Task<IActionResult> GetCard([FromQuery] long cardId)
        {
            var query = new GetSingleCardQuery { UserId = long.Parse(JwtUser.UserId), CardId = cardId };
            var result = await Mediator.Send(query);
            return !result.IsSuccessful ? BadRequest(result) : Ok(result);
        }

        [HttpGet("GetCardsByFilter")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedList<CardResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PagedList), StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Ok for successful card details retrieval")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns unauthorized if the user is not authenticated")]
        [SwaggerOperation("GetCardsByFilter")]
        public async Task<IActionResult> GetCard([FromQuery] CardFilter filter)
        {
            var query = _mapper.Map<GetCardsQuery>(filter);
            query.UserId = long.Parse(JwtUser.UserId);
            query.Role = JwtUser.Role;
            var result = await Mediator.Send(query);
            return !result.IsSuccessful ? BadRequest(result) : Ok(result);
        }
    }
}
