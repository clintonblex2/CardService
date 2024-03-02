using CardService.Application.Common.Behaviors;
using CardService.Application.Common.Extensions;
using CardService.Application.Common.Helpers;
using CardService.Application.Common.Interfaces;
using CardService.Application.Common.Models.Configurations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CardService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddConfig<AppSettings>(configuration.GetSection("AppSettings"));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }
    }
}
