using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment_KernalAI.Configuration.Providers.OpenAI
{
    public sealed class OpenAIProvider : IKernelProvider
    {
        private OpenAISettings _settings;

        internal const string Key = "OpenAI";

        public OpenAIProvider(IOptions<OpenAISettings> settings)
        {
            _settings = settings.Value;
        }

        public void Configure(IKernelBuilder kernelBuilder)
        {
            kernelBuilder.AddOpenAIChatCompletion(modelId: _settings.ModelId,
                apiKey: _settings.ApiKey,
                orgId: _settings.OrgId,
                serviceId: _settings.ServiceId);
        }
    }
}
