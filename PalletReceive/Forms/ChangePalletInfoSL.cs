using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using PalletReceive.DMServices;

namespace PalletReceive
{
    public partial class ChangePalletInfoSL : Form
    {
        public DMExportContract PalletInfo { get; set; }

        int maxSurface = 150;//, maxBoxes = 100, maxPieces = 1000;

        public ChangePalletInfoSL()
        {
            InitializeComponent();
        }

        private void ChangePalletInfoSL_Load(object sender, EventArgs e)
        {
            lbPalletNum.Text = PalletInfo.PalletNum;
            lbItemId.Text = PalletInfo.ItemNumber;
            lbProductName.Text = PalletInfo.ItemDesc;
            cmbGrade.Text = PalletInfo.Grade;
            tbShade.Text = PalletInfo.Shade;
            tbCaliber.Text = PalletInfo.Caliber;
            lbSize.Text = PalletInfo.Size;
            
            cmbLine.Text = PalletInfo.LineOfOrigin.ToString();
            cmbMarpak.Text = PalletInfo.WhichMarpak.ToString();
            cmbLGVorForklift.Text = PalletInfo.LGVOrForklift.ToString();
            numTotalBoxesOnPallet.Text = PalletInfo.BoxesOnPallet.ToString("n0");
            //numTotalPieces.Text = PalletInfo.totalPiecesOnPalletField.ToString("n3");
            //numTotalBoxesOnPallet.Text = PalletInfo.BoxesOnPallet.ToString("n3");

            tbSearch.Focus();
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

                if (PalletInfo == null || PalletInfo.PalletNum.Length == 0)
                {
                    msg = "No Pallet found.";
                    MessageBox.Show(msg, "Approve Failure [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "");
                    msg = string.Empty;
                }
                else
                {
                    DMExportContract row = new DMExportContract();
                    DMCheckService client = new DMCheckService();

                    row.PalletNum = PalletInfo.PalletNum;
                    row.RecordId = PalletInfo.RecordId;
                    //row.RecordIdSpecified1 = true;
                    row.ItemNumber = lbItemId.Text;
                    row.Grade = cmbGrade.Text;
                    row.Shade = tbShade.Text;
                    row.Caliber = tbCaliber.Text;                    
                    //row.ShiftSpecified1 = false;

                    row.Size = PalletInfo.Size;
                    row.LineOfOrigin = cmbLine.SelectedIndex + 1;
                    //row.LineOfOriginSpecified1 = true;
                    row.WhichMarpak = cmbMarpak.SelectedIndex + 1;
                    //row.WhichMarpakSpecified1 = true;
                    row.LGVOrForklift = cmbLGVorForklift.Text.Equals("LGV") ? PalletTransportBy.LGV : PalletTransportBy.Forklift;
                    //row.LGVOrForkliftSpecified1 = true;
                    //row.TotalSurface = decimal.Parse(numTotalBoxesOnPallet.Text);
                    //row.TotalSurfaceSpecified1 = true;
                    row.IsApprovedBySL = true;
                    //row.IsApprovedBySLSpecified1 = true;
                    row.IsApprovedByFG = false;
                    //row.IsApprovedByFGSpecified1 = false;
                    
                    //row.totalPiecesOnPalletField = Convert.ToInt16(Convert.ToDecimal(numTotalPieces.Text));
                    //row.totalPiecesOnPalletFieldSpecified1 = true;
                    row.BoxesOnPallet = Convert.ToInt16(Convert.ToDecimal(numTotalBoxesOnPallet.Text));
                    //row.BoxesOnPalletSpecified1 = true;
                    row.DeviceName = AppVariables.DeviceName;
                    row.DeviceUser = AppVariables.UpdatedBy;

                    //Now run the command to update Pallet info
                    bool result1, result2;
                    client.UpdateAndConfirmPalletReceive(row, out result1, out result2);

                    if (result1) 
                    {
                        row.ShiftDate = PalletInfo.ShiftDate;
                        row.TimeStamp = PalletInfo.TimeStamp;
                        row.Shift = PalletInfo.Shift;

                        msg = "Pallet '" + PalletInfo.PalletNum + "' has been saved and confirmed.";
                        MessageBox.Show(msg, "Approve [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "PalletNum:" + PalletInfo.PalletNum +
                            ", ItemCode:" + row.ItemNumber +
                            ", RecId:" + PalletInfo.RecordId.ToString() +
                            ", Grade:" + row.Grade +
                            ", Shade:" + row.Shade +
                            ", Caliber:" + row.Caliber +
                            ", TotalBoxes:" + numTotalBoxesOnPallet.Text +
                            ", DeviceName:" + AppVariables.DeviceName +
                            ", DeviceUser:" + AppVariables.UpdatedBy);
                        msg = string.Empty;

                        DBClass db = new DBClass();
                        if (db.GetPallet(row.PalletNum).Rows.Count.Equals(0))
                        {
                            db.InsertPallet(row);
                        }
                        db.UpdatePalletBySL(row, true);

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

                new DBClass().SubmitMessage(msg, "ChangePalletInfo.btnSaveApprove_Click", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {

            }

            tbShade.Focus();
        }

        
        private void btnFind_Click(object sender, EventArgs e)
        {
            int itemLen = 21, lineLen = 2, shadeLen = 3, caliberLen = 2, gradeLen = 2;
            string msg = string.Empty;

            if (string.IsNullOrEmpty(tbSearch.Text.Trim()))
            {
                msg = "Please enter 'Sorting Line Barcode' to search.";
                new DBClass().SubmitMessage(msg, "ChangePalletInfoSL.btnFind_Click", "");
                MessageBox.Show(msg, "Search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                msg = string.Empty;

                tbSearch.Focus();
                return;
            }

            if (tbSearch.Text.Trim().Length >= (itemLen + lineLen + shadeLen + caliberLen + gradeLen))
            {
                string itemIdNew = tbSearch.Text.Substring(0, itemLen).Replace(".", " ").Trim();
                if (lbItemId.Text != itemIdNew)
                {
                    lbProductName.Text = string.Empty;
                    lbSize.Text = string.Empty;
                }
                lbItemId.Text = itemIdNew;
                cmbLine.Text = tbSearch.Text.Substring(itemLen - 1, lineLen).Replace("."," ").Trim();
                tbShade.Text = tbSearch.Text.Substring(itemLen + lineLen, shadeLen).Replace(".", " ").Trim();
                tbCaliber.Text = tbSearch.Text.Substring(itemLen + lineLen + shadeLen, caliberLen).Replace(".", " ").Trim();
                cmbGrade.Text = tbSearch.Text.Substring(itemLen + lineLen + shadeLen + caliberLen, gradeLen).Replace(".", " ").Trim();

                tbSearch.Text = string.Empty;
            }
            else
            {
                msg = "'Sorting Line Barcode' OR it's length is not correct. Please check again.";
                new DBClass().SubmitMessage(msg, "ChangePalletInfoSL.btnFind_Click", "");
                MessageBox.Show(msg, "Search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                msg = string.Empty;

                tbSearch.Focus();
            }
        }

        private void numTotalSurface_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == '.')
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void numTotalSurface_TextChanged(object sender, EventArgs e)
        {
            string msg = "";
            if (decimal.Parse(numTotalBoxesOnPallet.Text) > maxSurface)
            {
                msg = "Value cannot be more than " + maxSurface.ToString();
                new DBClass().SubmitMessage(msg, "ChangePalletInfo.numTotalSurface_TextChanged", "PalletNum:" + PalletInfo.PalletNum + ", TotalBoxes=" + numTotalBoxesOnPallet.Text);

                numTotalBoxesOnPallet.Text = PalletInfo.TotalSurface.ToString("n3");
                MessageBox.Show(msg, "Validate [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
                numTotalBoxesOnPallet.Focus();
            }
        }

        private void tbSearch_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                btnFind_Click(sender, e);
                e.Handled = true;
            }
        }

        
    }
}