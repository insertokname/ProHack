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
            label2 = new Label();
            retryButton = new Button();
            pictureBox1 = new PictureBox();
            catchAnythingCheckbox = new CheckBox();
            isSpecialComboBox = new ComboBox();
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
            idNumeric.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            idNumeric.Name = "idNumeric";
            idNumeric.Size = new Size(113, 33);
            idNumeric.TabIndex = 0;
            idNumeric.Visible = false;
            idNumeric.ValueChanged += idNumeric_ValueChanged;
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
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.Font = new Font("Segoe UI Semibold", 9.25F, FontStyle.Bold);
            label2.Location = new Point(440, 29);
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
            retryButton.Size = new Size(568, 77);
            retryButton.TabIndex = 5;
            retryButton.Text = "Select";
            retryButton.UseVisualStyleBackColor = false;
            retryButton.Click += retryButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(12, 113);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(568, 274);
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
            // isSpecialComboBox
            // 
            isSpecialComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            isSpecialComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            isSpecialComboBox.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            isSpecialComboBox.FormattingEnabled = true;
            isSpecialComboBox.Location = new Point(131, 74);
            isSpecialComboBox.Name = "isSpecialComboBox";
            isSpecialComboBox.Size = new Size(449, 33);
            isSpecialComboBox.TabIndex = 8;
            isSpecialComboBox.SelectedIndexChanged += isSpecialComboBox_SelectedIndexChanged;
            // 
            // PokemonSelect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(592, 482);
            Controls.Add(isSpecialComboBox);
            Controls.Add(catchAnythingCheckbox);
            Controls.Add(pictureBox1);
            Controls.Add(retryButton);
            Controls.Add(label2);
            Controls.Add(idLabel);
            Controls.Add(idNumeric);
            MinimumSize = new Size(608, 521);
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
        private Label label2;
        private Button retryButton;
        private PictureBox pictureBox1;
        private CheckBox catchAnythingCheckbox;
        private ComboBox isSpecialComboBox;
    }
}