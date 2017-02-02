namespace UcmaBotDTMF
{
    partial class frmActiveCallWin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmActiveCallWin));
            this.sst = new System.Windows.Forms.StatusStrip();
            this.tsslIvrStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslLanguage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslEndpointUri = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslActiveCalls = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEnglish = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpanish = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEndpoint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.tbs = new System.Windows.Forms.ToolStrip();
            this.dgvActiveCalls = new System.Windows.Forms.DataGridView();
            this.txtTraceOutput = new System.Windows.Forms.TextBox();
            this.textToSpeechToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ttsBing = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmnAmazon = new System.Windows.Forms.ToolStripMenuItem();
            this.sst.SuspendLayout();
            this.tbs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvActiveCalls)).BeginInit();
            this.SuspendLayout();
            // 
            // sst
            // 
            this.sst.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.sst.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslIvrStatus,
            this.tsslLanguage,
            this.tsslEndpointUri,
            this.tsslActiveCalls});
            this.sst.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.sst.Location = new System.Drawing.Point(0, 739);
            this.sst.Name = "sst";
            this.sst.Size = new System.Drawing.Size(633, 20);
            this.sst.TabIndex = 1;
            this.sst.Text = "statusStrip1";
            // 
            // tsslIvrStatus
            // 
            this.tsslIvrStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslIvrStatus.Name = "tsslIvrStatus";
            this.tsslIvrStatus.Size = new System.Drawing.Size(112, 15);
            this.tsslIvrStatus.Text = "IVR Status : Stopped";
            this.tsslIvrStatus.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // tsslLanguage
            // 
            this.tsslLanguage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslLanguage.Name = "tsslLanguage";
            this.tsslLanguage.Size = new System.Drawing.Size(106, 15);
            this.tsslLanguage.Text = "Language : English";
            // 
            // tsslEndpointUri
            // 
            this.tsslEndpointUri.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslEndpointUri.Name = "tsslEndpointUri";
            this.tsslEndpointUri.Size = new System.Drawing.Size(82, 15);
            this.tsslEndpointUri.Text = "Endpoint Uri : ";
            // 
            // tsslActiveCalls
            // 
            this.tsslActiveCalls.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslActiveCalls.Name = "tsslActiveCalls";
            this.tsslActiveCalls.Size = new System.Drawing.Size(83, 15);
            this.tsslActiveCalls.Text = "Active Calls : 0";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLanguage,
            this.tsmiEndpoint,
            this.tsmiSettings,
            this.tsmiQuit,
            this.textToSpeechToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(37, 22);
            this.toolStripDropDownButton1.Text = "IVR";
            // 
            // tsmiLanguage
            // 
            this.tsmiLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEnglish,
            this.tsmiSpanish});
            this.tsmiLanguage.Name = "tsmiLanguage";
            this.tsmiLanguage.Size = new System.Drawing.Size(154, 22);
            this.tsmiLanguage.Text = "Language";
            // 
            // tsmiEnglish
            // 
            this.tsmiEnglish.Checked = true;
            this.tsmiEnglish.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiEnglish.Name = "tsmiEnglish";
            this.tsmiEnglish.Size = new System.Drawing.Size(115, 22);
            this.tsmiEnglish.Text = "English";
            this.tsmiEnglish.Click += new System.EventHandler(this.tsmiEnglish_Click);
            // 
            // tsmiSpanish
            // 
            this.tsmiSpanish.Name = "tsmiSpanish";
            this.tsmiSpanish.Size = new System.Drawing.Size(115, 22);
            this.tsmiSpanish.Text = "Spanish";
            this.tsmiSpanish.Click += new System.EventHandler(this.tsmiSpanish_Click);
            // 
            // tsmiEndpoint
            // 
            this.tsmiEndpoint.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiStart,
            this.tsmiStop});
            this.tsmiEndpoint.Name = "tsmiEndpoint";
            this.tsmiEndpoint.Size = new System.Drawing.Size(154, 22);
            this.tsmiEndpoint.Text = "Endpoint";
            this.tsmiEndpoint.Click += new System.EventHandler(this.tsmiEndpoint_Click);
            // 
            // tsmiStart
            // 
            this.tsmiStart.Name = "tsmiStart";
            this.tsmiStart.Size = new System.Drawing.Size(152, 22);
            this.tsmiStart.Text = "Start";
            this.tsmiStart.Click += new System.EventHandler(this.tsmiStart_Click);
            // 
            // tsmiStop
            // 
            this.tsmiStop.Checked = true;
            this.tsmiStop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiStop.Name = "tsmiStop";
            this.tsmiStop.Size = new System.Drawing.Size(152, 22);
            this.tsmiStop.Text = "Stop";
            this.tsmiStop.Click += new System.EventHandler(this.tsmiStop_Click);
            // 
            // tsmiSettings
            // 
            this.tsmiSettings.Name = "tsmiSettings";
            this.tsmiSettings.Size = new System.Drawing.Size(154, 22);
            this.tsmiSettings.Text = "Settings";
            this.tsmiSettings.Click += new System.EventHandler(this.tsmiSettings_Click);
            // 
            // tsmiQuit
            // 
            this.tsmiQuit.Name = "tsmiQuit";
            this.tsmiQuit.Size = new System.Drawing.Size(154, 22);
            this.tsmiQuit.Text = "Quit";
            this.tsmiQuit.Click += new System.EventHandler(this.tsmiQuit_Click);
            // 
            // tbs
            // 
            this.tbs.BackColor = System.Drawing.Color.Snow;
            this.tbs.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tbs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.tbs.Location = new System.Drawing.Point(0, 0);
            this.tbs.Name = "tbs";
            this.tbs.Size = new System.Drawing.Size(633, 25);
            this.tbs.TabIndex = 0;
            this.tbs.Text = "toolStrip1";
            this.tbs.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tbs_ItemClicked);
            // 
            // dgvActiveCalls
            // 
            this.dgvActiveCalls.AllowUserToAddRows = false;
            this.dgvActiveCalls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvActiveCalls.Location = new System.Drawing.Point(0, 26);
            this.dgvActiveCalls.MultiSelect = false;
            this.dgvActiveCalls.Name = "dgvActiveCalls";
            this.dgvActiveCalls.ReadOnly = true;
            this.dgvActiveCalls.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvActiveCalls.Size = new System.Drawing.Size(637, 248);
            this.dgvActiveCalls.TabIndex = 2;
            this.dgvActiveCalls.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dgvActiveCalls_RowStateChanged);
            this.dgvActiveCalls.SelectionChanged += new System.EventHandler(this.dgvActiveCalls_SelectionChanged);
            // 
            // txtTraceOutput
            // 
            this.txtTraceOutput.Location = new System.Drawing.Point(0, 275);
            this.txtTraceOutput.Multiline = true;
            this.txtTraceOutput.Name = "txtTraceOutput";
            this.txtTraceOutput.Size = new System.Drawing.Size(633, 461);
            this.txtTraceOutput.TabIndex = 3;
            // 
            // textToSpeechToolStripMenuItem
            // 
            this.textToSpeechToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ttsBing,
            this.tsmnAmazon});
            this.textToSpeechToolStripMenuItem.Name = "textToSpeechToolStripMenuItem";
            this.textToSpeechToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.textToSpeechToolStripMenuItem.Text = "Text To Speech";
            // 
            // ttsBing
            // 
            this.ttsBing.Name = "ttsBing";
            this.ttsBing.Size = new System.Drawing.Size(152, 22);
            this.ttsBing.Text = "Bing";
            this.ttsBing.Click += new System.EventHandler(this.ttsBing_Click);
            // 
            // tsmnAmazon
            // 
            this.tsmnAmazon.Name = "tsmnAmazon";
            this.tsmnAmazon.Size = new System.Drawing.Size(152, 22);
            this.tsmnAmazon.Text = "Amazon";
            this.tsmnAmazon.Click += new System.EventHandler(this.tsmnAmazon_Click);
            // 
            // frmActiveCallWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 759);
            this.Controls.Add(this.txtTraceOutput);
            this.Controls.Add(this.dgvActiveCalls);
            this.Controls.Add(this.sst);
            this.Controls.Add(this.tbs);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmActiveCallWin";
            this.Text = "IVR Standalone Application";
            this.Load += new System.EventHandler(this.frmActiveCallWin_Load);
            this.sst.ResumeLayout(false);
            this.sst.PerformLayout();
            this.tbs.ResumeLayout(false);
            this.tbs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvActiveCalls)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip sst;
        private System.Windows.Forms.ToolStripStatusLabel tsslIvrStatus;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem tsmiLanguage;
        private System.Windows.Forms.ToolStripMenuItem tsmiEnglish;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpanish;
        private System.Windows.Forms.ToolStripMenuItem tsmiEndpoint;
        private System.Windows.Forms.ToolStripMenuItem tsmiStart;
        private System.Windows.Forms.ToolStripMenuItem tsmiStop;
        private System.Windows.Forms.ToolStripMenuItem tsmiSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuit;
        private System.Windows.Forms.ToolStrip tbs;
        private System.Windows.Forms.ToolStripStatusLabel tsslLanguage;
        private System.Windows.Forms.ToolStripStatusLabel tsslEndpointUri;
        private System.Windows.Forms.ToolStripStatusLabel tsslActiveCalls;
        private System.Windows.Forms.DataGridView dgvActiveCalls;
        private System.Windows.Forms.TextBox txtTraceOutput;
        private System.Windows.Forms.ToolStripMenuItem textToSpeechToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ttsBing;
        private System.Windows.Forms.ToolStripMenuItem tsmnAmazon;
    }
}