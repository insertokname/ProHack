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
            label1 = new Label();
            label2 = new Label();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            selectPokemonButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 25F, FontStyle.Bold);
            label1.ForeColor = Color.Crimson;
            label1.Location = new Point(36, 9);
            label1.Name = "label1";
            label1.Size = new Size(338, 92);
            label1.TabIndex = 18;
            label1.Text = "WARNING\r\nUSE RESPONSABILY!";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label2.ForeColor = Color.Crimson;
            label2.Location = new Point(12, 120);
            label2.Name = "label2";
            label2.Size = new Size(392, 145);
            label2.TabIndex = 20;
            label2.Text = "DO NOT go to sleep while the bot is running. \r\nDO NOT leave it on for tens of hours unwatched.\r\n\r\nIf you are banned all your progress will be lost.\r\n\r\nUSE RESPONSABILY!";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            checkBox1.ForeColor = Color.Crimson;
            checkBox1.Location = new Point(208, 297);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(196, 42);
            checkBox1.TabIndex = 21;
            checkBox1.Text = "I have read the message\r\n and i understand the risks.\r\n";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            checkBox2.ForeColor = Color.Crimson;
            checkBox2.Location = new Point(208, 268);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(167, 23);
            checkBox2.TabIndex = 22;
            checkBox2.Text = "Don't show this again.";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // selectPokemonButton
            // 
            selectPokemonButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            selectPokemonButton.BackColor = Color.LavenderBlush;
            selectPokemonButton.FlatStyle = FlatStyle.Flat;
            selectPokemonButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            selectPokemonButton.ForeColor = Color.Crimson;
            selectPokemonButton.Location = new Point(208, 345);
            selectPokemonButton.Name = "selectPokemonButton";
            selectPokemonButton.Size = new Size(196, 44);
            selectPokemonButton.TabIndex = 23;
            selectPokemonButton.Text = "Continue";
            selectPokemonButton.UseVisualStyleBackColor = false;
            selectPokemonButton.Click += selectPokemonButton_Click;
            // 
            // WarningForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(416, 401);
            Controls.Add(selectPokemonButton);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
    }
}