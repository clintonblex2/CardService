using AutoMapper;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Models.Responses;
using CardService.Application.UseCases.Card.Commands;
using CardService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardService.Application.UseCases.Card.Handlers
{
    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, ResponseModel>
    {
        private readonly ILogger<CreateCardCommandHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateCardCommandHandler(
            ILogger<CreateCardCommandHandler> logger,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _logger = logger;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ResponseModel> Handle(CreateCardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(CreateCardCommandHandler)} {JsonConvert.SerializeObject(request)}");

            _uow.Repository<CardEntity>().Insert(_mapper.Map<CardEntity>(request));
            await _uow.Complete(cancellationToken);

            return ResponseModel.Success(BaseStrings.SUCCESSFUL_CARD_CREATION);
        }
    }
}
