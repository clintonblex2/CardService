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
    public class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand, ResponseModel>
    {
        private readonly ILogger<DeleteCardCommandHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DeleteCardCommandHandler(
            ILogger<DeleteCardCommandHandler> logger,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _logger = logger;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ResponseModel> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(DeleteCardCommandHandler)} {JsonConvert.SerializeObject(request)}");

            var card = await _uow.Repository<CardEntity>().FindAsync(x => x.Id == request.CardId);
            card!.IsDeleted = true;
            await _uow.Complete(cancellationToken);

            return ResponseModel.Success(BaseStrings.SUCCESSFUL_CARD_DELETE);
        }
    }
}
