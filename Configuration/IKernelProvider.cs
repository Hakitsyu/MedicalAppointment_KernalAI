using Microsoft.SemanticKernel;

namespace MedicalAppointment_KernalAI.Configuration
{
    public interface IKernelProvider
    {
        void Configure(IKernelBuilder kernelBuilder);
    }
}
