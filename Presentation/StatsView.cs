using Domain.Models;
using Infrastructure.Database;
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
    public partial class StatsView : Form
    {
        public StatsView()
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
                lastSpecialLabel.Text = (now - lastSpecialEncounter.EncounterTime).ToString("hh':'mm':'ss");
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Database.OnUpdate -= HandleDatabaseUpdate;
            base.OnFormClosed(e);
        }
    }
}
