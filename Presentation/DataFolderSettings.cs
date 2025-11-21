using Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation
{
    public partial class DataFolderSettings : Form
    {
        public DataFolderSettings()
        {
            InitializeComponent();
        }

        private void DataFolderSettings_Load(object sender, EventArgs e)
        {
            textBox1.Text = Database.Tables.DataDownloadUrl;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Database.Tables.DataDownloadUrl = textBox1.Text;
            Database.Save();
        }
    }
}
