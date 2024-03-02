using CardService.Application.Common.Models.Requests;
using CardService.Application.Common.Models.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace CardService.Application.UseCases.Card.Queries
{
    public class GetCardsQuery : CardFilter, IRequest<PagedList<CardResponseModel>>
    {
        [JsonIgnore]
        public long UserId { get; set; }
        [JsonIgnore]
        public string? Role { get; set; }
    }
}
