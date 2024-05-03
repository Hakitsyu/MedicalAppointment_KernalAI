using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalAppointment_KernalAI
{
    public class Chat : BackgroundService
    {
        private readonly Kernel _kernel;

        public Chat(Kernel kernel)
        {
            _kernel = kernel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ChatHistory chatMessages = new ChatHistory(@"
               My name is Jam and I am an experienced doctor with over 50 years of experience as a general practitioner, currently working at a Hospital in Toronto, Canada. I work to improve the lives of my patients.

For all patients treated, I request information that adds value to the outcome of the consultation.
The patient needs to tell me at least the following information for me to reach a conclusion:
- What symptoms?
- How long have you had symptoms?
- Age?
- Gender?

Due to the large volume of Dengue and Covid cases, these two diseases need to be alert.
If I believe the symptoms are Covid or Dengue, I request a vaccine test to confirm it is one of these two diseases.

When I understand or believe I understand the problem reported by the patient, I write a prescription with the medications that should help the patient.");
            IChatCompletionService chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.Write("User > ");
                chatMessages.AddUserMessage(Console.ReadLine()!);

                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
                };

                IAsyncEnumerable<StreamingChatMessageContent> result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                        chatMessages,
                        executionSettings: openAIPromptExecutionSettings,
                        kernel: _kernel,
                        cancellationToken: stoppingToken);

                ChatMessageContent? chatMessageContent = null;
                await foreach (var content in result)
                {
                    if (content.Role.HasValue)
                    {
                        Console.Write("Assistant > ");
                        chatMessageContent = new(
                            content.Role ?? AuthorRole.Assistant,
                            content.ModelId!,
                            content.Content!,
                            content.InnerContent,
                            content.Encoding,
                            content.Metadata
                        );
                    }

                    Console.Write(content.Content);
                    chatMessageContent!.Content += content.Content;
                }

                Console.WriteLine();
                chatMessages.AddMessage(chatMessageContent!);
            }
        }
    }
}
