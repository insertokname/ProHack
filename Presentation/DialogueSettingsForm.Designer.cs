namespace Presentation
{
    partial class DialogueSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogueSettingsForm));
            label2 = new Label();
            trackBar1 = new TrackBar();
            label1 = new Label();
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(152, 151, 26);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(531, 27);
            label2.TabIndex = 4;
            label2.Text = "After setting the dialogue speed go in game, press esc, go to options and hit apply";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackBar1
            // 
            trackBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trackBar1.BackColor = Color.FromArgb(40, 40, 40);
            trackBar1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            trackBar1.LargeChange = 1;
            trackBar1.Location = new Point(12, 79);
            trackBar1.Maximum = 175;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(531, 45);
            trackBar1.TabIndex = 5;
            trackBar1.TickFrequency = 0;
            trackBar1.TickStyle = TickStyle.Both;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(12, 49);
            label1.Name = "label1";
            label1.Size = new Size(531, 27);
            label1.TabIndex = 6;
            label1.Text = "Current Value: 0";
            label1.TextAlign = ContentAlignment.MiddleCenter;
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
            greenLimeThemeComponent1.TabIndex = 31;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // DialogueSettingsForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(555, 136);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(label1);
            Controls.Add(trackBar1);
            Controls.Add(label2);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "DialogueSettingsForm";
            Text = "Dialogue Settings";
            Load += DialogueSettings_Load;
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private TrackBar trackBar1;
        private Label label1;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
    }
}