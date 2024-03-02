using CardService.Application.Common.Models.Responses;
using MediatR;

namespace CardService.Application.UseCases.Card.Queries
{
    public record GetSortByQuery : IRequest<ResponseModel<List<string>>>;
}
