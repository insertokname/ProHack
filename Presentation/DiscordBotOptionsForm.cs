using Infrastructure.Database;
using Infrastructure.Discord;
using Infrastructure.Discord.Announcments;

namespace Presentation
{
    public partial class DiscordBotOptionsForm : Form
    {
        private readonly DiscordBot _discordBot;
        public DiscordBotOptionsForm(DiscordBot discordBot)
        {
            _discordBot = discordBot;
            InitializeComponent();
            textBox1.Text = Database.Tables.DiscordBotToken ?? string.Empty;
            textBox2.Text = Database.Tables.AuthorizedUserId.ToString() ?? string.Empty;
            UpdateIsRunningLabel();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateIsRunningLabel();
        }

        private void UpdateIsRunningLabel()
        {
            if (_discordBot.IsConnected)
            {
                label2.Text = "Discord bot started";
                label2.ForeColor = Color.Green;
            }
            else if (_discordBot.IsConnecting)
            {
                label2.Text = "Discord bot is logging in";
                label2.ForeColor = Color.Orange;
            }
            else
            {
                label2.Text = "Discord bot not started";
                label2.ForeColor = Color.Crimson;
            }
        }

        private async void selectPokemonButton_Click(object sender, EventArgs e)
        {
            if (selectPokemonButton.Enabled == false)
            {
                return;
            }

            selectPokemonButton.Enabled = false;

            try
            {
                await RestartBotAsync();
            }
            finally
            {
                selectPokemonButton.Enabled = true;
                UpdateIsRunningLabel();
            }
        }

        private async Task RestartBotAsync()
        {
            try
            {
                if (_discordBot.IsConnected || _discordBot.IsConnecting)
                {
                    await _discordBot.StopAsync();
                }


                await _discordBot.StartAsync();
            }
            catch (Exception ex)
            {
                errorTextBox.Text = $"Got an error while trying to restart bot! {ex.Message}";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Database.Tables.DiscordBotToken = textBox1.Text.Trim();
            Database.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var strId = textBox2.Text.Trim();

            if (!ulong.TryParse(strId, out var intId))
            {
                MessageBox.Show("invalid number!");
                return;
            }

            Database.Tables.AuthorizedUserId = intId;
            Database.Save();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                await _discordBot.SendAnnouncement(new TestAnnouncement());

            }
            catch (Exception ex)
            {
                errorTextBox.Text = $"Got error while sending test message: {ex}";
            }
        }

        private void DiscordBotOptions_Load(object sender, EventArgs e)
        {

        }
    }
}
