using MediatR;

namespace CardService.Application.UseCases.User.Commands
{
    public sealed record AuthenticateUserCommand(
        string Email,
        string Password) : IRequest<string?>;
}
