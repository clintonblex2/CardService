using CardService.Application.Common.Models.Responses;
using MediatR;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace CardService.Application.UseCases.Card.Commands
{
    public class UpdateCardCommand : IRequest<ResponseModel>
    {
        public long CardId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Color { get; set; }

        public string Status { get; set; }

        [JsonIgnore]
        public long UserId { get; set; }
    }
}
