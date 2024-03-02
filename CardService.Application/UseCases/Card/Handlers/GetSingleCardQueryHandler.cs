using AutoMapper;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Models.Responses;
using CardService.Application.UseCases.Card.Queries;
using CardService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardService.Application.UseCases.Card.Handlers
{
    public class GetSingleCardQueryHandler : IRequestHandler<GetSingleCardQuery, ResponseModel<CardResponseModel>>
    {
        private readonly ILogger<GetSingleCardQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public GetSingleCardQueryHandler(ILogger<GetSingleCardQueryHandler> logger, IMapper mapper, IUnitOfWork uow)
        {
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ResponseModel<CardResponseModel>> Handle(GetSingleCardQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(GetSingleCardQueryHandler)} {JsonConvert.SerializeObject(request)}");

            var card = await _uow.Repository<CardEntity>().FindAsync(x => x.Id == request.CardId && x.UserId == request.UserId && !x.IsDeleted,
                                                                        source => source.User);

            if (card is null)
                return ResponseModel<CardResponseModel>.Failure(BaseStrings.CARD_NOT_EXIST);

            return ResponseModel<CardResponseModel>.Success(_mapper.Map<CardResponseModel>(card));
        }
    }
}
