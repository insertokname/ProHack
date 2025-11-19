namespace Presentation
{
    partial class UpdateRequestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateRequestForm));
            label1 = new Label();
            richTextBox1 = new RichTextBox();
            label2 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            button1 = new Button();
            updateButton = new Button();
            skipUpdateButton = new Button();
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(243, 25);
            label1.TabIndex = 17;
            label1.Tag = "FontSize=L";
            label1.Text = "New update available:";
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.FromArgb(40, 40, 40);
            richTextBox1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            richTextBox1.ForeColor = Color.FromArgb(152, 151, 26);
            richTextBox1.Location = new Point(12, 83);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(601, 325);
            richTextBox1.TabIndex = 18;
            richTextBox1.TabStop = false;
            richTextBox1.Tag = "FontSize=S";
            richTextBox1.Text = "";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(152, 151, 26);
            label2.Location = new Point(13, 55);
            label2.Name = "label2";
            label2.Size = new Size(144, 25);
            label2.TabIndex = 19;
            label2.Text = "Patch notes:";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Controls.Add(button1, 0, 0);
            tableLayoutPanel1.Controls.Add(updateButton, 2, 0);
            tableLayoutPanel1.Controls.Add(skipUpdateButton, 1, 0);
            tableLayoutPanel1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            tableLayoutPanel1.ForeColor = Color.FromArgb(152, 151, 26);
            tableLayoutPanel1.Location = new Point(12, 414);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(601, 50);
            tableLayoutPanel1.TabIndex = 20;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.Dock = DockStyle.Fill;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(152, 151, 26);
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(194, 44);
            button1.TabIndex = 17;
            button1.Tag = "FontSize=S";
            button1.Text = "Update settings";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // updateButton
            // 
            updateButton.BackColor = Color.FromArgb(40, 40, 40);
            updateButton.Dock = DockStyle.Fill;
            updateButton.FlatStyle = FlatStyle.Flat;
            updateButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            updateButton.ForeColor = Color.FromArgb(152, 151, 26);
            updateButton.Location = new Point(403, 3);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(195, 44);
            updateButton.TabIndex = 12;
            updateButton.Tag = "FontSize=S";
            updateButton.Text = "Update";
            updateButton.UseVisualStyleBackColor = false;
            updateButton.Click += updateButton_Click;
            // 
            // skipUpdateButton
            // 
            skipUpdateButton.BackColor = Color.FromArgb(40, 40, 40);
            skipUpdateButton.Dock = DockStyle.Fill;
            skipUpdateButton.FlatStyle = FlatStyle.Flat;
            skipUpdateButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            skipUpdateButton.ForeColor = Color.FromArgb(152, 151, 26);
            skipUpdateButton.Location = new Point(203, 3);
            skipUpdateButton.Name = "skipUpdateButton";
            skipUpdateButton.Size = new Size(194, 44);
            skipUpdateButton.TabIndex = 16;
            skipUpdateButton.Tag = "FontSize=S";
            skipUpdateButton.Text = "Skip this update";
            skipUpdateButton.UseVisualStyleBackColor = false;
            skipUpdateButton.Click += skipUpdateButton_Click;
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(-20, -8);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(247, 23);
            greenLimeThemeComponent1.TabIndex = 29;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // UpdateRequestForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(625, 476);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(label2);
            Controls.Add(richTextBox1);
            Controls.Add(label1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "UpdateRequestForm";
            Text = "Update Request";
            Load += UpdateRequestForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private RichTextBox richTextBox1;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel1;
        private Button skipUpdateButton;
        private Button updateButton;
        private Button button1;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
    }
}