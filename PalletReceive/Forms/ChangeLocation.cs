using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PalletReceive.Forms
{
    public partial class ChangeLocation : Form
    {
        public ChangeLocation()
        {
            InitializeComponent();
            tbLocation.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //AppVariables.DefaultLocation = tbLocation.Text.Trim();

            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbLocation_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                //AppVariables.DefaultLocation = tbLocation.Text.Trim(); 
                
                e.Handled = true;
            }
        }

        private void tbLocation_TextChanged(object sender, EventArgs e)
        {
            if (tbLocation.Text.Length > 0)
            {
                tbLocation.Text = tbLocation.Text.ToUpper();
                tbLocation.SelectionStart = tbLocation.Text.Length;
                if (tbLocation.Text == "\r\n")
                {
                    tbLocation.Text = string.Empty;
                }
            }
        }
    }
}