namespace PalletReceive
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.numRowsUpload = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.chkIsSLApproveRequired = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chkIsLocationRequired = new System.Windows.Forms.CheckBox();
            this.chkHasManualEntryAllowed = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 5);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(0, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(5, 263);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Right;
            this.label3.Location = new System.Drawing.Point(235, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(5, 263);
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(5, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(230, 25);
            this.label4.Text = "Application Settings:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(5, 150);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(230, 42);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Orange;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(0, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 42);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(158, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 42);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.numRowsUpload);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.chkIsSLApproveRequired);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(230, 57);
            // 
            // numRowsUpload
            // 
            this.numRowsUpload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numRowsUpload.Location = new System.Drawing.Point(143, 30);
            this.numRowsUpload.Name = "numRowsUpload";
            this.numRowsUpload.Size = new System.Drawing.Size(87, 22);
            this.numRowsUpload.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Left;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(0, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 27);
            this.label5.Text = "Max row to Upload:";
            // 
            // chkIsSLApproveRequired
            // 
            this.chkIsSLApproveRequired.BackColor = System.Drawing.Color.Black;
            this.chkIsSLApproveRequired.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkIsSLApproveRequired.ForeColor = System.Drawing.Color.White;
            this.chkIsSLApproveRequired.Location = new System.Drawing.Point(0, 0);
            this.chkIsSLApproveRequired.Name = "chkIsSLApproveRequired";
            this.chkIsSLApproveRequired.Size = new System.Drawing.Size(230, 30);
            this.chkIsSLApproveRequired.TabIndex = 0;
            this.chkIsSLApproveRequired.Text = "Is SL Approval Required";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Controls.Add(this.chkHasManualEntryAllowed);
            this.panel3.Controls.Add(this.chkIsLocationRequired);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(5, 87);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(230, 63);
            // 
            // chkIsLocationRequired
            // 
            this.chkIsLocationRequired.BackColor = System.Drawing.Color.Black;
            this.chkIsLocationRequired.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkIsLocationRequired.ForeColor = System.Drawing.Color.White;
            this.chkIsLocationRequired.Location = new System.Drawing.Point(0, 0);
            this.chkIsLocationRequired.Name = "chkIsLocationRequired";
            this.chkIsLocationRequired.Size = new System.Drawing.Size(230, 30);
            this.chkIsLocationRequired.TabIndex = 0;
            this.chkIsLocationRequired.Text = "Ware-house Location required";
            // 
            // chkHasManualEntryAllowed
            // 
            this.chkHasManualEntryAllowed.BackColor = System.Drawing.Color.Black;
            this.chkHasManualEntryAllowed.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkHasManualEntryAllowed.ForeColor = System.Drawing.Color.White;
            this.chkHasManualEntryAllowed.Location = new System.Drawing.Point(0, 30);
            this.chkHasManualEntryAllowed.Name = "chkHasManualEntryAllowed";
            this.chkHasManualEntryAllowed.Size = new System.Drawing.Size(230, 30);
            this.chkHasManualEntryAllowed.TabIndex = 1;
            this.chkHasManualEntryAllowed.Text = "Has Manual Entry Allowed";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.Menu = this.mainMenu1;
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkIsSLApproveRequired;
        private System.Windows.Forms.NumericUpDown numRowsUpload;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox chkIsLocationRequired;
        private System.Windows.Forms.CheckBox chkHasManualEntryAllowed;
    }
}