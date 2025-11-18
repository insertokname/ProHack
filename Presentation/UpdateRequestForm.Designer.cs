namespace Presentation
{
    partial class UpdateRequestForm
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
            label2 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            skipUpdateButton = new Button();
            updateButton = new Button();
            button1 = new Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 25F, FontStyle.Bold);
            label1.ForeColor = Color.Crimson;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(359, 46);
            label1.TabIndex = 17;
            label1.Text = "New update available:";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.LavenderBlush;
            textBox1.Font = new Font("Segoe UI", 14F);
            textBox1.ForeColor = Color.Crimson;
            textBox1.Location = new Point(12, 83);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(601, 325);
            textBox1.TabIndex = 18;
            textBox1.TabStop = false;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
            label2.ForeColor = Color.Crimson;
            label2.Location = new Point(13, 55);
            label2.Name = "label2";
            label2.Size = new Size(112, 25);
            label2.TabIndex = 19;
            label2.Text = "Patch notes:";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Controls.Add(button1, 0, 0);
            tableLayoutPanel1.Controls.Add(updateButton, 2, 0);
            tableLayoutPanel1.Controls.Add(skipUpdateButton, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 414);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(601, 50);
            tableLayoutPanel1.TabIndex = 20;
            // 
            // skipUpdateButton
            // 
            skipUpdateButton.BackColor = Color.LavenderBlush;
            skipUpdateButton.Dock = DockStyle.Fill;
            skipUpdateButton.FlatStyle = FlatStyle.Flat;
            skipUpdateButton.Font = new Font("Segoe UI Semibold", 12.25F, FontStyle.Bold);
            skipUpdateButton.ForeColor = Color.Crimson;
            skipUpdateButton.Location = new Point(203, 3);
            skipUpdateButton.Name = "skipUpdateButton";
            skipUpdateButton.Size = new Size(194, 44);
            skipUpdateButton.TabIndex = 16;
            skipUpdateButton.Text = "Skip this update";
            skipUpdateButton.UseVisualStyleBackColor = false;
            skipUpdateButton.Click += skipUpdateButton_Click;
            // 
            // updateButton
            // 
            updateButton.BackColor = Color.LavenderBlush;
            updateButton.Dock = DockStyle.Fill;
            updateButton.FlatStyle = FlatStyle.Flat;
            updateButton.Font = new Font("Segoe UI Semibold", 12.25F, FontStyle.Bold);
            updateButton.ForeColor = Color.Crimson;
            updateButton.Location = new Point(403, 3);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(195, 44);
            updateButton.TabIndex = 12;
            updateButton.Text = "Update";
            updateButton.UseVisualStyleBackColor = false;
            updateButton.Click += updateButton_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.LavenderBlush;
            button1.Dock = DockStyle.Fill;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI Semibold", 12.25F, FontStyle.Bold);
            button1.ForeColor = Color.Crimson;
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(194, 44);
            button1.TabIndex = 17;
            button1.Text = "Update settings";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // UpdateRequestForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(625, 476);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "UpdateRequestForm";
            Text = "Update Request";
            Load += UpdateRequestForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel1;
        private Button skipUpdateButton;
        private Button updateButton;
        private Button button1;
    }
}