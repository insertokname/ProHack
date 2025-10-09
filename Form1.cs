using SimpleMem;
using System.Diagnostics;

namespace ProQol
{
    public partial class Form1 : Form
    {
        private Memory _memory;
        public bool IsGameOpened;

        public static readonly MultiLevelPtr<float> PlayerXPos = new MultiLevelPtr<float>(0x01502A18, 0xB8, 0x0, 0x19C);
        public static readonly MultiLevelPtr<float> PlayerYPos = new MultiLevelPtr<float>(0x01502A18, 0xB8, 0x0, 0x1A0);
        public static readonly MultiLevelPtr<int> CurrentEncounterId = new MultiLevelPtr<int>(0x01502A18, 0xB8, 0x0, 0x8B4);
        public static readonly MultiLevelPtr<int> IsBattling = new MultiLevelPtr<int>(0x01502A18, 0xB8, 0x0, 0x838);
        public static readonly MultiLevelPtr<int> IsSpecial1 = new MultiLevelPtr<int>(0x01502A18, 0xB8, 0x0, 0x8C0);
        public static readonly MultiLevelPtr<int> IsSpecial2 = new MultiLevelPtr<int>(0x01502A18, 0xB8, 0x0, 0x8C4);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadGame();
        }

        private void LoadGame()
        {
            try
            {
                _memory = new Memory("PROClient", "GameAssembly.dll");
                UpdateGameLoaded(true);
            }
            catch
            {
                UpdateGameLoaded(false);
            }

        }

        public void UpdateGameLoaded(bool isGameOpened)
        {
            if (isGameOpened)
            {
                gameLabel.Text = Constants.GameStatus.FOUND_TEXT;
                gameLabel.ForeColor = Constants.GameStatus.FOUND_COLOR;
            }
            else
            {
                gameLabel.Text = Constants.GameStatus.NOT_FOUND_TEXT;
                gameLabel.ForeColor = Constants.GameStatus.NOT_FOUND_COLOR;
            }
            retryButton.Visible = !isGameOpened;
            renderTimer.Enabled = isGameOpened;
            IsGameOpened = isGameOpened;
        }

        public void UpdateIsBattling(bool isBattling)
        {
            isBattlingLabel.Visible = isBattling;
        }

        public void UpdateIsSpecial(bool isSpecial)
        {
            IsSpecialEncounter.Visible = isSpecial;
        }

        public float? ReadPlayerPosX()
        {
            if (!IsGameOpened)
            {
                return null;
            }
            return _memory.ReadValueFromMlPtr<float>(PlayerXPos);
        }
        public float? ReadPlayerPosY()
        {
            if (!IsGameOpened)
            {
                return null;
            }
            return _memory.ReadValueFromMlPtr<float>(PlayerYPos);
        }

        public int? GetCurrentEncounterId()
        {
            if (!IsGameOpened)
            {
                return null;
            }
            return _memory.ReadValueFromMlPtr<int>(CurrentEncounterId);
        }

        public bool GetIsBattling()
        {
            int? res = null;
            if (IsGameOpened)
            {
                res = _memory.ReadValueFromMlPtr<int>(IsBattling);
            }
            return res != null && res != 0;
        }

        public bool GetIsSpecialEncounter()
        {
            int? res1 = null;
            if (IsGameOpened)
            {
                res1 = _memory.ReadValueFromMlPtr<int>(IsSpecial1);
            }

            int? res2 = null;
            if (IsGameOpened)
            {
                res2 = _memory.ReadValueFromMlPtr<int>(IsSpecial2);
            }
            return ((res1 != null && res1 != 0) || (res2 != null && res2 != 0));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadGame();
        }

        private void renderTimer_Tick(object sender, EventArgs e)
        {
            if (_memory.Process.HasExited)
            {
                UpdateGameLoaded(false);
            }
            posXLabel.Text = ReadPlayerPosX()?.ToString() ?? Constants.Default.POSITION_NOT_FOUND;
            posYLabel.Text = ReadPlayerPosY()?.ToString() ?? Constants.Default.POSITION_NOT_FOUND;

            bool isBattling = GetIsBattling();

            UpdateIsBattling(isBattling);
            if (isBattling)
            {
                currentEncounterIdLabel.Text = GetCurrentEncounterId()?.ToString() ?? Constants.Default.POSITION_NOT_FOUND;
                UpdateIsSpecial(GetIsSpecialEncounter());
            }
            else
            {
                currentEncounterIdLabel.Text = Constants.Default.NO_CURRENT_ENCOUNTER;
                UpdateIsSpecial(false);
            }

        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            Controller.SendKeyDown(_memory.Process, Keys.A);
            Debug.WriteLine("down");
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            Controller.SendKeyUp(_memory.Process, Keys.A);
            Debug.WriteLine("up");
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            Controller.SendKeyDown(_memory.Process, Keys.D);
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            Controller.SendKeyUp(_memory.Process, Keys.D);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Controller.SendKeyPress(_memory.Process, Keys.D4);
        }
    }
}
