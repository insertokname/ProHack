using Application;
using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation
{
    public partial class PokemonSelect : Form
    {
        public PokemonTargetModel? PokemonTargetModel { get; set; } = null;

        public PokemonSelect()
        {
            InitializeComponent();
        }

        private void PokemonSelect_Load(object sender, EventArgs e)
        {
            isSpecialComboBox.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(PokemonTargetModel.IsSpecialTargeting)))
            {
                isSpecialComboBox.Items.Add(value);
            }

            isSpecialComboBox.SelectedItem = Constants.PokemonTargetModel.DefaultTarget().specialTargeting;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (PokemonTargetModel != null)
            {
                base.OnClosed(e);
            }
            else
            {
                var res = MessageBox.Show("Do you want to select the current pokemon?", "Warning", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    PokemonTargetModel = GetPokemonTargetModel();
                }
                base.OnClosed(e);
            }
        }
        private void retryButton_Click(object sender, EventArgs e)
        {
            PokemonTargetModel = GetPokemonTargetModel();
            Close();
        }

        private PokemonTargetModel GetPokemonTargetModel()
        {
            int? id = null;
            if (!catchAnythingCheckbox.Checked)
            {
                id = (int)idNumeric.Value;
            }

            var output = new PokemonTargetModel()
            {
                Id = id,
                specialTargeting = (PokemonTargetModel.IsSpecialTargeting)isSpecialComboBox.SelectedItem!,
            };
            return output;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            idLabel.Visible = !catchAnythingCheckbox.Checked;
            idNumeric.Visible = !catchAnythingCheckbox.Checked;
        }

        private void idNumeric_ValueChanged(object sender, EventArgs e)
        {

        }

        private void isSpecialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
