namespace ProQol
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label1 = new Label();
            gameLabel = new Label();
            posXLabel = new Label();
            retryButton = new Button();
            renderTimer = new System.Windows.Forms.Timer(components);
            posYLabel = new Label();
            label3 = new Label();
            currentEncounterIdLabel = new Label();
            label4 = new Label();
            isBattlingLabel = new Label();
            IsSpecialEncounter = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            label1.Location = new Point(12, 57);
            label1.Name = "label1";
            label1.Size = new Size(125, 32);
            label1.TabIndex = 0;
            label1.Text = "Position x:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // gameLabel
            // 
            gameLabel.AutoSize = true;
            gameLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gameLabel.ForeColor = Color.Red;
            gameLabel.Location = new Point(12, 9);
            gameLabel.Name = "gameLabel";
            gameLabel.Size = new Size(193, 32);
            gameLabel.TabIndex = 1;
            gameLabel.Text = "Game not found";
            // 
            // posXLabel
            // 
            posXLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            posXLabel.AutoSize = true;
            posXLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            posXLabel.Location = new Point(204, 57);
            posXLabel.Name = "posXLabel";
            posXLabel.Size = new Size(58, 32);
            posXLabel.TabIndex = 3;
            posXLabel.Text = "N/A";
            // 
            // retryButton
            // 
            retryButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            retryButton.BackColor = Color.LavenderBlush;
            retryButton.FlatStyle = FlatStyle.Flat;
            retryButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            retryButton.ForeColor = Color.Crimson;
            retryButton.Location = new Point(166, 451);
            retryButton.Name = "retryButton";
            retryButton.Size = new Size(96, 83);
            retryButton.TabIndex = 4;
            retryButton.Text = "Retry find";
            retryButton.UseVisualStyleBackColor = false;
            retryButton.Click += button1_Click;
            // 
            // renderTimer
            // 
            renderTimer.Interval = 10;
            renderTimer.Tick += renderTimer_Tick;
            // 
            // posYLabel
            // 
            posYLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            posYLabel.AutoSize = true;
            posYLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            posYLabel.Location = new Point(204, 105);
            posYLabel.Name = "posYLabel";
            posYLabel.Size = new Size(58, 32);
            posYLabel.TabIndex = 6;
            posYLabel.Text = "N/A";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            label3.Location = new Point(12, 105);
            label3.Name = "label3";
            label3.Size = new Size(125, 32);
            label3.TabIndex = 5;
            label3.Text = "Position y:";
            // 
            // currentEncounterIdLabel
            // 
            currentEncounterIdLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            currentEncounterIdLabel.AutoSize = true;
            currentEncounterIdLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            currentEncounterIdLabel.Location = new Point(204, 170);
            currentEncounterIdLabel.Name = "currentEncounterIdLabel";
            currentEncounterIdLabel.Size = new Size(58, 32);
            currentEncounterIdLabel.TabIndex = 8;
            currentEncounterIdLabel.Text = "N/A";
            // 
            // label4
            // 
            label4.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            label4.Location = new Point(12, 154);
            label4.Name = "label4";
            label4.Size = new Size(167, 71);
            label4.TabIndex = 7;
            label4.Text = "Current encounter id:";
            // 
            // isBattlingLabel
            // 
            isBattlingLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            isBattlingLabel.ForeColor = Color.Green;
            isBattlingLabel.Location = new Point(12, 239);
            isBattlingLabel.Name = "isBattlingLabel";
            isBattlingLabel.Size = new Size(167, 39);
            isBattlingLabel.TabIndex = 9;
            isBattlingLabel.Text = "Is battling";
            isBattlingLabel.Visible = false;
            // 
            // IsSpecialEncounter
            // 
            IsSpecialEncounter.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            IsSpecialEncounter.ForeColor = Color.MediumPurple;
            IsSpecialEncounter.Location = new Point(12, 288);
            IsSpecialEncounter.Name = "IsSpecialEncounter";
            IsSpecialEncounter.Size = new Size(214, 39);
            IsSpecialEncounter.TabIndex = 10;
            IsSpecialEncounter.Text = "Special encounter";
            IsSpecialEncounter.Visible = false;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.BackColor = Color.LavenderBlush;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.Crimson;
            button1.Location = new Point(12, 352);
            button1.Name = "button1";
            button1.Size = new Size(96, 83);
            button1.TabIndex = 11;
            button1.Text = "Left";
            button1.UseVisualStyleBackColor = false;
            button1.MouseDown += button1_MouseDown;
            button1.MouseUp += button1_MouseUp;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.BackColor = Color.LavenderBlush;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.Crimson;
            button2.Location = new Point(166, 352);
            button2.Name = "button2";
            button2.Size = new Size(96, 83);
            button2.TabIndex = 12;
            button2.Text = "Right";
            button2.UseVisualStyleBackColor = false;
            button2.MouseDown += button2_MouseDown;
            button2.MouseUp += button2_MouseUp;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button3.BackColor = Color.LavenderBlush;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.Crimson;
            button3.Location = new Point(12, 451);
            button3.Name = "button3";
            button3.Size = new Size(96, 83);
            button3.TabIndex = 13;
            button3.Text = "4";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(274, 546);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(IsSpecialEncounter);
            Controls.Add(isBattlingLabel);
            Controls.Add(label1);
            Controls.Add(posXLabel);
            Controls.Add(currentEncounterIdLabel);
            Controls.Add(label4);
            Controls.Add(posYLabel);
            Controls.Add(label3);
            Controls.Add(retryButton);
            Controls.Add(gameLabel);
            MinimumSize = new Size(290, 476);
            Name = "Form1";
            Text = "ProQol";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label gameLabel;
        private Label posXLabel;
        private Button retryButton;
        private System.Windows.Forms.Timer renderTimer;
        private Label posYLabel;
        private Label label3;
        private Label currentEncounterIdLabel;
        private Label label4;
        private Label isBattlingLabel;
        private Label IsSpecialEncounter;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}
