namespace Presentation
{
    partial class SupportMeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SupportMeForm));
            pictureBox1 = new PictureBox();
            gameLabel = new Label();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.buy_me_a_coffee;
            pictureBox1.Location = new Point(12, 109);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(422, 126);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 26;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // gameLabel
            // 
            gameLabel.AutoSize = true;
            gameLabel.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gameLabel.ForeColor = Color.Red;
            gameLabel.Location = new Point(12, 9);
            gameLabel.Name = "gameLabel";
            gameLabel.Size = new Size(326, 32);
            gameLabel.TabIndex = 27;
            gameLabel.Text = "Thank you for using my app!";
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(12, 55);
            label1.Name = "label1";
            label1.Size = new Size(422, 51);
            label1.TabIndex = 28;
            label1.Text = "Any support would help me add more updates to the client and it would let me focus more on this project.";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.star_github;
            pictureBox2.Location = new Point(12, 241);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(422, 77);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 29;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // SupportMeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(446, 334);
            Controls.Add(pictureBox2);
            Controls.Add(label1);
            Controls.Add(gameLabel);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SupportMeForm";
            Text = "Support Me";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label gameLabel;
        private Label label1;
        private PictureBox pictureBox2;
    }
}