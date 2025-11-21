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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiscordBotOptionsForm));
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
            greenLimeThemeComponent1 = new Infrastructure.Theme.ThemeApplier();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = Color.FromArgb(40, 40, 40);
            textBox1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            textBox1.ForeColor = Color.FromArgb(152, 151, 26);
            textBox1.Location = new Point(12, 37);
            textBox1.Name = "textBox1";
            textBox1.PasswordChar = '*';
            textBox1.Size = new Size(588, 23);
            textBox1.TabIndex = 0;
            textBox1.Tag = "FontSize=S";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(152, 151, 26);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(78, 25);
            label1.TabIndex = 1;
            label1.Text = "Token:";
            // 
            // checkBox1
            // 
            checkBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.BackColor = Color.FromArgb(40, 40, 40);
            checkBox1.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            checkBox1.ForeColor = Color.FromArgb(152, 151, 26);
            checkBox1.Location = new Point(607, 38);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(67, 22);
            checkBox1.TabIndex = 2;
            checkBox1.Tag = "FontSize=S";
            checkBox1.Text = "Show ";
            checkBox1.UseVisualStyleBackColor = false;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(152, 151, 26);
            label2.Location = new Point(471, 167);
            label2.Name = "label2";
            label2.Size = new Size(262, 24);
            label2.TabIndex = 3;
            label2.Text = "Discord bot not started";
            // 
            // selectPokemonButton
            // 
            selectPokemonButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            selectPokemonButton.BackColor = Color.FromArgb(40, 40, 40);
            selectPokemonButton.FlatStyle = FlatStyle.Flat;
            selectPokemonButton.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            selectPokemonButton.ForeColor = Color.FromArgb(152, 151, 26);
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
            errorTextBox.BackColor = Color.FromArgb(40, 40, 40);
            errorTextBox.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            errorTextBox.ForeColor = Color.FromArgb(152, 151, 26);
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
            label3.BackColor = Color.FromArgb(40, 40, 40);
            label3.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(152, 151, 26);
            label3.Location = new Point(12, 139);
            label3.Name = "label3";
            label3.Size = new Size(89, 25);
            label3.TabIndex = 24;
            label3.Text = "Errors:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(40, 40, 40);
            label4.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(152, 151, 26);
            label4.Location = new Point(12, 65);
            label4.Name = "label4";
            label4.Size = new Size(221, 25);
            label4.TabIndex = 26;
            label4.Text = "Authorized user id:";
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.BackColor = Color.FromArgb(40, 40, 40);
            textBox2.Font = new Font("Cascadia Code", 10F, FontStyle.Bold);
            textBox2.ForeColor = Color.FromArgb(152, 151, 26);
            textBox2.Location = new Point(12, 93);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(661, 23);
            textBox2.TabIndex = 25;
            textBox2.Tag = "FontSize=S";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(152, 151, 26);
            button1.Location = new Point(679, 93);
            button1.Name = "button1";
            button1.Size = new Size(54, 25);
            button1.TabIndex = 27;
            button1.Tag = "FontSize=XS";
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.BackColor = Color.FromArgb(40, 40, 40);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Cascadia Code", 8F, FontStyle.Bold);
            button2.ForeColor = Color.FromArgb(152, 151, 26);
            button2.Location = new Point(679, 37);
            button2.Name = "button2";
            button2.Size = new Size(54, 25);
            button2.TabIndex = 28;
            button2.Tag = "FontSize=XS";
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button3.BackColor = Color.FromArgb(40, 40, 40);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button3.ForeColor = Color.FromArgb(152, 151, 26);
            button3.Location = new Point(471, 269);
            button3.Name = "button3";
            button3.Size = new Size(262, 53);
            button3.TabIndex = 29;
            button3.Text = "Send test message";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // greenLimeThemeComponent1
            // 
            greenLimeThemeComponent1.BackColor = Color.FromArgb(40, 40, 40);
            greenLimeThemeComponent1.Enabled = false;
            greenLimeThemeComponent1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            greenLimeThemeComponent1.ForeColor = Color.FromArgb(152, 151, 26);
            greenLimeThemeComponent1.Location = new Point(-41, -13);
            greenLimeThemeComponent1.Name = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Size = new Size(142, 29);
            greenLimeThemeComponent1.TabIndex = 31;
            greenLimeThemeComponent1.TabStop = false;
            greenLimeThemeComponent1.Text = "greenLimeThemeComponent1";
            greenLimeThemeComponent1.Visible = false;
            // 
            // DiscordBotOptionsForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(745, 334);
            Controls.Add(greenLimeThemeComponent1);
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
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DiscordBotOptionsForm";
            Text = "Discord Bot Settings";
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
        private Infrastructure.Theme.ThemeApplier greenLimeThemeComponent1;
    }
}