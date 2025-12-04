namespace Presentation
{
    partial class SupportMeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SupportMeForm));
            pictureBox1 = new PictureBox();
            gameLabel = new Label();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.FromArgb(40, 40, 40);
            pictureBox1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            pictureBox1.ForeColor = Color.FromArgb(152, 151, 26);
            pictureBox1.Image = Properties.Resources.buy_me_a_coffee;
            pictureBox1.Location = new Point(12, 224);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(422, 126);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 26;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
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
            // label1
            // 
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(12, 55);
            label1.Name = "label1";
            label1.Size = new Size(422, 67);
            label1.TabIndex = 28;
            label1.Tag = "FontSize=S";
            label1.Text = "Any support would help me add more updates to the client and it would let me focus more on this project.";
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.FromArgb(40, 40, 40);
            pictureBox2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            pictureBox2.ForeColor = Color.FromArgb(152, 151, 26);
            pictureBox2.Image = Properties.Resources.star_github1;
            pictureBox2.Location = new Point(12, 356);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(422, 77);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 29;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.FromArgb(40, 40, 40);
            pictureBox3.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            pictureBox3.ForeColor = Color.FromArgb(152, 151, 26);
            pictureBox3.Image = Properties.Resources.honeygain;
            pictureBox3.Location = new Point(12, 151);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(293, 67);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 31;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(402, 12);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(18, 40);
            greenLimeThemeComponent1.TabIndex = 30;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(152, 151, 26);
            button1.Location = new Point(311, 151);
            button1.Name = "button1";
            button1.Size = new Size(123, 67);
            button1.TabIndex = 32;
            button1.TabStop = false;
            button1.Text = "Enable Honeygain";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // SupportMeForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(446, 445);
            Controls.Add(button1);
            Controls.Add(pictureBox3);
            Controls.Add(greenLimeThemeComponent1);
            Controls.Add(pictureBox2);
            Controls.Add(label1);
            Controls.Add(gameLabel);
            Controls.Add(pictureBox1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "SupportMeForm";
            Text = "Support me";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label gameLabel;
        private Label label1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
        private Button button1;
    }
}