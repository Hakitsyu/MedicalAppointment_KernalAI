using MedicalAppointment_KernalAI;
using MedicalAppointment_KernalAI.Configuration.Extensions;
using MedicalAppointment_KernalAI.Configuration.Providers.OpenAI.Extensions;
using MedicalAppointment_KernalAI.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Warning);
    })
    .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsettings.json"));

builder.ConfigureServices((context, services) =>
{
    services.AddHostedService<Chat>();

    var openAIConfiguration = context.Configuration.GetSection("Kernel:Providers:OpenAI");
    services
        .ConfigureKernel(context.Configuration, kernel =>
        {
            kernel.Services
                .AddLogging(logging =>
                    logging.AddDebug().SetMinimumLevel(LogLevel.Information))
                .UseOpenAITextToImage(openAIConfiguration);

            kernel.Plugins.AddFromType<SickNotesPlugin>();

        })
        .UseOpenAI(openAIConfiguration);
});

var host = builder.Build();
await host.RunAsync();