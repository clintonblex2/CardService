using CardService.Application.Common.Exceptions;
using FluentValidation;
using MediatR;

namespace CardService.Application.Common.Behaviors
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);

            List<FluentValidation.Results.ValidationFailure> failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new CustomException(failures);
            }

            return next();
        }
    }
}
