using CardService.Application.Common.Exceptions;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.UseCases.Card.Commands;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Application.UseCases.Card.Validators
{
    public class UpdateCardCommandValidator : AbstractValidator<UpdateCardCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCardCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.CardId)
            .NotEmpty().WithMessage(BaseStrings.CARD_ID_REQUIRED);
            
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(BaseStrings.CARD_NAME_REQUIRED);

            RuleFor(x => x.Status)
            .NotEmpty().WithMessage(BaseStrings.CARD_STATUS_REQUIRED)
            .Must(BeAValidEnumValue).WithMessage(BaseStrings.INVALID_CARD_STATUS);

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
                var isExist = _uow.Repository<CardEntity>().Exist(x => x.UserId == data.UserId && x.Id == data.CardId && !x.IsDeleted);
                if (!isExist)
                    context.AddFailure(new ValidationFailure(nameof(UpdateCardCommand.CardId), BaseStrings.CARD_NOT_EXIST));

                isExist = _uow.Repository<CardEntity>().Exist(x => x.UserId == data.UserId && x.Name == data.Name && x.Id != data.CardId && !x.IsDeleted);
                if (isExist)
                    context.AddFailure(new ValidationFailure(nameof(UpdateCardCommand.Name), BaseStrings.CARD_ALREADY_EXIST));
            });
        }

        private bool BeAValidEnumValue(string cardStatus)
        {
            return Enum.TryParse<Status>(cardStatus, true, out _);
        }
    }
}
