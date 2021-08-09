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
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            numRowsUpload.Value = Options.NumberOfRowsUpload;
            chkIsSLApproveRequired.Checked = Options.IsSlApprovalReq;
            chkIsLocationRequired.Checked = Options.IsLocationRequired;
            chkHasManualEntryAllowed.Checked = Options.HasManualEntryAllowed;
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (new DBClass().SaveSettings(Convert.ToInt16(numRowsUpload.Value),
                chkIsSLApproveRequired.Checked,
                chkIsLocationRequired.Checked,
                chkHasManualEntryAllowed.Checked))
            {
                Options.NumberOfRowsUpload = Convert.ToInt16(numRowsUpload.Value);
                Options.IsSlApprovalReq = chkIsSLApproveRequired.Checked;
                Options.IsLocationRequired = chkIsLocationRequired.Checked;
                Options.HasManualEntryAllowed = chkHasManualEntryAllowed.Checked;

                MessageBox.Show("New Settings have been saved successfully.", "Settings [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}