namespace Presentation
{
    partial class WarningForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarningForm));
            label1 = new Label();
            label2 = new Label();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            selectPokemonButton = new Button();
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
            label1.Location = new Point(112, 9);
            label1.Name = "label1";
            label1.Size = new Size(252, 64);
            label1.TabIndex = 18;
            label1.Tag = "FontSize=L";
            label1.Text = "WARNING\r\nUSE RESPONSABILY!";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(152, 151, 26);
            label2.Location = new Point(12, 92);
            label2.Name = "label2";
            label2.Size = new Size(437, 240);
            label2.TabIndex = 20;
            label2.Tag = "FontSize=M";
            label2.Text = "DO NOT go to sleep while the bot is running. \r\nDO NOT leave it on for tens of hours unwatched.\r\n\r\nIf you are banned all your progress will be lost.\r\n\r\nUSE RESPONSABILY!";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.BackColor = Color.FromArgb(40, 40, 40);
            checkBox1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            checkBox1.ForeColor = Color.FromArgb(152, 151, 26);
            checkBox1.Location = new Point(208, 375);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(251, 40);
            checkBox1.TabIndex = 21;
            checkBox1.Tag = "FontSize=S";
            checkBox1.Text = "I have read the message\r\n and i understand the risks.\r\n";
            checkBox1.UseVisualStyleBackColor = false;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.BackColor = Color.FromArgb(40, 40, 40);
            checkBox2.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            checkBox2.ForeColor = Color.FromArgb(152, 151, 26);
            checkBox2.Location = new Point(208, 346);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(203, 22);
            checkBox2.TabIndex = 22;
            checkBox2.Tag = "FontSize=S";
            checkBox2.Text = "Don't show this again.";
            checkBox2.UseVisualStyleBackColor = false;
            // 
            // selectPokemonButton
            // 
            selectPokemonButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            selectPokemonButton.BackColor = Color.FromArgb(40, 40, 40);
            selectPokemonButton.FlatStyle = FlatStyle.Flat;
            selectPokemonButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            selectPokemonButton.ForeColor = Color.FromArgb(152, 151, 26);
            selectPokemonButton.Location = new Point(208, 423);
            selectPokemonButton.Name = "selectPokemonButton";
            selectPokemonButton.Size = new Size(241, 44);
            selectPokemonButton.TabIndex = 23;
            selectPokemonButton.Text = "Continue";
            selectPokemonButton.UseVisualStyleBackColor = false;
            selectPokemonButton.Click += selectPokemonButton_Click;
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(12, 316);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(84, 37);
            greenLimeThemeComponent1.TabIndex = 29;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // WarningForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(461, 479);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(selectPokemonButton);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "WarningForm";
            Text = "WARNING";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Button selectPokemonButton;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
    }
}