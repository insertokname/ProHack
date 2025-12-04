namespace Presentation
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            gameLabel = new Label();
            label3 = new Label();
            label1 = new Label();
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            label2 = new Label();
            SuspendLayout();
            // 
            // gameLabel
            // 
            gameLabel.AutoSize = true;
            gameLabel.BackColor = Color.FromArgb(40, 40, 40);
            gameLabel.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            gameLabel.ForeColor = Color.FromArgb(152, 151, 26);
            gameLabel.Location = new Point(12, 9);
            gameLabel.Name = "gameLabel";
            gameLabel.Size = new Size(392, 32);
            gameLabel.TabIndex = 27;
            gameLabel.Tag = "FontSize=L";
            gameLabel.Text = "Thank you for using my app!";
            // 
            // label3
            // 
            label3.BackColor = Color.FromArgb(40, 40, 40);
            label3.Font = new Font("Cascadia Code", 14F, FontStyle.Bold | FontStyle.Underline);
            label3.ForeColor = Color.RoyalBlue;
            label3.Location = new Point(12, 117);
            label3.Name = "label3";
            label3.Size = new Size(180, 25);
            label3.TabIndex = 32;
            label3.Tag = "FontSize=M;ForeColor=Override;Font=Override";
            label3.Text = "Privacy policy";
            label3.Click += label3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(12, 55);
            label1.Name = "label1";
            label1.Size = new Size(144, 25);
            label1.TabIndex = 33;
            label1.Tag = "FontSize=M";
            label1.Text = "App version:";
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(335, 40);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(32, 40);
            greenLimeThemeComponent1.TabIndex = 30;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(152, 151, 26);
            label2.Location = new Point(12, 85);
            label2.Name = "label2";
            label2.Size = new Size(232, 25);
            label2.TabIndex = 34;
            label2.Tag = "FontSize=M";
            label2.Text = "Made by insertokname";
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(404, 153);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(gameLabel);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "AboutForm";
            Text = "About";
            Load += AboutForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label gameLabel;
        private Label label3;
        private Label label1;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
        private Label label2;
    }
}