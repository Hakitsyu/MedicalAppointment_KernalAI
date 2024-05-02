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
            ChatHistory chatMessages = [];
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
