using CardService.Application.Common.Enums;
using CardService.Application.Common.Models.Responses;
using CardService.Application.UseCases.Card.Queries;
using MediatR;

namespace CardService.Application.UseCases.Card.Handlers
{
    public class GetSortByQueryHandler : IRequestHandler<GetSortByQuery, ResponseModel<List<string>>>
    {
        public Task<ResponseModel<List<string>>> Handle(GetSortByQuery request, CancellationToken cancellationToken)
        {
            var statuses = Enum.GetValues(typeof(SortBy))
                                    .Cast<SortBy>()
                                    .Select(s => s.ToString())
                                    .ToList();

            return Task.FromResult(ResponseModel<List<string>>.Success(statuses));
        }
    }
}
