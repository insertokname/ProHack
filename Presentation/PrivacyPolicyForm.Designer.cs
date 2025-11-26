namespace Presentation
{
    partial class PrivacyPolicyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrivacyPolicyForm));
            label1 = new Label();
            continueButton = new Button();
            textBox1 = new TextBox();
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 18F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(123, 28);
            label1.Name = "label1";
            label1.Size = new Size(210, 32);
            label1.TabIndex = 18;
            label1.Tag = "FontSize=L";
            label1.Text = "Privacy policy";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // continueButton
            // 
            continueButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            continueButton.BackColor = Color.FromArgb(40, 40, 40);
            continueButton.FlatStyle = FlatStyle.Flat;
            continueButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            continueButton.ForeColor = Color.FromArgb(152, 151, 26);
            continueButton.Location = new Point(240, 423);
            continueButton.Name = "continueButton";
            continueButton.Size = new Size(209, 44);
            continueButton.TabIndex = 23;
            continueButton.Text = "I Agree";
            continueButton.UseVisualStyleBackColor = false;
            continueButton.Click += button_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(40, 40, 40);
            textBox1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            textBox1.ForeColor = Color.FromArgb(152, 151, 26);
            textBox1.Location = new Point(12, 92);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(437, 261);
            textBox1.TabIndex = 30;
            textBox1.Tag = "FontSize=S";
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(28, 359);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(95, 80);
            greenLimeThemeComponent1.TabIndex = 29;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // PrivacyPolicyForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(461, 479);
            Controls.Add(textBox1);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(continueButton);
            Controls.Add(label1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PrivacyPolicyForm";
            Text = "Privacy Policy";
            Load += PrivacyPolicyForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button continueButton;
        private TextBox textBox1;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
    }
}