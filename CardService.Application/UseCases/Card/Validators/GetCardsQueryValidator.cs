using CardService.Application.Common.Helpers;
using CardService.Application.UseCases.Card.Queries;
using CardService.Domain.Enums;
using FluentValidation;

namespace CardService.Application.UseCases.Card.Validators
{
    public class GetCardsQueryValidator : AbstractValidator<GetCardsQuery>
    {
        public GetCardsQueryValidator()
        {
            // Conditional validation for SortBy
            When(x => !string.IsNullOrEmpty(x.SortBy), () =>
            {
                RuleFor(x => x.SortBy)
                .Must(BeAValidEnumValue).WithMessage(BaseStrings.INVALID_SORT_BY);
            });

        }

        private bool BeAValidEnumValue(string sortBy)
        {
            return Enum.TryParse<SortBy>(sortBy, false, out _);
        }
    }
}
