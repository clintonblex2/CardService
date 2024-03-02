using CardService.Application.Common.Models.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace CardService.Application.UseCases.Card.Commands
{
    public class DeleteCardCommand : IRequest<ResponseModel>
    {
        public long CardId { get; set; }
        [JsonIgnore]
        public long UserId { get; set; }
    }
}
