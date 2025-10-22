using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Discord;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Presentation
{
    public partial class LoadingForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        public LoadingForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }


        private void progressBar1_Click(object sender, EventArgs e)
        {
        }

        private async void Startup_Load(object sender, EventArgs e)
        {
            await InitializeAsync();
            MainBotForm form = _serviceProvider.GetRequiredService<MainBotForm>();
            Hide();
            form.ShowDialog();
            Close();
        }

        private async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
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
                    _serviceProvider.GetRequiredService<PokedexManager>().Load();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load pokedex: {ex}");
                }
            });

            try
            {
                var bot = _serviceProvider.GetRequiredService<DiscordBot>();
                await bot.InitCommandService();
                await bot.StartAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start Discord bot: {ex}");
            }
        }
    }
}
