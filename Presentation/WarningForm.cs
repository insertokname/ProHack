using Infrastructure.Database;

namespace Presentation
{
    public partial class WarningForm : Form
    {
        public bool accepted = false;
        public WarningForm()
        {
            InitializeComponent();
        }

        private void selectPokemonButton_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                MessageBox.Show("You must read the message and confirm you understand before opening the app!");
                return;
            }

            if (checkBox2.Checked)
            {
                Database.Tables.ShowWarningMessage = false;
                Database.Save();
            }
            accepted = true;
            Close();
        }
    }
}
