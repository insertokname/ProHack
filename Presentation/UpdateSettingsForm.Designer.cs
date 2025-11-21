namespace Presentation
{
    partial class UpdateSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateSettingsForm));
            autoCheckUpdatesCheckbox = new CheckBox();
            autoUpdateCheckBox = new CheckBox();
            button1 = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            progressBar1 = new ProgressBar();
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            button2 = new Button();
            SuspendLayout();
            // 
            // autoCheckUpdatesCheckbox
            // 
            autoCheckUpdatesCheckbox.AutoSize = true;
            autoCheckUpdatesCheckbox.BackColor = Color.FromArgb(40, 40, 40);
            autoCheckUpdatesCheckbox.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            autoCheckUpdatesCheckbox.ForeColor = Color.FromArgb(152, 151, 26);
            autoCheckUpdatesCheckbox.Location = new Point(12, 12);
            autoCheckUpdatesCheckbox.Name = "autoCheckUpdatesCheckbox";
            autoCheckUpdatesCheckbox.Size = new Size(259, 22);
            autoCheckUpdatesCheckbox.TabIndex = 1;
            autoCheckUpdatesCheckbox.Tag = "FontSize=S";
            autoCheckUpdatesCheckbox.Text = "Check for updates when opened";
            autoCheckUpdatesCheckbox.UseVisualStyleBackColor = false;
            autoCheckUpdatesCheckbox.CheckedChanged += autoCheckUpdatesCheckbox_CheckedChanged;
            // 
            // autoUpdateCheckBox
            // 
            autoUpdateCheckBox.AutoSize = true;
            autoUpdateCheckBox.BackColor = Color.FromArgb(40, 40, 40);
            autoUpdateCheckBox.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            autoUpdateCheckBox.ForeColor = Color.FromArgb(152, 151, 26);
            autoUpdateCheckBox.Location = new Point(12, 41);
            autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            autoUpdateCheckBox.Size = new Size(307, 22);
            autoUpdateCheckBox.TabIndex = 2;
            autoUpdateCheckBox.Tag = "FontSize=S";
            autoUpdateCheckBox.Text = "Automatically update when available";
            autoUpdateCheckBox.UseVisualStyleBackColor = false;
            autoUpdateCheckBox.CheckedChanged += autoUpdateCheckBox_CheckedChanged;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(152, 151, 26);
            button1.Location = new Point(12, 113);
            button1.Name = "button1";
            button1.Size = new Size(307, 102);
            button1.TabIndex = 4;
            button1.Text = "Check for updates";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(40, 40, 40);
            textBox1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            textBox1.ForeColor = Color.FromArgb(152, 151, 26);
            textBox1.Location = new Point(130, 74);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(129, 23);
            textBox1.TabIndex = 3;
            textBox1.Tag = "FontSize=S";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(12, 74);
            label1.Name = "label1";
            label1.Size = new Size(112, 18);
            label1.TabIndex = 27;
            label1.Tag = "FontSize=S";
            label1.Text = "Skip version:";
            // 
            // progressBar1
            // 
            progressBar1.BackColor = Color.FromArgb(40, 40, 40);
            progressBar1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            progressBar1.ForeColor = Color.FromArgb(152, 151, 26);
            progressBar1.Location = new Point(12, 226);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(307, 32);
            progressBar1.TabIndex = 28;
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(-8, -6);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(155, 19);
            greenLimeThemeComponent1.TabIndex = 29;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(40, 40, 40);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            button2.ForeColor = Color.FromArgb(152, 151, 26);
            button2.Location = new Point(265, 74);
            button2.Name = "button2";
            button2.Size = new Size(54, 23);
            button2.TabIndex = 30;
            button2.Tag = "FontSize=XS";
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = false;
            // 
            // UpdateSettingsForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(326, 224);
            Controls.Add(button2);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(progressBar1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(autoUpdateCheckBox);
            Controls.Add(autoCheckUpdatesCheckbox);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "UpdateSettingsForm";
            Text = "Update Settings";
            Load += UpdateSettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox autoCheckUpdatesCheckbox;
        private CheckBox autoUpdateCheckBox;
        private Button button1;
        private TextBox textBox1;
        private Label label1;
        private ProgressBar progressBar1;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
        private Button button2;
    }
}