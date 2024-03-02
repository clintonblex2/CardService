using CardService.Application.Common.Models.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace CardService.Application.UseCases.Card.Commands
{
    public class CreateCardCommand : IRequest<ResponseModel>
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Color { get; set; }

        [JsonIgnore]
        public long UserId { get; set; }
    }
}
