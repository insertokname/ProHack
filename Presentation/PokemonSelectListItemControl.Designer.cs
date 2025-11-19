namespace Presentation
{
    partial class PokemonSelectListItemControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            themeApplier1 = new Infrastructure.Theme.ThemeApplier();
            label3 = new Label();
            label2 = new Label();
            button1 = new Button();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(40, 40, 40);
            panel1.Controls.Add(themeApplier1);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            panel1.ForeColor = Color.FromArgb(152, 151, 26);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(110, 110);
            panel1.TabIndex = 1;
            panel1.Tag = "BackColor=Secondary1;";
            panel1.Paint += panel1_Paint;
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
            themeApplier1.TabIndex = 4;
            themeApplier1.TabStop = false;
            themeApplier1.Text = "themeApplier1";
            themeApplier1.Visible = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.FromArgb(40, 40, 40);
            label3.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(152, 151, 26);
            label3.Location = new Point(0, 92);
            label3.Name = "label3";
            label3.Size = new Size(112, 18);
            label3.TabIndex = 3;
            label3.Tag = "FontSize=S;BackColor=Secondary1;";
            label3.Text = "Must be shiny";
            label3.Visible = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(152, 151, 26);
            label2.Location = new Point(0, 68);
            label2.Name = "label2";
            label2.Size = new Size(112, 18);
            label2.TabIndex = 2;
            label2.Tag = "FontSize=S;BackColor=Secondary1;";
            label2.Text = "Must be event";
            label2.Visible = false;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(204, 36, 29);
            button1.Location = new Point(82, 3);
            button1.Name = "button1";
            button1.Size = new Size(23, 30);
            button1.TabIndex = 1;
            button1.TabStop = false;
            button1.Tag = "FontSize=S;ForeColor=Danger;";
            button1.Text = "X";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(0, 40);
            label1.Name = "label1";
            label1.Size = new Size(32, 18);
            label1.TabIndex = 0;
            label1.Tag = "FontSize=S;BackColor=Secondary1;";
            label1.Text = "Id:";
            // 
            // PokemonSelectListItemControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel1);
            Name = "PokemonSelectListItemControl";
            Size = new Size(110, 110);
            Load += PokemonSelectListItemControl_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label3;
        private Label label2;
        public Button button1;
        private Label label1;
        private Infrastructure.Theme.ThemeApplier themeApplier1;
    }
}
