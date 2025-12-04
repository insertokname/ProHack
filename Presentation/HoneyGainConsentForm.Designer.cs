namespace Presentation
{
    partial class HoneyGainConsentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HoneyGainConsentForm));
            label1 = new Label();
            label2 = new Label();
            selectPokemonButton = new Button();
            button1 = new Button();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
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
            label1.Location = new Point(23, 13);
            label1.Name = "label1";
            label1.Size = new Size(420, 32);
            label1.TabIndex = 18;
            label1.Tag = "FontSize=L";
            label1.Text = "Support me by using honeygain";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(152, 151, 26);
            label2.Location = new Point(12, 72);
            label2.Name = "label2";
            label2.Size = new Size(437, 211);
            label2.TabIndex = 20;
            label2.Tag = "FontSize=M";
            label2.Text = resources.GetString("label2.Text");
            // 
            // selectPokemonButton
            // 
            selectPokemonButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            selectPokemonButton.BackColor = Color.FromArgb(40, 40, 40);
            selectPokemonButton.FlatStyle = FlatStyle.Flat;
            selectPokemonButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            selectPokemonButton.ForeColor = Color.FromArgb(152, 151, 26);
            selectPokemonButton.Location = new Point(229, 370);
            selectPokemonButton.Name = "selectPokemonButton";
            selectPokemonButton.Size = new Size(220, 44);
            selectPokemonButton.TabIndex = 23;
            selectPokemonButton.Text = "I accept";
            selectPokemonButton.UseVisualStyleBackColor = false;
            selectPokemonButton.Click += selectPokemonButton_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(204, 36, 29);
            button1.Location = new Point(12, 370);
            button1.Name = "button1";
            button1.Size = new Size(204, 44);
            button1.TabIndex = 30;
            button1.Tag = "ForeColor=Danger";
            button1.Text = "Decline";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.BackColor = Color.FromArgb(40, 40, 40);
            label3.Font = new Font("Cascadia Code", 9.75F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.RoyalBlue;
            label3.Location = new Point(221, 426);
            label3.Name = "label3";
            label3.Size = new Size(121, 25);
            label3.TabIndex = 31;
            label3.Tag = "FontSize=S;ForeColor=Override;Font=Override";
            label3.Text = "Privacy policy";
            label3.Click += label3_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top;
            label4.BackColor = Color.FromArgb(40, 40, 40);
            label4.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(152, 151, 26);
            label4.Location = new Point(12, 426);
            label4.Name = "label4";
            label4.Size = new Size(212, 25);
            label4.TabIndex = 32;
            label4.Tag = "FontSize=S;";
            label4.Text = "Info about Honeygain SDK:";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top;
            label5.BackColor = Color.FromArgb(40, 40, 40);
            label5.Font = new Font("Cascadia Code", 9.75F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.RoyalBlue;
            label5.Location = new Point(343, 426);
            label5.Name = "label5";
            label5.Size = new Size(106, 25);
            label5.TabIndex = 33;
            label5.Tag = "FontSize=S;ForeColor=Override;Font=Override";
            label5.Text = "Terms of Use";
            label5.Click += label5_Click;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top;
            label6.BackColor = Color.FromArgb(40, 40, 40);
            label6.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label6.ForeColor = Color.FromArgb(152, 151, 26);
            label6.Location = new Point(12, 298);
            label6.Name = "label6";
            label6.Size = new Size(193, 25);
            label6.TabIndex = 34;
            label6.Tag = "FontSize=S;";
            label6.Text = "Honeygain is curently:";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Top;
            label7.BackColor = Color.FromArgb(40, 40, 40);
            label7.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label7.ForeColor = Color.FromArgb(152, 151, 26);
            label7.Location = new Point(193, 298);
            label7.Name = "label7";
            label7.Size = new Size(170, 25);
            label7.TabIndex = 35;
            label7.Tag = "FontSize=S;";
            label7.Text = "disabled";
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(12, 326);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(84, 37);
            greenLimeThemeComponent1.TabIndex = 29;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // HoneyGainConsentForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(461, 458);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label3);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(button1);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(selectPokemonButton);
            Controls.Add(label2);
            Controls.Add(label1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "HoneyGainConsentForm";
            Text = "Honeygain accept";
            Load += HoneyGainConsentForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Button selectPokemonButton;
        private Button button1;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
    }
}