using Infrastructure.Database;
using static Infrastructure.UpdateManager;

namespace Presentation
{
    public partial class UpdateRequestForm : Form
    {
        public bool WantsToUpdate { get; set; } = false;

        private readonly UpdateInfo _updateInfo;
        private readonly IServiceProvider _serviceProvider;

        public UpdateRequestForm(UpdateInfo updateInfo, IServiceProvider serviceProvider)
        {
            _updateInfo = updateInfo;
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        private void UpdateRequestForm_Load(object sender, EventArgs e)
        {
            label1.Text = $"New update available: {_updateInfo.VersionCode}";
            richTextBox1.Text = _updateInfo.Body;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            WantsToUpdate = true;
            Close();
        }

        private void skipUpdateButton_Click(object sender, EventArgs e)
        {
            Database.Tables.SkipUpdateVersion = _updateInfo.VersionCode;
            Database.Save();
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateSettingsForm updateSettingsForm = new(_serviceProvider);
            updateSettingsForm.ShowDialog();
        }
    }
}
