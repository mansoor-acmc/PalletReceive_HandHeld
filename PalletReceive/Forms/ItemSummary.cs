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
    public partial class ItemSummary : Form
    {
        public string ItemNumber { get; set; }

        public ItemSummary()
        {
            InitializeComponent();
        }

        private void ItemSummary_Load(object sender, EventArgs e)
        {
            tbSearch.Text = ItemNumber;
            DataBind();
        }

        private void DataBind()
        {
            string msg = string.Empty;
            try
            {
                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);
                lbItemName.Text = string.Empty;

                DMCheckService client = new DMCheckService();
                var rows = client.SummaryPallets(ItemNumber);

                gridPallets.DataSource = rows;
                this.dataGridTableStyle1.MappingName = gridPallets.DataSource.GetType().Name;
                gridPallets.Refresh();
                if (rows.Count() > 0)
                    lbItemName.Text = rows[0].itemDescriptionField;
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "ItemSummary.btnFind_Click", "");
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

                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "ItemSummary.btnFind_Click", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {
                tbSearch.Focus();
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
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                btnFind_Click(sender, e);
                e.Handled = true;
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            
            tbSearch.Text = tbSearch.Text.Trim().Replace("\r\n", "");
            
            if (string.IsNullOrEmpty(tbSearch.Text.Trim()))
            {
                msg = "Please enter Item # to search.";
                new DBClass().SubmitMessage(msg, "ItemSummary.btnFind_Click", "");
                MessageBox.Show(msg, "Item Search [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                msg = string.Empty;
                SystemSounds.Question.Play();
                tbSearch.Focus();
                return;
            }

            ItemNumber = tbSearch.Text;
            DataBind();
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
                    DMSummaryContract row=((DMSummaryContract[])gridPallets.DataSource).ElementAt(gridPallets.CurrentRowIndex);
                    
                    if (row != null)
                    {
                        ItemGroupDetail frmItemGroup = new ItemGroupDetail();
                        frmItemGroup.ItemSummary = row;
                        frmItemGroup.ShowDialog();
                        frmItemGroup.Dispose();
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (gridPallets.DataSource != null && gridPallets.CurrentRowIndex != -1)
            {
                DMSummaryContract row = ((DMSummaryContract[])gridPallets.DataSource).ElementAt(gridPallets.CurrentRowIndex);

                if (row != null)
                {
                    ItemGroupDetail frmItemGroup = new ItemGroupDetail();
                    frmItemGroup.ItemSummary = row;
                    frmItemGroup.ShowDialog();
                    frmItemGroup.Dispose();
                }
            }
        }

        

        
    }
}