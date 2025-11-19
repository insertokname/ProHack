namespace Presentation
{
    partial class StatsViewForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatsViewForm));
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
            selectPokemonButton = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            tableLayoutPanel5.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57.046978F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42.953022F));
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
            tableLayoutPanel5.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            tableLayoutPanel5.ForeColor = Color.FromArgb(152, 151, 26);
            tableLayoutPanel5.Location = new Point(0, 0);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 5;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.Size = new Size(298, 229);
            tableLayoutPanel5.TabIndex = 21;
            // 
            // timePerSpecialLabel
            // 
            timePerSpecialLabel.AutoSize = true;
            timePerSpecialLabel.BackColor = Color.FromArgb(40, 40, 40);
            timePerSpecialLabel.Dock = DockStyle.Fill;
            timePerSpecialLabel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            timePerSpecialLabel.ForeColor = Color.FromArgb(152, 151, 26);
            timePerSpecialLabel.Location = new Point(173, 180);
            timePerSpecialLabel.Name = "timePerSpecialLabel";
            timePerSpecialLabel.Size = new Size(122, 49);
            timePerSpecialLabel.TabIndex = 12;
            timePerSpecialLabel.Tag = "FontSize=S";
            timePerSpecialLabel.Text = "0";
            timePerSpecialLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // encountersPerSpecialLabel
            // 
            encountersPerSpecialLabel.AutoSize = true;
            encountersPerSpecialLabel.BackColor = Color.FromArgb(40, 40, 40);
            encountersPerSpecialLabel.Dock = DockStyle.Fill;
            encountersPerSpecialLabel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            encountersPerSpecialLabel.ForeColor = Color.FromArgb(152, 151, 26);
            encountersPerSpecialLabel.Location = new Point(3, 180);
            encountersPerSpecialLabel.Name = "encountersPerSpecialLabel";
            encountersPerSpecialLabel.Size = new Size(164, 49);
            encountersPerSpecialLabel.TabIndex = 11;
            encountersPerSpecialLabel.Tag = "FontSize=S";
            encountersPerSpecialLabel.Text = "Avarage encounters per special:";
            encountersPerSpecialLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(164, 45);
            label1.TabIndex = 0;
            label1.Tag = "FontSize=S";
            label1.Text = "Total encounters:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // totalEncountersLabel
            // 
            totalEncountersLabel.AutoSize = true;
            totalEncountersLabel.BackColor = Color.FromArgb(40, 40, 40);
            totalEncountersLabel.Dock = DockStyle.Fill;
            totalEncountersLabel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            totalEncountersLabel.ForeColor = Color.FromArgb(152, 151, 26);
            totalEncountersLabel.Location = new Point(173, 0);
            totalEncountersLabel.Name = "totalEncountersLabel";
            totalEncountersLabel.Size = new Size(122, 45);
            totalEncountersLabel.TabIndex = 3;
            totalEncountersLabel.Tag = "FontSize=S";
            totalEncountersLabel.Text = "0";
            totalEncountersLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lablel
            // 
            lablel.AutoSize = true;
            lablel.BackColor = Color.FromArgb(40, 40, 40);
            lablel.Dock = DockStyle.Fill;
            lablel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            lablel.ForeColor = Color.FromArgb(152, 151, 26);
            lablel.Location = new Point(3, 45);
            lablel.Name = "lablel";
            lablel.Size = new Size(164, 45);
            lablel.TabIndex = 5;
            lablel.Tag = "FontSize=S";
            lablel.Text = "Encounters in last hour:";
            lablel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // encountersInLastHourLabel
            // 
            encountersInLastHourLabel.AutoSize = true;
            encountersInLastHourLabel.BackColor = Color.FromArgb(40, 40, 40);
            encountersInLastHourLabel.Dock = DockStyle.Fill;
            encountersInLastHourLabel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            encountersInLastHourLabel.ForeColor = Color.FromArgb(152, 151, 26);
            encountersInLastHourLabel.Location = new Point(173, 45);
            encountersInLastHourLabel.Name = "encountersInLastHourLabel";
            encountersInLastHourLabel.Size = new Size(122, 45);
            encountersInLastHourLabel.TabIndex = 6;
            encountersInLastHourLabel.Tag = "FontSize=S";
            encountersInLastHourLabel.Text = "0";
            encountersInLastHourLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // encountersSinceLastSpecial
            // 
            encountersSinceLastSpecial.AutoSize = true;
            encountersSinceLastSpecial.BackColor = Color.FromArgb(40, 40, 40);
            encountersSinceLastSpecial.Dock = DockStyle.Fill;
            encountersSinceLastSpecial.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            encountersSinceLastSpecial.ForeColor = Color.FromArgb(152, 151, 26);
            encountersSinceLastSpecial.Location = new Point(173, 90);
            encountersSinceLastSpecial.Name = "encountersSinceLastSpecial";
            encountersSinceLastSpecial.Size = new Size(122, 45);
            encountersSinceLastSpecial.TabIndex = 8;
            encountersSinceLastSpecial.Tag = "FontSize=S";
            encountersSinceLastSpecial.Text = "0";
            encountersSinceLastSpecial.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(40, 40, 40);
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(152, 151, 26);
            label4.Location = new Point(3, 90);
            label4.Name = "label4";
            label4.Size = new Size(164, 45);
            label4.TabIndex = 7;
            label4.Tag = "FontSize=S";
            label4.Text = "Encounters since last special:";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // isBattlingLabel
            // 
            isBattlingLabel.AutoSize = true;
            isBattlingLabel.BackColor = Color.FromArgb(40, 40, 40);
            isBattlingLabel.Dock = DockStyle.Fill;
            isBattlingLabel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            isBattlingLabel.ForeColor = Color.FromArgb(152, 151, 26);
            isBattlingLabel.Location = new Point(3, 135);
            isBattlingLabel.Name = "isBattlingLabel";
            isBattlingLabel.Size = new Size(164, 45);
            isBattlingLabel.TabIndex = 9;
            isBattlingLabel.Tag = "FontSize=S";
            isBattlingLabel.Text = "last special seen:";
            isBattlingLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lastSpecialLabel
            // 
            lastSpecialLabel.AutoSize = true;
            lastSpecialLabel.BackColor = Color.FromArgb(40, 40, 40);
            lastSpecialLabel.Dock = DockStyle.Fill;
            lastSpecialLabel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            lastSpecialLabel.ForeColor = Color.FromArgb(152, 151, 26);
            lastSpecialLabel.Location = new Point(173, 135);
            lastSpecialLabel.Name = "lastSpecialLabel";
            lastSpecialLabel.Size = new Size(122, 45);
            lastSpecialLabel.TabIndex = 10;
            lastSpecialLabel.Tag = "FontSize=S";
            lastSpecialLabel.Text = "0";
            lastSpecialLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // selectPokemonButton
            // 
            selectPokemonButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            selectPokemonButton.BackColor = Color.FromArgb(40, 40, 40);
            selectPokemonButton.FlatStyle = FlatStyle.Flat;
            selectPokemonButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            selectPokemonButton.ForeColor = Color.FromArgb(152, 151, 26);
            selectPokemonButton.Location = new Point(3, 235);
            selectPokemonButton.Name = "selectPokemonButton";
            selectPokemonButton.Size = new Size(295, 44);
            selectPokemonButton.TabIndex = 24;
            selectPokemonButton.Text = "Reset stats";
            selectPokemonButton.UseVisualStyleBackColor = false;
            selectPokemonButton.Click += selectPokemonButton_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 5000;
            timer1.Tick += timer1_Tick;
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(0, 0);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(0, 0);
            greenLimeThemeComponent1.TabIndex = 29;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // StatsViewForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(301, 282);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(selectPokemonButton);
            Controls.Add(tableLayoutPanel5);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "StatsViewForm";
            Text = "Stats";
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
        private Button selectPokemonButton;
        private System.Windows.Forms.Timer timer1;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
    }
}