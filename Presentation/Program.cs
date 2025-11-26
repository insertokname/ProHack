using Infrastructure;
using Infrastructure.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Presentation
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            System.Windows.Forms.Application.ThreadException
                += new ThreadExceptionEventHandler(ThreadExceptionEventHandler);

            ApplicationConfiguration.Initialize();

            var host = CreateHostBuilder().Build();
            var serviceProvider = host.Services!;

            System.Windows.Forms.Application.Run(serviceProvider.GetRequiredService<LoadingForm>());
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Forms
                    services.AddTransient<MainBotForm>();
                    services.AddTransient<DiscordBotOptionsForm>();
                    services.AddTransient<DialogueSettingsForm>();
                    services.AddTransient<PokemonSelectForm>();
                    services.AddTransient<LoadingForm>();

                    //Singletons
                    services.AddSingleton<DiscordBot>();
                    services.AddSingleton<MemoryManager>();
                    services.AddSingleton<PokedexManager>();

                    //Services
                    services.AddHttpClient();
                    services.AddTransient<UpdateManager>();
                    services.AddTransient<UpdateDataManager>();
                    services.AddTransient<LoginTrackerManager>();
                });
        }

        public static event EventHandler<ThreadExceptionEventArgs>? OnThreadException;

        private static void ThreadExceptionEventHandler(object sender, ThreadExceptionEventArgs e)
        {
            OnThreadException?.Invoke(sender, e);
        }
    }
}