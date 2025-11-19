using Infrastructure.Database;
using Infrastructure.Theme;
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
    public partial class ThemeSettingsForm : Form
    {
        public ThemeSettingsForm()
        {
            InitializeComponent();
        }

        private void ThemeSettingsForm_Load(object sender, EventArgs e)
        {
            if (ThemeManager.SelectedTheme is Themes.GruvboxLight)
            {
                radioButton2.Checked = true;
            }
            else if (ThemeManager.SelectedTheme is Themes.Classic)
            {
                radioButton3.Checked = true;
            }
            else
            {
                radioButton1.Checked = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.SelectedTheme = new Themes.Gruvbox();
            Database.Tables.CurrentTheme = ThemeManager.ToString(ThemeManager.SelectedTheme);
            Database.Save();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.SelectedTheme = new Themes.GruvboxLight();
            Database.Tables.CurrentTheme = ThemeManager.ToString(ThemeManager.SelectedTheme);
            Database.Save();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.SelectedTheme = new Themes.Classic();
            Database.Tables.CurrentTheme = ThemeManager.ToString(ThemeManager.SelectedTheme);
            Database.Save();
        }

        private void radioButton4_CheckedChanged_1(object sender, EventArgs e)
        {

        }
    }
}
