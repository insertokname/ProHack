namespace Presentation
{
    partial class DiscordBotOptionsForm
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
            components = new System.ComponentModel.Container();
            textBox1 = new TextBox();
            label1 = new Label();
            checkBox1 = new CheckBox();
            label2 = new Label();
            selectPokemonButton = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            errorTextBox = new TextBox();
            label3 = new Label();
            label4 = new Label();
            textBox2 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = Color.Lavender;
            textBox1.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            textBox1.ForeColor = Color.MediumPurple;
            textBox1.Location = new Point(12, 37);
            textBox1.Name = "textBox1";
            textBox1.PasswordChar = '*';
            textBox1.Size = new Size(588, 25);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            label1.ForeColor = Color.MediumPurple;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(67, 25);
            label1.TabIndex = 1;
            label1.Text = "Token:";
            // 
            // checkBox1
            // 
            checkBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.BackColor = Color.GhostWhite;
            checkBox1.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            checkBox1.ForeColor = Color.MediumPurple;
            checkBox1.Location = new Point(607, 38);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(67, 23);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Show ";
            checkBox1.UseVisualStyleBackColor = false;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label2.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(471, 167);
            label2.Name = "label2";
            label2.Size = new Size(262, 24);
            label2.TabIndex = 3;
            label2.Text = "Discord bot not started";
            // 
            // selectPokemonButton
            // 
            selectPokemonButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            selectPokemonButton.BackColor = Color.Lavender;
            selectPokemonButton.FlatStyle = FlatStyle.Flat;
            selectPokemonButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            selectPokemonButton.ForeColor = Color.MediumPurple;
            selectPokemonButton.Location = new Point(471, 206);
            selectPokemonButton.Name = "selectPokemonButton";
            selectPokemonButton.Size = new Size(262, 53);
            selectPokemonButton.TabIndex = 22;
            selectPokemonButton.Text = "Restart bot";
            selectPokemonButton.UseVisualStyleBackColor = false;
            selectPokemonButton.Click += selectPokemonButton_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // errorTextBox
            // 
            errorTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            errorTextBox.BackColor = Color.Lavender;
            errorTextBox.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            errorTextBox.ForeColor = Color.Crimson;
            errorTextBox.Location = new Point(12, 167);
            errorTextBox.Multiline = true;
            errorTextBox.Name = "errorTextBox";
            errorTextBox.ReadOnly = true;
            errorTextBox.ScrollBars = ScrollBars.Vertical;
            errorTextBox.Size = new Size(455, 155);
            errorTextBox.TabIndex = 23;
            errorTextBox.Text = "Nothing yet";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            label3.ForeColor = Color.MediumPurple;
            label3.Location = new Point(12, 139);
            label3.Name = "label3";
            label3.Size = new Size(67, 25);
            label3.TabIndex = 24;
            label3.Text = "Errors:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            label4.ForeColor = Color.MediumPurple;
            label4.Location = new Point(12, 65);
            label4.Name = "label4";
            label4.Size = new Size(174, 25);
            label4.TabIndex = 26;
            label4.Text = "Authorized user id:";
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.BackColor = Color.Lavender;
            textBox2.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            textBox2.ForeColor = Color.MediumPurple;
            textBox2.Location = new Point(12, 93);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(661, 25);
            textBox2.TabIndex = 25;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = Color.Lavender;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.MediumPurple;
            button1.Location = new Point(679, 93);
            button1.Name = "button1";
            button1.Size = new Size(54, 25);
            button1.TabIndex = 27;
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.BackColor = Color.Lavender;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.ForeColor = Color.MediumPurple;
            button2.Location = new Point(679, 37);
            button2.Name = "button2";
            button2.Size = new Size(54, 25);
            button2.TabIndex = 28;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button3.BackColor = Color.Lavender;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.ForeColor = Color.MediumPurple;
            button3.Location = new Point(471, 269);
            button3.Name = "button3";
            button3.Size = new Size(262, 53);
            button3.TabIndex = 29;
            button3.Text = "Send test message";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // DiscordBotOptions
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.GhostWhite;
            ClientSize = new Size(745, 334);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label4);
            Controls.Add(textBox2);
            Controls.Add(label3);
            Controls.Add(errorTextBox);
            Controls.Add(selectPokemonButton);
            Controls.Add(label2);
            Controls.Add(checkBox1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Name = "DiscordBotOptions";
            Text = "DiscordBotOptions";
            Load += DiscordBotOptions_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Label label1;
        private CheckBox checkBox1;
        private Label label2;
        private Button selectPokemonButton;
        private System.Windows.Forms.Timer timer1;
        private TextBox errorTextBox;
        private Label label3;
        private Label label4;
        private TextBox textBox2;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}