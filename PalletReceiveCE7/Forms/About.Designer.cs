namespace PalletReceiveCE7.Forms
{
    partial class About
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
            this.lbDbFile = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lbDesc = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbDbFile
            // 
            this.lbDbFile.ForeColor = System.Drawing.Color.White;
            this.lbDbFile.Location = new System.Drawing.Point(5, 139);
            this.lbDbFile.Name = "lbDbFile";
            this.lbDbFile.Size = new System.Drawing.Size(310, 36);
            this.lbDbFile.Text = "label1";
            this.lbDbFile.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.button1.Location = new System.Drawing.Point(117, 187);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 40);
            this.button1.TabIndex = 10;
            this.button1.Text = "OK";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbDesc
            // 
            this.lbDesc.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.lbDesc.ForeColor = System.Drawing.Color.White;
            this.lbDesc.Location = new System.Drawing.Point(5, 57);
            this.lbDesc.Name = "lbDesc";
            this.lbDesc.Size = new System.Drawing.Size(310, 38);
            this.lbDesc.Text = "label1";
            this.lbDesc.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbVersion
            // 
            this.lbVersion.ForeColor = System.Drawing.Color.White;
            this.lbVersion.Location = new System.Drawing.Point(5, 107);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(310, 20);
            this.lbVersion.Text = "label1";
            this.lbVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbName
            // 
            this.lbName.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.lbName.ForeColor = System.Drawing.Color.White;
            this.lbName.Location = new System.Drawing.Point(2, 18);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(313, 34);
            this.lbName.Text = "label1";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(318, 295);
            this.ControlBox = false;
            this.Controls.Add(this.lbDbFile);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbDesc);
            this.Controls.Add(this.lbVersion);
            this.Controls.Add(this.lbName);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimizeBox = false;
            this.Name = "About";
            this.Text = "About";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbDbFile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbDesc;
        private System.Windows.Forms.Label lbVersion;
        private System.Windows.Forms.Label lbName;
    }
}