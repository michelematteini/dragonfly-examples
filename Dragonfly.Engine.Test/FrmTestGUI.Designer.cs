namespace Dragonfly.Engine.Test
{
    partial class FrmTestGUI
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTestGUI));
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.chkVSync = new System.Windows.Forms.CheckBox();
            this.chkSound = new System.Windows.Forms.CheckBox();
            this.chkKeepTesting = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbResolution = new System.Windows.Forms.ComboBox();
            this.chkFullScreen = new System.Windows.Forms.CheckBox();
            this.btnRunAllTests = new System.Windows.Forms.Button();
            this.chkCaptureErrors = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbGraphicAPI = new System.Windows.Forms.ComboBox();
            this.pnlTests = new System.Windows.Forms.Panel();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.lstLogContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlSettings.SuspendLayout();
            this.lstLogContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSettings.BackColor = System.Drawing.Color.White;
            this.pnlSettings.Controls.Add(this.chkVSync);
            this.pnlSettings.Controls.Add(this.chkSound);
            this.pnlSettings.Controls.Add(this.chkKeepTesting);
            this.pnlSettings.Controls.Add(this.label3);
            this.pnlSettings.Controls.Add(this.label2);
            this.pnlSettings.Controls.Add(this.cbResolution);
            this.pnlSettings.Controls.Add(this.chkFullScreen);
            this.pnlSettings.Controls.Add(this.btnRunAllTests);
            this.pnlSettings.Controls.Add(this.chkCaptureErrors);
            this.pnlSettings.Controls.Add(this.label1);
            this.pnlSettings.Controls.Add(this.cbGraphicAPI);
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(980, 70);
            this.pnlSettings.TabIndex = 0;
            // 
            // chkVSync
            // 
            this.chkVSync.AutoSize = true;
            this.chkVSync.Location = new System.Drawing.Point(523, 34);
            this.chkVSync.Name = "chkVSync";
            this.chkVSync.Size = new System.Drawing.Size(57, 17);
            this.chkVSync.TabIndex = 10;
            this.chkVSync.Text = "VSync";
            this.chkVSync.UseVisualStyleBackColor = true;
            // 
            // chkSound
            // 
            this.chkSound.AutoSize = true;
            this.chkSound.Checked = true;
            this.chkSound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSound.Location = new System.Drawing.Point(736, 34);
            this.chkSound.Name = "chkSound";
            this.chkSound.Size = new System.Drawing.Size(57, 17);
            this.chkSound.TabIndex = 9;
            this.chkSound.Text = "Sound";
            this.chkSound.UseVisualStyleBackColor = true;
            // 
            // chkKeepTesting
            // 
            this.chkKeepTesting.AutoSize = true;
            this.chkKeepTesting.Checked = true;
            this.chkKeepTesting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKeepTesting.Location = new System.Drawing.Point(583, 34);
            this.chkKeepTesting.Name = "chkKeepTesting";
            this.chkKeepTesting.Size = new System.Drawing.Size(147, 17);
            this.chkKeepTesting.TabIndex = 8;
            this.chkKeepTesting.Text = "Keep testing (ESC to quit)";
            this.chkKeepTesting.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label3.Location = new System.Drawing.Point(580, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Test Time";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label2.Location = new System.Drawing.Point(288, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Resolution";
            // 
            // cbResolution
            // 
            this.cbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbResolution.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbResolution.FormattingEnabled = true;
            this.cbResolution.Location = new System.Drawing.Point(291, 32);
            this.cbResolution.Name = "cbResolution";
            this.cbResolution.Size = new System.Drawing.Size(146, 21);
            this.cbResolution.TabIndex = 5;
            // 
            // chkFullScreen
            // 
            this.chkFullScreen.AutoSize = true;
            this.chkFullScreen.Location = new System.Drawing.Point(443, 34);
            this.chkFullScreen.Name = "chkFullScreen";
            this.chkFullScreen.Size = new System.Drawing.Size(74, 17);
            this.chkFullScreen.TabIndex = 4;
            this.chkFullScreen.Text = "Fullscreen";
            this.chkFullScreen.UseVisualStyleBackColor = true;
            // 
            // btnRunAllTests
            // 
            this.btnRunAllTests.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunAllTests.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnRunAllTests.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRunAllTests.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRunAllTests.ForeColor = System.Drawing.Color.White;
            this.btnRunAllTests.Location = new System.Drawing.Point(817, 30);
            this.btnRunAllTests.Name = "btnRunAllTests";
            this.btnRunAllTests.Size = new System.Drawing.Size(158, 23);
            this.btnRunAllTests.TabIndex = 3;
            this.btnRunAllTests.Text = "Run All Selected Tests >";
            this.btnRunAllTests.UseVisualStyleBackColor = false;
            this.btnRunAllTests.Click += new System.EventHandler(this.btnRunAllTests_Click);
            // 
            // chkCaptureErrors
            // 
            this.chkCaptureErrors.AutoSize = true;
            this.chkCaptureErrors.Location = new System.Drawing.Point(164, 34);
            this.chkCaptureErrors.Name = "chkCaptureErrors";
            this.chkCaptureErrors.Size = new System.Drawing.Size(112, 17);
            this.chkCaptureErrors.TabIndex = 2;
            this.chkCaptureErrors.Text = "Capture test errors";
            this.chkCaptureErrors.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Graphic API";
            // 
            // cbGraphicAPI
            // 
            this.cbGraphicAPI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGraphicAPI.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbGraphicAPI.FormattingEnabled = true;
            this.cbGraphicAPI.Location = new System.Drawing.Point(12, 32);
            this.cbGraphicAPI.Name = "cbGraphicAPI";
            this.cbGraphicAPI.Size = new System.Drawing.Size(146, 21);
            this.cbGraphicAPI.TabIndex = 0;
            this.cbGraphicAPI.SelectedIndexChanged += new System.EventHandler(this.cbGraphicAPI_SelectedIndexChanged);
            // 
            // pnlTests
            // 
            this.pnlTests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTests.Location = new System.Drawing.Point(0, 70);
            this.pnlTests.Name = "pnlTests";
            this.pnlTests.Size = new System.Drawing.Size(980, 324);
            this.pnlTests.TabIndex = 1;
            // 
            // lstLog
            // 
            this.lstLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLog.BackColor = System.Drawing.Color.Indigo;
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstLog.ContextMenuStrip = this.lstLogContext;
            this.lstLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(1, 397);
            this.lstLog.Name = "lstLog";
            this.lstLog.ScrollAlwaysVisible = true;
            this.lstLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstLog.Size = new System.Drawing.Size(978, 119);
            this.lstLog.TabIndex = 2;
            // 
            // lstLogContext
            // 
            this.lstLogContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToFileToolStripMenuItem,
            this.reloadFromFileToolStripMenuItem});
            this.lstLogContext.Name = "lstLogContext";
            this.lstLogContext.Size = new System.Drawing.Size(159, 48);
            // 
            // saveToFileToolStripMenuItem
            // 
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.saveToFileToolStripMenuItem.Text = "Save to file";
            this.saveToFileToolStripMenuItem.Click += new System.EventHandler(this.saveToFileToolStripMenuItem_Click);
            // 
            // reloadFromFileToolStripMenuItem
            // 
            this.reloadFromFileToolStripMenuItem.Name = "reloadFromFileToolStripMenuItem";
            this.reloadFromFileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.reloadFromFileToolStripMenuItem.Text = "Reload from file";
            this.reloadFromFileToolStripMenuItem.Click += new System.EventHandler(this.reloadFromFileToolStripMenuItem_Click);
            // 
            // FrmTestGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 519);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.pnlTests);
            this.Controls.Add(this.pnlSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmTestGUI";
            this.Text = "Dragonfly Engine Tests";
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.lstLogContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.Panel pnlTests;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbGraphicAPI;
        private System.Windows.Forms.CheckBox chkCaptureErrors;
        private System.Windows.Forms.Button btnRunAllTests;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbResolution;
        private System.Windows.Forms.CheckBox chkFullScreen;
        private System.Windows.Forms.CheckBox chkKeepTesting;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.ContextMenuStrip lstLogContext;
        private System.Windows.Forms.ToolStripMenuItem saveToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadFromFileToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkSound;
        private System.Windows.Forms.CheckBox chkVSync;
    }
}