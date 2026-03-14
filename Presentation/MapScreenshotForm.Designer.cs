namespace Presentation
{
    partial class MapScreenshotForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            checkBox1 = new CheckBox();
            themeApplier1 = new Infrastructure.Theme.ThemeApplier();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BackColor = Color.FromArgb(40, 40, 40);
            pictureBox1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            pictureBox1.ForeColor = Color.FromArgb(152, 151, 26);
            pictureBox1.Location = new Point(14, 14);
            pictureBox1.Margin = new Padding(5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1033, 720);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(152, 151, 26);
            button1.Location = new Point(1057, 14);
            button1.Margin = new Padding(5);
            button1.Name = "button1";
            button1.Size = new Size(186, 65);
            button1.TabIndex = 1;
            button1.Text = "Take Screenshot";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_ClickAsync;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.BackColor = Color.FromArgb(40, 40, 40);
            button2.Enabled = false;
            button2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button2.ForeColor = Color.FromArgb(152, 151, 26);
            button2.Location = new Point(1057, 228);
            button2.Margin = new Padding(5);
            button2.Name = "button2";
            button2.Size = new Size(186, 65);
            button2.TabIndex = 3;
            button2.Text = "Save image";
            button2.UseVisualStyleBackColor = false;
            button2.Visible = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button3.BackColor = Color.FromArgb(40, 40, 40);
            button3.Enabled = false;
            button3.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button3.ForeColor = Color.FromArgb(152, 151, 26);
            button3.Location = new Point(1057, 153);
            button3.Margin = new Padding(5);
            button3.Name = "button3";
            button3.Size = new Size(186, 65);
            button3.TabIndex = 4;
            button3.Text = "Clear image";
            button3.UseVisualStyleBackColor = false;
            button3.Visible = false;
            button3.Click += button3_Click;
            // 
            // checkBox1
            // 
            checkBox1.BackColor = Color.FromArgb(40, 40, 40);
            checkBox1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            checkBox1.ForeColor = Color.FromArgb(152, 151, 26);
            checkBox1.Location = new Point(1057, 87);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(186, 58);
            checkBox1.TabIndex = 5;
            checkBox1.Tag = "";
            checkBox1.Text = "High res (SLOWER)";
            checkBox1.TextAlign = ContentAlignment.MiddleCenter;
            checkBox1.UseVisualStyleBackColor = false;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // themeApplier1
            // 
            themeApplier1.BackColor = Color.FromArgb(40, 40, 40);
            themeApplier1.Enabled = false;
            themeApplier1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            themeApplier1.ForeColor = Color.FromArgb(152, 151, 26);
            themeApplier1.Location = new Point(0, 0);
            themeApplier1.Margin = new Padding(5);
            themeApplier1.Name = "themeApplier1";
            themeApplier1.Size = new Size(81, 10);
            themeApplier1.TabIndex = 2;
            themeApplier1.TabStop = false;
            themeApplier1.Text = "themeApplier1";
            themeApplier1.Visible = false;
            // 
            // MapScreenshotForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1257, 748);
            Controls.Add(checkBox1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(themeApplier1);
            Controls.Add(button1);
            Controls.Add(pictureBox1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5);
            Name = "MapScreenshotForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MapScreenshotForm";
            Load += MapScreenshotForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private CheckBox checkBox1;
        private Infrastructure.Theme.ThemeApplier themeApplier1;
    }
}