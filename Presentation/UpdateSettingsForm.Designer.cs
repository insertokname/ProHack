namespace Presentation
{
    partial class UpdateSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateSettingsForm));
            autoCheckUpdatesCheckbox = new CheckBox();
            autoUpdateCheckBox = new CheckBox();
            button1 = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // autoCheckUpdatesCheckbox
            // 
            autoCheckUpdatesCheckbox.AutoSize = true;
            autoCheckUpdatesCheckbox.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            autoCheckUpdatesCheckbox.ForeColor = Color.Crimson;
            autoCheckUpdatesCheckbox.Location = new Point(12, 12);
            autoCheckUpdatesCheckbox.Name = "autoCheckUpdatesCheckbox";
            autoCheckUpdatesCheckbox.Size = new Size(229, 23);
            autoCheckUpdatesCheckbox.TabIndex = 1;
            autoCheckUpdatesCheckbox.Text = "Check for updates when opened";
            autoCheckUpdatesCheckbox.UseVisualStyleBackColor = true;
            autoCheckUpdatesCheckbox.CheckedChanged += autoCheckUpdatesCheckbox_CheckedChanged;
            // 
            // autoUpdateCheckBox
            // 
            autoUpdateCheckBox.AutoSize = true;
            autoUpdateCheckBox.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            autoUpdateCheckBox.ForeColor = Color.Crimson;
            autoUpdateCheckBox.Location = new Point(12, 41);
            autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            autoUpdateCheckBox.Size = new Size(259, 23);
            autoUpdateCheckBox.TabIndex = 2;
            autoUpdateCheckBox.Text = "Automatically update when available";
            autoUpdateCheckBox.UseVisualStyleBackColor = true;
            autoUpdateCheckBox.CheckedChanged += autoUpdateCheckBox_CheckedChanged;
            // 
            // button1
            // 
            button1.BackColor = Color.LavenderBlush;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI Semibold", 12.25F, FontStyle.Bold);
            button1.ForeColor = Color.Crimson;
            button1.Location = new Point(10, 95);
            button1.Name = "button1";
            button1.Size = new Size(261, 85);
            button1.TabIndex = 4;
            button1.Text = "Check for updates";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.LavenderBlush;
            textBox1.ForeColor = Color.Crimson;
            textBox1.Location = new Point(144, 66);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(127, 23);
            textBox1.TabIndex = 3;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            label1.ForeColor = Color.Crimson;
            label1.Location = new Point(10, 67);
            label1.Name = "label1";
            label1.Size = new Size(88, 19);
            label1.TabIndex = 27;
            label1.Text = "Skip version:";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(8, 190);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(264, 32);
            progressBar1.TabIndex = 28;
            // 
            // UpdateSettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(278, 190);
            Controls.Add(progressBar1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(autoUpdateCheckBox);
            Controls.Add(autoCheckUpdatesCheckbox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "UpdateSettingsForm";
            Text = "Update Settings";
            Load += UpdateSettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox autoCheckUpdatesCheckbox;
        private CheckBox autoUpdateCheckBox;
        private Button button1;
        private TextBox textBox1;
        private Label label1;
        private ProgressBar progressBar1;
    }
}