using Application;
using Domain;
using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Discord;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Properties;
using System.Diagnostics;

namespace Presentation
{
    public partial class MainBotForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MemoryManager _memoryManager;
        private AutoFarm? _autoFarm = null;
        private readonly List<PointF> _registeredPositions = [];
        private List<PokemonTargetModel> _pokemonTargetModels = Constants.PokemonTargetModel.DefaultTarget();
        private StatsViewForm? statsView = null;
        private DateTime _timeStarted = DateTime.Now;


        public MainBotForm(
            IServiceProvider serviceProvider,
            MemoryManager memoryManager)
        {
            _serviceProvider = serviceProvider;
            _memoryManager = memoryManager;

            InitializeComponent();
            Program.OnThreadException += HandleUncaughtException;
            ShortCutManager.RegisterHotKey(Handle, ShortCutManager.HOTKEY_ID, 2, (int)Keys.F8);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadGame();
            UpdateShowBuyMeACoffeeButton();
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

        public void UpdateShowBuyMeACoffeeButton()
        {
            var show = Database.Tables.ShowBuyMeACoffee && Database.Tables.LoginCount > 4;
            dontShowBuyMeACoffeeButton.Visible = show;
            dontShowBuyMeACoffeeButton.Enabled = show;
            if (show)
            {
                Size = new Size(554, 794);
            }
            else
            {
                Size = new Size(554, 741);
            }
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
            _autoFarm?.StopThread();
            _autoFarm = null;

            try
            {

                _autoFarm = new(
                    _serviceProvider.GetRequiredService<DiscordBot>(),
                    _memoryManager,
                    GetPointComponentBySelectedAxis(_registeredPositions[0]),
                    GetPointComponentBySelectedAxis(_registeredPositions[1]),
                    _pokemonTargetModels,
                    selectAxisCheckbox.Checked);
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
            posXLabel.Text = _memoryManager.PlayerXPos.ToString();
            posYLabel.Text = _memoryManager.PlayerYPos.ToString();

            bool isBattling = _memoryManager.IsBattling;

            UpdateIsBattling(isBattling);
            if (isBattling)
            {
                currentEncounterIdLabel.Text = _memoryManager.CurrentEncounterId.ToString();
                noMenuLabel.Visible = _memoryManager.IsNoMenuSelected;
                isShiny.Visible = _memoryManager.ShinyForm != 0;
                isEvent.Visible = _memoryManager.EventForm != 0;
            }
            else
            {
                currentEncounterIdLabel.Text = Constants.Default.NO_CURRENT_ENCOUNTER;
                noMenuLabel.Visible = false;
                isShiny.Visible = false;
                isEvent.Visible = false;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            StartBot();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var registeredPoint = _memoryManager.PlayerPos;
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
            PokemonSelectForm pokemonSelect = _serviceProvider.GetRequiredService<PokemonSelectForm>();
            pokemonSelect.PokemonTargetModels = _pokemonTargetModels;
            pokemonSelect.ShowDialog();
        }

        private void test1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (statsView == null)
            {
                statsView = new StatsViewForm();
                statsView.Show();
                statsView.FormClosed += (s, e) => { statsView = null; };
            }
        }

        private void timeSinceStartedTimer_Tick(object sender, EventArgs e)
        {
            timeSinceStartLabel.Text = (_timeStarted - DateTime.Now).ToString("hh':'mm':'ss");
        }


        private void dialogueSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new()
            {
                FileName = "https://www.buymeacoffee.com/insertokname",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            Database.Tables.ShowBuyMeACoffee = false;
            Database.Save();
            UpdateShowBuyMeACoffeeButton();
        }

        private void dialogueSettingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogueSettingsForm dialogueSettings = _serviceProvider.GetRequiredService<DialogueSettingsForm>();
            dialogueSettings.ShowDialog();
        }

        private void discordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DiscordBotOptionsForm botOptions = _serviceProvider.GetRequiredService<DiscordBotOptionsForm>();
            botOptions.ShowDialog();
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupportMeForm supportMeForm = new();
            supportMeForm.ShowDialog();
        }

        private void updateSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateSettingsForm updateSettingsForm = new(_serviceProvider);
            updateSettingsForm.ShowDialog();
        }

        private void themeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeSettingsForm themeSettingsForm = new();
            themeSettingsForm.ShowDialog();
        }

        private void dataFolderSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataFolderSettings dataFolderSettings = new();
            dataFolderSettings.ShowDialog();
        }
    }
}
