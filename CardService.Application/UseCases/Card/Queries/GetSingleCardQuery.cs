using CardService.Application.Common.Models.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace CardService.Application.UseCases.Card.Queries
{
    public class GetSingleCardQuery : IRequest<ResponseModel<CardResponseModel>>
    {
        public long CardId { get; set; }

        [JsonIgnore]
        public required long UserId { get; set; }
    }
}
