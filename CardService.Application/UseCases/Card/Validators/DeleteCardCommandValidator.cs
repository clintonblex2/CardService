using CardService.Application.Common.Exceptions;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.UseCases.Card.Commands;
using CardService.Domain.Entities;
using FluentValidation;

namespace CardService.Application.UseCases.Card.Validators
{
    public class DeleteCardCommandValidator : AbstractValidator<DeleteCardCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCardCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.CardId)
            .NotEmpty().WithMessage("Card Id is required");

            RuleFor(x => x).Custom((data, context) =>
            {
                var isExist = _uow.Repository<CardEntity>().Exist(x => x.UserId == data.UserId && x.Id == data.CardId && !x.IsDeleted);
                if (!isExist)
                    throw new CustomException(BaseStrings.CARD_NOT_EXIST);
            });
        }
    }
}
