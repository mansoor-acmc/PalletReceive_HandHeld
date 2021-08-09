using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PalletReceiveCE7.DMServices;
using System.Net;

namespace PalletReceiveCE7
{
    public partial class ChangePalletInfo : Form
    {
        public DMExportContract PalletInfo { get; set; }

        int maxSurface = 150;//, maxBoxes = 100, maxPieces = 1000;

        public ChangePalletInfo()
        {
            InitializeComponent();
        }
                

        private void ChangePalletInfo_Load(object sender, EventArgs e)
        {
            lbPalletNum.Text = PalletInfo.palletNumField;
            lbItemId.Text = PalletInfo.itemNumberField;
            lbProductName.Text = PalletInfo.itemDescField;
            cmbGrade.Text = PalletInfo.gradeField;
            tbShade.Text = PalletInfo.shadeField;
            tbCaliber.Text = PalletInfo.caliberField;
            lbSize.Text = PalletInfo.sizeField;
            lbShiftDate.Text = PalletInfo.shiftDateField.ToString("dd-MMM-yyyy");
            cmbShift.Text = ShiftName(PalletInfo.shiftField);
            cmbLine.Text = PalletInfo.lineOfOriginField.ToString();
            cmbMarpak.Text = PalletInfo.whichMarpakField.ToString();
            cmbLGVorForklift.Text = PalletInfo.lGVOrForkliftField.ToString();
            numTotalBoxesOnPallet.Text = PalletInfo.boxesOnPalletField.ToString("n0");
            //numTotalPieces.Text = PalletInfo.totalPiecesOnPalletField.ToString("n3");
            //numTotalBoxesOnPallet.Text = PalletInfo.boxesOnPalletField.ToString("n3");

            tbShade.Focus();
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
        
        private void numTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar=='.')
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSaveApprove_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            try
            {
                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);

                if (PalletInfo == null || PalletInfo.palletNumField.Length==0)
                {
                    msg = "No Pallet found.";
                    MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "");
                    msg = string.Empty;
                }
                else
                {
                    //int numOfRows = 0;
                    //bool isSlApprovalReq = false;
                    //new DBClass().GetSettings(ref numOfRows, ref isSlApprovalReq);
                    if (Options.IsSlApprovalReq && PalletInfo.isApprovedBySLField == false)
                    {
                        msg = "SL Approval is required.";
                        new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "");
                        MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;
                        return;
                    }

                    DMExportContract row = new DMExportContract();
                    DMCheckService client = new DMCheckService();

                    row.palletNumField = PalletInfo.palletNumField;
                    row.recordIdField = PalletInfo.recordIdField;
                    row.recordIdFieldSpecified1 = true;
                    row.itemNumberField = PalletInfo.itemNumberField;
                    row.gradeField = cmbGrade.Text;
                    row.shadeField = tbShade.Text;
                    row.caliberField = tbCaliber.Text;
                    /*row.shiftField = cmbShift.SelectedIndex + 1;
                    row.shiftFieldSpecified1 = true;
                    row.shiftDateField = PalletInfo.shiftDateField;
                    row.shiftDateFieldSpecified1 = true;
                    row.timeStampField = PalletInfo.timeStampField;
                    row.timeStampFieldSpecified1 = true;*/
                    row.sizeField = PalletInfo.sizeField;
                    row.lineOfOriginField = cmbLine.SelectedIndex + 1;
                    row.lineOfOriginFieldSpecified1 = true;
                    row.whichMarpakField = cmbMarpak.SelectedIndex + 1;
                    row.whichMarpakFieldSpecified1 = true;
                    row.lGVOrForkliftField = cmbLGVorForklift.Text.Equals("LGV") ? PalletTransportBy.LGV : PalletTransportBy.Forklift;
                    row.lGVOrForkliftFieldSpecified1 = true;
                    //row.totalSurfaceField = decimal.Parse(numTotalSurface.Text);
                    //row.totalSurfaceFieldSpecified1 = true;
                    row.isApprovedBySLField = true;
                    row.isApprovedBySLFieldSpecified1 = true;
                    row.isApprovedByFGField = true;
                    row.isApprovedByFGFieldSpecified1 = true;
                    
                    //row.totalPiecesOnPalletField = Convert.ToInt16(Convert.ToDecimal(numTotalPieces.Text));
                    //row.totalPiecesOnPalletFieldSpecified1 = true;
                    row.boxesOnPalletField = Convert.ToInt16(Convert.ToDecimal(numTotalBoxesOnPallet.Text));
                    row.boxesOnPalletFieldSpecified1 = true;
                    row.deviceNameField = AppVariables.DeviceName;
                    row.deviceUserField = AppVariables.UpdatedBy;

                    bool result1, result2;
                    client.UpdateAndConfirmPalletReceive(row, out result1, out result2);

                    if (result1)
                    {
                        row.shiftDateField = PalletInfo.shiftDateField;
                        row.timeStampField = PalletInfo.timeStampField;
                        row.shiftField = PalletInfo.shiftField;

                        msg = "Pallet '" + PalletInfo.palletNumField + "' has been saved and confirmed.";
                        MessageBox.Show(msg, "Approve [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "PalletNum:"+PalletInfo.palletNumField + 
                            ", RecId:" + PalletInfo.recordIdField.ToString()+
                            ", Grade:"+row.gradeField+
                            ", Shade:"+row.shadeField+
                            ", Caliber:"+row.caliberField+
                            ", TotalBoxes:" + numTotalBoxesOnPallet.Text+
                            ", DeviceName:"+AppVariables.DeviceName+
                            ", DeviceUser:"+AppVariables.UpdatedBy);
                        msg = string.Empty;

                        DBClass db = new DBClass();
                        if (db.GetPallet(row.palletNumField).Rows.Count.Equals(0))
                        {
                            db.InsertPallet(row);
                        }
                        db.UpdatePallet(row, true);

                        PalletInfo = row;
                        this.DialogResult = DialogResult.OK;
                    }
                    this.Close();
                }
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure || exp.Status == WebExceptionStatus.ProtocolError)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "");
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

                new DBClass().SubmitMessage(msg , "ChangePalletInfo.btnSaveApprove_Click", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {

            }

            tbShade.Focus();
        }
        
        private void numTotalSurface_TextChanged(object sender, EventArgs e)
        {
            string msg = "";
            if (decimal.Parse(numTotalBoxesOnPallet.Text) > maxSurface)
            {
                msg = "Value cannot be more than " + maxSurface.ToString();
                new DBClass().SubmitMessage(msg, "ChangePalletInfo.numTotalSurface_TextChanged", "PalletNum:" + PalletInfo.palletNumField + ", TotalSurface=" + numTotalBoxesOnPallet.Text);

                numTotalBoxesOnPallet.Text = PalletInfo.totalSurfaceField.ToString("n3");
                MessageBox.Show(msg, "Validate [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
                numTotalBoxesOnPallet.Focus();
            }
        }

        

        

        
    }
}