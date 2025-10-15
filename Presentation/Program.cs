using Application;
using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Presentation
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            System.Windows.Forms.Application.ThreadException
                += new ThreadExceptionEventHandler(ThreadExceptionEventHandler);

            ApplicationConfiguration.Initialize();

            var host = CreateHostBuilder().Build();
            var serviceProvider = host.Services!;

            try
            {
                Database.Load();
            }
            catch
            {
                Database.Tables = new DatabaseTables();
            }
            Database.Save();

            try
            {
                var bot = serviceProvider.GetRequiredService<DiscordBot>();
                await bot.InitCommandService();
                await bot.StartAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start Discord bot: {ex}");
            }

            System.Windows.Forms.Application.Run(serviceProvider.GetRequiredService<Form1>());
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<Form1>();
                    services.AddTransient<DiscordBotOptions>();

                    services.AddSingleton<DiscordBot>();
                    services.AddSingleton<MemoryManager>();
                });
        }

        public static event EventHandler<ThreadExceptionEventArgs>? OnThreadException;

        private static void ThreadExceptionEventHandler(object sender, ThreadExceptionEventArgs e)
        {
            OnThreadException?.Invoke(sender, e);
        }
    }
}