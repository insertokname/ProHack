namespace Presentation
{
    partial class PokemonSelect
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
            idNumeric = new NumericUpDown();
            idLabel = new Label();
            retryButton = new Button();
            pictureBox1 = new PictureBox();
            catchAnythingCheckbox = new CheckBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            mustBeShinyCheckbox = new CheckBox();
            mustBeEventCheckbox = new CheckBox();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)idNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // idNumeric
            // 
            idNumeric.BackColor = Color.LavenderBlush;
            idNumeric.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            idNumeric.ForeColor = Color.Crimson;
            idNumeric.Location = new Point(191, 11);
            idNumeric.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            idNumeric.Name = "idNumeric";
            idNumeric.Size = new Size(113, 29);
            idNumeric.TabIndex = 0;
            idNumeric.Visible = false;
            idNumeric.ValueChanged += idNumeric_ValueChanged;
            // 
            // idLabel
            // 
            idLabel.AutoSize = true;
            idLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            idLabel.Location = new Point(155, 13);
            idLabel.Name = "idLabel";
            idLabel.Size = new Size(29, 21);
            idLabel.TabIndex = 1;
            idLabel.Text = "Id:";
            idLabel.Visible = false;
            // 
            // retryButton
            // 
            retryButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            retryButton.BackColor = Color.LavenderBlush;
            retryButton.FlatStyle = FlatStyle.Flat;
            retryButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            retryButton.ForeColor = Color.Crimson;
            retryButton.Location = new Point(12, 598);
            retryButton.Name = "retryButton";
            retryButton.Size = new Size(282, 77);
            retryButton.TabIndex = 5;
            retryButton.Text = "Add target";
            retryButton.UseVisualStyleBackColor = false;
            retryButton.Click += retryButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(12, 46);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(568, 405);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // catchAnythingCheckbox
            // 
            catchAnythingCheckbox.AutoSize = true;
            catchAnythingCheckbox.Checked = true;
            catchAnythingCheckbox.CheckState = CheckState.Checked;
            catchAnythingCheckbox.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            catchAnythingCheckbox.Location = new Point(12, 12);
            catchAnythingCheckbox.Name = "catchAnythingCheckbox";
            catchAnythingCheckbox.Size = new Size(137, 25);
            catchAnythingCheckbox.TabIndex = 7;
            catchAnythingCheckbox.Text = "Catch anything";
            catchAnythingCheckbox.UseVisualStyleBackColor = true;
            catchAnythingCheckbox.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Location = new Point(12, 457);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(568, 135);
            flowLayoutPanel1.TabIndex = 9;
            flowLayoutPanel1.WrapContents = false;
            // 
            // mustBeShinyCheckbox
            // 
            mustBeShinyCheckbox.AutoSize = true;
            mustBeShinyCheckbox.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            mustBeShinyCheckbox.Location = new Point(450, 12);
            mustBeShinyCheckbox.Name = "mustBeShinyCheckbox";
            mustBeShinyCheckbox.Size = new Size(130, 25);
            mustBeShinyCheckbox.TabIndex = 10;
            mustBeShinyCheckbox.Text = "Must be shiny";
            mustBeShinyCheckbox.UseVisualStyleBackColor = true;
            mustBeShinyCheckbox.CheckedChanged += mustBeShinyCheckbox_CheckedChanged;
            // 
            // mustBeEventCheckbox
            // 
            mustBeEventCheckbox.AutoSize = true;
            mustBeEventCheckbox.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            mustBeEventCheckbox.Location = new Point(310, 12);
            mustBeEventCheckbox.Name = "mustBeEventCheckbox";
            mustBeEventCheckbox.Size = new Size(134, 25);
            mustBeEventCheckbox.TabIndex = 11;
            mustBeEventCheckbox.Text = "Must be event";
            mustBeEventCheckbox.UseVisualStyleBackColor = true;
            mustBeEventCheckbox.CheckedChanged += mustBeEventCheckbox_CheckedChanged;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button1.BackColor = Color.LavenderBlush;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.Crimson;
            button1.Location = new Point(300, 598);
            button1.Name = "button1";
            button1.Size = new Size(280, 77);
            button1.TabIndex = 12;
            button1.Text = "Close";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // PokemonSelect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(592, 683);
            Controls.Add(button1);
            Controls.Add(mustBeEventCheckbox);
            Controls.Add(mustBeShinyCheckbox);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(catchAnythingCheckbox);
            Controls.Add(pictureBox1);
            Controls.Add(retryButton);
            Controls.Add(idLabel);
            Controls.Add(idNumeric);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimumSize = new Size(608, 521);
            Name = "PokemonSelect";
            Text = "PokemonSelect";
            Load += PokemonSelect_Load;
            ((System.ComponentModel.ISupportInitialize)idNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown idNumeric;
        private Label idLabel;
        private Button retryButton;
        private PictureBox pictureBox1;
        private CheckBox catchAnythingCheckbox;
        private FlowLayoutPanel flowLayoutPanel1;
        private CheckBox mustBeShinyCheckbox;
        private CheckBox mustBeEventCheckbox;
        private Button button1;
    }
}