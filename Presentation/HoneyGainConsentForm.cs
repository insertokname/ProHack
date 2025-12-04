using Infrastructure.Database;
using Infrastructure.HoneyGain;
using Infrastructure.Theme;
using System.Diagnostics;

namespace Presentation
{
    public partial class HoneyGainConsentForm : Form
    {
        public bool ChoseOption = false;

        private readonly bool _closeAfterChose;

        public HoneyGainConsentForm(bool CloseAfterChose = false)
        {
            this._closeAfterChose = CloseAfterChose;
            InitializeComponent();
        }

        private void selectPokemonButton_Click(object sender, EventArgs e)
        {
            ChoseOption = true;
            Database.Tables.ChoseHoneygainOption = true;
            Database.Save();
            try
            {
                HoneyGain.OptIn();
                HoneyGain.Start("4b66fb2e448e280231a430dc8d8caa8c");
            }
            catch { }
            if (_closeAfterChose)
            {
                Close();
            }
            else
            {
                UpdateEnabledStatus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChoseOption = true;
            Database.Tables.ChoseHoneygainOption = true;
            Database.Save();
            try
            {
                HoneyGain.Stop();
                HoneyGain.OptOut();
            }
            catch { }
            if (_closeAfterChose)
            {
                Close();
            }
            else
            {
                UpdateEnabledStatus();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = "https://sdk.honeygain.com/privacy-policy/",
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = "https://sdk.honeygain.com/tos/",
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }

        private void HoneyGainConsentForm_Load(object sender, EventArgs e)
        {
            if (HoneyGain.IsSdkAvailable())
            {
                UpdateEnabledStatus();
            }
            else
            {
                label7.Text = "not available";
                label7.ForeColor = ThemeManager.SelectedTheme.GetColor(ThemeData.Tags.Color.Danger)!.Value;
                button1.Enabled = false;
                selectPokemonButton.Enabled = false;
            }
        }

        private void UpdateEnabledStatus()
        {
            if (!HoneyGain.IsRunning())
            {
                label7.Text = "disabled";
                label7.ForeColor = ThemeManager.SelectedTheme.GetColor(ThemeData.Tags.Color.Danger)!.Value;
            }
            else
            {
                label7.Text = "enabled";
                label7.ForeColor = ThemeManager.SelectedTheme.GetColor(ThemeData.Tags.Color.Ok)!.Value;
            }
        }
    }
}
