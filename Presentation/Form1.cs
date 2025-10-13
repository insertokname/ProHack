using Application;
using Domain;
using Presentation.Properties;
using SimpleMem;
using System.Diagnostics;

namespace Presentation
{
    public partial class Form1 : Form
    {
        private readonly MemoryManager _memoryManager;
        private AutoFarm? _autoFarm = null;
        private readonly List<PointF> _registeredPositions = [];
        private PokemonTargetModel? _pokemonTargetModel = null;
        private StatsView? statsView = null;
        private DateTime _timeStarted = DateTime.Now;

        public Form1()
        {
            InitializeComponent();
            _memoryManager = new();
            Program.OnThreadException += HandleUncaughtException;
            ShortCutManager.RegisterHotKey(Handle, ShortCutManager.HOTKEY_ID, 2, (int)Keys.F8);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadGame();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            statsView?.Close();
            base.OnFormClosing(e);
            _autoFarm?.StopThread();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == ShortCutManager.HOTKEY_ID)
            {
                StopBot();
            }
            base.WndProc(ref m);
        }

        private void LoadGame()
        {
            UpdateGameLoaded(_memoryManager.LoadGame());
        }

        private void HandleUncaughtException(object? sender, ThreadExceptionEventArgs e)
        {
            UpdateGameLoaded(false, e.Exception.Message);
        }

        public void UpdateGameLoaded(bool isGameOpened, string? errorMessage = null)
        {
            if (isGameOpened)
            {
                gameLabel.Text = Constants.GameStatus.FOUND_TEXT;
                gameLabel.ForeColor = Constants.GameStatus.FOUND_COLOR;
                UpdateRegisteredPoints();
                selectPokemonButton.Visible = true;
            }
            else
            {
                gameLabel.Text = Constants.GameStatus.NOT_FOUND_TEXT;
                gameLabel.ForeColor = Constants.GameStatus.NOT_FOUND_COLOR;
                StopBot();
                _registeredPositions.Clear();
                UpdateRegisteredPoints();
                registerPositionButton.Visible = false;
                selectPokemonButton.Visible = false;
            }

            if (errorMessage != null)
            {
                UpdateErrorTextBox(errorMessage);
            }
            else
            {
                UpdateErrorTextBox(null);
            }

            retryButton.Visible = !isGameOpened;
            renderTimer.Enabled = isGameOpened;
        }

        public void UpdateIsBattling(bool isBattling)
        {
            isBattlingLabel.Visible = isBattling;
        }

        public void UpdateIsSpecial(bool isSpecial)
        {
            IsSpecialEncounter.Visible = isSpecial;
        }

        public void UpdateRegisteredPoints()
        {
            registerPos1Label.Visible = false;
            registerPos2Label.Visible = false;
            clearLastRegisterPositionButton.Visible = false;
            startBotButton.Visible = false;
            selectAxisCheckbox.Visible = false;
            selectAxisPicturebox.Visible = false;

            registerPositionButton.Visible = true;

            if (_registeredPositions.Count >= 1)
            {
                registerPos1Label.Visible = true;
                registerPos1Label.Text = $"position {GetSelectedAxis()} : {GetPointComponentBySelectedAxis(_registeredPositions[0])}";
                clearLastRegisterPositionButton.Visible = true;
                selectAxisCheckbox.Visible = true;
                selectAxisPicturebox.Visible = true;
                UpdateSelectedAxis();
            }

            if (_registeredPositions.Count >= 2)
            {
                registerPos2Label.Visible = true;
                registerPos2Label.Text = $"position {GetSelectedAxis()} : {GetPointComponentBySelectedAxis(_registeredPositions[1])}";
                clearLastRegisterPositionButton.Visible = true;
                registerPositionButton.Visible = false;
                startBotButton.Visible = true;
            }
        }

        public void UpdateSelectedAxis()
        {
            selectAxisPicturebox.Image?.Dispose();
            if (selectAxisCheckbox.Checked)
            {
                selectAxisPicturebox.Image = Resources.yDirection;
            }
            else
            {
                selectAxisPicturebox.Image = Resources.xDirection;
            }
        }

        public void UpdateBot()
        {
            clearLastRegisterPositionButton.Visible = false;
            selectAxisCheckbox.Visible = false;
            startBotButton.Visible = false;
            stopButton.Visible = false;
            errorTextBox.Visible = false;

            if (_autoFarm?.IsRunning != true)
            {
                clearLastRegisterPositionButton.Visible = true;
                selectAxisCheckbox.Visible = true;
                startBotButton.Visible = true;
            }

            if (_autoFarm?.IsRunning == true)
            {
                stopButton.Visible = true;
            }

            if (_autoFarm?.PauseReason != null)
            {
                UpdateErrorTextBox(_autoFarm.PauseReason);
            }
        }

        public void UpdateErrorTextBox(string? errorText)
        {
            if (errorText == null)
            {
                errorTextBox.Visible = false;
            }
            else
            {
                errorTextBox.Text = errorText;
                errorTextBox.Visible = true;
            }
        }

        public float GetPointComponentBySelectedAxis(PointF point)
        {
            if (selectAxisCheckbox.Checked)
            {
                return point.Y;
            }
            else
            {
                return point.X;
            }

        }

        public string GetSelectedAxis()
        {
            if (selectAxisCheckbox.Checked)
            {
                return "Y";
            }
            else
            {
                return "X";
            }
        }

        private void StartBot()
        {
            if (_pokemonTargetModel == null)
            {
                var res = MessageBox.Show(
                    "No pokemon was selected! This means all encountered pokemon will be caught! Are you sure you want to continue?",
                    "Warning",
                    MessageBoxButtons.YesNo);
                if (res != DialogResult.Yes)
                {
                    return;
                }
            }
            _autoFarm?.StopThread();
            _autoFarm = null;

            try
            {

                _autoFarm = new(
                    _memoryManager,
                    GetPointComponentBySelectedAxis(_registeredPositions[0]),
                    GetPointComponentBySelectedAxis(_registeredPositions[1]),
                    selectAxisCheckbox.Checked,
                    _pokemonTargetModel);
                _autoFarm.Start();
                UpdateBot();
            }
            catch (ArgumentException e)
            {
                UpdateGameLoaded(true, e.Message);
                _autoFarm = null;
            }
        }

        private void StopBot()
        {
            _autoFarm?.StopThread();
            UpdateBot();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadGame();
        }

        private void renderTimer_Tick(object sender, EventArgs e)
        {
            posXLabel.Text = _memoryManager.GetPlayerPosX().ToString();
            posYLabel.Text = _memoryManager.GetPlayerPosY().ToString();

            bool isBattling = _memoryManager.GetIsBattling();

            UpdateIsBattling(isBattling);
            if (isBattling)
            {
                currentEncounterIdLabel.Text = _memoryManager.GetCurrentEncounterId().ToString();
                itemMenuSelectedLabel.Visible = _memoryManager.GetIsItemMenuSelected();
                noMenuLabel.Visible = _memoryManager.GetIsNoMenuSelected();
                UpdateIsSpecial(_memoryManager.GetIsSpecialEncounter());
            }
            else
            {
                currentEncounterIdLabel.Text = Constants.Default.NO_CURRENT_ENCOUNTER;
                itemMenuSelectedLabel.Visible = false;
                noMenuLabel.Visible = false;
                UpdateIsSpecial(false);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            StartBot();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var registeredPoint = _memoryManager.GetPlayerPos();
            _registeredPositions.Add(registeredPoint);
            UpdateRegisteredPoints();
        }

        private void clearLastRegisterPositionButton_Click(object sender, EventArgs e)
        {
            _registeredPositions.RemoveAt(_registeredPositions.Count - 1);
            UpdateRegisteredPoints();
        }

        private void selectAxisCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRegisteredPoints();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopBot();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            PokemonSelect pokemonSelect = new();
            pokemonSelect.ShowDialog();
            if (pokemonSelect.PokemonTargetModel != null)
            {
                _pokemonTargetModel = pokemonSelect.PokemonTargetModel;
            }
        }

        private void test1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (statsView == null)
            {
                statsView = new StatsView();
                statsView.Show();
                statsView.FormClosed += (s, e) => { statsView = null; };
            }
        }

        private void timeSinceStartedTimer_Tick(object sender, EventArgs e)
        {
            timeSinceStartLabel.Text = (_timeStarted - DateTime.Now).ToString("hh':'mm':'ss");
        }
    }
}
