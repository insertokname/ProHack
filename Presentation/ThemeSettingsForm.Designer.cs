namespace Presentation
{
    partial class ThemeSettingsForm
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
            themeApplier1 = new Infrastructure.Theme.ThemeApplier();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            SuspendLayout();
            // 
            // themeApplier1
            // 
            themeApplier1.BackColor = Color.FromArgb(40, 40, 40);
            themeApplier1.Enabled = false;
            themeApplier1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            themeApplier1.ForeColor = Color.FromArgb(152, 151, 26);
            themeApplier1.Location = new Point(0, 0);
            themeApplier1.Name = "themeApplier1";
            themeApplier1.Size = new Size(0, 0);
            themeApplier1.TabIndex = 0;
            themeApplier1.TabStop = false;
            themeApplier1.Text = "themeApplier1";
            themeApplier1.Visible = false;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.BackColor = Color.FromArgb(40, 40, 40);
            radioButton1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            radioButton1.ForeColor = Color.FromArgb(152, 151, 26);
            radioButton1.Location = new Point(12, 12);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(107, 29);
            radioButton1.TabIndex = 1;
            radioButton1.TabStop = true;
            radioButton1.Tag = "FontSize=M";
            radioButton1.Text = "Gruvbox";
            radioButton1.UseVisualStyleBackColor = false;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.BackColor = Color.FromArgb(40, 40, 40);
            radioButton2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            radioButton2.ForeColor = Color.FromArgb(152, 151, 26);
            radioButton2.Location = new Point(12, 47);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(173, 29);
            radioButton2.TabIndex = 2;
            radioButton2.TabStop = true;
            radioButton2.Tag = "FontSize=M";
            radioButton2.Text = "Light Gruvbox";
            radioButton2.UseVisualStyleBackColor = false;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.BackColor = Color.FromArgb(40, 40, 40);
            radioButton3.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            radioButton3.ForeColor = Color.FromArgb(152, 151, 26);
            radioButton3.Location = new Point(12, 82);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(107, 29);
            radioButton3.TabIndex = 3;
            radioButton3.Tag = "FontSize=M";
            radioButton3.Text = "Classic";
            radioButton3.UseVisualStyleBackColor = false;
            radioButton3.CheckedChanged += radioButton3_CheckedChanged;
            // 
            // ThemeSettingsForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(239, 116);
            Controls.Add(radioButton3);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(themeApplier1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5);
            MaximizeBox = false;
            Name = "ThemeSettingsForm";
            Text = "Theme Settings";
            Load += ThemeSettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Infrastructure.Theme.ThemeApplier themeApplier1;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
    }
}