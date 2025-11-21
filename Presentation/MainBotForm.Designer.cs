namespace Presentation
{
    partial class MainBotForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainBotForm));
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
            isEvent = new Label();
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
            isShiny = new Label();
            noMenuLabel = new Label();
            selectPokemonButton = new Button();
            menuStrip1 = new MenuStrip();
            test1ToolStripMenuItem = new ToolStripMenuItem();
            dialogueSettingsToolStripMenuItem = new ToolStripMenuItem();
            dataFolderSettingsToolStripMenuItem = new ToolStripMenuItem();
            dialogueSettingsToolStripMenuItem1 = new ToolStripMenuItem();
            themeSettingsToolStripMenuItem = new ToolStripMenuItem();
            updateSettingsToolStripMenuItem = new ToolStripMenuItem();
            discordToolStripMenuItem = new ToolStripMenuItem();
            donateToolStripMenuItem = new ToolStripMenuItem();
            timeSinceStartLabel = new Label();
            timeSinceStartedTimer = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            pictureBox1 = new PictureBox();
            label5 = new Label();
            dontShowBuyMeACoffeeButton = new Button();
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            botStartLayoutPanel.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)selectAxisPicturebox).BeginInit();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(73, 15);
            label1.TabIndex = 0;
            label1.Tag = "FontSize=XS";
            label1.Text = "Position x:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // gameLabel
            // 
            gameLabel.AutoSize = true;
            gameLabel.BackColor = Color.FromArgb(40, 40, 40);
            gameLabel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            gameLabel.ForeColor = Color.FromArgb(152, 151, 26);
            gameLabel.Location = new Point(15, 24);
            gameLabel.Name = "gameLabel";
            gameLabel.Size = new Size(166, 25);
            gameLabel.TabIndex = 1;
            gameLabel.Text = "Game not found";
            // 
            // posXLabel
            // 
            posXLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            posXLabel.AutoSize = true;
            posXLabel.BackColor = Color.FromArgb(40, 40, 40);
            posXLabel.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            posXLabel.ForeColor = Color.FromArgb(152, 151, 26);
            posXLabel.Location = new Point(158, 0);
            posXLabel.Name = "posXLabel";
            posXLabel.Size = new Size(25, 15);
            posXLabel.TabIndex = 3;
            posXLabel.Tag = "FontSize=XS";
            posXLabel.Text = "N/A";
            // 
            // retryButton
            // 
            retryButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            retryButton.BackColor = Color.FromArgb(40, 40, 40);
            retryButton.FlatAppearance.BorderSize = 3;
            retryButton.FlatStyle = FlatStyle.Flat;
            retryButton.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            retryButton.ForeColor = Color.FromArgb(152, 151, 26);
            retryButton.Location = new Point(12, 613);
            retryButton.Name = "retryButton";
            retryButton.Size = new Size(514, 77);
            retryButton.TabIndex = 4;
            retryButton.Tag = "FontSize=L";
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
            posYLabel.BackColor = Color.FromArgb(40, 40, 40);
            posYLabel.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            posYLabel.ForeColor = Color.FromArgb(152, 151, 26);
            posYLabel.Location = new Point(158, 22);
            posYLabel.Name = "posYLabel";
            posYLabel.Size = new Size(25, 15);
            posYLabel.TabIndex = 6;
            posYLabel.Tag = "FontSize=XS";
            posYLabel.Text = "N/A";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.FromArgb(40, 40, 40);
            label3.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(152, 151, 26);
            label3.Location = new Point(3, 22);
            label3.Name = "label3";
            label3.Size = new Size(73, 15);
            label3.TabIndex = 5;
            label3.Tag = "FontSize=XS";
            label3.Text = "Position y:";
            // 
            // currentEncounterIdLabel
            // 
            currentEncounterIdLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            currentEncounterIdLabel.AutoSize = true;
            currentEncounterIdLabel.BackColor = Color.FromArgb(40, 40, 40);
            currentEncounterIdLabel.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            currentEncounterIdLabel.ForeColor = Color.FromArgb(152, 151, 26);
            currentEncounterIdLabel.Location = new Point(158, 44);
            currentEncounterIdLabel.Name = "currentEncounterIdLabel";
            currentEncounterIdLabel.Size = new Size(25, 15);
            currentEncounterIdLabel.TabIndex = 8;
            currentEncounterIdLabel.Tag = "FontSize=XS";
            currentEncounterIdLabel.Text = "N/A";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(40, 40, 40);
            label4.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(152, 151, 26);
            label4.Location = new Point(3, 44);
            label4.Name = "label4";
            label4.Size = new Size(115, 22);
            label4.TabIndex = 7;
            label4.Tag = "FontSize=XS";
            label4.Text = "Current encounter id:";
            // 
            // isBattlingLabel
            // 
            isBattlingLabel.AutoSize = true;
            isBattlingLabel.BackColor = Color.FromArgb(40, 40, 40);
            isBattlingLabel.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            isBattlingLabel.ForeColor = Color.FromArgb(152, 151, 26);
            isBattlingLabel.Location = new Point(3, 66);
            isBattlingLabel.Name = "isBattlingLabel";
            isBattlingLabel.Size = new Size(73, 15);
            isBattlingLabel.TabIndex = 9;
            isBattlingLabel.Tag = "FontSize=XS";
            isBattlingLabel.Text = "Is battling";
            isBattlingLabel.Visible = false;
            // 
            // isEvent
            // 
            isEvent.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            isEvent.AutoSize = true;
            isEvent.BackColor = Color.FromArgb(40, 40, 40);
            isEvent.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            isEvent.ForeColor = Color.FromArgb(152, 151, 26);
            isEvent.Location = new Point(146, 66);
            isEvent.Name = "isEvent";
            isEvent.Size = new Size(37, 15);
            isEvent.TabIndex = 10;
            isEvent.Tag = "FontSize=XS";
            isEvent.Text = "Event";
            isEvent.Visible = false;
            // 
            // startBotButton
            // 
            startBotButton.BackColor = Color.FromArgb(40, 40, 40);
            startBotButton.Dock = DockStyle.Fill;
            startBotButton.FlatAppearance.BorderSize = 3;
            startBotButton.FlatStyle = FlatStyle.Flat;
            startBotButton.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            startBotButton.ForeColor = Color.FromArgb(152, 151, 26);
            startBotButton.Location = new Point(3, 3);
            startBotButton.Name = "startBotButton";
            startBotButton.Size = new Size(423, 77);
            startBotButton.TabIndex = 11;
            startBotButton.Tag = "FontSize=L";
            startBotButton.Text = "Start Bot";
            startBotButton.UseVisualStyleBackColor = false;
            startBotButton.Visible = false;
            startBotButton.Click += button1_Click_1;
            // 
            // registerPositionButton
            // 
            registerPositionButton.BackColor = Color.FromArgb(40, 40, 40);
            registerPositionButton.Dock = DockStyle.Fill;
            registerPositionButton.FlatAppearance.BorderSize = 3;
            registerPositionButton.FlatStyle = FlatStyle.Flat;
            registerPositionButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            registerPositionButton.ForeColor = Color.FromArgb(152, 151, 26);
            registerPositionButton.Location = new Point(260, 3);
            registerPositionButton.Name = "registerPositionButton";
            registerPositionButton.Size = new Size(251, 77);
            registerPositionButton.TabIndex = 12;
            registerPositionButton.Tag = "FontSize=M";
            registerPositionButton.Text = "Register position";
            registerPositionButton.UseVisualStyleBackColor = false;
            registerPositionButton.Click += button2_Click;
            // 
            // botStartLayoutPanel
            // 
            botStartLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            botStartLayoutPanel.BackColor = Color.FromArgb(40, 40, 40);
            botStartLayoutPanel.ColumnCount = 2;
            botStartLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            botStartLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85F));
            botStartLayoutPanel.Controls.Add(startBotButton, 0, 0);
            botStartLayoutPanel.Controls.Add(tableLayoutPanel3, 1, 0);
            botStartLayoutPanel.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            botStartLayoutPanel.ForeColor = Color.FromArgb(152, 151, 26);
            botStartLayoutPanel.Location = new Point(12, 352);
            botStartLayoutPanel.Name = "botStartLayoutPanel";
            botStartLayoutPanel.RowCount = 1;
            botStartLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            botStartLayoutPanel.Size = new Size(514, 83);
            botStartLayoutPanel.TabIndex = 13;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel3.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(selectAxisCheckbox, 0, 0);
            tableLayoutPanel3.Controls.Add(selectAxisPicturebox, 0, 1);
            tableLayoutPanel3.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            tableLayoutPanel3.ForeColor = Color.FromArgb(152, 151, 26);
            tableLayoutPanel3.Location = new Point(432, 3);
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
            selectAxisCheckbox.BackColor = Color.FromArgb(40, 40, 40);
            selectAxisCheckbox.BackgroundImageLayout = ImageLayout.Stretch;
            selectAxisCheckbox.Dock = DockStyle.Fill;
            selectAxisCheckbox.FlatStyle = FlatStyle.Flat;
            selectAxisCheckbox.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            selectAxisCheckbox.ForeColor = Color.FromArgb(152, 151, 26);
            selectAxisCheckbox.Location = new Point(3, 3);
            selectAxisCheckbox.Name = "selectAxisCheckbox";
            selectAxisCheckbox.Size = new Size(73, 23);
            selectAxisCheckbox.TabIndex = 12;
            selectAxisCheckbox.Tag = "FontSize=S";
            selectAxisCheckbox.Text = "Y axis";
            selectAxisCheckbox.UseVisualStyleBackColor = false;
            selectAxisCheckbox.Visible = false;
            selectAxisCheckbox.CheckedChanged += selectAxisCheckbox_CheckedChanged;
            // 
            // selectAxisPicturebox
            // 
            selectAxisPicturebox.BackColor = Color.FromArgb(40, 40, 40);
            selectAxisPicturebox.Dock = DockStyle.Fill;
            selectAxisPicturebox.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            selectAxisPicturebox.ForeColor = Color.FromArgb(152, 151, 26);
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
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(registerPos2Label, 1, 0);
            tableLayoutPanel2.Controls.Add(registerPos1Label, 0, 0);
            tableLayoutPanel2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            tableLayoutPanel2.ForeColor = Color.FromArgb(152, 151, 26);
            tableLayoutPanel2.Location = new Point(12, 177);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Size = new Size(514, 83);
            tableLayoutPanel2.TabIndex = 15;
            // 
            // registerPos2Label
            // 
            registerPos2Label.AutoSize = true;
            registerPos2Label.BackColor = Color.FromArgb(40, 40, 40);
            registerPos2Label.Dock = DockStyle.Fill;
            registerPos2Label.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            registerPos2Label.ForeColor = Color.FromArgb(152, 151, 26);
            registerPos2Label.Location = new Point(260, 0);
            registerPos2Label.Name = "registerPos2Label";
            registerPos2Label.Size = new Size(251, 83);
            registerPos2Label.TabIndex = 2;
            registerPos2Label.Tag = "FontSize=L";
            registerPos2Label.Text = "Position y:";
            registerPos2Label.TextAlign = ContentAlignment.MiddleCenter;
            registerPos2Label.Visible = false;
            // 
            // registerPos1Label
            // 
            registerPos1Label.AutoSize = true;
            registerPos1Label.BackColor = Color.FromArgb(40, 40, 40);
            registerPos1Label.Dock = DockStyle.Fill;
            registerPos1Label.Font = new Font("Cascadia Code", 24F, FontStyle.Bold);
            registerPos1Label.ForeColor = Color.FromArgb(152, 151, 26);
            registerPos1Label.Location = new Point(3, 0);
            registerPos1Label.Name = "registerPos1Label";
            registerPos1Label.Size = new Size(251, 83);
            registerPos1Label.TabIndex = 1;
            registerPos1Label.Tag = "FontSize=L";
            registerPos1Label.Text = "Position x:";
            registerPos1Label.TextAlign = ContentAlignment.MiddleCenter;
            registerPos1Label.Visible = false;
            // 
            // clearLastRegisterPositionButton
            // 
            clearLastRegisterPositionButton.BackColor = Color.FromArgb(40, 40, 40);
            clearLastRegisterPositionButton.Dock = DockStyle.Fill;
            clearLastRegisterPositionButton.FlatAppearance.BorderSize = 3;
            clearLastRegisterPositionButton.FlatStyle = FlatStyle.Flat;
            clearLastRegisterPositionButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            clearLastRegisterPositionButton.ForeColor = Color.FromArgb(152, 151, 26);
            clearLastRegisterPositionButton.Location = new Point(3, 3);
            clearLastRegisterPositionButton.Name = "clearLastRegisterPositionButton";
            clearLastRegisterPositionButton.Size = new Size(251, 77);
            clearLastRegisterPositionButton.TabIndex = 16;
            clearLastRegisterPositionButton.Tag = "FontSize=M";
            clearLastRegisterPositionButton.Text = "Clear position";
            clearLastRegisterPositionButton.UseVisualStyleBackColor = false;
            clearLastRegisterPositionButton.Visible = false;
            clearLastRegisterPositionButton.Click += clearLastRegisterPositionButton_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(clearLastRegisterPositionButton, 0, 0);
            tableLayoutPanel1.Controls.Add(registerPositionButton, 1, 0);
            tableLayoutPanel1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            tableLayoutPanel1.ForeColor = Color.FromArgb(152, 151, 26);
            tableLayoutPanel1.Location = new Point(12, 266);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(514, 83);
            tableLayoutPanel1.TabIndex = 17;
            // 
            // stopButton
            // 
            stopButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            stopButton.BackColor = Color.FromArgb(40, 40, 40);
            stopButton.FlatAppearance.BorderSize = 3;
            stopButton.FlatStyle = FlatStyle.Flat;
            stopButton.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            stopButton.ForeColor = Color.FromArgb(152, 151, 26);
            stopButton.Location = new Point(12, 525);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(511, 82);
            stopButton.TabIndex = 19;
            stopButton.Tag = "FontSize=L";
            stopButton.Text = "Stop\r\n(Ctrl + F8)";
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Visible = false;
            stopButton.Click += stopButton_Click;
            // 
            // errorTextBox
            // 
            errorTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            errorTextBox.BackColor = Color.FromArgb(40, 40, 40);
            errorTextBox.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            errorTextBox.ForeColor = Color.FromArgb(152, 151, 26);
            errorTextBox.Location = new Point(12, 441);
            errorTextBox.Multiline = true;
            errorTextBox.Name = "errorTextBox";
            errorTextBox.ReadOnly = true;
            errorTextBox.ScrollBars = ScrollBars.Vertical;
            errorTextBox.Size = new Size(511, 78);
            errorTextBox.TabIndex = 19;
            errorTextBox.Visible = false;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 69.3548355F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.64516F));
            tableLayoutPanel5.Controls.Add(label1, 0, 0);
            tableLayoutPanel5.Controls.Add(posXLabel, 1, 0);
            tableLayoutPanel5.Controls.Add(label3, 0, 1);
            tableLayoutPanel5.Controls.Add(posYLabel, 1, 1);
            tableLayoutPanel5.Controls.Add(currentEncounterIdLabel, 1, 2);
            tableLayoutPanel5.Controls.Add(label4, 0, 2);
            tableLayoutPanel5.Controls.Add(isBattlingLabel, 0, 3);
            tableLayoutPanel5.Controls.Add(isEvent, 1, 3);
            tableLayoutPanel5.Controls.Add(isShiny, 1, 4);
            tableLayoutPanel5.Controls.Add(noMenuLabel, 0, 4);
            tableLayoutPanel5.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            tableLayoutPanel5.ForeColor = Color.FromArgb(152, 151, 26);
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
            // isShiny
            // 
            isShiny.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            isShiny.AutoSize = true;
            isShiny.BackColor = Color.FromArgb(40, 40, 40);
            isShiny.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            isShiny.ForeColor = Color.FromArgb(152, 151, 26);
            isShiny.Location = new Point(146, 88);
            isShiny.Name = "isShiny";
            isShiny.Size = new Size(37, 15);
            isShiny.TabIndex = 11;
            isShiny.Tag = "FontSize=XS";
            isShiny.Text = "Shiny";
            isShiny.Visible = false;
            // 
            // noMenuLabel
            // 
            noMenuLabel.AutoSize = true;
            noMenuLabel.BackColor = Color.FromArgb(40, 40, 40);
            noMenuLabel.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            noMenuLabel.ForeColor = Color.FromArgb(152, 151, 26);
            noMenuLabel.Location = new Point(3, 88);
            noMenuLabel.Name = "noMenuLabel";
            noMenuLabel.Size = new Size(103, 15);
            noMenuLabel.TabIndex = 12;
            noMenuLabel.Tag = "FontSize=XS";
            noMenuLabel.Text = "No menu selected";
            noMenuLabel.Visible = false;
            // 
            // selectPokemonButton
            // 
            selectPokemonButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            selectPokemonButton.BackColor = Color.FromArgb(40, 40, 40);
            selectPokemonButton.FlatAppearance.BorderSize = 3;
            selectPokemonButton.FlatStyle = FlatStyle.Flat;
            selectPokemonButton.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            selectPokemonButton.ForeColor = Color.FromArgb(152, 151, 26);
            selectPokemonButton.Location = new Point(207, 59);
            selectPokemonButton.Name = "selectPokemonButton";
            selectPokemonButton.Size = new Size(319, 112);
            selectPokemonButton.TabIndex = 21;
            selectPokemonButton.Tag = "FontSize=L";
            selectPokemonButton.Text = "Search mode";
            selectPokemonButton.UseVisualStyleBackColor = false;
            selectPokemonButton.Click += button1_Click_2;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.FromArgb(40, 40, 40);
            menuStrip1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            menuStrip1.ForeColor = Color.FromArgb(152, 151, 26);
            menuStrip1.Items.AddRange(new ToolStripItem[] { test1ToolStripMenuItem, dialogueSettingsToolStripMenuItem, donateToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(538, 26);
            menuStrip1.TabIndex = 22;
            menuStrip1.Tag = "FontSize=S";
            menuStrip1.Text = "menuStrip1";
            // 
            // test1ToolStripMenuItem
            // 
            test1ToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            test1ToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            test1ToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            test1ToolStripMenuItem.Name = "test1ToolStripMenuItem";
            test1ToolStripMenuItem.Size = new Size(100, 22);
            test1ToolStripMenuItem.Tag = "FontSize=S";
            test1ToolStripMenuItem.Text = "View Stats";
            test1ToolStripMenuItem.Click += test1ToolStripMenuItem_Click;
            // 
            // dialogueSettingsToolStripMenuItem
            // 
            dialogueSettingsToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            dialogueSettingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dataFolderSettingsToolStripMenuItem, dialogueSettingsToolStripMenuItem1, themeSettingsToolStripMenuItem, updateSettingsToolStripMenuItem, discordToolStripMenuItem });
            dialogueSettingsToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            dialogueSettingsToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            dialogueSettingsToolStripMenuItem.Name = "dialogueSettingsToolStripMenuItem";
            dialogueSettingsToolStripMenuItem.Size = new Size(84, 22);
            dialogueSettingsToolStripMenuItem.Tag = "FontSize=S";
            dialogueSettingsToolStripMenuItem.Text = "Settings";
            dialogueSettingsToolStripMenuItem.Click += dialogueSettingsToolStripMenuItem_Click;
            // 
            // dataFolderSettingsToolStripMenuItem
            // 
            dataFolderSettingsToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            dataFolderSettingsToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            dataFolderSettingsToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            dataFolderSettingsToolStripMenuItem.Name = "dataFolderSettingsToolStripMenuItem";
            dataFolderSettingsToolStripMenuItem.Size = new Size(236, 22);
            dataFolderSettingsToolStripMenuItem.Tag = "FontSize=S";
            dataFolderSettingsToolStripMenuItem.Text = "Data folder settings";
            dataFolderSettingsToolStripMenuItem.Click += dataFolderSettingsToolStripMenuItem_Click;
            // 
            // dialogueSettingsToolStripMenuItem1
            // 
            dialogueSettingsToolStripMenuItem1.BackColor = Color.FromArgb(40, 40, 40);
            dialogueSettingsToolStripMenuItem1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            dialogueSettingsToolStripMenuItem1.ForeColor = Color.FromArgb(152, 151, 26);
            dialogueSettingsToolStripMenuItem1.Name = "dialogueSettingsToolStripMenuItem1";
            dialogueSettingsToolStripMenuItem1.Size = new Size(236, 22);
            dialogueSettingsToolStripMenuItem1.Tag = "FontSize=S";
            dialogueSettingsToolStripMenuItem1.Text = "Dialogue settings";
            dialogueSettingsToolStripMenuItem1.Visible = false;
            dialogueSettingsToolStripMenuItem1.Click += dialogueSettingsToolStripMenuItem1_Click;
            // 
            // themeSettingsToolStripMenuItem
            // 
            themeSettingsToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            themeSettingsToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            themeSettingsToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            themeSettingsToolStripMenuItem.Name = "themeSettingsToolStripMenuItem";
            themeSettingsToolStripMenuItem.Size = new Size(236, 22);
            themeSettingsToolStripMenuItem.Tag = "FontSize=S";
            themeSettingsToolStripMenuItem.Text = "Theme settings";
            themeSettingsToolStripMenuItem.Click += themeSettingsToolStripMenuItem_Click;
            // 
            // updateSettingsToolStripMenuItem
            // 
            updateSettingsToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            updateSettingsToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            updateSettingsToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            updateSettingsToolStripMenuItem.Name = "updateSettingsToolStripMenuItem";
            updateSettingsToolStripMenuItem.Size = new Size(236, 22);
            updateSettingsToolStripMenuItem.Tag = "FontSize=S";
            updateSettingsToolStripMenuItem.Text = "Update settings";
            updateSettingsToolStripMenuItem.Click += updateSettingsToolStripMenuItem_Click;
            // 
            // discordToolStripMenuItem
            // 
            discordToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            discordToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            discordToolStripMenuItem.ForeColor = Color.MediumPurple;
            discordToolStripMenuItem.Name = "discordToolStripMenuItem";
            discordToolStripMenuItem.Size = new Size(236, 22);
            discordToolStripMenuItem.Tag = "FontSize=S;ForeColor=Override;";
            discordToolStripMenuItem.Text = "Discord Settings";
            discordToolStripMenuItem.Click += discordToolStripMenuItem_Click;
            // 
            // donateToolStripMenuItem
            // 
            donateToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            donateToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            donateToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            donateToolStripMenuItem.Size = new Size(100, 22);
            donateToolStripMenuItem.Tag = "FontSize=S";
            donateToolStripMenuItem.Text = "Support me";
            donateToolStripMenuItem.Click += donateToolStripMenuItem_Click;
            // 
            // timeSinceStartLabel
            // 
            timeSinceStartLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            timeSinceStartLabel.BackColor = Color.FromArgb(40, 40, 40);
            timeSinceStartLabel.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            timeSinceStartLabel.ForeColor = Color.FromArgb(152, 151, 26);
            timeSinceStartLabel.Location = new Point(450, 4);
            timeSinceStartLabel.Name = "timeSinceStartLabel";
            timeSinceStartLabel.Size = new Size(78, 22);
            timeSinceStartLabel.TabIndex = 23;
            timeSinceStartLabel.Tag = "FontSize=XS";
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
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(152, 151, 26);
            label2.Location = new Point(345, 3);
            label2.Name = "label2";
            label2.Size = new Size(104, 22);
            label2.TabIndex = 24;
            label2.Tag = "FontSize=XS";
            label2.Text = "Session lenght:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.FromArgb(40, 40, 40);
            pictureBox1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            pictureBox1.ForeColor = Color.FromArgb(152, 151, 26);
            pictureBox1.Image = Properties.Resources.buy_me_a_coffee;
            pictureBox1.Location = new Point(358, 704);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(154, 42);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 25;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // label5
            // 
            label5.BackColor = Color.FromArgb(40, 40, 40);
            label5.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label5.ForeColor = Color.FromArgb(152, 151, 26);
            label5.Location = new Point(12, 704);
            label5.Name = "label5";
            label5.Size = new Size(347, 42);
            label5.TabIndex = 26;
            label5.Tag = "FontSize=S;";
            label5.Text = "If you enjoy using my app or it has helped you out please consider supporting me\r\n";
            // 
            // dontShowBuyMeACoffeeButton
            // 
            dontShowBuyMeACoffeeButton.BackColor = Color.FromArgb(40, 40, 40);
            dontShowBuyMeACoffeeButton.FlatAppearance.BorderColor = SystemColors.Control;
            dontShowBuyMeACoffeeButton.FlatAppearance.BorderSize = 0;
            dontShowBuyMeACoffeeButton.FlatStyle = FlatStyle.Flat;
            dontShowBuyMeACoffeeButton.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            dontShowBuyMeACoffeeButton.ForeColor = Color.FromArgb(152, 151, 26);
            dontShowBuyMeACoffeeButton.Location = new Point(510, 702);
            dontShowBuyMeACoffeeButton.Margin = new Padding(0);
            dontShowBuyMeACoffeeButton.Name = "dontShowBuyMeACoffeeButton";
            dontShowBuyMeACoffeeButton.Size = new Size(34, 42);
            dontShowBuyMeACoffeeButton.TabIndex = 27;
            dontShowBuyMeACoffeeButton.Tag = "FontSize=L";
            dontShowBuyMeACoffeeButton.Text = "X";
            dontShowBuyMeACoffeeButton.UseVisualStyleBackColor = false;
            dontShowBuyMeACoffeeButton.Click += button1_Click_3;
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
            greenLimeThemeComponent1.TabIndex = 30;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // MainBotForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(538, 755);
            Controls.Add(pictureBox1);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(dontShowBuyMeACoffeeButton);
            Controls.Add(label5);
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
            Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainBotForm";
            Text = "PROHack";
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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
        private Label isEvent;
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
        private Label isShiny;
        private Label noMenuLabel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem test1ToolStripMenuItem;
        private Label timeSinceStartLabel;
        private System.Windows.Forms.Timer timeSinceStartedTimer;
        private Label label2;
        private ToolStripMenuItem dialogueSettingsToolStripMenuItem;
        private PictureBox pictureBox1;
        private Label label5;
        private Button dontShowBuyMeACoffeeButton;
        private ToolStripMenuItem dialogueSettingsToolStripMenuItem1;
        private ToolStripMenuItem discordToolStripMenuItem;
        private ToolStripMenuItem updateSettingsToolStripMenuItem;
        private ToolStripMenuItem donateToolStripMenuItem;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
        private ToolStripMenuItem themeSettingsToolStripMenuItem;
        private ToolStripMenuItem dataFolderSettingsToolStripMenuItem;
    }
}
