using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextToImage;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextToImage;
using Microsoft.SemanticKernel.AI.TextToImage;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace MedicalAppointment_KernalAI.Configuration.Providers.OpenAI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseOpenAI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenAISettings(configuration);

            services.AddKeyedTransient<IKernelProvider, OpenAIProvider>(OpenAIProvider.Key);

            return services;
        }

        public static IServiceCollection UseOpenAITextToImage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenAISettings(configuration);

            services.AddSingleton<ITextToImageService>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IOptions<OpenAISettings>>()
                    .Value;

                var httpClient = serviceProvider.GetService<HttpClient>();

                return new OpenAITextToImageService(
                    configuration.ApiKey,
                    configuration.OrgId,
                    httpClient,
                    serviceProvider.GetService<ILoggerFactory>());
            });

            return services;
        }

        static IServiceCollection AddOpenAISettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenAISettings>(
                configuration);

            return services;
        }
    }
}
