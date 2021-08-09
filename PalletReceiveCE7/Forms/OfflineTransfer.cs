using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using PalletReceiveCE7.DMServices;
using System.Net;

namespace PalletReceiveCE7.Forms
{
    public partial class OfflineTransfer : Form
    {
        DateTime? dtStart = null; //time difference to test if pallet scan manually.
        bool isManual = false; //Is scan pallet manually

        public OfflineTransfer()
        {
            InitializeComponent();
        }

        void ClearFields()
        {
            tbLocation.Text = tbSearch.Text = string.Empty;
        }

        private void DataBind()
        {
            DataTable dt = new DBClass().GetRemainingTransfers();
            gridPallets.DataSource = dt;
            //this.dataGridTableStyle1.MappingName = gridPallets.DataSource.GetType().Name;
            gridPallets.Refresh();
            lbTotal.Text = "Rows: " + dt.Rows.Count.ToString();
            if (dt.Rows.Count > 0)
            {
                gridPallets.Select(dt.Rows.Count - 1);
                gridPallets.CurrentRowIndex = dt.Rows.Count - 1;
            }
        }

        private void OfflineTransfer_Load(object sender, EventArgs e)
        {
            ClearFields();
            DataBind();
            tbSearch.Focus();
        }

        private void gridPallets_DoubleClick(object sender, EventArgs e)
        {
            //Point pt = gridPallets.PointToClient(Control.MousePosition);
            //DataGrid.HitTestInfo info = gridPallets.HitTest(pt.X, pt.Y);
            //if (info.Type == DataGrid.HitTestType.Cell || info.Type == DataGrid.HitTestType.RowHeader)
            //{
            //    if (gridPallets.DataSource != null && info.Row == gridPallets.CurrentRowIndex)
            //    {
            //        DataRow row = ((DataTable)gridPallets.DataSource).Rows[gridPallets.CurrentRowIndex];
            //        if (row != null)
            //        {
            //            tbSearch.Text = row["Pallet"].ToString();                        
            //            tbLocation.Text = row["Location"].ToString();
            //        }
            //    }
            //}
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

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void tbLocation_KeyPress(object sender, KeyPressEventArgs e)
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

                if (string.IsNullOrEmpty(tbLocation.Text.Trim()))
                {
                    msg = "Location is empty. Cannot continue for Confirm.";
                    new DBClass().SubmitMessage(msg, "OfflineTransfer.tbLocation_KeyPress", "");
                    MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    tbLocation.Focus();
                }
                else if (tbSearch.Text.Trim().Equals(tbLocation.Text.Trim()))
                {
                    msg = "Wrong Location. Location text cannot be same as Pallet number.";
                    new DBClass().SubmitMessage(msg, "OfflineTransfer.btnApprove_Click", "");
                    MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    tbLocation.Text = string.Empty;
                    tbLocation.Focus();
                }
                else if (AppVariables.WarehouseLocations != null && AppVariables.WarehouseLocations.Count().Equals(0))
                {
                    msg = "Unable to validate Warehouse location. Wifi was down when you were logged in. \n\rPlease close program and open again in the Wifi zone.";
                    new DBClass().SubmitMessage(msg, "OfflineTransfer.tbLocation_KeyPress", "");
                    MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    tbLocation.Focus();
                    
                }
                else if (!AppVariables.WarehouseLocations.Exists(t => t.locationField.Equals(tbLocation.Text.Trim())))
                {
                    msg = "Location \"" + tbLocation.Text.Trim() + "\" is not correct.";
                    SystemSounds.Question.Play();
                    MessageBox.Show(msg, "Transfer Pallets [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;

                    tbLocation.Focus();
                    e.Handled = false;
                }
                else
                {
                    btnFind_Click(sender, e);
                    e.Handled = true;
                }
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

                            tbLocation.Focus();
                            msg = "Manual data-entry for Location is not allowed.";
                            new DBClass().SubmitMessage(msg, "OfflineTransfer.tbLocation_KeyPress", "");
                            MessageBox.Show(msg, "search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                            msg = string.Empty;
                            dtStart = null;
                            tbLocation.Text = string.Empty;

                            e.Handled = true;
                        }
                    }
                }
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

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            string msg = string.Empty;

            if (!dtStart.HasValue)
            {
                dtStart = DateTime.Now;
            }

            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                tbLocation.Focus();
                if (dtStart.HasValue)
                {
                    TimeSpan ts = DateTime.Now - dtStart.Value;
                    dtStart = null;
                    if (ts.TotalMilliseconds > 500)//if manually entered
                        isManual = true;
                }

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
                            new DBClass().SubmitMessage(msg, "OfflineTransfer.tbSearch_KeyPress", "");
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
            DBClass db = new DBClass();
            tbSearch.Text = tbSearch.Text.Trim().Replace("\r\n", "");
            tbLocation.Text = tbLocation.Text.Trim().Replace("\r\n", "");
            if (string.IsNullOrEmpty(tbSearch.Text.Trim()))
            {
                msg = "Please enter Pallet # to continue.";
                new DBClass().SubmitMessage(msg, "OfflineTransfer.btnFind_Click", "");
                MessageBox.Show(msg, "search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                msg = string.Empty;
                SystemSounds.Question.Play();
                tbSearch.Focus();
                return;
            }
            else if (tbSearch.Text.Trim().Length <= 5)
            {
                msg = "Pallet: " + tbSearch.Text.Trim() + " is not correct.";
                new DBClass().SubmitMessage(msg, "OfflineTransfer.btnFind_Click", "");
                MessageBox.Show(msg, "search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                msg = string.Empty;
                SystemSounds.Question.Play();
                tbSearch.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(tbLocation.Text.Trim()))
            {
                msg = "Please enter Location # to continue.";
                new DBClass().SubmitMessage(msg, "OfflineTransfer.btnFind_Click", "");
                MessageBox.Show(msg, "search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                msg = string.Empty;
                SystemSounds.Question.Play();
                tbLocation.Focus();
                return;
            }
            else if (tbSearch.Text.Trim().Equals(tbLocation.Text.Trim()))
            {
                msg = "Wrong Location. Location text cannot be same as Pallet #.";
                new DBClass().SubmitMessage(msg, "OfflineTransfer.btnFind_Click", "");
                MessageBox.Show(msg, "search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                msg = string.Empty;
                SystemSounds.Question.Play();
                tbLocation.Text = string.Empty;
                tbLocation.Focus();
                return;
            }
            DataTable dt = db.GetTransferPallet(tbSearch.Text.Trim(), tbLocation.Text.Trim());
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["PalletNum"].ToString() == tbSearch.Text.Trim() &&
                    dt.Rows[0]["Location"].ToString() == tbLocation.Text.Trim() &&
                    dt.Rows[0]["IsTransferred"].ToString() == "True")
                {
                    msg = "Pallet=" + tbSearch.Text.Trim() + " is already TRANSFERRED.";
                }
                else if (dt.Rows[0]["PalletNum"].ToString() == tbSearch.Text.Trim() &&
                    dt.Rows[0]["Location"].ToString() == tbLocation.Text.Trim() &&
                    dt.Rows[0]["IsTransferred"].ToString() == "False")
                {
                    msg = "Pallet=" + tbSearch.Text.Trim() + " is already Inserted and ready for Transfer.";
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, "Transfer Pallet [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    ClearFields();
                    tbSearch.Text = string.Empty;

                    SystemSounds.Question.Play();
                    tbSearch.Focus();
                    return;
                }
            }

            if (db.InsertTransferPallet(tbSearch.Text.Trim(), tbLocation.Text.Trim(), isManual, AppVariables.UpdatedBy) > 0)
            {
                msg = "Pallet and Location have been INSERTED for Transfer.";
                new DBClass().SubmitMessage(msg, "OfflineTransfer.btnFind_Click", "");
                //MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;

                tbLocation.Text = tbSearch.Text.Trim();
                isManual = false;

                DataBind();
                ClearFields();
                SystemSounds.Beep.Play();
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
                    msg = "Do you want to remove Pallet: " + dr["Pallet"].ToString() + " from the list?";
                    if (MessageBox.Show(msg, "Remove Pallet [" + AppVariables.DeviceName + "]", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        new DBClass().DeletePalletForTransfer(dr["ID"].ToString());
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
            else
            {
                msg = "No pallet has been selected to delete.";
                SystemSounds.Question.Play();
                MessageBox.Show(msg, "Transfer Pallets [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            tbSearch.Focus();
        }

        private string CheckDuplicatePallets(List<LocationHistory> checkDupl)
        {
            List<LocationHistory> checkedSuccess = new List<LocationHistory>();
            foreach (LocationHistory row in checkDupl)
            {
                if (checkedSuccess.Exists(t => t.PalletNum.Equals(row.PalletNum)))
                {
                    //gridPallets.Select(dt.Rows.Count - 1);
                    //gridPallets.CurrentRowIndex = dt.Rows.Count - 1;
                    return row.PalletNum;
                }
                checkedSuccess.Add(row);
            }

            return string.Empty;
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            int counted = 0, transferred = 0;

            if (gridPallets.DataSource != null)
            {

                DataTable dt = ((DataTable)(gridPallets.DataSource));
                if (dt != null && dt.Rows.Count > 0)
                {
                    btnTransfer.Enabled = false;
                    try
                    {
                        if (!DBClass.CheckInternet())
                            throw new Exception(AppVariables.NetworkDown);

                        List<LocationHistory> lines = new List<LocationHistory>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            LocationHistory contract = new LocationHistory()
                            {
                                PalletNum = dr["Pallet"].ToString(),
                                Location = dr["Location"].ToString(),
                                IsManual = bool.Parse(dr["IsManual"].ToString()),
                                IsManualSpecified = true,
                                UserName = dr["userName"].ToString(),
                                DeviceName = AppVariables.DeviceName + "-" + AppVariables.DeviceIP
                            };
                            lines.Add(contract);
                        }

                        DMCheckService client = new DMCheckService();
                        int dataLen = lines.Count;
                        List<LocationHistory> dataChunks;
                        bool hasSomePallets = false;
                        string somePallets = string.Empty;

                        while (dataLen > 0)
                        {
                            dataChunks = lines.Skip(counted).Take(Options.NumberOfRowsUpload).ToList();
                            string duplPallets = CheckDuplicatePallets(dataChunks);
                            if (!string.IsNullOrEmpty(duplPallets))
                                throw new Exception("Remove duplicate pallet: " + duplPallets);

                            var itemsUpdated = client.TransferPalletsToNewLocation(dataChunks.ToArray());
                            if (itemsUpdated.Count() > 0)//If pallets transferred to new location then remove them from screen
                            {
                                transferred += new DBClass().UpdateTransferPallets(itemsUpdated.ToList());
                            }
                            else
                            {
                                hasSomePallets = true;
                                somePallets += string.Join(",", dataChunks.Select(t => t.PalletNum).ToArray());
                            }

                            dataLen -= Options.NumberOfRowsUpload;
                            counted += Options.NumberOfRowsUpload;
                        }


                        if (transferred.Equals(0))
                            throw new Exception("No pallet has been transferred. Check Network Admin.");
                        else if (hasSomePallets)
                            throw new Exception("Some pallets have not been transferred: " + somePallets);


                        msg = transferred.ToString() + " Pallets' Location transfer have been done Successfully.";
                        SystemSounds.Beep.Play();
                        MessageBox.Show(msg, "Transfer Pallets [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;
                    }
                    catch (WebException exp)
                    {
                        if (exp.Status == WebExceptionStatus.ConnectFailure || exp.Status == WebExceptionStatus.ProtocolError)
                        {
                            msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                            new DBClass().SubmitMessage(msg, "OfflineTransfer.btnTransfer_Click", "");
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
                        SystemSounds.Question.Play();
                        new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "OfflineTransfer.btnTransfer_Click", "");
                        MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;
                    }
                    finally
                    {
                        DataBind();

                        tbSearch.Focus();
                        btnTransfer.Enabled = true;
                        btnTransfer.ResumeLayout(true);
                        btnTransfer.Refresh();
                    }
                }
                else
                {
                    msg = "No pallet has been selected to transfer.";
                    SystemSounds.Question.Play();
                    MessageBox.Show(msg, "Transfer Pallets [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    tbSearch.Focus();
                }
            }
        }

        
    }
}