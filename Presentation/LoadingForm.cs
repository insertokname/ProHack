using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Discord;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using static Infrastructure.UpdateManager;

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
            label1.Text = "Loading database";
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
            });

            if (Database.Tables.ShowWarningMessage)
            {
                WarningForm warningForm = new();
                Hide();
                warningForm.ShowDialog();
                if (!warningForm.accepted)
                {
                    System.Windows.Forms.Application.Exit();
                }
                Show();
            }

            if (Database.Tables.CheckUpdates)
            {
                label1.Text = "Updating...";
                UpdateManager updateManager = _serviceProvider.GetRequiredService<UpdateManager>();
                var downloadInfo = await updateManager.GetDownloadInfo();
                var updateState = updateManager.CheckAvailableUpdate(downloadInfo);
                if (updateState == AvailableUpdateState.UpdateAvailable && downloadInfo != null && Database.Tables.SkipUpdateVersion != downloadInfo.VersionCode)
                {
                    bool wantsToUpdate = Database.Tables.AutomaticallyUpdate;
                    if (!Database.Tables.AutomaticallyUpdate)
                    {
                        progressBar1.MarqueeAnimationSpeed = int.MaxValue;
                        UpdateRequestForm updateRequestForm = new(downloadInfo, _serviceProvider);
                        updateRequestForm.ShowDialog();
                        progressBar1.MarqueeAnimationSpeed = 15;
                        wantsToUpdate = updateRequestForm.WantsToUpdate;
                    }
                    if (wantsToUpdate)
                    {
                        var curVersionPath = Environment.ProcessPath;

                        progressBar1.Value = 0;
                        progressBar1.Style = ProgressBarStyle.Continuous;
                        var progress = new Progress<double>(value =>
                        {
                            var percent = Math.Max(0, Math.Min(100, (int)(value * 100)));
                            if (progressBar1.InvokeRequired)
                                progressBar1.Invoke(() => progressBar1.Value = percent);
                            else
                                progressBar1.Value = percent;
                        });

                        var newVersionPath = await updateManager.DownloadNewVersion(downloadInfo, progress);
                        progressBar1.Style = ProgressBarStyle.Marquee;
                        progressBar1.Value = 50;
                        if (newVersionPath != null && curVersionPath != null)
                        {
                            await UpdateAndReboot(curVersionPath, newVersionPath);
                        }
                    }
                }
                updateManager.Dispose();
            }

            label1.Text = "Loading pokedex";
            await Task.Run(() =>
            {
                try
                {
                    _serviceProvider.GetRequiredService<PokedexManager>().Load();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load pokedex: {ex}");
                }
            });

            label1.Text = "Starting discord bot";
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
