namespace PalletReceive.Forms
{
    partial class OfflineTransfer
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
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemClose = new System.Windows.Forms.MenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.panelLocation = new System.Windows.Forms.Panel();
            this.tbLocation = new System.Windows.Forms.TextBox();
            this.lbLocationForTextbox = new System.Windows.Forms.Label();
            this.panelPallet = new System.Windows.Forms.Panel();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.lbForPalletNum = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.lbTotal = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.gridPallets = new System.Windows.Forms.DataGrid();
            this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
            this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn2 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.panelSearch.SuspendLayout();
            this.panelLocation.SuspendLayout();
            this.panelPallet.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.menuItemClose);
            this.menuItem1.Text = "File";
            // 
            // menuItemClose
            // 
            this.menuItemClose.Text = "Close";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Right;
            this.label3.Location = new System.Drawing.Point(237, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(3, 267);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(0, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(3, 267);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 1);
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.Black;
            this.panelSearch.Controls.Add(this.panelLocation);
            this.panelSearch.Controls.Add(this.panelPallet);
            this.panelSearch.Controls.Add(this.label5);
            this.panelSearch.Controls.Add(this.btnFind);
            this.panelSearch.Controls.Add(this.label10);
            this.panelSearch.Controls.Add(this.label9);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(3, 1);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(234, 48);
            // 
            // panelLocation
            // 
            this.panelLocation.BackColor = System.Drawing.Color.Black;
            this.panelLocation.Controls.Add(this.tbLocation);
            this.panelLocation.Controls.Add(this.lbLocationForTextbox);
            this.panelLocation.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLocation.Location = new System.Drawing.Point(0, 25);
            this.panelLocation.Name = "panelLocation";
            this.panelLocation.Size = new System.Drawing.Size(157, 20);
            // 
            // tbLocation
            // 
            this.tbLocation.BackColor = System.Drawing.Color.Black;
            this.tbLocation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tbLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLocation.ForeColor = System.Drawing.Color.White;
            this.tbLocation.Location = new System.Drawing.Point(56, 0);
            this.tbLocation.MaxLength = 10;
            this.tbLocation.Name = "tbLocation";
            this.tbLocation.Size = new System.Drawing.Size(101, 21);
            this.tbLocation.TabIndex = 0;
            this.tbLocation.TextChanged += new System.EventHandler(this.tbLocation_TextChanged);
            this.tbLocation.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbLocation_KeyPress);
            // 
            // lbLocationForTextbox
            // 
            this.lbLocationForTextbox.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbLocationForTextbox.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lbLocationForTextbox.ForeColor = System.Drawing.Color.White;
            this.lbLocationForTextbox.Location = new System.Drawing.Point(0, 0);
            this.lbLocationForTextbox.Name = "lbLocationForTextbox";
            this.lbLocationForTextbox.Size = new System.Drawing.Size(56, 20);
            this.lbLocationForTextbox.Text = "Location:";
            // 
            // panelPallet
            // 
            this.panelPallet.BackColor = System.Drawing.Color.Black;
            this.panelPallet.Controls.Add(this.tbSearch);
            this.panelPallet.Controls.Add(this.lbForPalletNum);
            this.panelPallet.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPallet.Location = new System.Drawing.Point(0, 2);
            this.panelPallet.Name = "panelPallet";
            this.panelPallet.Size = new System.Drawing.Size(157, 23);
            // 
            // tbSearch
            // 
            this.tbSearch.BackColor = System.Drawing.Color.Black;
            this.tbSearch.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tbSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSearch.ForeColor = System.Drawing.Color.White;
            this.tbSearch.Location = new System.Drawing.Point(56, 0);
            this.tbSearch.MaxLength = 10;
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(101, 21);
            this.tbSearch.TabIndex = 0;
            this.tbSearch.TextChanged += new System.EventHandler(this.tbSearch_TextChanged);
            this.tbSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSearch_KeyPress);
            // 
            // lbForPalletNum
            // 
            this.lbForPalletNum.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbForPalletNum.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lbForPalletNum.ForeColor = System.Drawing.Color.White;
            this.lbForPalletNum.Location = new System.Drawing.Point(0, 0);
            this.lbForPalletNum.Name = "lbForPalletNum";
            this.lbForPalletNum.Size = new System.Drawing.Size(56, 23);
            this.lbForPalletNum.Text = "Pallet #";
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Right;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(157, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(5, 45);
            // 
            // btnFind
            // 
            this.btnFind.BackColor = System.Drawing.Color.DimGray;
            this.btnFind.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnFind.ForeColor = System.Drawing.Color.White;
            this.btnFind.Location = new System.Drawing.Point(162, 2);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(72, 45);
            this.btnFind.TabIndex = 2;
            this.btnFind.Text = "&Add";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(0, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(234, 1);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Black;
            this.label9.Dock = System.Windows.Forms.DockStyle.Top;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(234, 2);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(3, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(234, 1);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Controls.Add(this.label15);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(234, 15);
            // 
            // label15
            // 
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label15.ForeColor = System.Drawing.Color.Yellow;
            this.label15.Location = new System.Drawing.Point(0, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(234, 15);
            this.label15.Text = "Pallets && Locations Scanned:";
            // 
            // lbTotal
            // 
            this.lbTotal.BackColor = System.Drawing.Color.Black;
            this.lbTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTotal.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lbTotal.ForeColor = System.Drawing.Color.White;
            this.lbTotal.Location = new System.Drawing.Point(70, 0);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(94, 40);
            this.lbTotal.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Black;
            this.panel7.Controls.Add(this.lbTotal);
            this.panel7.Controls.Add(this.btnTransfer);
            this.panel7.Controls.Add(this.btnDelete);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(3, 228);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(234, 40);
            // 
            // btnTransfer
            // 
            this.btnTransfer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btnTransfer.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnTransfer.ForeColor = System.Drawing.Color.White;
            this.btnTransfer.Location = new System.Drawing.Point(0, 0);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(70, 40);
            this.btnTransfer.TabIndex = 8;
            this.btnTransfer.Text = "&Transfer";
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Red;
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(164, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(70, 40);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "&Del Row";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // gridPallets
            // 
            this.gridPallets.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.gridPallets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPallets.Location = new System.Drawing.Point(3, 65);
            this.gridPallets.Name = "gridPallets";
            this.gridPallets.Size = new System.Drawing.Size(234, 163);
            this.gridPallets.TabIndex = 14;
            this.gridPallets.TableStyles.Add(this.dataGridTableStyle1);
            this.gridPallets.DoubleClick += new System.EventHandler(this.gridPallets_DoubleClick);
            this.gridPallets.CurrentCellChanged += new System.EventHandler(this.gridPallets_CurrentCellChanged);
            // 
            // dataGridTableStyle1
            // 
            this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn1);
            this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn2);
            this.dataGridTableStyle1.MappingName = "Transfers";
            // 
            // dataGridTextBoxColumn1
            // 
            this.dataGridTextBoxColumn1.Format = "";
            this.dataGridTextBoxColumn1.FormatInfo = null;
            this.dataGridTextBoxColumn1.HeaderText = "Pallet";
            this.dataGridTextBoxColumn1.MappingName = "Pallet";
            this.dataGridTextBoxColumn1.Width = 100;
            // 
            // dataGridTextBoxColumn2
            // 
            this.dataGridTextBoxColumn2.Format = "";
            this.dataGridTextBoxColumn2.FormatInfo = null;
            this.dataGridTextBoxColumn2.HeaderText = "Location";
            this.dataGridTextBoxColumn2.MappingName = "Location";
            this.dataGridTextBoxColumn2.Width = 100;
            // 
            // OfflineTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.gridPallets);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.Menu = this.mainMenu1;
            this.Name = "OfflineTransfer";
            this.Text = "Offline Transfer";
            this.Load += new System.EventHandler(this.OfflineTransfer_Load);
            this.panelSearch.ResumeLayout(false);
            this.panelLocation.ResumeLayout(false);
            this.panelPallet.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItemClose;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Panel panelLocation;
        private System.Windows.Forms.TextBox tbLocation;
        private System.Windows.Forms.Label lbLocationForTextbox;
        private System.Windows.Forms.Panel panelPallet;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.Label lbForPalletNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGrid gridPallets;
        private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
        private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn1;
        private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn2;
    }
}