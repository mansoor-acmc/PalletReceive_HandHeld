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
    public partial class ItemGroupDetail : Form
    {
        public DMSummaryContract ItemSummary { get; set; }

        public ItemGroupDetail()
        {
            InitializeComponent();
        }

        private void ItemGroupDetail_Load(object sender, EventArgs e)
        {
            lbItemId.Text = ItemSummary.itemNumberField;
            lbProductName.Text = ItemSummary.itemDescriptionField;
            lbGradeShadeCaliber.Text = ItemSummary.gradeField + " / " + ItemSummary.shadeField + " / " + ItemSummary.caliberField;
            lbTotal.Text = "Total rows: " + ItemSummary.numOfPalletsField.ToString();

            DataBind();
        }

        private void DataBind()
        {
            string msg = string.Empty;
            try
            {
                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);
                
                DMCheckService client = new DMCheckService();
                var rows = client.ItemGroupPallets(ItemSummary.itemNumberField,
                    ItemSummary.gradeField,
                    ItemSummary.shadeField,
                    ItemSummary.caliberField);
                lbTotal.Text = "Total rows: " + rows.Count().ToString();

                gridPallets.DataSource = rows;
                this.dataGridTableStyle1.MappingName = gridPallets.DataSource.GetType().Name;
                gridPallets.Refresh();
               
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure || exp.Status == WebExceptionStatus.ProtocolError)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "ItemGroupDetail.DataBind", "");
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

                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "ItemGroupDetail.DataBind", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {
                
            }
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
            DataGrid.HitTestInfo info = gridPallets.HitTest(pt.X, pt.Y);
            if (info.Type == DataGrid.HitTestType.Cell || info.Type == DataGrid.HitTestType.RowHeader)
            {
                if (gridPallets.DataSource != null && info.Row == gridPallets.CurrentRowIndex)
                {
                    OpenItem();
                }
            }
        }

        private void OpenItem()
        {
            List<DMExportContract> rows = ((DMExportContract[])gridPallets.DataSource).ToList();
            DMExportContract row = rows.ElementAt(gridPallets.CurrentRowIndex);
            string msg = string.Empty;

            if (row != null)
            {
                if (row.isPostedField)
                {
                    msg = "Pallet:"+row.palletNumField+" has already used for creating Production Order OR already Transfered to Invertory Journal. \r\nCannot open for editing.";
                    new DBClass().SubmitMessage(msg, "ItemGroupDetail.OpenItem", "");
                    MessageBox.Show(msg, "Open Pallet [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    msg = string.Empty;
                    return;
                }

                bool isReFetchPallet = false;
                if (Char.IsNumber(row.palletNumField, 0))
                    isReFetchPallet = true;

                if (AppVariables.RoleName == RoleType.SortingLine)
                {
                    ChangePalletInfoSL frmChangePallet = new ChangePalletInfoSL();
                    frmChangePallet.PalletInfo = row;
                    frmChangePallet.ShowDialog();
                    row = frmChangePallet.PalletInfo;
                    frmChangePallet.Dispose();
                }
                else
                {
                    ChangePalletInfo frmChangePallet = new ChangePalletInfo();
                    frmChangePallet.PalletInfo = row;
                    frmChangePallet.ShowDialog();
                    row = frmChangePallet.PalletInfo;
                    frmChangePallet.Dispose();
                }
                if (isReFetchPallet)
                {
                    DMCheckService client = new DMCheckService();
                    row = client.GetPalletInfoByRecordId(row.recordIdField, true);

                }
                rows.RemoveAt(gridPallets.CurrentRowIndex);
                rows.Insert(gridPallets.CurrentRowIndex, row);
                gridPallets.DataSource = rows.ToArray();
                gridPallets.Refresh();
                //DataBind();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (gridPallets.DataSource != null && gridPallets.CurrentRowIndex != -1)
            {
                OpenItem();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            try
            {
                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);

                if (gridPallets.DataSource != null && gridPallets.CurrentRowIndex != -1)
                {
                    DMExportContract PalletInfo = ((DMExportContract[])gridPallets.DataSource).ElementAt(gridPallets.CurrentRowIndex);

                    if (PalletInfo == null)
                    {
                        msg = "No Pallet found OR you did not search a Pallet.";
                        new DBClass().SubmitMessage(msg, "ItemGroupDetail.btnPrint_Click", "");
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
                            new DBClass().SubmitMessage(msg, "ItemGroupDetail.btnPrint_Click", "PalletNum:" + PalletInfo.palletNumField + ", RecId:" + PalletInfo.recordIdField.ToString());
                            MessageBox.Show(msg, "Print Again [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            msg = string.Empty;
                        }
                    }
                }
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure || exp.Status == WebExceptionStatus.ProtocolError)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "ItemGroupDetail.btnPrint_Click", "");
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

                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "ItemGroupDetail.btnPrint_Click", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {
                
            }
        }

        

        
    }
}