namespace Presentation
{
    partial class PokemonSelect
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
            idNumeric = new NumericUpDown();
            idLabel = new Label();
            mustBeSpecial = new CheckBox();
            label2 = new Label();
            retryButton = new Button();
            pictureBox1 = new PictureBox();
            catchAnythingCheckbox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)idNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // idNumeric
            // 
            idNumeric.BackColor = Color.LavenderBlush;
            idNumeric.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            idNumeric.ForeColor = Color.Crimson;
            idNumeric.Location = new Point(12, 74);
            idNumeric.Name = "idNumeric";
            idNumeric.Size = new Size(113, 33);
            idNumeric.TabIndex = 0;
            idNumeric.Visible = false;
            // 
            // idLabel
            // 
            idLabel.AutoSize = true;
            idLabel.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            idLabel.Location = new Point(12, 46);
            idLabel.Name = "idLabel";
            idLabel.Size = new Size(34, 25);
            idLabel.TabIndex = 1;
            idLabel.Text = "Id:";
            idLabel.Visible = false;
            // 
            // mustBeSpecial
            // 
            mustBeSpecial.AutoSize = true;
            mustBeSpecial.Font = new Font("Segoe UI Semibold", 10.25F, FontStyle.Bold);
            mustBeSpecial.Location = new Point(179, 80);
            mustBeSpecial.Name = "mustBeSpecial";
            mustBeSpecial.Size = new Size(126, 23);
            mustBeSpecial.TabIndex = 2;
            mustBeSpecial.Text = "Must be special";
            mustBeSpecial.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI Semibold", 9.25F, FontStyle.Bold);
            label2.Location = new Point(165, 31);
            label2.Name = "label2";
            label2.Size = new Size(140, 41);
            label2.TabIndex = 3;
            label2.Text = "Special pokemon are shiny or event forms";
            // 
            // retryButton
            // 
            retryButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            retryButton.BackColor = Color.LavenderBlush;
            retryButton.FlatStyle = FlatStyle.Flat;
            retryButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            retryButton.ForeColor = Color.Crimson;
            retryButton.Location = new Point(12, 393);
            retryButton.Name = "retryButton";
            retryButton.Size = new Size(303, 77);
            retryButton.TabIndex = 5;
            retryButton.Text = "Select";
            retryButton.UseVisualStyleBackColor = false;
            retryButton.Click += retryButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 113);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(303, 274);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // catchAnythingCheckbox
            // 
            catchAnythingCheckbox.AutoSize = true;
            catchAnythingCheckbox.Checked = true;
            catchAnythingCheckbox.CheckState = CheckState.Checked;
            catchAnythingCheckbox.Font = new Font("Segoe UI Semibold", 10.25F, FontStyle.Bold);
            catchAnythingCheckbox.Location = new Point(12, 20);
            catchAnythingCheckbox.Name = "catchAnythingCheckbox";
            catchAnythingCheckbox.Size = new Size(123, 23);
            catchAnythingCheckbox.TabIndex = 7;
            catchAnythingCheckbox.Text = "Catch anything";
            catchAnythingCheckbox.UseVisualStyleBackColor = true;
            catchAnythingCheckbox.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // PokemonSelect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(327, 482);
            Controls.Add(catchAnythingCheckbox);
            Controls.Add(pictureBox1);
            Controls.Add(retryButton);
            Controls.Add(label2);
            Controls.Add(mustBeSpecial);
            Controls.Add(idLabel);
            Controls.Add(idNumeric);
            Name = "PokemonSelect";
            Text = "PokemonSelect";
            Load += PokemonSelect_Load;
            ((System.ComponentModel.ISupportInitialize)idNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown idNumeric;
        private Label idLabel;
        private CheckBox mustBeSpecial;
        private Label label2;
        private Button retryButton;
        private PictureBox pictureBox1;
        private CheckBox catchAnythingCheckbox;
    }
}