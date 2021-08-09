using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PalletReceiveCE7
{
    public partial class Search : Form
    {
        public Search()
        {
            InitializeComponent();
        }

        

        private void btnFind_Click(object sender, EventArgs e)
        {
            DataBind();
            tbSearch.Focus();
        }

        void DataBind()
        {
            lbNumOfRec.Text = "";
            this.SuspendLayout();
            //panelMessageBg.Visible = true;

            this.ResumeLayout();

            if (!string.IsNullOrEmpty(tbSearch.Text.Trim()))
            {
                DataTable dt = new DBClass().SearchItems(tbSearch.Text.Trim());
                palletInfoDataGrid.DataSource = dt;
                lbNumOfRec.Text = "Records:" + dt.Rows.Count.ToString();
                lbTitle.Text = "Found Serials:";
                if (dt.Rows.Count == 0)
                {
                    lbTitle.Text = "No Serials:";
                    MessageBox.Show("No pallet found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                }
            }
            else
                MessageBox.Show("Enter some text in the 'Search' textbox to search.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            
            //panelMessageBg.Visible = false;
            tbSearch.Focus();

        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                btnFind_Click(sender, e);
                e.Handled = true;
            }
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (tbSearch.Text.Length > 0)
            {
                tbSearch.Text = tbSearch.Text.ToUpper();
                tbSearch.SelectionStart = tbSearch.Text.Length;
                if (tbSearch.Text == "\r\n")
                {
                    tbSearch.Text = string.Empty;
                }
            }
        }

        private void Search_Load(object sender, EventArgs e)
        {
            tbSearch.Focus();
        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}