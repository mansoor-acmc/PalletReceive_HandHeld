using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Media;
using PalletReceive.DMServices;

namespace PalletReceive
{
    public partial class Home : Form
    {
        public Login LoginForm { get; set; }
        public DMExportContract PalletInfo { get; set; }

        DateTime? dtStart = null; //time difference to test if pallet scan manually.
        bool isManual = false; //Is scan pallet manually

        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            ClearFields();
            tbSearch.Focus();
        }

        private void Home_Activated(object sender, EventArgs e)
        {
            this.Text = AppVariables.DeviceName + " / " + AppVariables.UpdatedBy;
            
            if (AppVariables.RoleName == RoleType.Admin)
            {
                btnCancel.Visible = true;
                //if (!menuItemFile.MenuItems.Contains(menuItemPing)) menuItemFile.MenuItems.Add(menuItemPing);
                if (!menuItemFile.MenuItems.Contains(menuItemShrink)) menuItemFile.MenuItems.Add(menuItemShrink);
                if (!menuItemFile.MenuItems.Contains(menuItemDelOldData)) menuItemFile.MenuItems.Add(menuItemDelOldData);
                if (!menuItemEdit.MenuItems.Contains(menuItemChangePassword)) menuItemEdit.MenuItems.Add(menuItemChangePassword);
                if (!menuItemFile.MenuItems.Contains(menuItemSettings)) menuItemFile.MenuItems.Add(menuItemSettings);
            }
            else if (AppVariables.RoleName == RoleType.FinishedGoods)
            {
                btnCancel.Visible = true;
                //if (menuItemFile.MenuItems.Contains(menuItemPing)) menuItemFile.MenuItems.Remove(menuItemPing);
                if (menuItemFile.MenuItems.Contains(menuItemShrink)) menuItemFile.MenuItems.Remove(menuItemShrink);
                if (menuItemFile.MenuItems.Contains(menuItemDelOldData)) menuItemFile.MenuItems.Remove(menuItemDelOldData);
                if (menuItemEdit.MenuItems.Contains(menuItemChangePassword)) menuItemEdit.MenuItems.Remove(menuItemChangePassword);
                if (menuItemFile.MenuItems.Contains(menuItemSettings)) menuItemFile.MenuItems.Remove(menuItemSettings);
            }
            else if (AppVariables.RoleName == RoleType.SortingLine)
            {
                btnCancel.Visible = false;
                //if (menuItemFile.MenuItems.Contains(menuItemPing)) menuItemFile.MenuItems.Remove(menuItemPing);
                if (menuItemFile.MenuItems.Contains(menuItemShrink)) menuItemFile.MenuItems.Remove(menuItemShrink);
                if (menuItemFile.MenuItems.Contains(menuItemDelOldData)) menuItemFile.MenuItems.Remove(menuItemDelOldData); 
                if (menuItemEdit.MenuItems.Contains(menuItemChangePassword)) menuItemEdit.MenuItems.Remove(menuItemChangePassword);
                if (menuItemFile.MenuItems.Contains(menuItemSettings)) menuItemFile.MenuItems.Remove(menuItemSettings);
            }

            if (Options.IsLocationRequired)
            {
                panelLocation.Visible = true;
                panelPallet.Dock = DockStyle.Top;
                lbForPalletNum.Dock = DockStyle.Left;
                tbSearch.Dock = DockStyle.Fill;
            }
            else
            {
                panelLocation.Visible = false;
                panelPallet.Dock = DockStyle.Fill;
                lbForPalletNum.Dock = DockStyle.Top;
                tbSearch.Dock = DockStyle.Bottom;
            }
        }

        private void menuItemTestService_Click(object sender, EventArgs e)
        {
            string strMsg = string.Empty;
            try
            {

                DMCheckService client = new DMCheckService();
                strMsg = client.GetPing();
                SystemSounds.Beep.Play();
                MessageBox.Show(strMsg, "Testing Webservice");
            }
            catch (WebException exp)
            {
                string msg = string.Empty;
                if (exp.Status == WebExceptionStatus.ConnectFailure)
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                else
                    msg = exp.Message;
                new DBClass().SubmitMessage(msg, "Home.menuItemTestService_Click", "");
                MessageBox.Show(msg, "Connect Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void menuItemLogOff_Click(object sender, EventArgs e)
        {
            AppVariables.UpdatedBy = string.Empty;
            AppVariables.RoleName = RoleType.FinishedGoods;

            ClearFields();
            this.PalletInfo = new DMExportContract();

            Login loginScreen = LoginForm;
            loginScreen.HomeForm = this;
            loginScreen.Show();

            this.Hide();
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItemShrinkDB_Click(object sender, EventArgs e)
        {
            if (new DBClass().ShrinkDatabase())
            {
                MessageBox.Show("Database has been shrinked successfully.", "Shrink DB [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
            tbSearch.Focus();
        }

        private void menuItemDelOldData_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to delete the old data?", "Delete Old data", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (new DBClass().DeleteOldPallets())
                {
                    MessageBox.Show("Database has been deleted successfully.", "Delete Old data [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                }
            }
            tbSearch.Focus();
        }

        private void menuItemChangePassword_Click(object sender, EventArgs e)
        {
            new ChangePassword().ShowDialog();
            tbSearch.Focus();
        }

        
        private void btnFind_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            try
            {
                if (AppVariables.RoleName == RoleType.FinishedGoods)
                {
                    if (PalletInfo != null && !string.IsNullOrEmpty(PalletInfo.palletNumField))                
                    {
                        if (!PalletInfo.isApprovedByFGField)
                        {
                            tbSearch.Text = string.Empty;
                            throw new Exception("Pallet#"+PalletInfo.palletNumField+" has not been approved yet.\nPlease approve it before scanning next pallet.");
                        }
                    }
                }

                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);

                tbSearch.Text = tbSearch.Text.Trim().Replace("\r\n", "");
                if (string.IsNullOrEmpty(tbSearch.Text.Trim()))
                {
                    msg = "Please enter Pallet # to search.";
                    new DBClass().SubmitMessage(msg, "Home.btnFind_Click", "" );
                    MessageBox.Show(msg, "search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                    msg = string.Empty;
                    
                    tbSearch.Focus();
                    return;
                }
                DMCheckService client = new DMCheckService();
                PalletInfo = client.GetPalletInfo(tbSearch.Text);
                DataBindPallet();
                //May be user wants to approve duplicate Pallet. Then just show this message
                if (PalletInfo.isApprovedByFGField)
                {
                    msg = "Pallet:" + PalletInfo.palletNumField + " has already been approved by FG on '" + PalletInfo.fGApprovalTimeField.ToString("dd/MM/yyyy hh:mm:ss tt")+"'.";
                    new DBClass().SubmitMessage(msg, "Home.btnFind_Click", "");
                    MessageBox.Show(msg, "Search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    
                    tbSearch.Focus();
                }
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "Home.btnFind_Click", "");
                    MessageBox.Show(msg, "Connect Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
                else
                    throw new Exception(exp.Message, exp);
            }
            catch (Exception exp)
            {
                if (exp.Message.ToLower().Contains("external component"))
                    msg = "Please try again to search.";
                else
                    msg = exp.Message;

                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "Home.btnFind_Click", "");
                MessageBox.Show(msg, "Issue [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;                
            }
        }

        private void DataBindPallet()
        {
            if (PalletInfo != null && !string.IsNullOrEmpty( PalletInfo.itemNumberField))
            {
                EnDisControls(true);
                tbLocation.Focus();
                
                lbPalletNum.Text = PalletInfo.palletNumField;
                lbIsConfirmedBySL.Visible = lbIsConfirmedByFG.Visible = lbIsPosted.Visible = lbCancelled.Visible = false;
                btnApprove.Enabled = btnEdit.Enabled = btnCancel.Enabled = true;

                if (PalletInfo.isApprovedBySLField)
                {
                    lbIsConfirmedBySL.Visible = true;
                    tbSearch.Focus();
                }
                if (PalletInfo.isApprovedByFGField)
                {
                    lbIsConfirmedByFG.Visible = true;
                    tbSearch.Focus();
                }
                if (PalletInfo.isCancelledField)
                {
                    lbIsConfirmedBySL.Visible = lbIsConfirmedByFG.Visible = false;
                    lbCancelled.Visible = true;
                    tbSearch.Focus();
                }
                if (PalletInfo.isPostedField)
                {
                    lbIsConfirmedBySL.Visible = lbIsConfirmedByFG.Visible = false;
                    lbIsPosted.Visible = true;
                    btnApprove.Enabled = btnEdit.Enabled = btnCancel.Enabled = false;
                    tbSearch.Focus();
                }
                lbItemId.Text = PalletInfo.itemNumberField;
                lbProductName.Text = PalletInfo.itemDescField;
                lbGradeShadeCaliber.Text = PalletInfo.gradeField + " / " + PalletInfo.shadeField + " / " + PalletInfo.caliberField;
                lbSize.Text = PalletInfo.sizeField;
                lbShiftDate.Text = PalletInfo.shiftDateField.ToString("dd-MMM-yyyy");
                lbShift.Text = ShiftName(PalletInfo.shiftField);
                lbLineOfOrigin.Text = PalletInfo.lineOfOriginField.ToString();
                lbMarpak.Text = PalletInfo.whichMarpakField.ToString();
                lbLGVorForklift.Text = PalletInfo.lGVOrForkliftField.ToString();
                lbTotalSurface.Text = PalletInfo.totalSurfaceField.ToString("n3");
                //lbTotalPieces.Text = PalletInfo.totalPiecesOnPalletField.ToString("n3");
                lbTotalBoxes.Text = PalletInfo.boxesOnPalletField.ToString("n0");
                lbLocation.Text = string.Empty;
                if (!string.IsNullOrEmpty(PalletInfo.whLocationIdField))
                    lbLocation.Text = PalletInfo.whLocationIdField;

                
                tbSearch.Text = string.Empty;
                tbLocation.Text = string.Empty;
                SystemSounds.Beep.Play();

                
            }
            else
            {
                string palletNum="";
                if (PalletInfo != null)
                    palletNum = PalletInfo.palletNumField;
                string msg = "Pallet# " + palletNum + " does not exit. Please check again" ;
                new DBClass().SubmitMessage(msg, "Home.DataBindPallet()", "");
                ClearFields();
                EnDisControls(false);
                SystemSounds.Question.Play();
            }
        }

        private string ShiftName(int shift)
        {
            if (shift.Equals(1))
                return "Shift A";
            else if (shift.Equals(2))
                return "Shift B";
            else if (shift.Equals(3))
                return "Shift C";
            return "";
        }

        private void EnDisControls(bool isEnable)
        {
            btnApprove.Enabled=(isEnable);
            btnEdit.Enabled=(isEnable);
        }
        private void ClearFields()
        {
            lbPalletNum.Text = string.Empty;
            lbIsConfirmedBySL.Visible = false;
            lbIsConfirmedByFG.Visible = false;
            lbIsPosted.Visible = false;
            lbCancelled.Visible = false;
            lbItemId.Text = string.Empty;
            lbProductName.Text = string.Empty;
            lbGradeShadeCaliber.Text = string.Empty;
            lbSize.Text = string.Empty;
            lbShiftDate.Text = string.Empty;
            lbShift.Text = string.Empty;
            lbLineOfOrigin.Text = string.Empty;
            lbMarpak.Text = string.Empty;
            lbLGVorForklift.Text = string.Empty;
            lbTotalSurface.Text = string.Empty;
            lbTotalBoxes.Text = string.Empty;
            lbLocation.Text = string.Empty;
            //lbTotalPieces.Text = string.Empty;
            //lbTotalBoxesOnPallet.Text = string.Empty;
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

                tbLocation.Focus();
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
                            new DBClass().SubmitMessage(msg, "Home.tbSearch_KeyPress", "");
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

                if (AppVariables.WarehouseLocations != null && AppVariables.WarehouseLocations.Count().Equals(0))
                {
                    try
                    {
                        DMCheckService dmCheck = new DMCheckService();
                        AppVariables.WarehouseLocations = dmCheck.GetWHLocations().ToList();
                    }
                    catch
                    {
                        if (AppVariables.WarehouseLocations != null && AppVariables.WarehouseLocations.Count().Equals(0))
                        {
                            msg = "Unable to validate Warehouse location. Wifi was down when you were logged in. \n\rPlease close program and open again in the Wifi zone.";
                            new DBClass().SubmitMessage(msg, "Home.tbLocation_KeyPress", "");
                            MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            msg = string.Empty;
                            tbLocation.Focus();
                            return;
                        }
                    }
                }

                if (string.IsNullOrEmpty(tbLocation.Text.Trim()))
                {
                    msg = "Location is empty. Cannot continue for Confirm.";
                    new DBClass().SubmitMessage(msg, "Home.tbLocation_KeyPress", "");
                    MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    tbLocation.Focus();
                }
                else if (tbSearch.Text.Trim().Equals(tbLocation.Text.Trim()))
                {
                    msg = "Wrong Location. Location text cannot be same as Pallet number.";
                    new DBClass().SubmitMessage(msg, "Home.btnApprove_Click", "");
                    MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    tbLocation.Text = string.Empty;
                    tbLocation.Focus();
                }
                else if (!AppVariables.WarehouseLocations.Exists(t => t.locationField.Equals(tbLocation.Text.Trim())))
                {
                    msg = "Location: \"" + tbLocation.Text.Trim() + "\" is not correct.";
                    SystemSounds.Question.Play();
                    MessageBox.Show(msg, "Transfer Pallets [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;

                    tbLocation.Focus();
                    e.Handled = false;
                }
                else
                {
                    lbLocation.Text = tbLocation.Text.Trim();
                    btnApprove_Click(sender, e);
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

                            tbLocation.Focus();
                            msg = "Manual data-entry for Location is not allowed.";
                            new DBClass().SubmitMessage(msg, "Home.tbLocation_KeyPress", "");
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

        
        private void btnApprove_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            

            try
            {
                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);

                if (PalletInfo == null)
                {
                    msg = "No Pallet found OR you did not search a Pallet.";
                    new DBClass().SubmitMessage(msg, "Home.btnApprove_Click", "");
                    MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
                else
                {
                    //int numOfRows = 0;
                    //bool isSlApprovalReq = false;
                    //new DBClass().GetSettings(ref numOfRows, ref isSlApprovalReq);
                    
                    bool isFromSL = true;
                    if (AppVariables.RoleName == RoleType.SortingLine)
                        isFromSL = true;
                    else if (AppVariables.RoleName == RoleType.FinishedGoods)
                    {
                        isFromSL = false;
                        if (Options.IsSlApprovalReq && PalletInfo.isApprovedBySLField == false)
                        {
                            msg = "SL Approval is required.";
                            new DBClass().SubmitMessage(msg, "Home.btnApprove_Click", "");
                            MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            msg = string.Empty;
                            return;
                        }
                        //If Rejected pallet is not approved by SL then FG should not be able to approve it.
                        if (PalletInfo.gradeField.Equals("R") && !PalletInfo.isApprovedBySLField)
                        {
                            msg = "SL hasn't approved yet. Cannot Approve.";
                            new DBClass().SubmitMessage(msg, "Home.btnApprove_Click", "");
                            MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            msg = string.Empty;
                            return;
                        }
                    }

                    DMCheckService client = new DMCheckService();
                    string result1 = string.Empty;
                    PalletInfo.deviceNameField = AppVariables.DeviceName;
                    PalletInfo.deviceUserField = AppVariables.UpdatedBy;

                    if (Options.IsLocationRequired)
                        result1 = client.ConfirmPalletAndLocationReceive(PalletInfo.palletNumField,
                            tbLocation.Text.Trim(),
                            PalletInfo.recordIdField,
                            true,
                            AppVariables.DeviceName,
                            AppVariables.UpdatedBy,
                            isFromSL,
                            true);
                    else
                    {
                        bool checkResult, checkResult1;
                        client.ConfirmPalletReceive(PalletInfo.palletNumField,
                            PalletInfo.recordIdField,
                            true,
                            AppVariables.DeviceName,
                            AppVariables.UpdatedBy,
                            isFromSL,
                            true,
                            out checkResult,
                            out checkResult1);
                        if (!checkResult)
                            result1 = "Cannot approve";
                    }

                    if (string.IsNullOrEmpty(result1))
                    {
                        msg = "Pallet '" + PalletInfo.palletNumField + "' has been confirmed.";
                        new DBClass().SubmitMessage(msg, "Home.btnApprove_Click", "PalletNum:" + PalletInfo.palletNumField + ", RecId:" + PalletInfo.recordIdField.ToString() + ", Confirmed by:" + (isFromSL ? "SL" : "FG") + ", DeviceUser: " + AppVariables.UpdatedBy + ", DeviceName: " + AppVariables.DeviceName);
                        MessageBox.Show(msg, "Confirm [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;

                        lbCancelled.Visible = false;
                        if (isFromSL)
                            PalletInfo.isApprovedBySLField = true;
                        else
                            PalletInfo.isApprovedByFGField = true;

                        lbIsConfirmedBySL.Visible = PalletInfo.isApprovedBySLField;
                        lbIsConfirmedByFG.Visible = PalletInfo.isApprovedByFGField;

                        //lbIsConfirmed.Text = "Confirmed";

                        DBClass db = new DBClass();
                        if (db.GetPallet(PalletInfo.palletNumField).Rows.Count.Equals(0))
                        {
                            db.InsertPallet(PalletInfo);
                        }
                        db.UpdatePallet(PalletInfo, false);
                    }
                    else
                    {
                        new DBClass().SubmitMessage(result1, "Home.btnApprove_Click", "PalletNum:" + PalletInfo.palletNumField + ", RecId:" + PalletInfo.recordIdField.ToString() + ", Confirmed by:" + (isFromSL ? "SL" : "FG") + ", DeviceUser: " + AppVariables.UpdatedBy + ", DeviceName: " + AppVariables.DeviceName);
                        MessageBox.Show(result1, "Confirm [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;
                    }
                    
                }
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "Home.btnApprove_Click", "");
                    MessageBox.Show(msg, "Connect Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
                else
                    throw new Exception(exp.Message, exp);
            }
            catch (Exception exp)
            {
                if (exp.Message.ToLower().Contains("external component"))
                    msg = "Please try again to search.";
                else
                    msg = exp.Message;

                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "Home.btnApprove_Click", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {
                tbSearch.Focus();
            }

            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;


            try
            {
                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);

                if (PalletInfo == null)
                {
                    msg = "No Pallet found OR you did not search a Pallet.";
                    new DBClass().SubmitMessage(msg, "Home.btnCancel_Click", "");
                    MessageBox.Show(msg, "Cancel Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
                else
                {
                    //int numOfRows = 0;
                    //bool isSlApprovalReq = false;
                    //new DBClass().GetSettings(ref numOfRows, ref isSlApprovalReq);

                    
                    if (AppVariables.RoleName == RoleType.FinishedGoods)
                    {

                        if (Options.IsSlApprovalReq && PalletInfo.isApprovedBySLField == false)
                        {
                            msg = "SL Approval is required.";
                            new DBClass().SubmitMessage(msg, "Home.btnCancel_Click", "");
                            MessageBox.Show(msg, "Cancel Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            msg = string.Empty;
                            return;
                        }
                    }

                    DMCheckService client = new DMCheckService();
                    bool result1, result2;
                    PalletInfo.deviceNameField = AppVariables.DeviceName;
                    PalletInfo.deviceUserField = AppVariables.UpdatedBy;
                    //PalletInfo.isCancelledField = true;
                    client.CancelPalletReceive(PalletInfo.palletNumField,
                        PalletInfo.recordIdField,                        
                        true,
                        AppVariables.DeviceName,
                        AppVariables.UpdatedBy,                                                
                        out result1,
                        out result2);

                    if (result1)
                    {
                        msg = "Pallet '" + PalletInfo.palletNumField + "' has been 'Cancelled'.";
                        new DBClass().SubmitMessage(msg, "Home.btnCancel_Click", "PalletNum:" + PalletInfo.palletNumField + ", RecId:" + PalletInfo.recordIdField.ToString() + ", Cancel by: FG, DeviceUser: " + AppVariables.UpdatedBy + ", DeviceName: " + AppVariables.DeviceName);
                        MessageBox.Show(msg, "Cancel [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;
                        lbIsConfirmedByFG.Visible = lbIsConfirmedBySL.Visible = false;
                        lbCancelled.Visible = true;
                        
                        //lbIsConfirmed.Text = "Confirmed";

                        DBClass db = new DBClass();
                        if (db.GetPallet(PalletInfo.palletNumField).Rows.Count.Equals(0))
                        {
                            db.InsertPallet(PalletInfo);
                        }
                        db.UpdatePallet(PalletInfo, false);
                    }

                }
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "Home.btnCancel_Click", "");
                    MessageBox.Show(msg, "Connect Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
                else
                    throw new Exception(exp.Message, exp);
            }
            catch (Exception exp)
            {
                if (exp.Message.ToLower().Contains("external component"))
                    msg = "Please try again to search.";
                else
                    msg = exp.Message;

                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "Home.btnCancel_Click", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {
                tbSearch.Focus();
            }
        }
        
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (PalletInfo != null)
            {
                if (AppVariables.RoleName == RoleType.FinishedGoods)
                {
                    ChangePalletInfo palletInfoForm = new ChangePalletInfo();
                    palletInfoForm.PalletInfo = PalletInfo;
                    DialogResult result = palletInfoForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        PalletInfo = palletInfoForm.PalletInfo;
                        DataBindPallet();
                    }
                    palletInfoForm.Dispose();
                }
                else
                {
                    ChangePalletInfoSL palletInfoForm = new ChangePalletInfoSL();
                    palletInfoForm.PalletInfo = PalletInfo;
                    DialogResult result = palletInfoForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        PalletInfo = palletInfoForm.PalletInfo;
                        DataBindPallet();
                    }
                    palletInfoForm.Dispose();
                }
            }
            tbSearch.Focus();
        }

        private void menuItemSearch_Click(object sender, EventArgs e)
        {
            Search frmSearch = new Search();
            frmSearch.ShowDialog();
            frmSearch.Dispose();
            tbSearch.Focus();
        }

        private void menuItemPrintAgain_Click(object sender, EventArgs e)
        {
            string msg=string.Empty;

            try
            {
                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);


                if (PalletInfo == null)
                {
                    msg = "No Pallet found OR you did not search a Pallet.";
                    new DBClass().SubmitMessage(msg, "Home.menuItemPrintAgain_Click", "");
                    MessageBox.Show(msg, "Print Again [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
                else if (PalletInfo != null)
                {
                    DMCheckService client = new DMCheckService();
                    bool isUpdated = false, isUpdated1 = false;
                    client.PrintAgainPallet(PalletInfo.palletNumField, PalletInfo.recordIdField, true, AppVariables.DeviceName, AppVariables.UpdatedBy, out isUpdated, out isUpdated1);
                    if (isUpdated)
                    {
                        msg = "Pallet '" + PalletInfo.palletNumField + "' has been sent for Re-Print.";
                        new DBClass().SubmitMessage(msg, "Home.menuItemPrintAgain_Click", "PalletNum:" + PalletInfo.palletNumField + ", RecId:" + PalletInfo.recordIdField.ToString());
                        MessageBox.Show(msg, "Print Again [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;
                    }
                }
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "Home.menuItemPrintAgain_Click", "");
                    MessageBox.Show(msg, "Connect Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
                else
                    throw new Exception(exp.Message, exp);
            }
            catch (Exception exp)
            {
                if (exp.Message.ToLower().Contains("external component"))
                    msg = "Please try again to print.";
                else
                    msg = exp.Message;

                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "Home.menuItemPrintAgain_Click", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {
                tbSearch.Focus();
            }

            
        }

        private void menuItemOfflineMode_Click(object sender, EventArgs e)
        {
            OfflineScan frmOfflineScan = new OfflineScan();
            frmOfflineScan.Show();
        }

        private void menuItemSettings_Click(object sender, EventArgs e)
        {
            Settings frmSettings = new Settings();
            frmSettings.Show();
        }

        private void menuItemSummary_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            if (PalletInfo == null)
            {
                msg = "No Pallet found OR you did not search a Pallet.";
                new DBClass().SubmitMessage(msg, "Home.menuItemSummary_Click", "");
                MessageBox.Show(msg, "Item Summary [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            else
            {
                ItemSummary frmSummary = new ItemSummary();
                frmSummary.ItemNumber = PalletInfo.itemNumberField;
                frmSummary.ShowDialog();
                frmSummary.Dispose();
                tbSearch.Focus();
            }

        }

        private void menuItemDownMar1_Click(object sender, EventArgs e)
        {
            DownMarpak(1);
        }

        private void menuItemDownMar2_Click(object sender, EventArgs e)
        {
            DownMarpak(2);
        }

        private void DownMarpak(int whichMarpak)
        {
            string msg=string.Empty;

            try
            {
                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);
                bool result1, result2;
                DMCheckService client = new DMCheckService();
                client.CreateDowntimeForMarpak(whichMarpak, true, out result1, out result2);

                if (result1)
                {
                    msg = "Downtime for Marpak "+whichMarpak.ToString()+" has been recorded.";
                    new DBClass().SubmitMessage(msg, "Home.DownMarpak", "");
                    MessageBox.Show(msg, "Marpak Downtime [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "Home.DownMarpak", "");
                    MessageBox.Show(msg, "Connect Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                }
                else
                    throw new Exception(exp.Message, exp);
            }
            catch (Exception exp)
            {
                if (exp.Message.ToLower().Contains("external component"))
                    msg = "Please try again to print.";
                else
                    msg = exp.Message;

                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "Home.DownMarpak", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {
                tbSearch.Focus();
            }
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            new Forms.About().Show();
        }

        private void menuItemTransfer_Click(object sender, EventArgs e)
        {
            new Forms.OfflineTransfer().Show();
        }

       
        
    }
}