namespace Presentation
{
    partial class StatsView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel5 = new TableLayoutPanel();
            timePerSpecialLabel = new Label();
            encountersPerSpecialLabel = new Label();
            label1 = new Label();
            totalEncountersLabel = new Label();
            lablel = new Label();
            encountersInLastHourLabel = new Label();
            encountersSinceLastSpecial = new Label();
            label4 = new Label();
            isBattlingLabel = new Label();
            lastSpecialLabel = new Label();
            tableLayoutPanel5.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64.5614F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35.4385948F));
            tableLayoutPanel5.Controls.Add(timePerSpecialLabel, 1, 4);
            tableLayoutPanel5.Controls.Add(encountersPerSpecialLabel, 0, 4);
            tableLayoutPanel5.Controls.Add(label1, 0, 0);
            tableLayoutPanel5.Controls.Add(totalEncountersLabel, 1, 0);
            tableLayoutPanel5.Controls.Add(lablel, 0, 1);
            tableLayoutPanel5.Controls.Add(encountersInLastHourLabel, 1, 1);
            tableLayoutPanel5.Controls.Add(encountersSinceLastSpecial, 1, 2);
            tableLayoutPanel5.Controls.Add(label4, 0, 2);
            tableLayoutPanel5.Controls.Add(isBattlingLabel, 0, 3);
            tableLayoutPanel5.Controls.Add(lastSpecialLabel, 1, 3);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(0, 0);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 5;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.Size = new Size(285, 229);
            tableLayoutPanel5.TabIndex = 21;
            // 
            // timePerSpecialLabel
            // 
            timePerSpecialLabel.AutoSize = true;
            timePerSpecialLabel.Dock = DockStyle.Fill;
            timePerSpecialLabel.Font = new Font("Segoe UI", 12F);
            timePerSpecialLabel.ForeColor = SystemColors.ControlText;
            timePerSpecialLabel.Location = new Point(187, 180);
            timePerSpecialLabel.Name = "timePerSpecialLabel";
            timePerSpecialLabel.Size = new Size(95, 49);
            timePerSpecialLabel.TabIndex = 12;
            timePerSpecialLabel.Text = "0";
            timePerSpecialLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // encountersPerSpecialLabel
            // 
            encountersPerSpecialLabel.AutoSize = true;
            encountersPerSpecialLabel.Dock = DockStyle.Fill;
            encountersPerSpecialLabel.Font = new Font("Segoe UI", 12F);
            encountersPerSpecialLabel.ForeColor = SystemColors.ControlText;
            encountersPerSpecialLabel.Location = new Point(3, 180);
            encountersPerSpecialLabel.Name = "encountersPerSpecialLabel";
            encountersPerSpecialLabel.Size = new Size(178, 49);
            encountersPerSpecialLabel.TabIndex = 11;
            encountersPerSpecialLabel.Text = "Avarage encounters per special:";
            encountersPerSpecialLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(178, 45);
            label1.TabIndex = 0;
            label1.Text = "Total encounters:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // totalEncountersLabel
            // 
            totalEncountersLabel.AutoSize = true;
            totalEncountersLabel.Dock = DockStyle.Fill;
            totalEncountersLabel.Font = new Font("Segoe UI", 12F);
            totalEncountersLabel.Location = new Point(187, 0);
            totalEncountersLabel.Name = "totalEncountersLabel";
            totalEncountersLabel.Size = new Size(95, 45);
            totalEncountersLabel.TabIndex = 3;
            totalEncountersLabel.Text = "0";
            totalEncountersLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lablel
            // 
            lablel.AutoSize = true;
            lablel.Dock = DockStyle.Fill;
            lablel.Font = new Font("Segoe UI", 12F);
            lablel.Location = new Point(3, 45);
            lablel.Name = "lablel";
            lablel.Size = new Size(178, 45);
            lablel.TabIndex = 5;
            lablel.Text = "Encounters in last hour:";
            lablel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // encountersInLastHourLabel
            // 
            encountersInLastHourLabel.AutoSize = true;
            encountersInLastHourLabel.Dock = DockStyle.Fill;
            encountersInLastHourLabel.Font = new Font("Segoe UI", 12F);
            encountersInLastHourLabel.Location = new Point(187, 45);
            encountersInLastHourLabel.Name = "encountersInLastHourLabel";
            encountersInLastHourLabel.Size = new Size(95, 45);
            encountersInLastHourLabel.TabIndex = 6;
            encountersInLastHourLabel.Text = "0";
            encountersInLastHourLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // encountersSinceLastSpecial
            // 
            encountersSinceLastSpecial.AutoSize = true;
            encountersSinceLastSpecial.Dock = DockStyle.Fill;
            encountersSinceLastSpecial.Font = new Font("Segoe UI", 12F);
            encountersSinceLastSpecial.Location = new Point(187, 90);
            encountersSinceLastSpecial.Name = "encountersSinceLastSpecial";
            encountersSinceLastSpecial.Size = new Size(95, 45);
            encountersSinceLastSpecial.TabIndex = 8;
            encountersSinceLastSpecial.Text = "0";
            encountersSinceLastSpecial.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(3, 90);
            label4.Name = "label4";
            label4.Size = new Size(178, 45);
            label4.TabIndex = 7;
            label4.Text = "Encounters since last special:";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // isBattlingLabel
            // 
            isBattlingLabel.AutoSize = true;
            isBattlingLabel.Dock = DockStyle.Fill;
            isBattlingLabel.Font = new Font("Segoe UI", 12F);
            isBattlingLabel.ForeColor = SystemColors.ControlText;
            isBattlingLabel.Location = new Point(3, 135);
            isBattlingLabel.Name = "isBattlingLabel";
            isBattlingLabel.Size = new Size(178, 45);
            isBattlingLabel.TabIndex = 9;
            isBattlingLabel.Text = "last special seen:";
            isBattlingLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lastSpecialLabel
            // 
            lastSpecialLabel.AutoSize = true;
            lastSpecialLabel.Dock = DockStyle.Fill;
            lastSpecialLabel.Font = new Font("Segoe UI", 12F);
            lastSpecialLabel.ForeColor = SystemColors.ControlText;
            lastSpecialLabel.Location = new Point(187, 135);
            lastSpecialLabel.Name = "lastSpecialLabel";
            lastSpecialLabel.Size = new Size(95, 45);
            lastSpecialLabel.TabIndex = 10;
            lastSpecialLabel.Text = "0";
            lastSpecialLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // StatsView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(285, 229);
            Controls.Add(tableLayoutPanel5);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "StatsView";
            Text = "StatsView";
            Load += StatsView_Load;
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel5;
        private Label timePerSpecialLabel;
        private Label encountersPerSpecialLabel;
        private Label label1;
        private Label totalEncountersLabel;
        private Label lablel;
        private Label encountersInLastHourLabel;
        private Label encountersSinceLastSpecial;
        private Label label4;
        private Label isBattlingLabel;
        private Label lastSpecialLabel;
    }
}