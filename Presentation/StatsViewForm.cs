using Domain.Models;
using Infrastructure.Database;
using System.Data;

namespace Presentation
{
    public partial class StatsViewForm : Form
    {
        public StatsViewForm()
        {
            InitializeComponent();
        }

        private void StatsView_Load(object sender, EventArgs e)
        {
            UpdateStats();
            Database.OnUpdate += HandleDatabaseUpdate;
        }

        private void HandleDatabaseUpdate()
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke((Action)UpdateStats);
            }
            else
            {
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
            var lastHour = DateTime.Now - TimeSpan.FromHours(1);
            var now = DateTime.Now;
            var stats = Database.Tables.EncounterStatsModels.OrderBy(s => s.EncounterTime).ToList();
            totalEncountersLabel.Text = stats.Count.ToString();
            encountersInLastHourLabel.Text = stats.Where(s => lastHour <= s.EncounterTime && s.EncounterTime <= now).Count().ToString();
            int lastSpecialEncounterIndex = stats.FindLastIndex(s => s.IsSpecial);

            if (lastSpecialEncounterIndex == -1)
            {
                encountersSinceLastSpecial.Text = "N/A";
                lastSpecialLabel.Text = "N/A";
            }
            else
            {
                EncounterStatsModel lastSpecialEncounter = stats[lastSpecialEncounterIndex];
                encountersSinceLastSpecial.Text = (stats.Count - lastSpecialEncounterIndex).ToString();
                var delay = (now - lastSpecialEncounter.EncounterTime);
                lastSpecialLabel.Text = formatDelay(delay);
            }


            var specialEncounters = stats.Where(s => s.IsSpecial).ToList();
            var specialIndexes = specialEncounters.Select(s => stats.IndexOf(s)).ToList();
            if (specialEncounters.Count() < 1)
            {
                timePerSpecialLabel.Text = "N/A";
            }
            else
            {
                var avarage = stats.Count / (double)specialEncounters.Count;
                timePerSpecialLabel.Text = avarage.ToString("n2");
            }
        }

        private string formatDelay(TimeSpan delay)
        {
            return $"{delay.TotalHours:0}h {delay.Minutes:0}m {delay.Seconds:0}s";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Database.OnUpdate -= HandleDatabaseUpdate;
            base.OnFormClosed(e);
        }

        private void selectPokemonButton_Click(object sender, EventArgs e)
        {
            Database.Tables.EncounterStatsModels.Clear();
            Database.Save();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var stats = Database.Tables.EncounterStatsModels.OrderBy(s => s.EncounterTime).ToList();
            int lastSpecialEncounterIndex = stats.FindLastIndex(s => s.IsSpecial);
            if (lastSpecialEncounterIndex == -1)
            {
                lastSpecialLabel.Text = "N/A";
                return;
            }
            EncounterStatsModel lastSpecialEncounter = stats[lastSpecialEncounterIndex];
            var delay = (now - lastSpecialEncounter.EncounterTime);
            lastSpecialLabel.Text = formatDelay(delay);
        }
    }
}
