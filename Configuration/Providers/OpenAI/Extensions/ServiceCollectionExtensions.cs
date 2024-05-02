using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        static IServiceCollection AddOpenAISettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenAISettings>(
                configuration);

            return services;
        }
    }
}
