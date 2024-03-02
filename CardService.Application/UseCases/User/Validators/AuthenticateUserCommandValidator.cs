using CardService.Application.UseCases.User.Commands;
using FluentValidation;

namespace CardService.Application.UseCases.User.Validators
{
    public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
    {
        public AuthenticateUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
