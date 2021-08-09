using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using PalletReceiveCE7.WebRefDeviceOps;
using System.Net;
using System.Net.Sockets;
using PalletReceiveCE7.DMServices;

namespace PalletReceiveCE7
{
    public partial class Login : Form
    {
        public Home HomeForm { get; set; }
        Timer timePing = new Timer();

        int timeInterval = 60000;   //after 60 seconds

        public Login()
        {
            InitializeComponent();

            
                //first shrink the db
            if (new DBClass().DeleteOldPallets())
                new DBClass().ShrinkDatabase();
           

            timePing.Interval = timeInterval;
            timePing.Tick += new EventHandler(timePing_Tick);
        }

        private void Login_Load(object sender, EventArgs e)
        {
            string msg = string.Empty;
            //lbAppVersion.Text = AppVariables.VersionNumber;
            lbLinkAppVersion.Text = AppVariables.VersionNumber;
            AppVariables.DeviceName = System.Net.Dns.GetHostName();
            //AppVariables.DefaultLocation = "A2";
            AppVariables.WarehouseLocations = new List<WmsLocationContract>();

            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        AppVariables.DeviceIP = ip.ToString();
                        break;
                    }
                }

                if (!DBClass.CheckInternet())
                    throw new Exception(AppVariables.NetworkDown);

                DMCheckService client = new DMCheckService();
                string getPing = client.GetPing();                          
                AppVariables.WarehouseLocations = client.GetWHLocations().ToList();
                client.Dispose(); 
            }
            catch (WebException exp)
            {
                if (exp.Status == WebExceptionStatus.ConnectFailure || exp.Status == WebExceptionStatus.ProtocolError)
                {
                    msg = "Unable to connect to Dynamics AX. Please contact Network administrator.";
                    new DBClass().SubmitMessage(msg, "Login.Login_Load", "");
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
                new DBClass().SubmitMessage(msg + " [" + exp.Message + "]", "Login.Login_Load", "");
                MessageBox.Show(msg, "Error [" + AppVariables.DeviceName + "]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                msg = string.Empty;
            }
            finally
            {
                new DBClass().GetSettings();
                timePing.Enabled = true;
                tbUsername.Focus();
            }
        }
        
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbUsername.Text.Trim()))
            {
                MessageBox.Show("Please enter 'Username' to continue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                tbUsername.Focus();
                SystemSounds.Question.Play();
            }
            else
            {
                UserInfo userInfo = new DBClass().CheckLogin(tbUsername.Text.Trim(), tbPassword.Text.Trim());
                if (userInfo != null)
                {
                    this.SuspendLayout();

                    SystemSounds.Beep.Play();

                    AppVariables.UpdatedBy = userInfo.UserName;
                    if (userInfo.Role == "SortingLine")
                        AppVariables.RoleName = RoleType.SortingLine;
                    else if (userInfo.Role == "FinishedGoods")
                        AppVariables.RoleName = RoleType.FinishedGoods;
                    else if (userInfo.Role == "Admin")
                        AppVariables.RoleName = RoleType.Admin;

                    if (AppVariables.WarehouseLocations != null && AppVariables.WarehouseLocations.Count().Equals(0))
                    {
                        try
                        {
                            DMCheckService dmCheck = new DMCheckService();
                            AppVariables.WarehouseLocations = dmCheck.GetWHLocations().ToList();
                        }
                        catch { }
                    }

                    
                    SystemSounds.Beep.Play();

                    Home homeScreen = null;
                    if (HomeForm != null)
                        homeScreen = HomeForm;
                    else
                        homeScreen = new Home();

                    homeScreen.LoginForm = this;
                    homeScreen.Show();

                    this.Hide();
                    this.ResumeLayout();
                }
                else
                {
                    MessageBox.Show("Login failed. Please enter correct username and/or password.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    tbUsername.Focus();
                    SystemSounds.Question.Play();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {            
            Application.Exit();       
        }

        void timePing_Tick(object sender, EventArgs e)
        {
            try
            {
                if (DBClass.CheckInternet()) //If network is ready.
                {
                    string userName = string.IsNullOrEmpty(AppVariables.UpdatedBy) ? string.Empty : AppVariables.UpdatedBy;
                    DeviceOps client = new DeviceOps();
                    client.BeginPing(new DeviceMessage()
                    {
                        Message = "Ping",
                        DeviceIP = AppVariables.DeviceIP,
                        ProjectName = AppVariables.ProjectName,
                        Username = userName,
                        DeviceName = AppVariables.DeviceName,
                        DateOccur = DateTime.Now,
                        DateOccurSpecified = true
                    }, new AsyncCallback(PingCallback), client);

                    new DBClass().SendMessagesAgain();


                }
            }
            catch { }
        }
        static void PingCallback(IAsyncResult result)
        {
            try
            {
                DeviceOps client = (DeviceOps)result.AsyncState;
                bool isPinged = false;
                if (result.IsCompleted)
                    client.EndPing(result, out isPinged, out isPinged);
            }
            catch //(Exception exp)
            {
                //new DBClass().SubmitMessage(exp.Message, "PingCallback", exp.StackTrace);

                //MessageBox.Show(exp.Message);
            }
        }
        

        private void Login_Activated(object sender, EventArgs e)
        {
            tbPassword.Text = string.Empty;
            tbUsername.Focus();
        }

        private void tbUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                tbPassword.Focus();
            }
        }

        private void tbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void lbLinkAppVersion_Click(object sender, EventArgs e)
        {
            new Forms.About().Show();
        }
        
    }
}