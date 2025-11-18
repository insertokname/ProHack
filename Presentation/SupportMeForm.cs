using System.Diagnostics;

namespace Presentation
{
    public partial class SupportMeForm : Form
    {
        public SupportMeForm()
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
    }
}
