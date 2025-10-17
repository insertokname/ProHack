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
        public List<PokemonTargetModel> PokemonTargetModels { get; set; } = [];

        private List<PokemonSelectListItemControl> panelItems = [];

        public PokemonSelect()
        {
            InitializeComponent();
        }

        private void PokemonSelect_Load(object sender, EventArgs e)
        {
            foreach (var model in PokemonTargetModels)
            {
                AddModelToList(model);
            }
        }
        private void retryButton_Click(object sender, EventArgs e)
        {
            var model = GetPokemonTargetModel();
            PokemonTargetModels.Add(model);
            AddModelToList(model);
        }

        private void AddModelToList(PokemonTargetModel model)
        {
            var newItem = new PokemonSelectListItemControl(model);
            panelItems.Add(newItem);
            newItem.Parent = flowLayoutPanel1;

            newItem.button1.Click += (s, e) =>
            {
                var indx = panelItems.IndexOf(newItem);
                PokemonTargetModels.RemoveAt(indx);
                panelItems.RemoveAt(indx);
                newItem.Dispose();
            };
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
                MustBeEvent = mustBeEventCheckbox.Checked,
                MustBeShiny = mustBeShinyCheckbox.Checked,
            };

            catchAnythingCheckbox.Checked = true;
            mustBeEventCheckbox.Checked = false;
            mustBeShinyCheckbox.Checked = false;

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

        private void mustBeEventCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            mustBeShinyCheckbox.Checked = false;
            mustBeShinyCheckbox.Visible = !mustBeEventCheckbox.Checked;
        }

        private void mustBeShinyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            mustBeEventCheckbox.Checked = false;
            mustBeEventCheckbox.Visible = !mustBeShinyCheckbox.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
