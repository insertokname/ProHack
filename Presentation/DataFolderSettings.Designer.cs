namespace Presentation
{
    partial class DataFolderSettings
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
            label1 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            themeApplier1 = new Infrastructure.Theme.ThemeApplier();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(9, 9);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(111, 25);
            label1.TabIndex = 29;
            label1.Tag = "FontSize=M";
            label1.Text = "DATA url:";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(40, 40, 40);
            textBox1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            textBox1.ForeColor = Color.FromArgb(152, 151, 26);
            textBox1.Location = new Point(14, 39);
            textBox1.Margin = new Padding(5);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(655, 23);
            textBox1.TabIndex = 28;
            textBox1.TabStop = false;
            textBox1.Tag = "FontSize=S";
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(152, 151, 26);
            button1.Location = new Point(677, 39);
            button1.Name = "button1";
            button1.Size = new Size(54, 23);
            button1.TabIndex = 31;
            button1.Tag = "FontSize=XS";
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // themeApplier1
            // 
            themeApplier1.BackColor = Color.FromArgb(40, 40, 40);
            themeApplier1.Enabled = false;
            themeApplier1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            themeApplier1.ForeColor = Color.FromArgb(152, 151, 26);
            themeApplier1.Location = new Point(0, -1);
            themeApplier1.Margin = new Padding(5);
            themeApplier1.Name = "themeApplier1";
            themeApplier1.Size = new Size(23, 11);
            themeApplier1.TabIndex = 30;
            themeApplier1.TabStop = false;
            themeApplier1.Text = "themeApplier1";
            themeApplier1.Visible = false;
            // 
            // DataFolderSettings
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(743, 72);
            Controls.Add(button1);
            Controls.Add(themeApplier1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5);
            Name = "DataFolderSettings";
            Text = "DataFolderSettings";
            Load += DataFolderSettings_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private Infrastructure.Theme.ThemeApplier themeApplier1;
    }
}