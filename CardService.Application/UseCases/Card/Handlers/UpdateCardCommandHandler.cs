using AutoMapper;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Models.Responses;
using CardService.Application.UseCases.Card.Commands;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardService.Application.UseCases.Card.Handlers
{
    public class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, ResponseModel>
    {
        private readonly ILogger<UpdateCardCommandHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateCardCommandHandler(
            ILogger<UpdateCardCommandHandler> logger,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _logger = logger;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ResponseModel> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(UpdateCardCommandHandler)} {JsonConvert.SerializeObject(request)}");

            var card = await _uow.Repository<CardEntity>().FindAsync(x => x.Id == request.CardId);
            card!.Name = request.Name;
            card.Description = request.Description;
            card.Color = request.Color;
            card.Status = Enum.Parse<Status>(request.Status);
            card.DateUpdated = DateTime.UtcNow;

            await _uow.Complete(cancellationToken);

            return ResponseModel.Success(BaseStrings.SUCCESSFUL_CARD_UPDATE);
        }
    }
}
