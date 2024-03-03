using AutoMapper;
using CardService.Application.Common.Extensions;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Models.Responses;
using CardService.Application.UseCases.Card.Queries;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardService.Application.UseCases.Card.Handlers
{
    public class GetCardsQueryHandler : IRequestHandler<GetCardsQuery, PagedList<CardResponseModel>>
    {
        private readonly ILogger<GetCardsQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public GetCardsQueryHandler(ILogger<GetCardsQueryHandler> logger, IMapper mapper, IUnitOfWork uow)
        {
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<PagedList<CardResponseModel>> Handle(GetCardsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(GetCardsQueryHandler)} {JsonConvert.SerializeObject(request)}");

            var cards = _uow.Repository<CardEntity>().FilterAsNoTracking(
                filter: x => !x.IsDeleted,
                include: source => source.Include(i => i.User));

            if (request.Role != null && request.Role.Contains("Member"))
                cards = cards.Where(x => x.UserId == request.UserId);

            // Apply filters
            if(Enum.TryParse(request.Status, true, out Status cardStatus))
                cards = cards.ApplyFilter(c => c.Status == cardStatus);

            cards = cards.ApplyFilter(c => string.IsNullOrEmpty(request.Name) || c.Name.Contains(request.Name));
            cards = cards.ApplyFilter(c => string.IsNullOrEmpty(request.Color) || c.Color == request.Color);
            cards = cards.ApplyFilter(c => !request.From.HasValue || c.DateCreated >= request.From.Value);
            cards = cards.ApplyFilter(c => !request.To.HasValue || c.DateCreated <= request.To.Value);

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
                cards = cards.OrderByProperty(request.SortBy, request.SortBy.Contains("DateCreated") ? false : true);

            if (!cards.Any())
                return PagedList<CardResponseModel>.Failure(BaseStrings.CARD_NOT_EXIST);

            var result = PaginationExtension.ToPagedList(cards.AsQueryable(), request.PageNumber, request.PageSize);
            return _mapper.Map<PagedList<CardResponseModel>>(result);
        }
    }
}