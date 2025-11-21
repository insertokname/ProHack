namespace Presentation
{
    partial class PokemonSelectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokemonSelectForm));
            IdNumeric = new NumericUpDown();
            idLabel = new Label();
            AddTargetButton = new Button();
            pictureBox1 = new PictureBox();
            CatchAnythingCheckbox = new CheckBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            MustBeShinyCheckbox = new CheckBox();
            MustBeEventCheckbox = new CheckBox();
            CloseButton = new Button();
            RightButton = new Button();
            LeftButton = new Button();
            label1 = new Label();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openFileToolStripMenuItem = new ToolStripMenuItem();
            saveTargetsToolStripMenuItem = new ToolStripMenuItem();
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            ((System.ComponentModel.ISupportInitialize)IdNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // IdNumeric
            // 
            IdNumeric.AllowDrop = true;
            IdNumeric.Anchor = AnchorStyles.Bottom;
            IdNumeric.BackColor = Color.FromArgb(40, 40, 40);
            IdNumeric.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            IdNumeric.ForeColor = Color.FromArgb(152, 151, 26);
            IdNumeric.Location = new Point(193, 94);
            IdNumeric.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            IdNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            IdNumeric.Name = "IdNumeric";
            IdNumeric.Size = new Size(113, 35);
            IdNumeric.TabIndex = 0;
            IdNumeric.Tag = "FontSize=L";
            IdNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
            IdNumeric.Visible = false;
            IdNumeric.ValueChanged += IdNumeric_ValueChanged;
            // 
            // idLabel
            // 
            idLabel.Anchor = AnchorStyles.Bottom;
            idLabel.AutoSize = true;
            idLabel.BackColor = Color.FromArgb(40, 40, 40);
            idLabel.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            idLabel.ForeColor = Color.FromArgb(152, 151, 26);
            idLabel.Location = new Point(155, 99);
            idLabel.Name = "idLabel";
            idLabel.Size = new Size(32, 18);
            idLabel.TabIndex = 1;
            idLabel.Tag = "FontSize=S";
            idLabel.Text = "Id:";
            idLabel.Visible = false;
            // 
            // AddTargetButton
            // 
            AddTargetButton.Anchor = AnchorStyles.Bottom;
            AddTargetButton.BackColor = Color.FromArgb(40, 40, 40);
            AddTargetButton.FlatAppearance.BorderSize = 3;
            AddTargetButton.FlatStyle = FlatStyle.Flat;
            AddTargetButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            AddTargetButton.ForeColor = Color.FromArgb(152, 151, 26);
            AddTargetButton.Location = new Point(12, 669);
            AddTargetButton.Name = "AddTargetButton";
            AddTargetButton.Size = new Size(282, 77);
            AddTargetButton.TabIndex = 5;
            AddTargetButton.TabStop = false;
            AddTargetButton.Text = "Add target";
            AddTargetButton.UseVisualStyleBackColor = false;
            AddTargetButton.Click += AddTargetButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Bottom;
            pictureBox1.BackColor = Color.FromArgb(40, 40, 40);
            pictureBox1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            pictureBox1.ForeColor = Color.FromArgb(152, 151, 26);
            pictureBox1.Image = Properties.Resources.any;
            pictureBox1.Location = new Point(12, 132);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(568, 390);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // CatchAnythingCheckbox
            // 
            CatchAnythingCheckbox.Anchor = AnchorStyles.Bottom;
            CatchAnythingCheckbox.AutoSize = true;
            CatchAnythingCheckbox.BackColor = Color.FromArgb(40, 40, 40);
            CatchAnythingCheckbox.Checked = true;
            CatchAnythingCheckbox.CheckState = CheckState.Checked;
            CatchAnythingCheckbox.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            CatchAnythingCheckbox.ForeColor = Color.FromArgb(152, 151, 26);
            CatchAnythingCheckbox.Location = new Point(12, 97);
            CatchAnythingCheckbox.Name = "CatchAnythingCheckbox";
            CatchAnythingCheckbox.Size = new Size(139, 22);
            CatchAnythingCheckbox.TabIndex = 7;
            CatchAnythingCheckbox.TabStop = false;
            CatchAnythingCheckbox.Tag = "FontSize=S";
            CatchAnythingCheckbox.Text = "Catch anything";
            CatchAnythingCheckbox.UseVisualStyleBackColor = false;
            CatchAnythingCheckbox.CheckedChanged += CatchAnythingCheckbox_CheckedChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Bottom;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BackColor = Color.FromArgb(40, 40, 40);
            flowLayoutPanel1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            flowLayoutPanel1.ForeColor = Color.FromArgb(152, 151, 26);
            flowLayoutPanel1.Location = new Point(12, 528);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(568, 135);
            flowLayoutPanel1.TabIndex = 9;
            flowLayoutPanel1.WrapContents = false;
            // 
            // MustBeShinyCheckbox
            // 
            MustBeShinyCheckbox.Anchor = AnchorStyles.Bottom;
            MustBeShinyCheckbox.AutoSize = true;
            MustBeShinyCheckbox.BackColor = Color.FromArgb(40, 40, 40);
            MustBeShinyCheckbox.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            MustBeShinyCheckbox.ForeColor = Color.FromArgb(152, 151, 26);
            MustBeShinyCheckbox.Location = new Point(450, 101);
            MustBeShinyCheckbox.Name = "MustBeShinyCheckbox";
            MustBeShinyCheckbox.Size = new Size(131, 22);
            MustBeShinyCheckbox.TabIndex = 10;
            MustBeShinyCheckbox.TabStop = false;
            MustBeShinyCheckbox.Tag = "FontSize=S";
            MustBeShinyCheckbox.Text = "Must be shiny";
            MustBeShinyCheckbox.UseVisualStyleBackColor = false;
            MustBeShinyCheckbox.CheckedChanged += MustBeShinyCheckbox_CheckedChanged;
            // 
            // MustBeEventCheckbox
            // 
            MustBeEventCheckbox.Anchor = AnchorStyles.Bottom;
            MustBeEventCheckbox.AutoSize = true;
            MustBeEventCheckbox.BackColor = Color.FromArgb(40, 40, 40);
            MustBeEventCheckbox.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            MustBeEventCheckbox.ForeColor = Color.FromArgb(152, 151, 26);
            MustBeEventCheckbox.Location = new Point(310, 101);
            MustBeEventCheckbox.Name = "MustBeEventCheckbox";
            MustBeEventCheckbox.Size = new Size(131, 22);
            MustBeEventCheckbox.TabIndex = 11;
            MustBeEventCheckbox.TabStop = false;
            MustBeEventCheckbox.Tag = "FontSize=S";
            MustBeEventCheckbox.Text = "Must be event";
            MustBeEventCheckbox.UseVisualStyleBackColor = false;
            MustBeEventCheckbox.CheckedChanged += MustBeEventCheckbox_CheckedChanged;
            // 
            // CloseButton
            // 
            CloseButton.Anchor = AnchorStyles.Bottom;
            CloseButton.BackColor = Color.FromArgb(40, 40, 40);
            CloseButton.FlatAppearance.BorderSize = 3;
            CloseButton.FlatStyle = FlatStyle.Flat;
            CloseButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            CloseButton.ForeColor = Color.FromArgb(152, 151, 26);
            CloseButton.Location = new Point(300, 669);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(280, 77);
            CloseButton.TabIndex = 12;
            CloseButton.TabStop = false;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = false;
            CloseButton.Click += Close_Click;
            // 
            // RightButton
            // 
            RightButton.Anchor = AnchorStyles.Bottom;
            RightButton.BackColor = Color.FromArgb(40, 40, 40);
            RightButton.FlatAppearance.BorderSize = 3;
            RightButton.FlatStyle = FlatStyle.Flat;
            RightButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            RightButton.ForeColor = Color.FromArgb(152, 151, 26);
            RightButton.Location = new Point(525, 280);
            RightButton.Name = "RightButton";
            RightButton.Size = new Size(55, 87);
            RightButton.TabIndex = 13;
            RightButton.TabStop = false;
            RightButton.Text = ">";
            RightButton.UseVisualStyleBackColor = false;
            RightButton.Visible = false;
            RightButton.Click += RightButton_Click;
            // 
            // LeftButton
            // 
            LeftButton.Anchor = AnchorStyles.Bottom;
            LeftButton.BackColor = Color.FromArgb(40, 40, 40);
            LeftButton.FlatAppearance.BorderSize = 3;
            LeftButton.FlatStyle = FlatStyle.Flat;
            LeftButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            LeftButton.ForeColor = Color.FromArgb(152, 151, 26);
            LeftButton.Location = new Point(12, 280);
            LeftButton.Name = "LeftButton";
            LeftButton.Size = new Size(55, 87);
            LeftButton.TabIndex = 14;
            LeftButton.TabStop = false;
            LeftButton.Text = "<";
            LeftButton.UseVisualStyleBackColor = false;
            LeftButton.Visible = false;
            LeftButton.Click += LeftButton_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(12, 33);
            label1.Name = "label1";
            label1.Size = new Size(568, 58);
            label1.TabIndex = 15;
            label1.Tag = "FontSize=S";
            label1.Text = "If you don't find the right sprite here make sure you have the right version of the DATA folder! Check online for it. \r\nYou can use WASD shortcuts for switching pokemon forms and ids!\r\n";
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.FromArgb(40, 40, 40);
            menuStrip1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            menuStrip1.ForeColor = Color.FromArgb(152, 151, 26);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(592, 26);
            menuStrip1.TabIndex = 16;
            menuStrip1.Tag = "FontSize=S";
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, saveTargetsToolStripMenuItem });
            fileToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            fileToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(52, 22);
            fileToolStripMenuItem.Tag = "FontSize=S";
            fileToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            openFileToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            openFileToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new Size(180, 22);
            openFileToolStripMenuItem.Tag = "FontSize=S";
            openFileToolStripMenuItem.Text = "Open targets";
            openFileToolStripMenuItem.Click += OpenFileToolStripMenuItem_Click;
            // 
            // saveTargetsToolStripMenuItem
            // 
            saveTargetsToolStripMenuItem.BackColor = Color.FromArgb(40, 40, 40);
            saveTargetsToolStripMenuItem.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            saveTargetsToolStripMenuItem.ForeColor = Color.FromArgb(152, 151, 26);
            saveTargetsToolStripMenuItem.Name = "saveTargetsToolStripMenuItem";
            saveTargetsToolStripMenuItem.Size = new Size(180, 22);
            saveTargetsToolStripMenuItem.Tag = "FontSize=S";
            saveTargetsToolStripMenuItem.Text = "Save targets";
            saveTargetsToolStripMenuItem.Click += sSaveTargetsToolStripMenuItem_Click;
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
            // PokemonSelectForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(592, 752);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(label1);
            Controls.Add(LeftButton);
            Controls.Add(RightButton);
            Controls.Add(CloseButton);
            Controls.Add(MustBeEventCheckbox);
            Controls.Add(MustBeShinyCheckbox);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(CatchAnythingCheckbox);
            Controls.Add(pictureBox1);
            Controls.Add(AddTargetButton);
            Controls.Add(idLabel);
            Controls.Add(IdNumeric);
            Controls.Add(menuStrip1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MinimumSize = new Size(608, 521);
            Name = "PokemonSelectForm";
            Text = "Pokemon Select";
            Load += PokemonSelect_Load;
            KeyDown += PokemonSelect_KeyDown;
            ((System.ComponentModel.ISupportInitialize)IdNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown IdNumeric;
        private Label idLabel;
        private Button AddTargetButton;
        private PictureBox pictureBox1;
        private CheckBox CatchAnythingCheckbox;
        private FlowLayoutPanel flowLayoutPanel1;
        private CheckBox MustBeShinyCheckbox;
        private CheckBox MustBeEventCheckbox;
        private Button CloseButton;
        private Button RightButton;
        private Button LeftButton;
        private Label label1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripMenuItem saveTargetsToolStripMenuItem;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
    }
}