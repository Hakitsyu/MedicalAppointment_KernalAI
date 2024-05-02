using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace MedicalAppointment_KernalAI.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureKernel(this IServiceCollection services, IConfiguration configuration, Action<KernelBuilder> configureBuilder)
        {
            services.AddKernelSettings(configuration);

            services.AddTransient<Kernel>((serviceProvider) =>
            {
                KernelBuilder builder = new();
                configureBuilder(builder);

                var kernalSettings = serviceProvider.GetRequiredService<IOptions<KernelSettings>>().Value;

                var provider = serviceProvider.GetRequiredKeyedService<IKernelProvider>(kernalSettings.Provider);

                provider.Configure(builder);

                return builder.Build();
            });

            return services;
        }

        static IServiceCollection AddKernelSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KernelSettings>(configuration.GetSection("Kernel"));

            return services;
        }
    }
}
