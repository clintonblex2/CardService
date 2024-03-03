using CardService.Application.Common.Exceptions;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.UseCases.Card.Commands;
using CardService.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace CardService.Application.UseCases.Card.Validators
{
    public class DeleteCardCommandValidator : AbstractValidator<DeleteCardCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCardCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.CardId)
            .NotEmpty().WithMessage(BaseStrings.CARD_ID_REQUIRED);

            RuleFor(x => x).Custom((data, context) =>
            {
                var isExist = _uow.Repository<CardEntity>().Exist(x => x.UserId == data.UserId && x.Id == data.CardId && !x.IsDeleted);
                if (!isExist)
                    context.AddFailure(new ValidationFailure(nameof(DeleteCardCommand.CardId), BaseStrings.CARD_NOT_EXIST));
            });
        }
    }
}
