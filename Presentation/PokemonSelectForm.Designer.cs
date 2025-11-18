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
            ((System.ComponentModel.ISupportInitialize)IdNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // IdNumeric
            // 
            IdNumeric.Anchor = AnchorStyles.Bottom;
            IdNumeric.BackColor = Color.LavenderBlush;
            IdNumeric.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            IdNumeric.ForeColor = Color.Crimson;
            IdNumeric.Location = new Point(191, 97);
            IdNumeric.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            IdNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            IdNumeric.Name = "IdNumeric";
            IdNumeric.Size = new Size(113, 29);
            IdNumeric.TabIndex = 0;
            IdNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
            IdNumeric.Visible = false;
            IdNumeric.ValueChanged += IdNumeric_ValueChanged;
            // 
            // idLabel
            // 
            idLabel.Anchor = AnchorStyles.Bottom;
            idLabel.AutoSize = true;
            idLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            idLabel.Location = new Point(155, 99);
            idLabel.Name = "idLabel";
            idLabel.Size = new Size(29, 21);
            idLabel.TabIndex = 1;
            idLabel.Text = "Id:";
            idLabel.Visible = false;
            // 
            // AddTargetButton
            // 
            AddTargetButton.Anchor = AnchorStyles.Bottom;
            AddTargetButton.BackColor = Color.LavenderBlush;
            AddTargetButton.FlatStyle = FlatStyle.Flat;
            AddTargetButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            AddTargetButton.ForeColor = Color.Crimson;
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
            CatchAnythingCheckbox.Checked = true;
            CatchAnythingCheckbox.CheckState = CheckState.Checked;
            CatchAnythingCheckbox.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            CatchAnythingCheckbox.Location = new Point(12, 98);
            CatchAnythingCheckbox.Name = "CatchAnythingCheckbox";
            CatchAnythingCheckbox.Size = new Size(137, 25);
            CatchAnythingCheckbox.TabIndex = 7;
            CatchAnythingCheckbox.TabStop = false;
            CatchAnythingCheckbox.Text = "Catch anything";
            CatchAnythingCheckbox.UseVisualStyleBackColor = true;
            CatchAnythingCheckbox.CheckedChanged += CatchAnythingCheckbox_CheckedChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Bottom;
            flowLayoutPanel1.AutoScroll = true;
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
            MustBeShinyCheckbox.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            MustBeShinyCheckbox.Location = new Point(450, 98);
            MustBeShinyCheckbox.Name = "MustBeShinyCheckbox";
            MustBeShinyCheckbox.Size = new Size(130, 25);
            MustBeShinyCheckbox.TabIndex = 10;
            MustBeShinyCheckbox.TabStop = false;
            MustBeShinyCheckbox.Text = "Must be shiny";
            MustBeShinyCheckbox.UseVisualStyleBackColor = true;
            MustBeShinyCheckbox.CheckedChanged += MustBeShinyCheckbox_CheckedChanged;
            // 
            // MustBeEventCheckbox
            // 
            MustBeEventCheckbox.Anchor = AnchorStyles.Bottom;
            MustBeEventCheckbox.AutoSize = true;
            MustBeEventCheckbox.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            MustBeEventCheckbox.Location = new Point(310, 98);
            MustBeEventCheckbox.Name = "MustBeEventCheckbox";
            MustBeEventCheckbox.Size = new Size(134, 25);
            MustBeEventCheckbox.TabIndex = 11;
            MustBeEventCheckbox.TabStop = false;
            MustBeEventCheckbox.Text = "Must be event";
            MustBeEventCheckbox.UseVisualStyleBackColor = true;
            MustBeEventCheckbox.CheckedChanged += MustBeEventCheckbox_CheckedChanged;
            // 
            // CloseButton
            // 
            CloseButton.Anchor = AnchorStyles.Bottom;
            CloseButton.BackColor = Color.LavenderBlush;
            CloseButton.FlatStyle = FlatStyle.Flat;
            CloseButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CloseButton.ForeColor = Color.Crimson;
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
            RightButton.BackColor = Color.LavenderBlush;
            RightButton.FlatStyle = FlatStyle.Flat;
            RightButton.Font = new Font("Segoe UI", 20F);
            RightButton.ForeColor = Color.Crimson;
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
            LeftButton.BackColor = Color.LavenderBlush;
            LeftButton.FlatStyle = FlatStyle.Flat;
            LeftButton.Font = new Font("Segoe UI", 20F);
            LeftButton.ForeColor = Color.Crimson;
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
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label1.ForeColor = Color.Crimson;
            label1.Location = new Point(12, 23);
            label1.Name = "label1";
            label1.Size = new Size(568, 66);
            label1.TabIndex = 15;
            label1.Text = "If you don't find the right sprite here make sure you have the right version of the DATA folder! Check online for it. \r\nYou can use WASD shortcuts for switching pokemon forms and ids!\r\n";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(592, 24);
            menuStrip1.TabIndex = 16;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, saveTargetsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new Size(142, 22);
            openFileToolStripMenuItem.Text = "Open targets";
            openFileToolStripMenuItem.Click += OpenFileToolStripMenuItem_Click;
            // 
            // saveTargetsToolStripMenuItem
            // 
            saveTargetsToolStripMenuItem.Name = "saveTargetsToolStripMenuItem";
            saveTargetsToolStripMenuItem.Size = new Size(142, 22);
            saveTargetsToolStripMenuItem.Text = "Save targets";
            saveTargetsToolStripMenuItem.Click += sSaveTargetsToolStripMenuItem_Click;
            // 
            // PokemonSelectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(592, 752);
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
    }
}