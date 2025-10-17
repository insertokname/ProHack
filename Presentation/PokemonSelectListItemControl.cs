using Domain;
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
    public partial class PokemonSelectListItemControl : UserControl
    {
        private PokemonTargetModel _pokemonTargetModel;
        public PokemonSelectListItemControl(PokemonTargetModel pokemonTargetModel)
        {
            _pokemonTargetModel = pokemonTargetModel;
            InitializeComponent();
        }

        private void PokemonSelectListItemControl_Load(object sender, EventArgs e)
        {
            label1.Text = $"Id: {_pokemonTargetModel.Id?.ToString() ?? "any"}";
            label2.Visible = _pokemonTargetModel.MustBeEvent;
            label3.Visible = _pokemonTargetModel.MustBeShiny;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
