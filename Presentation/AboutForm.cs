using Infrastructure;
using System.Diagnostics;

namespace Presentation
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new()
            {
                FileName = "https://www.buymeacoffee.com/insertokname",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new()
            {
                FileName = "https://github.com/insertokname/ProHack",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HoneyGainConsentForm honeyGainConsentForm = new HoneyGainConsentForm();
            honeyGainConsentForm.ShowDialog();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            label1.Text = $"App version: {VersionManager.GetVersionCode()}";
        }

        private void label3_Click(object sender, EventArgs e)
        {
            PrivacyPolicyForm privacyPolicyForm= new();
            privacyPolicyForm.ShowDialog();
        }
    }
}
