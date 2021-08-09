using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using PalletReceive.DMServices;
using System.Net;

namespace PalletReceive
{
    public partial class OfflineScan : Form
    {
        DateTime? dtStart = null; //time difference to test if pallet scan manually.
        bool isManual = false; //Is scan pallet manually

        public OfflineScan()
        {
            InitializeComponent();
        }

        void ClearFields()
        {
            cmbGrade.Text = tbCaliber.Text = tbShade.Text = numTotalBoxesOnPallet.Text = tbLocation.Text= string.Empty;
            lbPalletNum.Text = string.Empty;
        }

        private void DataBind()
        {
            DataTable dt = new DBClass().GetOfflinePallets();
            gridPallets.DataSource = dt;
            //this.dataGridTableStyle1.MappingName = gridPallets.DataSource.GetType().Name;
            gridPallets.Refresh();
            lbTotal.Text = "Total rows: " + dt.Rows.Count.ToString();
            if (dt.Rows.Count > 0)
            {
                gridPallets.Select(dt.Rows.Count - 1);
                gridPallets.CurrentRowIndex = dt.Rows.Count - 1;
            }
        }

        private void OfflineScan_Load(object sender, EventArgs e)
        {
            ClearFields();
            DataBind();

            if (Options.IsLocationRequired)
            {
                tbLocation.Visible = true;
                lbForLocation.Visible = true;
            }
            else
            {
                tbLocation.Visible = false;
                lbForLocation.Visible = false;
            }

            tbSearch.Focus();
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

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            string msg = string.Empty;

            if (!dtStart.HasValue)
            {
                dtStart = DateTime.Now;
            }

            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                if (dtStart.HasValue)
                {
                    TimeSpan ts = DateTime.Now - dtStart.Value;
                    dtStart = null;
                    if (ts.TotalMilliseconds > 500)//if manually entered
                        isManual = true;
                }

                btnFind_Click(sender, e);
                e.Handled = true;
            }
            else
            {
                if (Options.HasManualEntryAllowed == false)
                {
                    if (dtStart.HasValue)
                    {
                        TimeSpan ts = DateTime.Now - dtStart.Value;

                        if (ts.TotalMilliseconds > 500)//if manually entered
                        {

                            tbSearch.Focus();
                            msg = "Manual data-entry for Pallet # is not allowed.";
                            new DBClass().SubmitMessage(msg, "OfflineScan.tbSearch_KeyPress", "");
                            MessageBox.Show(msg, "search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                            msg = string.Empty;
                            dtStart = null;
                            tbSearch.Text = string.Empty;

                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            DBClass db=new DBClass();
            tbSearch.Text = tbSearch.Text.Trim().Replace("\r\n", "");
            ClearFields();
            if (string.IsNullOrEmpty(tbSearch.Text.Trim()))
            {
                msg = "Please enter Pallet # to search.";
                new DBClass().SubmitMessage(msg, "OfflineScan.btnFind_Click", "");
                MessageBox.Show(msg, "search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                msg = string.Empty;
                SystemSounds.Question.Play();
                tbSearch.Focus();
                return;
            }
            DataTable dt = db.GetPallet(tbSearch.Text.Trim());
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["IsOffline"].ToString() == "True" && dt.Rows[0]["IsApprovedBySL"].ToString() == "True")
                    msg = "Pallet=" + tbSearch.Text.Trim() + " is already inserted and Approved.";
                else if (dt.Rows[0]["IsOffline"].ToString() == "True" && dt.Rows[0]["IsApprovedBySL"].ToString() == "False")
                    msg = "Pallet=" + tbSearch.Text.Trim() + " is already inserted for Offline check.";
                MessageBox.Show(msg, "Offline Pallet [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
                ClearFields();
                tbSearch.Text = string.Empty;
                
                SystemSounds.Question.Play();
                tbSearch.Focus();
                return;
            }

            if (db.InsertOfflinePallet(tbSearch.Text.Trim()) > 0)
            {
                msg = "Pallet has been INSERTED for offline usage.";
                new DBClass().SubmitMessage(msg, "OfflineScan.btnFind_Click", "");
                //MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
                                
                lbPalletNum.Text = tbSearch.Text.Trim();
                tbSearch.Text = string.Empty;

                DataBind();
                SystemSounds.Beep.Play();
            }
            tbSearch.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DMExportContract cr = new DMExportContract();
            cr.PalletNum = lbPalletNum.Text;
            cr.Grade = cmbGrade.Text;
            cr.Shade = tbShade.Text;
            cr.Caliber = tbCaliber.Text;
            //if (!string.IsNullOrEmpty(numTotalBoxesOnPallet.Text))
            cr.BoxesOnPallet = int.Parse("0" + numTotalBoxesOnPallet.Text);
            cr.TotalSurface = decimal.Parse("0" + numTotalBoxesOnPallet.Text);
            cr.whLocationId = tbLocation.Text;

            string msg = string.Empty;
            if (new DBClass().UpdateOfflinePallet(cr) > 0)
            {
                msg = "Pallet '"+lbPalletNum.Text+"' has been UPDATED for offline usage.";
                new DBClass().SubmitMessage(msg, "OfflineScan.btnSave_Click", "");
                MessageBox.Show(msg, "Update Pallet [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;

                ClearFields();
                
                //SystemSounds.Beep.Play();
                DataBind();
            }
            tbSearch.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            if (gridPallets.DataSource != null && gridPallets.CurrentRowIndex != -1)
            {
                DataTable dt = ((DataTable)(gridPallets.DataSource));
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[gridPallets.CurrentRowIndex];
                    SystemSounds.Question.Play();
                    msg = "Do you want to delete Pallet: " + dr["Pallet"].ToString() + "?";
                    if (MessageBox.Show(msg, "Delete Pallet [" + AppVariables.DeviceName + "]", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        new DBClass().DeleteOfflinePallet(dr["Pallet"].ToString());
                        SystemSounds.Beep.Play();
                        DataBind();
                    }
                }
                else
                {
                    msg = "There is no row selected to delete.";
                    SystemSounds.Asterisk.Play();
                    MessageBox.Show(msg, "Delete Pallet [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                }
            }
            tbSearch.Focus();
        }

        private void gridPallets_CurrentCellChanged(object sender, EventArgs e)
        {
            Point pt = gridPallets.PointToClient(Control.MousePosition);
            DataGrid.HitTestInfo info = gridPallets.HitTest(pt.X, pt.Y);
            if (info.Type == DataGrid.HitTestType.Cell || info.Type == DataGrid.HitTestType.RowHeader)
            {
                gridPallets.Select(info.Row);
            }
        }

        private void gridPallets_DoubleClick(object sender, EventArgs e)
        {
           Point pt = gridPallets.PointToClient(Control.MousePosition);
           DataGrid.HitTestInfo info=gridPallets.HitTest(pt.X, pt.Y);
           if (info.Type == DataGrid.HitTestType.Cell || info.Type == DataGrid.HitTestType.RowHeader)
           {
               if (gridPallets.DataSource != null && info.Row == gridPallets.CurrentRowIndex)
               {
                   DataRow row = ((DataTable)gridPallets.DataSource).Rows[gridPallets.CurrentRowIndex];
                   if (row != null)
                   {
                       lbPalletNum.Text = row["Pallet"].ToString();
                       cmbGrade.Text = row["Grade"].ToString();
                       tbShade.Text = row["Shade"].ToString();
                       tbCaliber.Text = row["Caliber"].ToString();
                       numTotalBoxesOnPallet.Text = row["Boxes"].ToString();
                       tbLocation.Text = row["Location"].ToString();
                   }
               }
           }
        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            if (!DBClass.CheckInternet())
                throw new Exception(AppVariables.NetworkDown);
            int counted = 0;
            //int counted = 0, size = 0;
            //bool isSlApprovalReq = false;
            //new DBClass().GetSettings(ref size, ref isSlApprovalReq);

            if (gridPallets.DataSource != null)
            {
                
                DataTable dt = ((DataTable)(gridPallets.DataSource));
                if (dt != null && dt.Rows.Count > 0)
                {
                    btnSubmit.Enabled = false;
                    try
                    {
                        List<DMExportOfflineContract> lines = new List<DMExportOfflineContract>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            DMExportOfflineContract contract = new DMExportOfflineContract()
                            {
                                PalletNum = dr["Pallet"].ToString(),
                                Grade = dr["Grade"].ToString(),
                                Shade = dr["Shade"].ToString(),
                                Caliber = dr["Caliber"].ToString(),
                                BoxesOnPallet = decimal.Parse(dr["Boxes"].ToString()),
                                //BoxesOnPalletSpecified1 = true,
                                DeviceName = AppVariables.DeviceName,
                                DeviceUser = AppVariables.UpdatedBy,                                
                                IsOfflineMode = NoYes.Yes,
                                //isOfflineModeFieldSpecified1 = true,
                                whLocationId = dr["Location"].ToString()
                            };
                            if (AppVariables.RoleName == RoleType.SortingLine)
                            {
                                contract.IsApprovedBySL = true;
                                //contract.IsApprovedBySLSpecified1 = true;
                            }
                            else if (AppVariables.RoleName == RoleType.FinishedGoods)
                            {
                                contract.IsApprovedByFG = true;
                                //contract.IsApprovedByFGSpecified1 = true;
                            }

                            lines.Add(contract);
                        }

                        DMCheckService client = new DMCheckService();
                        int dataLen = lines.Count;
                        List<DMExportOfflineContract> dataChunks;

                        while (dataLen > 0)
                        {
                            dataChunks = lines.Skip(counted).Take(Options.NumberOfRowsUpload).ToList();

                            var itemsUpdated = client.UpdateOfflinePallets(dataChunks.ToArray());

                            DBClass db = new DBClass();
                            foreach (DMExportContract oneRow in itemsUpdated)
                            {
                                db.UpdateOfflinePalletAfterSync(oneRow);
                            }

                            dataLen -= Options.NumberOfRowsUpload;
                            counted += Options.NumberOfRowsUpload;
                        }

                        msg = "Offline Pallets have been updated to Dynamics Successfully.";
                        SystemSounds.Beep.Play();
                        MessageBox.Show(msg, "Sync Pallets [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;

                        DataBind();

                    }
                    catch (WebException exp)
                    {
                        if (exp.Status == WebExceptionStatus.ConnectFailure)
                        {
                            msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                            new DBClass().SubmitMessage(msg, "OfflineSync.btnSubmit_Click", "");
                            MessageBox.Show(msg, "Connect Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            msg = string.Empty;
                        }
                        else
                            throw new Exception(exp.Message, exp);
                    }
                    catch (Exception exp)
                    {
                        if (exp.Message.ToLower().Contains("external component"))
                            msg = "Please try again to submit.";
                        else
                            msg = exp.Message;

                        new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "OfflineSync.btnSubmit_Click", "");
                        MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;
                    }
                    finally
                    {
                        tbSearch.Focus();
                        btnSubmit.Enabled = true;
                        btnSubmit.ResumeLayout(true);
                        btnSubmit.Refresh();
                    }
                }
                else
                {
                    msg = "No pallet has been selected to submit.";
                    SystemSounds.Question.Play();
                    MessageBox.Show(msg, "Sync Pallets [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
            }
        }

        private void tbShade_TextChanged(object sender, EventArgs e)
        {
            if (tbShade.Text.Length > 0)
            {
                tbShade.Text = tbShade.Text.ToUpper();
                tbShade.SelectionStart = tbShade.Text.Length;
                if (tbShade.Text == "\r\n")
                {
                    tbShade.Text = string.Empty;
                }
            }
        }

        private void tbCaliber_TextChanged(object sender, EventArgs e)
        {
            if (tbCaliber.Text.Length > 0)
            {
                tbCaliber.Text = tbCaliber.Text.ToUpper();
                tbCaliber.SelectionStart = tbCaliber.Text.Length;
                if (tbCaliber.Text == "\r\n")
                {
                    tbCaliber.Text = string.Empty;
                }
            }
        }

        private void numTotalBoxesOnPallet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == '.')
                e.Handled = false;
            else
                e.Handled = true;
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