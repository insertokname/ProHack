using Infrastructure;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using static Infrastructure.UpdateManager;

namespace Presentation
{
    public partial class UpdateSettingsForm : Form
    {
        private readonly IServiceProvider _serviceProvider;

        private bool isUpdating = false;

        public UpdateSettingsForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (isUpdating)
            {
                return;
            }

            button1.Enabled = false;
            isUpdating = true;
            button1.Text = "Updating...";
            UpdateManager updateManager = _serviceProvider.GetRequiredService<UpdateManager>();
            var downloadInfo = await updateManager.GetDownloadInfo();
            var updateState = updateManager.CheckAvailableUpdate(downloadInfo);
            if (updateState != AvailableUpdateState.UpdateAvailable || downloadInfo == null)
            {
                MessageBox.Show("No update was found!");
                goto Cleanup;
            }
            else
            {
                var res = MessageBox.Show($"Newer version found ({downloadInfo.VersionCode})!\nRelease notes:\n{downloadInfo.Body}\nDo you want to update?", "Update available", MessageBoxButtons.YesNo);
                if (res != DialogResult.Yes)
                {
                    goto Cleanup;
                }
            }
            Size = new Size(342, 306);
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
            if (newVersionPath == null)
            {
                MessageBox.Show("An error occured while downloading the newest version! Try again later!");
                goto Cleanup;
            }
            var curVersionPath = Environment.ProcessPath;
            if (curVersionPath == null)
            {
                MessageBox.Show("An error occured while getting the path of the exe! Try again later!");
                goto Cleanup;
            }

            await UpdateAndReboot(curVersionPath, newVersionPath);

        Cleanup:
            Size = new Size(342, 263);
            updateManager.Dispose();
            button1.Enabled = true;
            isUpdating = false;
            button1.Text = "Check for updates";
        }

        private void autoCheckUpdatesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoUpdateCheckBox.Checked)
            {
                autoCheckUpdatesCheckbox.Checked = true;
            }
            Database.Tables.CheckUpdates = autoCheckUpdatesCheckbox.Checked;
            Database.Save();
        }

        private void autoUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Database.Tables.AutomaticallyUpdate = autoUpdateCheckBox.Checked;
            if (autoUpdateCheckBox.Checked)
            {
                autoCheckUpdatesCheckbox.Checked = true;
                Database.Tables.CheckUpdates = true;
            }
            Database.Save();
        }

        private void UpdateSettingsForm_Load(object sender, EventArgs e)
        {
            autoCheckUpdatesCheckbox.Checked = Database.Tables.CheckUpdates;
            autoUpdateCheckBox.Checked = Database.Tables.AutomaticallyUpdate;
            textBox1.Text = Database.Tables.SkipUpdateVersion;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Database.Tables.SkipUpdateVersion = textBox1.Text;
            Database.Save();
        }
    }
}
