using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CardService.Application.Common.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static T AddConfig<T>(this IServiceCollection services, IConfiguration configuration)
           where T : class, new()
        {
            return services.AddConfig<T>(configuration, options => { });
        }

        public static TSettings AddConfig<TSettings>(this IServiceCollection services, IConfiguration configuration, Action<BinderOptions> configureOptions)
            where TSettings : class, new()
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

            TSettings setting = configuration.Get<TSettings>(configureOptions);
            services.TryAddSingleton(setting);
            return setting;
        }
    }
}
