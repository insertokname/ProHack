namespace Presentation
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label1 = new Label();
            gameLabel = new Label();
            posXLabel = new Label();
            retryButton = new Button();
            renderTimer = new System.Windows.Forms.Timer(components);
            posYLabel = new Label();
            label3 = new Label();
            currentEncounterIdLabel = new Label();
            label4 = new Label();
            isBattlingLabel = new Label();
            IsSpecialEncounter = new Label();
            startBotButton = new Button();
            registerPositionButton = new Button();
            botStartLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            selectAxisCheckbox = new CheckBox();
            selectAxisPicturebox = new PictureBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            registerPos2Label = new Label();
            registerPos1Label = new Label();
            clearLastRegisterPositionButton = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            stopButton = new Button();
            errorTextBox = new TextBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            noMenuLabel = new Label();
            itemMenuSelectedLabel = new Label();
            selectPokemonButton = new Button();
            menuStrip1 = new MenuStrip();
            test1ToolStripMenuItem = new ToolStripMenuItem();
            timeSinceStartLabel = new Label();
            timeSinceStartedTimer = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            discordOptionToolStripMenuItem = new ToolStripMenuItem();
            botStartLayoutPanel.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)selectAxisPicturebox).BeginInit();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(60, 13);
            label1.TabIndex = 0;
            label1.Text = "Position x:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // gameLabel
            // 
            gameLabel.AutoSize = true;
            gameLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gameLabel.ForeColor = Color.Red;
            gameLabel.Location = new Point(12, 24);
            gameLabel.Name = "gameLabel";
            gameLabel.Size = new Size(193, 32);
            gameLabel.TabIndex = 1;
            gameLabel.Text = "Game not found";
            // 
            // posXLabel
            // 
            posXLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            posXLabel.AutoSize = true;
            posXLabel.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            posXLabel.Location = new Point(156, 0);
            posXLabel.Name = "posXLabel";
            posXLabel.Size = new Size(27, 13);
            posXLabel.TabIndex = 3;
            posXLabel.Text = "N/A";
            // 
            // retryButton
            // 
            retryButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            retryButton.BackColor = Color.LavenderBlush;
            retryButton.FlatStyle = FlatStyle.Flat;
            retryButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            retryButton.ForeColor = Color.Crimson;
            retryButton.Location = new Point(12, 613);
            retryButton.Name = "retryButton";
            retryButton.Size = new Size(385, 77);
            retryButton.TabIndex = 4;
            retryButton.Text = "Retry find";
            retryButton.UseVisualStyleBackColor = false;
            retryButton.Click += button1_Click;
            // 
            // renderTimer
            // 
            renderTimer.Interval = 10;
            renderTimer.Tick += renderTimer_Tick;
            // 
            // posYLabel
            // 
            posYLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            posYLabel.AutoSize = true;
            posYLabel.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            posYLabel.Location = new Point(156, 22);
            posYLabel.Name = "posYLabel";
            posYLabel.Size = new Size(27, 13);
            posYLabel.TabIndex = 6;
            posYLabel.Text = "N/A";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            label3.Location = new Point(3, 22);
            label3.Name = "label3";
            label3.Size = new Size(60, 13);
            label3.TabIndex = 5;
            label3.Text = "Position y:";
            // 
            // currentEncounterIdLabel
            // 
            currentEncounterIdLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            currentEncounterIdLabel.AutoSize = true;
            currentEncounterIdLabel.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            currentEncounterIdLabel.Location = new Point(156, 44);
            currentEncounterIdLabel.Name = "currentEncounterIdLabel";
            currentEncounterIdLabel.Size = new Size(27, 13);
            currentEncounterIdLabel.TabIndex = 8;
            currentEncounterIdLabel.Text = "N/A";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            label4.Location = new Point(3, 44);
            label4.Name = "label4";
            label4.Size = new Size(113, 13);
            label4.TabIndex = 7;
            label4.Text = "Current encounter id:";
            // 
            // isBattlingLabel
            // 
            isBattlingLabel.AutoSize = true;
            isBattlingLabel.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            isBattlingLabel.ForeColor = Color.Green;
            isBattlingLabel.Location = new Point(3, 66);
            isBattlingLabel.Name = "isBattlingLabel";
            isBattlingLabel.Size = new Size(58, 13);
            isBattlingLabel.TabIndex = 9;
            isBattlingLabel.Text = "Is battling";
            isBattlingLabel.Visible = false;
            // 
            // IsSpecialEncounter
            // 
            IsSpecialEncounter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            IsSpecialEncounter.AutoSize = true;
            IsSpecialEncounter.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            IsSpecialEncounter.ForeColor = Color.MediumPurple;
            IsSpecialEncounter.Location = new Point(140, 66);
            IsSpecialEncounter.Name = "IsSpecialEncounter";
            IsSpecialEncounter.Size = new Size(43, 13);
            IsSpecialEncounter.TabIndex = 10;
            IsSpecialEncounter.Text = "Special";
            IsSpecialEncounter.Visible = false;
            // 
            // startBotButton
            // 
            startBotButton.BackColor = Color.LavenderBlush;
            startBotButton.Dock = DockStyle.Fill;
            startBotButton.FlatStyle = FlatStyle.Flat;
            startBotButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            startBotButton.ForeColor = Color.Crimson;
            startBotButton.Location = new Point(3, 3);
            startBotButton.Name = "startBotButton";
            startBotButton.Size = new Size(294, 77);
            startBotButton.TabIndex = 11;
            startBotButton.Text = "Start Bot";
            startBotButton.UseVisualStyleBackColor = false;
            startBotButton.Visible = false;
            startBotButton.Click += button1_Click_1;
            // 
            // registerPositionButton
            // 
            registerPositionButton.BackColor = Color.LavenderBlush;
            registerPositionButton.Dock = DockStyle.Fill;
            registerPositionButton.FlatStyle = FlatStyle.Flat;
            registerPositionButton.Font = new Font("Segoe UI Semibold", 12.25F, FontStyle.Bold);
            registerPositionButton.ForeColor = Color.Crimson;
            registerPositionButton.Location = new Point(195, 3);
            registerPositionButton.Name = "registerPositionButton";
            registerPositionButton.Size = new Size(187, 77);
            registerPositionButton.TabIndex = 12;
            registerPositionButton.Text = "Register position";
            registerPositionButton.UseVisualStyleBackColor = false;
            registerPositionButton.Click += button2_Click;
            // 
            // botStartLayoutPanel
            // 
            botStartLayoutPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            botStartLayoutPanel.ColumnCount = 2;
            botStartLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            botStartLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85F));
            botStartLayoutPanel.Controls.Add(startBotButton, 0, 0);
            botStartLayoutPanel.Controls.Add(tableLayoutPanel3, 1, 0);
            botStartLayoutPanel.Location = new Point(12, 352);
            botStartLayoutPanel.Name = "botStartLayoutPanel";
            botStartLayoutPanel.RowCount = 1;
            botStartLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            botStartLayoutPanel.Size = new Size(385, 83);
            botStartLayoutPanel.TabIndex = 13;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(selectAxisCheckbox, 0, 0);
            tableLayoutPanel3.Controls.Add(selectAxisPicturebox, 0, 1);
            tableLayoutPanel3.Location = new Point(303, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(79, 77);
            tableLayoutPanel3.TabIndex = 12;
            // 
            // selectAxisCheckbox
            // 
            selectAxisCheckbox.AutoSize = true;
            selectAxisCheckbox.BackgroundImageLayout = ImageLayout.Stretch;
            selectAxisCheckbox.Dock = DockStyle.Fill;
            selectAxisCheckbox.FlatStyle = FlatStyle.Flat;
            selectAxisCheckbox.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            selectAxisCheckbox.Location = new Point(3, 3);
            selectAxisCheckbox.Name = "selectAxisCheckbox";
            selectAxisCheckbox.Size = new Size(73, 23);
            selectAxisCheckbox.TabIndex = 12;
            selectAxisCheckbox.Text = "Y axis";
            selectAxisCheckbox.UseVisualStyleBackColor = true;
            selectAxisCheckbox.Visible = false;
            selectAxisCheckbox.CheckedChanged += selectAxisCheckbox_CheckedChanged;
            // 
            // selectAxisPicturebox
            // 
            selectAxisPicturebox.Dock = DockStyle.Fill;
            selectAxisPicturebox.Image = Properties.Resources.yDirection;
            selectAxisPicturebox.Location = new Point(3, 32);
            selectAxisPicturebox.Name = "selectAxisPicturebox";
            selectAxisPicturebox.Size = new Size(73, 42);
            selectAxisPicturebox.SizeMode = PictureBoxSizeMode.Zoom;
            selectAxisPicturebox.TabIndex = 13;
            selectAxisPicturebox.TabStop = false;
            selectAxisPicturebox.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(registerPos2Label, 1, 0);
            tableLayoutPanel2.Controls.Add(registerPos1Label, 0, 0);
            tableLayoutPanel2.Location = new Point(12, 177);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Size = new Size(385, 83);
            tableLayoutPanel2.TabIndex = 15;
            // 
            // registerPos2Label
            // 
            registerPos2Label.AutoSize = true;
            registerPos2Label.Dock = DockStyle.Fill;
            registerPos2Label.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            registerPos2Label.Location = new Point(195, 0);
            registerPos2Label.Name = "registerPos2Label";
            registerPos2Label.Size = new Size(187, 83);
            registerPos2Label.TabIndex = 2;
            registerPos2Label.Text = "Position y:";
            registerPos2Label.TextAlign = ContentAlignment.MiddleCenter;
            registerPos2Label.Visible = false;
            // 
            // registerPos1Label
            // 
            registerPos1Label.AutoSize = true;
            registerPos1Label.Dock = DockStyle.Fill;
            registerPos1Label.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            registerPos1Label.Location = new Point(3, 0);
            registerPos1Label.Name = "registerPos1Label";
            registerPos1Label.Size = new Size(186, 83);
            registerPos1Label.TabIndex = 1;
            registerPos1Label.Text = "Position x:";
            registerPos1Label.TextAlign = ContentAlignment.MiddleCenter;
            registerPos1Label.Visible = false;
            // 
            // clearLastRegisterPositionButton
            // 
            clearLastRegisterPositionButton.BackColor = Color.LavenderBlush;
            clearLastRegisterPositionButton.Dock = DockStyle.Fill;
            clearLastRegisterPositionButton.FlatStyle = FlatStyle.Flat;
            clearLastRegisterPositionButton.Font = new Font("Segoe UI Semibold", 12.25F, FontStyle.Bold);
            clearLastRegisterPositionButton.ForeColor = Color.Crimson;
            clearLastRegisterPositionButton.Location = new Point(3, 3);
            clearLastRegisterPositionButton.Name = "clearLastRegisterPositionButton";
            clearLastRegisterPositionButton.Size = new Size(186, 77);
            clearLastRegisterPositionButton.TabIndex = 16;
            clearLastRegisterPositionButton.Text = "Clear position";
            clearLastRegisterPositionButton.UseVisualStyleBackColor = false;
            clearLastRegisterPositionButton.Visible = false;
            clearLastRegisterPositionButton.Click += clearLastRegisterPositionButton_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(clearLastRegisterPositionButton, 0, 0);
            tableLayoutPanel1.Controls.Add(registerPositionButton, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 266);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(385, 83);
            tableLayoutPanel1.TabIndex = 17;
            // 
            // stopButton
            // 
            stopButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            stopButton.BackColor = Color.LavenderBlush;
            stopButton.FlatStyle = FlatStyle.Flat;
            stopButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            stopButton.ForeColor = Color.Crimson;
            stopButton.Location = new Point(12, 525);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(382, 82);
            stopButton.TabIndex = 19;
            stopButton.Text = "Stop\r\n(Ctrl + F8)";
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Visible = false;
            stopButton.Click += stopButton_Click;
            // 
            // errorTextBox
            // 
            errorTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            errorTextBox.BackColor = Color.LavenderBlush;
            errorTextBox.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            errorTextBox.ForeColor = Color.Crimson;
            errorTextBox.Location = new Point(12, 441);
            errorTextBox.Multiline = true;
            errorTextBox.Name = "errorTextBox";
            errorTextBox.ScrollBars = ScrollBars.Vertical;
            errorTextBox.Size = new Size(382, 78);
            errorTextBox.TabIndex = 19;
            errorTextBox.Visible = false;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 69.3548355F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.64516F));
            tableLayoutPanel5.Controls.Add(noMenuLabel, 1, 4);
            tableLayoutPanel5.Controls.Add(itemMenuSelectedLabel, 0, 4);
            tableLayoutPanel5.Controls.Add(label1, 0, 0);
            tableLayoutPanel5.Controls.Add(posXLabel, 1, 0);
            tableLayoutPanel5.Controls.Add(label3, 0, 1);
            tableLayoutPanel5.Controls.Add(posYLabel, 1, 1);
            tableLayoutPanel5.Controls.Add(currentEncounterIdLabel, 1, 2);
            tableLayoutPanel5.Controls.Add(label4, 0, 2);
            tableLayoutPanel5.Controls.Add(isBattlingLabel, 0, 3);
            tableLayoutPanel5.Controls.Add(IsSpecialEncounter, 1, 3);
            tableLayoutPanel5.Location = new Point(15, 59);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 5;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.Size = new Size(186, 112);
            tableLayoutPanel5.TabIndex = 20;
            // 
            // noMenuLabel
            // 
            noMenuLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            noMenuLabel.AutoSize = true;
            noMenuLabel.Font = new Font("Segoe UI Semibold", 7F, FontStyle.Bold);
            noMenuLabel.ForeColor = Color.Green;
            noMenuLabel.Location = new Point(132, 88);
            noMenuLabel.Name = "noMenuLabel";
            noMenuLabel.Size = new Size(51, 24);
            noMenuLabel.TabIndex = 12;
            noMenuLabel.Text = "No menu selected";
            noMenuLabel.Visible = false;
            // 
            // itemMenuSelectedLabel
            // 
            itemMenuSelectedLabel.AutoSize = true;
            itemMenuSelectedLabel.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
            itemMenuSelectedLabel.ForeColor = Color.Green;
            itemMenuSelectedLabel.Location = new Point(3, 88);
            itemMenuSelectedLabel.Name = "itemMenuSelectedLabel";
            itemMenuSelectedLabel.Size = new Size(106, 13);
            itemMenuSelectedLabel.TabIndex = 11;
            itemMenuSelectedLabel.Text = "Item menu selected";
            itemMenuSelectedLabel.Visible = false;
            // 
            // selectPokemonButton
            // 
            selectPokemonButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            selectPokemonButton.BackColor = Color.LavenderBlush;
            selectPokemonButton.FlatStyle = FlatStyle.Flat;
            selectPokemonButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            selectPokemonButton.ForeColor = Color.Crimson;
            selectPokemonButton.Location = new Point(207, 59);
            selectPokemonButton.Name = "selectPokemonButton";
            selectPokemonButton.Size = new Size(190, 112);
            selectPokemonButton.TabIndex = 21;
            selectPokemonButton.Text = "Search mode";
            selectPokemonButton.UseVisualStyleBackColor = false;
            selectPokemonButton.Click += button1_Click_2;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.LavenderBlush;
            menuStrip1.Items.AddRange(new ToolStripItem[] { test1ToolStripMenuItem, discordOptionToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(409, 24);
            menuStrip1.TabIndex = 22;
            menuStrip1.Text = "menuStrip1";
            // 
            // test1ToolStripMenuItem
            // 
            test1ToolStripMenuItem.BackColor = Color.LavenderBlush;
            test1ToolStripMenuItem.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            test1ToolStripMenuItem.ForeColor = Color.Crimson;
            test1ToolStripMenuItem.Name = "test1ToolStripMenuItem";
            test1ToolStripMenuItem.Size = new Size(74, 20);
            test1ToolStripMenuItem.Text = "View Stats";
            test1ToolStripMenuItem.Click += test1ToolStripMenuItem_Click;
            // 
            // timeSinceStartLabel
            // 
            timeSinceStartLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            timeSinceStartLabel.BackColor = Color.LavenderBlush;
            timeSinceStartLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            timeSinceStartLabel.ForeColor = Color.Red;
            timeSinceStartLabel.Location = new Point(318, 4);
            timeSinceStartLabel.Name = "timeSinceStartLabel";
            timeSinceStartLabel.Size = new Size(81, 15);
            timeSinceStartLabel.TabIndex = 23;
            timeSinceStartLabel.Text = "00:00:00";
            timeSinceStartLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // timeSinceStartedTimer
            // 
            timeSinceStartedTimer.Enabled = true;
            timeSinceStartedTimer.Tick += timeSinceStartedTimer_Tick;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.BackColor = Color.LavenderBlush;
            label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(221, 4);
            label2.Name = "label2";
            label2.Size = new Size(88, 15);
            label2.TabIndex = 24;
            label2.Text = "Session lenght:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // discordOptionToolStripMenuItem
            // 
            discordOptionToolStripMenuItem.BackColor = Color.Lavender;
            discordOptionToolStripMenuItem.ForeColor = Color.MediumPurple;
            discordOptionToolStripMenuItem.Name = "discordOptionToolStripMenuItem";
            discordOptionToolStripMenuItem.Size = new Size(104, 20);
            discordOptionToolStripMenuItem.Text = "Discord Options";
            discordOptionToolStripMenuItem.Click += discordOptionToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(409, 702);
            Controls.Add(label2);
            Controls.Add(timeSinceStartLabel);
            Controls.Add(stopButton);
            Controls.Add(selectPokemonButton);
            Controls.Add(tableLayoutPanel5);
            Controls.Add(errorTextBox);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(retryButton);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(botStartLayoutPanel);
            Controls.Add(gameLabel);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "ProHack";
            Load += Form1_Load;
            botStartLayoutPanel.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)selectAxisPicturebox).EndInit();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label gameLabel;
        private Label posXLabel;
        private Button retryButton;
        private System.Windows.Forms.Timer renderTimer;
        private Label posYLabel;
        private Label label3;
        private Label currentEncounterIdLabel;
        private Label label4;
        private Label isBattlingLabel;
        private Label IsSpecialEncounter;
        private Button startBotButton;
        private Button registerPositionButton;
        private TableLayoutPanel botStartLayoutPanel;
        private TableLayoutPanel tableLayoutPanel2;
        private Label registerPos2Label;
        private Label registerPos1Label;
        private Button clearLastRegisterPositionButton;
        private CheckBox selectAxisCheckbox;
        private TableLayoutPanel tableLayoutPanel3;
        private PictureBox selectAxisPicturebox;
        private TableLayoutPanel tableLayoutPanel1;
        private Button stopButton;
        private TextBox errorTextBox;
        private TableLayoutPanel tableLayoutPanel5;
        private Button selectPokemonButton;
        private Label itemMenuSelectedLabel;
        private Label noMenuLabel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem test1ToolStripMenuItem;
        private Label timeSinceStartLabel;
        private System.Windows.Forms.Timer timeSinceStartedTimer;
        private Label label2;
        private ToolStripMenuItem discordOptionToolStripMenuItem;
    }
}
