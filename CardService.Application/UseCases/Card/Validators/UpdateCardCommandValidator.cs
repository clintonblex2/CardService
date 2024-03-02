using CardService.Application.Common.Exceptions;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.UseCases.Card.Commands;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using FluentValidation;
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
            .NotEmpty().WithMessage("Card Id is required");
            
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Card Name is required");

            RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Card Status is required")
            .Must(BeAValidEnumValue).WithMessage("Card Status must be a valid Status value");

            // Conditional validation for Color
            // Only apply the regex rule if Color is not null or empty
            When(x => !string.IsNullOrEmpty(x.Color), () =>
            {
                RuleFor(x => x.Color)
                    .Matches("^#[0-9a-fA-F]{6}$")
                    .WithMessage("Color should be in the format #RRGGBB where RR, GG, and BB are hexadecimal values.");
            });

            RuleFor(x => x).Custom((data, context) =>
            {
                var isExist = _uow.Repository<CardEntity>().Exist(x => x.UserId == data.UserId && x.Id == data.CardId && !x.IsDeleted);
                if (!isExist)
                    throw new CustomException(BaseStrings.CARD_NOT_EXIST);

                isExist = _uow.Repository<CardEntity>().Exist(x => x.UserId == data.UserId && x.Name == data.Name && x.Id != data.CardId && !x.IsDeleted);
                if (isExist)
                    throw new CustomException(BaseStrings.CARD_ALREADY_EXIST);
            });
        }

        private bool BeAValidEnumValue(string cardStatus)
        {
            return Enum.TryParse<Status>(cardStatus, true, out _);
        }
    }
}
