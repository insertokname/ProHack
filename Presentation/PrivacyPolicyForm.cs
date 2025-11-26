using Infrastructure.Database;
using Presentation.Properties;

namespace Presentation
{
    public partial class PrivacyPolicyForm : Form
    {
        public bool accepted = false;
        public PrivacyPolicyForm()
        {
            InitializeComponent();
        }

        private void PrivacyPolicyForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Resources.privacy_policy.ToString();
        }

        private void button_Click(object sender, EventArgs e)
        {
            Database.Tables.AgreedToPrivacyPolicy = true;
            Database.Save();
            accepted = true;
            Close();
        }
    }
}
