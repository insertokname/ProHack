namespace Presentation
{
    partial class PathingForm
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelScroll = new Panel();
            picturePath = new PictureBox();
            panelRight = new Panel();
            button1 = new Button();
            btnRecord = new Button();
            btnStop = new Button();
            cbHighRes = new CheckBox();
            btnRefreshView = new Button();
            btnOpen = new Button();
            btnSave = new Button();
            lblStatus = new Label();
            themeApplier1 = new Infrastructure.Theme.ThemeApplier();
            panelScroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picturePath).BeginInit();
            panelRight.SuspendLayout();
            SuspendLayout();
            // 
            // panelScroll
            // 
            panelScroll.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelScroll.AutoScroll = true;
            panelScroll.BackColor = Color.FromArgb(40, 40, 40);
            panelScroll.Controls.Add(picturePath);
            panelScroll.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            panelScroll.ForeColor = Color.FromArgb(152, 151, 26);
            panelScroll.Location = new Point(14, 14);
            panelScroll.Margin = new Padding(5);
            panelScroll.Name = "panelScroll";
            panelScroll.Size = new Size(1025, 720);
            panelScroll.TabIndex = 0;
            // 
            // picturePath
            // 
            picturePath.BackColor = Color.FromArgb(40, 40, 40);
            picturePath.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            picturePath.ForeColor = Color.FromArgb(152, 151, 26);
            picturePath.Location = new Point(0, 0);
            picturePath.Margin = new Padding(0);
            picturePath.Name = "picturePath";
            picturePath.Size = new Size(1025, 720);
            picturePath.SizeMode = PictureBoxSizeMode.AutoSize;
            picturePath.TabIndex = 0;
            picturePath.TabStop = false;
            // 
            // panelRight
            // 
            panelRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panelRight.BackColor = Color.FromArgb(40, 40, 40);
            panelRight.Controls.Add(button1);
            panelRight.Controls.Add(btnRecord);
            panelRight.Controls.Add(btnStop);
            panelRight.Controls.Add(cbHighRes);
            panelRight.Controls.Add(btnRefreshView);
            panelRight.Controls.Add(btnOpen);
            panelRight.Controls.Add(btnSave);
            panelRight.Controls.Add(lblStatus);
            panelRight.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            panelRight.ForeColor = Color.FromArgb(152, 151, 26);
            panelRight.Location = new Point(1049, 14);
            panelRight.Margin = new Padding(5);
            panelRight.Name = "panelRight";
            panelRight.Size = new Size(194, 720);
            panelRight.TabIndex = 1;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(40, 40, 40);
            button1.Enabled = false;
            button1.FlatAppearance.BorderColor = Color.FromArgb(152, 151, 26);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            button1.ForeColor = Color.FromArgb(152, 151, 26);
            button1.Location = new Point(0, 355);
            button1.Margin = new Padding(5);
            button1.Name = "button1";
            button1.Size = new Size(194, 55);
            button1.TabIndex = 7;
            button1.Text = "Save image";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // btnRecord
            // 
            btnRecord.BackColor = Color.FromArgb(40, 40, 40);
            btnRecord.FlatAppearance.BorderColor = Color.FromArgb(152, 151, 26);
            btnRecord.FlatStyle = FlatStyle.Flat;
            btnRecord.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            btnRecord.ForeColor = Color.FromArgb(152, 151, 26);
            btnRecord.Location = new Point(0, 0);
            btnRecord.Margin = new Padding(5);
            btnRecord.Name = "btnRecord";
            btnRecord.Size = new Size(194, 55);
            btnRecord.TabIndex = 0;
            btnRecord.Text = "▶ Record";
            btnRecord.UseVisualStyleBackColor = false;
            btnRecord.Click += btnRecord_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.FromArgb(40, 40, 40);
            btnStop.Enabled = false;
            btnStop.FlatAppearance.BorderColor = Color.FromArgb(152, 151, 26);
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            btnStop.ForeColor = Color.FromArgb(152, 151, 26);
            btnStop.Location = new Point(0, 60);
            btnStop.Margin = new Padding(5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(194, 55);
            btnStop.TabIndex = 1;
            btnStop.Text = "■ Stop";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // cbHighRes
            // 
            cbHighRes.BackColor = Color.FromArgb(40, 40, 40);
            cbHighRes.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            cbHighRes.ForeColor = Color.FromArgb(152, 151, 26);
            cbHighRes.Location = new Point(4, 127);
            cbHighRes.Margin = new Padding(5);
            cbHighRes.Name = "cbHighRes";
            cbHighRes.Size = new Size(186, 30);
            cbHighRes.TabIndex = 2;
            cbHighRes.Text = "High-res (slow)";
            cbHighRes.UseVisualStyleBackColor = false;
            // 
            // btnRefreshView
            // 
            btnRefreshView.BackColor = Color.FromArgb(40, 40, 40);
            btnRefreshView.FlatAppearance.BorderColor = Color.FromArgb(152, 151, 26);
            btnRefreshView.FlatStyle = FlatStyle.Flat;
            btnRefreshView.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            btnRefreshView.ForeColor = Color.FromArgb(152, 151, 26);
            btnRefreshView.Location = new Point(0, 162);
            btnRefreshView.Margin = new Padding(5);
            btnRefreshView.Name = "btnRefreshView";
            btnRefreshView.Size = new Size(194, 55);
            btnRefreshView.TabIndex = 3;
            btnRefreshView.Text = "Refresh View";
            btnRefreshView.UseVisualStyleBackColor = false;
            btnRefreshView.Click += btnRefreshView_Click;
            // 
            // btnOpen
            // 
            btnOpen.BackColor = Color.FromArgb(40, 40, 40);
            btnOpen.FlatAppearance.BorderColor = Color.FromArgb(152, 151, 26);
            btnOpen.FlatStyle = FlatStyle.Flat;
            btnOpen.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            btnOpen.ForeColor = Color.FromArgb(152, 151, 26);
            btnOpen.Location = new Point(0, 230);
            btnOpen.Margin = new Padding(5);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(194, 55);
            btnOpen.TabIndex = 4;
            btnOpen.Text = "Open Path…";
            btnOpen.UseVisualStyleBackColor = false;
            btnOpen.Click += btnOpen_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(40, 40, 40);
            btnSave.Enabled = false;
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(152, 151, 26);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            btnSave.ForeColor = Color.FromArgb(152, 151, 26);
            btnSave.Location = new Point(0, 290);
            btnSave.Margin = new Padding(5);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(194, 55);
            btnSave.TabIndex = 5;
            btnSave.Text = "Save Path…";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.FromArgb(40, 40, 40);
            lblStatus.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(152, 151, 26);
            lblStatus.Location = new Point(4, 420);
            lblStatus.Margin = new Padding(5);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(186, 292);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Ready.";
            // 
            // themeApplier1
            // 
            themeApplier1.BackColor = Color.FromArgb(40, 40, 40);
            themeApplier1.Enabled = false;
            themeApplier1.Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            themeApplier1.ForeColor = Color.FromArgb(152, 151, 26);
            themeApplier1.Location = new Point(0, 0);
            themeApplier1.Margin = new Padding(5);
            themeApplier1.Name = "themeApplier1";
            themeApplier1.Size = new Size(81, 10);
            themeApplier1.TabIndex = 7;
            themeApplier1.TabStop = false;
            themeApplier1.Text = "themeApplier1";
            themeApplier1.Visible = false;
            // 
            // PathingForm
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(1257, 748);
            Controls.Add(panelScroll);
            Controls.Add(panelRight);
            Controls.Add(themeApplier1);
            Font = new Font("Cascadia Code", 14F, FontStyle.Bold);
            ForeColor = Color.FromArgb(152, 151, 26);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5);
            Name = "PathingForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Path Recorder";
            Load += PathingForm_Load;
            panelScroll.ResumeLayout(false);
            panelScroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picturePath).EndInit();
            panelRight.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel      panelScroll;
        private PictureBox picturePath;
        private Panel      panelRight;
        private Button     btnRecord;
        private Button     btnStop;
        private CheckBox   cbHighRes;
        private Button     btnRefreshView;
        private Button     btnOpen;
        private Button     btnSave;
        private Label      lblStatus;
        private Infrastructure.Theme.ThemeApplier themeApplier1;
        private Button button1;
    }
}
