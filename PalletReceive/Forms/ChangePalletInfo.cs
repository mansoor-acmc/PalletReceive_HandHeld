using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PalletReceive.DMServices;
using System.Net;

namespace PalletReceive
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
            lbPalletNum.Text = PalletInfo.PalletNum;
            lbItemId.Text = PalletInfo.ItemNumber;
            lbProductName.Text = PalletInfo.ItemDesc;
            cmbGrade.Text = PalletInfo.Grade;
            tbShade.Text = PalletInfo.Shade;
            tbCaliber.Text = PalletInfo.Caliber;
            lbSize.Text = PalletInfo.Size;
            lbShiftDate.Text = PalletInfo.ShiftDate.ToString("dd-MMM-yyyy");
            cmbShift.Text = ShiftName(PalletInfo.Shift);
            cmbLine.Text = PalletInfo.LineOfOrigin.ToString();
            cmbMarpak.Text = PalletInfo.WhichMarpak.ToString();
            cmbLGVorForklift.Text = PalletInfo.LGVOrForklift.ToString();
            numTotalBoxesOnPallet.Text = PalletInfo.BoxesOnPallet.ToString("n0");
            //numTotalPieces.Text = PalletInfo.totalPiecesOnPalletField.ToString("n3");
            //numTotalBoxesOnPallet.Text = PalletInfo.BoxesOnPallet.ToString("n3");

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

                if (PalletInfo == null || PalletInfo.PalletNum.Length==0)
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
                    if (Options.IsSlApprovalReq && PalletInfo.IsApprovedBySL == false)
                    {
                        msg = "SL Approval is required.";
                        new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "");
                        MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        msg = string.Empty;
                        return;
                    }

                    DMExportContract row = new DMExportContract();
                    DMCheckService client = new DMCheckService();

                    row.PalletNum = PalletInfo.PalletNum;
                    row.RecordId = PalletInfo.RecordId;
                    row.RecordIdSpecified = true;
                    row.ItemNumber = PalletInfo.ItemNumber;
                    row.Grade = cmbGrade.Text;
                    row.Shade = tbShade.Text;
                    row.Caliber = tbCaliber.Text;
                    row.Shift = cmbShift.SelectedIndex + 1;
                    row.ShiftSpecified = true;
                    /*row.ShiftDate = PalletInfo.ShiftDate;
                    row.ShiftDateSpecified1 = true;
                    row.TimeStamp = PalletInfo.TimeStamp;
                    row.TimeStampSpecified1 = true;*/
                    row.Size = PalletInfo.Size;
                    row.LineOfOrigin = cmbLine.SelectedIndex + 1;
                    row.LineOfOriginSpecified = true;
                    row.WhichMarpak = cmbMarpak.SelectedIndex + 1;
                    row.WhichMarpakSpecified = true;
                    row.LGVOrForklift = cmbLGVorForklift.Text.Equals("LGV") ? PalletTransportBy.LGV : PalletTransportBy.Forklift;
                    row.LGVOrForkliftSpecified = true;
                    //row.TotalSurface = decimal.Parse(numTotalSurface.Text);
                    //row.TotalSurfaceSpecified1 = true;
                    row.IsApprovedBySL = true;
                    row.IsApprovedBySLSpecified = true;
                    row.IsApprovedByFG = true;
                    row.IsApprovedByFGSpecified = true;
                    
                    //row.totalPiecesOnPalletField = Convert.ToInt16(Convert.ToDecimal(numTotalPieces.Text));
                    //row.totalPiecesOnPalletFieldSpecified1 = true;
                    row.BoxesOnPallet = Convert.ToInt16(Convert.ToDecimal(numTotalBoxesOnPallet.Text));
                    row.BoxesOnPalletSpecified = true;
                    row.DeviceName = AppVariables.DeviceName;
                    row.DeviceUser = AppVariables.UpdatedBy;

                    bool result1, result2;
                    client.UpdateAndConfirmPalletReceive(row, out result1, out result2);

                    if (result1)
                    {
                        row.ShiftDate = PalletInfo.ShiftDate;
                        row.TimeStamp = PalletInfo.TimeStamp;
                        row.Shift = PalletInfo.Shift;

                        msg = "Pallet '" + PalletInfo.PalletNum + "' has been saved and confirmed.";
                        MessageBox.Show(msg, "Approve [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "PalletNum:"+PalletInfo.PalletNum + 
                            ", RecId:" + PalletInfo.RecordId.ToString()+
                            ", Grade:"+row.Grade+
                            ", Shade:"+row.Shade+
                            ", Caliber:"+row.Caliber+
                            ", TotalBoxes:" + numTotalBoxesOnPallet.Text+
                            ", DeviceName:"+AppVariables.DeviceName+
                            ", DeviceUser:"+AppVariables.UpdatedBy);
                        msg = string.Empty;

                        DBClass db = new DBClass();
                        if (db.GetPallet(row.PalletNum).Rows.Count.Equals(0))
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
                if (exp.Status == WebExceptionStatus.ConnectFailure)
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
                new DBClass().SubmitMessage(msg, "ChangePalletInfo.numTotalSurface_TextChanged", "PalletNum:" + PalletInfo.PalletNum + ", TotalSurface=" + numTotalBoxesOnPallet.Text);

                numTotalBoxesOnPallet.Text = PalletInfo.TotalSurface.ToString("n3");
                MessageBox.Show(msg, "Validate [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
                numTotalBoxesOnPallet.Focus();
            }
        }

        

        

        
    }
}