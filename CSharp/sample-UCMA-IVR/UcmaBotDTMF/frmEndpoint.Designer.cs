namespace UcmaBotDTMF
{
    partial class frmEndpoint
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
            this.txtSipUri = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTraceOutput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtDirectlineSecret = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbLanguages = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbActiveCalls = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblCallDetails = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSipUri
            // 
            this.txtSipUri.Enabled = false;
            this.txtSipUri.Location = new System.Drawing.Point(145, 10);
            this.txtSipUri.Name = "txtSipUri";
            this.txtSipUri.Size = new System.Drawing.Size(514, 21);
            this.txtSipUri.TabIndex = 34;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Sip Uri:";
            // 
            // txtTraceOutput
            // 
            this.txtTraceOutput.Location = new System.Drawing.Point(6, 269);
            this.txtTraceOutput.Multiline = true;
            this.txtTraceOutput.Name = "txtTraceOutput";
            this.txtTraceOutput.Size = new System.Drawing.Size(654, 463);
            this.txtTraceOutput.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 247);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Call Log:";
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(263, 113);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(112, 23);
            this.btnStop.TabIndex = 28;
            this.btnStop.Text = "Stop Endpoint";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(145, 113);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(112, 23);
            this.btnStart.TabIndex = 27;
            this.btnStart.Text = "Start Endpoint";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtDirectlineSecret
            // 
            this.txtDirectlineSecret.Enabled = false;
            this.txtDirectlineSecret.Location = new System.Drawing.Point(145, 76);
            this.txtDirectlineSecret.Name = "txtDirectlineSecret";
            this.txtDirectlineSecret.Size = new System.Drawing.Size(514, 21);
            this.txtDirectlineSecret.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Directline Secret :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Choose language:";
            // 
            // cmbLanguages
            // 
            this.cmbLanguages.FormattingEnabled = true;
            this.cmbLanguages.Location = new System.Drawing.Point(145, 44);
            this.cmbLanguages.Name = "cmbLanguages";
            this.cmbLanguages.Size = new System.Drawing.Size(271, 21);
            this.cmbLanguages.TabIndex = 36;
            this.cmbLanguages.SelectedIndexChanged += new System.EventHandler(this.cmbLanguages_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Active Calls:";
            // 
            // cmbActiveCalls
            // 
            this.cmbActiveCalls.Enabled = false;
            this.cmbActiveCalls.FormattingEnabled = true;
            this.cmbActiveCalls.Location = new System.Drawing.Point(145, 149);
            this.cmbActiveCalls.Name = "cmbActiveCalls";
            this.cmbActiveCalls.Size = new System.Drawing.Size(271, 21);
            this.cmbActiveCalls.TabIndex = 38;
            this.cmbActiveCalls.SelectedIndexChanged += new System.EventHandler(this.cmbActiveCalls_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Call Info:";
            // 
            // lblCallDetails
            // 
            this.lblCallDetails.AutoSize = true;
            this.lblCallDetails.Location = new System.Drawing.Point(142, 189);
            this.lblCallDetails.Name = "lblCallDetails";
            this.lblCallDetails.Size = new System.Drawing.Size(72, 13);
            this.lblCallDetails.TabIndex = 40;
            this.lblCallDetails.Text = "Call Details";
            // 
            // frmEndpoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 740);
            this.Controls.Add(this.lblCallDetails);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbActiveCalls);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbLanguages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSipUri);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtTraceOutput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtDirectlineSecret);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEndpoint";
            this.Text = "Standalone Endpoint (Speech & DTMF)";
            this.Load += new System.EventHandler(this.frmEndpoint_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSipUri;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTraceOutput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtDirectlineSecret;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbLanguages;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbActiveCalls;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblCallDetails;
    }
}

