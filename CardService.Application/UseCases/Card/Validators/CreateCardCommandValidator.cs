using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.UseCases.Card.Commands;
using CardService.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace CardService.Application.UseCases.Card.Validators
{
    public class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCardCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(BaseStrings.CARD_NAME_REQUIRED);

            // Conditional validation for Color
            // Only apply the regex rule if Color is not null or empty
            When(x => !string.IsNullOrEmpty(x.Color), () =>
            {
                RuleFor(x => x.Color)
                    .Matches("^#[A-Za-z0-9]{6}$")
                    .WithMessage(BaseStrings.INVALID_COLOR_FORMAT);
            });

            RuleFor(x => x).Custom((data, context) =>
            {
                var isExist = _uow.Repository<CardEntity>().Exist(x => x.UserId == data.UserId && x.Name == data.Name && !x.IsDeleted);

                if (isExist)
                    context.AddFailure(new ValidationFailure(nameof(CreateCardCommand.Name), BaseStrings.CARD_ALREADY_EXIST));
            });
        }
    }
}
