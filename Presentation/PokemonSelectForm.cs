using Domain;
using Infrastructure;
using Infrastructure.Theme;
using Presentation.Properties;
using System.Xml.Serialization;

namespace Presentation
{
    public partial class PokemonSelectForm : Form
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public List<PokemonTargetModel> PokemonTargetModels { get; set; } = [];

        private readonly XmlSerializer _serializer = new(typeof(List<PokemonTargetModel>));
        private readonly PokedexManager _pokedexManager;

        private readonly List<PokemonSelectListItemControl> _panelItems = [];

        private List<Bitmap> _carouselImages = [];
        private int _carouselIndex = 0;


        public PokemonSelectForm(PokedexManager pokedexManager)
        {
            _pokedexManager = pokedexManager;
            InitializeComponent();
        }

        #region event overrides

        private void PokemonSelect_Load(object sender, EventArgs e)
        {
            InitializeTargetList();
            IdNumeric.Maximum = _pokedexManager.MaxIndex ?? 10000;
            PokemonNameComboBox.DataSource = _pokedexManager.Pokedex?.Entries ?? [];
            PokemonNameComboBox.DisplayMember = "Name";
        }

        private void AddTargetButton_Click(object sender, EventArgs e)
        {
            var model = GetPokemonTargetModel();
            PokemonTargetModels.Add(model);
            AddModelToTargetList(model);
        }

        private void CatchAnythingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            idLabel.Visible = !CatchAnythingCheckbox.Checked;
            IdNumeric.Visible = !CatchAnythingCheckbox.Checked;
            PokemonNameLabel.Visible = !CatchAnythingCheckbox.Checked;
            PokemonNameComboBox.Visible = !CatchAnythingCheckbox.Checked;
            OtherFormsComboBox.Visible = !CatchAnythingCheckbox.Checked;
            OtherFormsLabel.Visible = !CatchAnythingCheckbox.Checked;
            UpdateDisplay();
        }

        private void IdNumeric_ValueChanged(object sender, EventArgs e)
        {
            int curId = (int)IdNumeric.Value;

            if (_pokedexManager.Pokedex == null)
            {
                UpdateDisplay();
                return;
            }

            if (!_pokedexManager.Pokedex.Entries.Any(e => e.ID == curId))
            {
                var closestIndex = _pokedexManager.Pokedex.Entries.MinBy(e => Math.Abs(e.ID - curId))?.ID;
                if (closestIndex == null)
                {
                    UpdateDisplay();
                    return;
                }

                IdNumeric.Value = closestIndex.Value;
            }

            UpdateDisplay();
        }

        private void PokemonNameComboBox_Click(object sender, EventArgs e)
        {
            PokemonNameComboBox.DroppedDown = true;
        }

        private void PokemonNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_pokedexManager.Pokedex != null)
            {
                var tmp = (PokedexModel.PokedexEntryModel?)PokemonNameComboBox.SelectedItem;
                if (tmp != null && IdNumeric.Value != tmp.ID)
                {
                    IdNumeric.Value = tmp.ID;
                }
            }
        }

        private void OtherFormsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var item = OtherFormsComboBox.SelectedItem as dynamic;
            if (DialogResult.Yes == MessageBox.Show(
                $"Do you want to switch pokemon form to: {item?.Name}?",
                "Switch form?",
                MessageBoxButtons.YesNo))
            {
                var id = item?.ID;
                if (id is int idInt)
                {
                    IdNumeric.Value = idInt;
                }
            }
        }

        private void MustBeEventCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            MustBeShinyCheckbox.Checked = false;
            MustBeShinyCheckbox.Visible = !MustBeEventCheckbox.Checked;
            UpdateDisplay();
        }

        private void MustBeShinyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            MustBeEventCheckbox.Checked = false;
            MustBeEventCheckbox.Visible = !MustBeShinyCheckbox.Checked;
            UpdateDisplay();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            _carouselIndex += 1;
            UpdateCarouselIndex();
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            _carouselIndex -= 1;
            UpdateCarouselIndex();
        }

        private void PokemonSelect_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.A)
            //{
            //    _carouselIndex += 1;
            //    UpdateCarouselIndex();
            //}
            //if (e.KeyCode == Keys.D)
            //{
            //    _carouselIndex -= 1;
            //    UpdateCarouselIndex();
            //}
            //if (e.KeyCode == Keys.W)
            //{
            //    if (!CatchAnythingCheckbox.Checked)
            //    {
            //        IdNumeric.Value++;
            //    }
            //}
            //if (e.KeyCode == Keys.S)
            //{
            //    if (!CatchAnythingCheckbox.Checked)
            //    {
            //        IdNumeric.Value--;
            //    }
            //}
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "target file (*.trgt)|*.trgt",
                Title = "Open target",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader sr = new(openFileDialog.FileName);
                    try
                    {
                        var newTargets = (List<PokemonTargetModel>)_serializer.Deserialize(sr)!;
                        PokemonTargetModels.Clear();
                        foreach (var panel in _panelItems)
                        {
                            panel.Dispose();
                        }
                        _panelItems.Clear();

                        PokemonTargetModels = newTargets;
                        InitializeTargetList();
                    }
                    catch { }
                    sr.Close();
                }
                catch { }
            }
        }

        private void SaveTargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "target file (*.trgt)|*.trgt",
                Title = "Save target",
                RestoreDirectory = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter sw = new(saveFileDialog.FileName);
                    try
                    {
                        _serializer.Serialize(sw, PokemonTargetModels);
                    }
                    catch { }
                    sw.Close();
                }
                catch { }
            }
        }
        #endregion

        #region business logic

        public void InitializeTargetList()
        {
            foreach (var model in PokemonTargetModels)
            {
                AddModelToTargetList(model);
            }
        }

        public void UpdateDisplay()
        {
            pictureBox1.Image?.Dispose();
            RightButton.Visible = false;
            LeftButton.Visible = false;

            if (CatchAnythingCheckbox.Checked)
            {
                pictureBox1.Image = Resources.any;
                return;
            }

            var intId = (int)IdNumeric.Value;
            var entry = _pokedexManager.Pokedex?.Entries.FirstOrDefault(e => e.ID == intId);
            if (entry == null)
            {
                pictureBox1.Image = Resources.unknown;
                return;
            }
            PokemonNameComboBox.SelectedItem = entry;
            var list = entry.LinkedDexEntries?
                .Select(lde =>
                {
                    var linkedPkmn =
                        _pokedexManager?
                        .Pokedex?
                        .Entries
                        .FirstOrDefault(p => p.ID == lde);

                    return new
                    {
                        Name = linkedPkmn?.Name ?? lde.ToString(),
                        ID = lde
                    };
                })
                .ToList();
            OtherFormsComboBox.DisplayMember = "Name";
            OtherFormsComboBox.DataSource = list;

            if (MustBeEventCheckbox.Checked || MustBeShinyCheckbox.Checked)
            {
                UpdateCarouselDisplay(entry);
            }
            else
            {
                pictureBox1.Image = _pokedexManager.GetAsset(entry);
            }
        }

        private void UpdateCarouselDisplay(PokedexModel.PokedexEntryModel entry)
        {
            RightButton.Visible = true;
            LeftButton.Visible = true;

            foreach (var bmp in _carouselImages)
            {
                bmp.Dispose();
            }
            _carouselImages.Clear();
            _carouselImages = MustBeShinyCheckbox.Checked ? _pokedexManager.GetShinyAssets(entry) : _pokedexManager.GetEventAssets(entry);

            if (_carouselImages.Count <= 0)
            {
                pictureBox1.Image = Resources.unknown;
                return;
            }

            UpdateCarouselIndex();
        }

        public void UpdateCarouselIndex()
        {
            if (_carouselImages.Count > 0)
            {
                var indx = _carouselIndex % _carouselImages.Count;
                while (indx < 0)
                {
                    indx += _carouselImages.Count;
                }
                pictureBox1.Image = _carouselImages[indx];
            }
        }

        public void AddModelToTargetList(PokemonTargetModel model)
        {
            var newItem = new PokemonSelectListItemControl(model);
            ThemeApplier.ApplyThemeToChildrenRecursive(newItem);
            _panelItems.Add(newItem);
            newItem.Parent = flowLayoutPanel1;

            newItem.button1.Click += (s, e) =>
            {
                var indx = _panelItems.IndexOf(newItem);
                PokemonTargetModels.RemoveAt(indx);
                _panelItems.RemoveAt(indx);
                newItem.Dispose();
            };
        }

        private PokemonTargetModel GetPokemonTargetModel()
        {
            int? id = null;
            if (!CatchAnythingCheckbox.Checked)
            {
                id = (int)IdNumeric.Value;
            }

            var output = new PokemonTargetModel()
            {
                Id = id,
                MustBeEvent = MustBeEventCheckbox.Checked,
                MustBeShiny = MustBeShinyCheckbox.Checked,
            };

            CatchAnythingCheckbox.Checked = true;
            MustBeEventCheckbox.Checked = false;
            MustBeShinyCheckbox.Checked = false;
            UpdateDisplay();

            return output;
        }
        #endregion

    }
}
