using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using System.Data;
using PalletReceiveCE7.DMServices;
using PalletReceiveCE7.WebRefDeviceOps;
using System.Net;

namespace PalletReceiveCE7
{
    public class DBClass
    {
        SqlCeConnection _connection;

        public DBClass()
        {
            InitConnection();
        }

        void InitConnection()
        {
            _connection = new SqlCeConnection();
            _connection.ConnectionString = ("Data Source ="
                        + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                        + ("\\PalletReceiveDB.sdf;"
                        + ("Password =" + "\"111111\";"))));
        }

        public bool ShrinkDatabase()
        {
            try
            {
                SqlCeEngine engine = new SqlCeEngine(_connection.ConnectionString);

                engine.Compact(_connection.ConnectionString);
                engine.Shrink();
            }
            catch (Exception exp)
            {
                string msg = exp.Message;
                System.Windows.Forms.MessageBox.Show(msg, "Shrink DB");
                return false;
            }
            finally
            {
                this._connection.Close();
            }
            return true;
        }

        #region Users
        public UserInfo CheckLogin(string userName, string password)
        {
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "SELECT * FROM Users WHERE UserName=@p1 AND Password=@p2;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar);

                cmd.Parameters[0].Value = userName;
                cmd.Parameters[1].Value = password;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                SqlCeDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    UserInfo info = new UserInfo()
                    {
                        UserId = int.Parse(dr["UserId"].ToString()),
                        UserName = dr["UserName"].ToString(),
                        Role = dr["Role"].ToString()
                    };
                    sqlString = "INSERT INTO LoginInfo (Username, LoginDate, DeviceName) Values (@p1, @p2, @p3);";
                    cmd = new SqlCeCommand();
                    cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@p2", SqlDbType.DateTime);
                    cmd.Parameters.Add("@p3", SqlDbType.NVarChar);

                    cmd.Parameters[0].Value = userName;
                    cmd.Parameters[1].Value = DateTime.Now;
                    cmd.Parameters[2].Value = System.Net.Dns.GetHostName();

                    cmd.CommandText = sqlString;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = this._connection;
                    cmd.ExecuteNonQuery();

                    return info;
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }
            return null;
        }

        public DataTable AllUsers()
        {
            DataTable dt = new DataTable("Users");

            string sqlString = "SELECT * FROM Users";

            SqlCeCommand cmd = new SqlCeCommand();
            SqlCeDataAdapter da = new SqlCeDataAdapter(cmd);
            try
            {
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();

                da.Fill(dt);
            }
            finally
            {
                this._connection.Close();
            }

            return dt;
        }

        public bool DeleteUser(int userId)
        {
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "DELETE FROM Users WHERE UserId = @p1;";
            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.Int);
                cmd.Parameters["@p1"].Value = userId;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();

                int rowsEffected = cmd.ExecuteNonQuery();
                if (rowsEffected > 0)
                    return true;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }
            return false;
        }

        public bool ChangeUserInfo(UserInfo user)
        {
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "";
            try
            {
                if (user.UserId.Equals(0))
                {
                    sqlString = "INSERT INTO Users (UserName, Password, Role) VALUES (@p1, @p2, @p3)";
                    cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@p2", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@p3", SqlDbType.NVarChar);

                    cmd.Parameters["@p1"].Value = user.UserName;
                    cmd.Parameters["@p2"].Value = user.Password;
                    cmd.Parameters["@p3"].Value = user.Role;
                }
                else
                {
                    sqlString = "UPDATE Users SET UserName=@p1, Password=@p2, Role=@p3 WHERE UserId=@p4;";
                    cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@p2", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@p3", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@p4", SqlDbType.Int);

                    cmd.Parameters["@p1"].Value = user.UserName;
                    cmd.Parameters["@p2"].Value = user.Password;
                    cmd.Parameters["@p3"].Value = user.Role;
                    cmd.Parameters["@p4"].Value = user.UserId;
                }

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();

                int rowsEffected = cmd.ExecuteNonQuery();
                if (rowsEffected > 0)
                    return true;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }
            return false;
        }
        #endregion
        

        #region Pallet Infos
        public DataTable SearchItems(string search)
        {
            DataTable dt = new DataTable("Items");
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "SELECT TOP (100) * FROM PalletInfo WHERE (IsApprovedByFG = 1 OR DeviceName != '') ";
            if (!string.IsNullOrEmpty(search))
                sqlString += " AND PalletNum LIKE '%" + search + "%'";
            sqlString += " ORDER BY PalletNum;";

            try
            {
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                adapter.Fill(dt);
            }
            finally
            {
                this._connection.Close();
            }
            return dt;
        }

        public DataTable GetPallet(string palletNum)
        {
            DataTable dt = new DataTable("Pallet");
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "SELECT * FROM PalletInfo WHERE PalletNum = @p1;";
            
            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters[0].Value = palletNum;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                adapter.Fill(dt);
            }
            finally
            {
                this._connection.Close();
            }
            return dt;
        }

        public bool DeleteOldPallets()
        {
            DateTime dtOld = DateTime.Now.AddDays(-8);
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "DELETE FROM PalletInfo WHERE ShiftDate < @p1;";
            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.DateTime);
                cmd.Parameters["@p1"].Value = dtOld;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();

                int rowsEffected = cmd.ExecuteNonQuery();
                //if (rowsEffected > 0)
                {
                    sqlString = "DELETE FROM LoginInfo WHERE LoginDate < @p1;";
                    cmd = new SqlCeCommand();
                    cmd.Parameters.Add("@p1", SqlDbType.DateTime);
                    cmd.Parameters["@p1"].Value = dtOld;

                    cmd.CommandText = sqlString;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = this._connection;

                    if (this._connection.State != ConnectionState.Open)
                        this._connection.Open();

                    rowsEffected = cmd.ExecuteNonQuery();
                    //if (rowsEffected > 0)
                    {
                        sqlString = "DELETE FROM DeviceMessage;";
                        cmd = new SqlCeCommand();
                        //cmd.Parameters.Add("@p1", SqlDbType.DateTime);
                        //cmd.Parameters["@p1"].Value = dtOld;

                        cmd.CommandText = sqlString;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = this._connection;

                        if (this._connection.State != ConnectionState.Open)
                            this._connection.Open();

                        rowsEffected = cmd.ExecuteNonQuery();


                        /*sqlString = "DELETE FROM PalletInfo WHERE PalletDateTime < @p1;";
                        cmd = new SqlCeCommand();
                        cmd.Parameters.Add("@p1", SqlDbType.DateTime);
                        cmd.Parameters["@p1"].Value = dtOld;

                        cmd.CommandText = sqlString;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = this._connection;

                        if (this._connection.State != ConnectionState.Open)
                            this._connection.Open();

                        rowsEffected = cmd.ExecuteNonQuery();*/

                        sqlString = "DELETE FROM TransferPallet WHERE IsTransferred = 1 AND DateInsert < @p1;";
                        cmd = new SqlCeCommand();
                        cmd.Parameters.Add("@p1", SqlDbType.DateTime);
                        cmd.Parameters["@p1"].Value = dtOld;

                        cmd.CommandText = sqlString;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = this._connection;

                        if (this._connection.State != ConnectionState.Open)
                            this._connection.Open();

                        rowsEffected = cmd.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show(exp.Message, "Delete DB");
                return false;
            }
            finally
            {
                this._connection.Close();
            }
            //return true;
        }

        public int InsertPallet(DMExportContract dr)
        {            
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "INSERT INTO PalletInfo (PalletNum, Grade, Shade, Caliber, ItemArticle, Shift, ShiftDate, PalletDateTime, "+
                "LineOfOrigin, WhichMarpak, LGVorForklift, Size, RecId, TotalSurface, TotalPiecesOnPallet, TotalBoxesOnPallet) " +
                "VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16);";
            
            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p3", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p4", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p5", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p6", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p7", SqlDbType.DateTime);
                cmd.Parameters.Add("@p8", SqlDbType.DateTime);
                cmd.Parameters.Add("@p9", SqlDbType.TinyInt);
                cmd.Parameters.Add("@p10", SqlDbType.TinyInt);
                cmd.Parameters.Add("@p11", SqlDbType.TinyInt);
                cmd.Parameters.Add("@p12", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p13", SqlDbType.BigInt);
                cmd.Parameters.Add("@p14", SqlDbType.Real);
                cmd.Parameters.Add("@p15", SqlDbType.SmallInt);
                cmd.Parameters.Add("@p16", SqlDbType.TinyInt);

                cmd.Parameters["@p1"].Value = dr.palletNumField;
                cmd.Parameters["@p2"].Value = dr.gradeField;
                cmd.Parameters["@p3"].Value = dr.shadeField;
                cmd.Parameters["@p4"].Value = dr.caliberField;
                cmd.Parameters["@p5"].Value = dr.itemNumberField;
                cmd.Parameters["@p6"].Value = dr.shiftField;
                cmd.Parameters["@p7"].Value = dr.shiftDateField;
                cmd.Parameters["@p8"].Value = dr.timeStampField;
                cmd.Parameters["@p9"].Value = dr.lineOfOriginField;
                cmd.Parameters["@p10"].Value = dr.whichMarpakField;
                cmd.Parameters["@p11"].Value = dr.lGVOrForkliftField;
                cmd.Parameters["@p12"].Value = dr.sizeField;
                cmd.Parameters["@p13"].Value = dr.recordIdField;
                cmd.Parameters["@p14"].Value = dr.totalSurfaceField;
                cmd.Parameters["@p15"].Value = dr.totalPiecesOnPalletField;
                cmd.Parameters["@p16"].Value = dr.boxesOnPalletField;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }
            
        }

        public int UpdatePalletBySL(DMExportContract dr, bool isManualUpdated)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "UPDATE PalletInfo SET DeviceName=@p1, UpdatedBy=@p2, IsApprovedBySL=@p3";
            if (isManualUpdated)
                sqlString += ", IsManualUpdate=@p4";
            sqlString += " WHERE PalletNum = @p5;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p3", SqlDbType.Bit);
                if (isManualUpdated)
                    cmd.Parameters.Add("@p4", SqlDbType.Bit);
                cmd.Parameters.Add("@p5", SqlDbType.NVarChar);

                cmd.Parameters["@p1"].Value = dr.deviceNameField;
                cmd.Parameters["@p2"].Value = dr.deviceUserField;
                cmd.Parameters["@p3"].Value = true;
                if (isManualUpdated)
                    cmd.Parameters["@p4"].Value = true;
                cmd.Parameters["@p5"].Value = dr.palletNumField;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }
        }

        public int UpdatePallet(DMExportContract dr, bool isManualUpdated)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "UPDATE PalletInfo SET DeviceName=@p1, UpdatedBy=@p2 ";
            if (AppVariables.RoleName == RoleType.SortingLine)
                sqlString += ", IsApprovedBySL=@p3";
            else if (AppVariables.RoleName == RoleType.FinishedGoods)
                sqlString += ", IsApprovedByFG=@p3";
            else if (AppVariables.RoleName == RoleType.Admin)
                sqlString += ", IsApprovedBySL=@p3, IsApprovedByFG=@p3";
            if (isManualUpdated)
                sqlString += ", IsManualUpdate=@p4";            
            sqlString += " WHERE PalletNum = @p5;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p3", SqlDbType.Bit);
                if (isManualUpdated)
                    cmd.Parameters.Add("@p4", SqlDbType.Bit);
                cmd.Parameters.Add("@p5", SqlDbType.NVarChar);

                cmd.Parameters["@p1"].Value = dr.deviceNameField;
                cmd.Parameters["@p2"].Value = dr.deviceUserField;
                cmd.Parameters["@p3"].Value = true;
                if (isManualUpdated)
                    cmd.Parameters["@p4"].Value = true;
                cmd.Parameters["@p5"].Value = dr.palletNumField;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }
        }
        #endregion

        #region Offline Transfer

        public DataTable GetRemainingTransfers()
        {
            DataTable dt = new DataTable("Transfers");
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "SELECT PalletNum as Pallet, Location, ID, IsManual, UserName FROM TransferPallet WHERE IsTransferred=0 ORDER BY ID;";

            try
            {
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                adapter.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }

            return dt;
        }

        public DataTable GetTransferPallet(string palletNum, string location)
        {
            DataTable dt = new DataTable("Pallet");
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            //string sqlString = "SELECT TOP (1) * FROM TransferPallet WHERE PalletNum = @p1 AND Location=@p2 ORDER BY DateInsert DESC;";
            string sqlString = "SELECT TOP (1) * FROM TransferPallet WHERE PalletNum = @p1 AND IsTransferred=1 ORDER BY DateInsert DESC;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar, 10);
                //cmd.Parameters.Add("@p2", SqlDbType.NVarChar, 10);

                cmd.Parameters["@p1"].Value = palletNum;
                //cmd.Parameters["@p2"].Value = location;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                adapter.Fill(dt);
            }
            finally
            {
                this._connection.Close();
            }
            return dt;
        }

        public int InsertTransferPallet(string palletNum, string location, Boolean isManual, string userName)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "INSERT INTO TransferPallet (PalletNum, Location, DateInsert, IsManual, UserName) VALUES(@p1, @p2, @p3, @p4, @p5);";
            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add("@p3", SqlDbType.DateTime);
                cmd.Parameters.Add("@p4", SqlDbType.Bit);
                cmd.Parameters.Add("@p5", SqlDbType.NVarChar, 10);


                cmd.Parameters["@p1"].Value = palletNum;
                cmd.Parameters["@p2"].Value = location;
                cmd.Parameters["@p3"].Value = DateTime.Now;
                cmd.Parameters["@p4"].Value = isManual;
                cmd.Parameters["@p5"].Value = userName;


                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }
        }

        public int UpdateTransferPallets(List<LocationHistory> lines)
        {
            int count = 0;
            foreach (var line in lines)
            {
                count += UpdateTransferPallet(line.PalletNum, line.Location, false);
            }
            this._connection.Close();

            return count;
        }

        public int UpdateTransferPallet(string palletNum, string location, bool closeConn)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "UPDATE TransferPallet SET IsTransferred=@p1 WHERE PalletNum=@p2 AND Location=@p3;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.Bit);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar, 10);
                cmd.Parameters.Add("@p3", SqlDbType.NVarChar, 10);

                cmd.Parameters["@p1"].Value = true;
                cmd.Parameters["@p2"].Value = palletNum;
                cmd.Parameters["@p3"].Value = location;


                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (closeConn)
                    this._connection.Close();
            }

        }

        public int DeletePalletForTransfer(string pId)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "DELETE FROM TransferPallet WHERE ID=@p1 AND IsTransferred=@p2;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.BigInt);
                cmd.Parameters.Add("@p2", SqlDbType.Bit);

                cmd.Parameters["@p1"].Value = pId;
                cmd.Parameters["@p2"].Value = false;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }

        }
        #endregion

        #region Offline Mode
        /*public DataTable GetOfflineOnePallet(string palletNum)
        {
            DataTable dt = new DataTable("Pallet");
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "SELECT PalletNum as Pallet, Grade, Shade, Caliber, TotalBoxesOnPallet as Boxes FROM PalletInfo WHERE PalletNum=@p1;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters["@p1"].Value = palletNum;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                adapter.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }

            return dt;
        }
        */
        public DataTable GetOfflinePallets()
        {
            DataTable dt = new DataTable("Pallets");
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "SELECT PalletNum as Pallet, Grade, Shade, Caliber, TotalSurface as Boxes, Location FROM PalletInfo WHERE IsOffline=1 AND RecId=0;";

            try
            {
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                adapter.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }

            return dt;
        }

        public int InsertOfflinePallet(string palletNum)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "INSERT INTO PalletInfo (PalletNum, UpdatedBy, DeviceName, UpdatedDate, IsOffline, ";
            if (AppVariables.RoleName == RoleType.SortingLine)
                sqlString += "IsApprovedBySL,";
            else if (AppVariables.RoleName == RoleType.FinishedGoods)
                sqlString += "IsApprovedByFG,";
            sqlString += " RecId) VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7);";
            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p3", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p4", SqlDbType.DateTime);
                cmd.Parameters.Add("@p5", SqlDbType.Bit);
                cmd.Parameters.Add("@p6", SqlDbType.Bit);
                cmd.Parameters.Add("@p7", SqlDbType.BigInt);

                cmd.Parameters["@p1"].Value = palletNum;
                cmd.Parameters["@p2"].Value = AppVariables.UpdatedBy;
                cmd.Parameters["@p3"].Value = AppVariables.DeviceName;
                cmd.Parameters["@p4"].Value = DateTime.Now;
                cmd.Parameters["@p5"].Value = true;
                cmd.Parameters["@p6"].Value = false;
                cmd.Parameters["@p7"].Value = 0;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }
        }

        public int DeleteOfflinePallet(string palletNum)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "DELETE FROM PalletInfo WHERE PalletNum=@p1 AND IsOffline=@p2;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p2", SqlDbType.Bit);

                cmd.Parameters["@p1"].Value = palletNum;
                cmd.Parameters["@p2"].Value = true;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }

        }

        public int UpdateOfflinePallet(DMExportContract dr)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "UPDATE PalletInfo SET Grade=@p1, Shade=@p2, Caliber=@p3, TotalBoxesOnPallet=@p4, TotalSurface=@p8, UpdatedDate=@p7, Location=@p9 WHERE PalletNum=@p5 AND IsOffline=@p6;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p3", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p4", SqlDbType.TinyInt);
                cmd.Parameters.Add("@p5", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p6", SqlDbType.Bit);
                cmd.Parameters.Add("@p7", SqlDbType.DateTime);
                cmd.Parameters.Add("@p8", SqlDbType.Real);
                cmd.Parameters.Add("@p9", SqlDbType.NVarChar);

                cmd.Parameters["@p1"].Value = dr.gradeField;
                cmd.Parameters["@p2"].Value = dr.shadeField;
                cmd.Parameters["@p3"].Value = dr.caliberField;
                cmd.Parameters["@p4"].Value = dr.boxesOnPalletField;
                cmd.Parameters["@p5"].Value = dr.palletNumField;
                cmd.Parameters["@p6"].Value = true;
                cmd.Parameters["@p7"].Value = DateTime.Now;
                cmd.Parameters["@p8"].Value = dr.totalSurfaceField;
                cmd.Parameters["@p9"].Value = dr.whLocationIdField;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }

        }

        public int UpdateOfflinePalletAfterSync(DMExportContract dr)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "UPDATE PalletInfo SET ItemArticle=@p1, Shift=@p2, ShiftDate=@p3, PalletDateTime=@p4, LineOfOrigin=@p5, " +
                "WhichMarpak=@p6, LGVorForklift=@p7, RecId=@p8, TotalSurface=@p9, Size=@p10, " +
                "Grade=@p13, Shade=@p14, Caliber=@p15 ";
            if (AppVariables.RoleName == RoleType.SortingLine)
                sqlString += ", IsApprovedBySL=@p16";
            else if (AppVariables.RoleName == RoleType.FinishedGoods)
                sqlString += ", IsApprovedByFG=@p16";
            else if (AppVariables.RoleName == RoleType.Admin)
                sqlString += ", IsApprovedBySL=@p16, IsApprovedByFG=@p16";
            sqlString += ", TotalPiecesOnPallet=@p17, TotalBoxesOnPallet=@p18, Location=@p19 WHERE PalletNum=@p11 AND IsOffline=@p12;";

            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p3", SqlDbType.DateTime);
                cmd.Parameters.Add("@p4", SqlDbType.DateTime);
                cmd.Parameters.Add("@p5", SqlDbType.TinyInt);
                cmd.Parameters.Add("@p6", SqlDbType.TinyInt);
                cmd.Parameters.Add("@p7", SqlDbType.TinyInt);
                cmd.Parameters.Add("@p8", SqlDbType.BigInt);
                cmd.Parameters.Add("@p9", SqlDbType.Real);
                cmd.Parameters.Add("@p10", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p11", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p12", SqlDbType.Bit);
                cmd.Parameters.Add("@p13", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p14", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p15", SqlDbType.NVarChar);
                cmd.Parameters.Add("@p16", SqlDbType.Bit);
                cmd.Parameters.Add("@p17", SqlDbType.SmallInt);
                cmd.Parameters.Add("@p18", SqlDbType.TinyInt);
                cmd.Parameters.Add("@p19", SqlDbType.NVarChar);

                cmd.Parameters["@p1"].Value = dr.itemNumberField;
                cmd.Parameters["@p2"].Value = dr.shiftField;
                cmd.Parameters["@p3"].Value = dr.shiftDateField;
                cmd.Parameters["@p4"].Value = dr.timeStampField;
                cmd.Parameters["@p5"].Value = dr.lineOfOriginField;
                cmd.Parameters["@p6"].Value = dr.whichMarpakField;
                cmd.Parameters["@p7"].Value = dr.lGVOrForkliftField;
                cmd.Parameters["@p8"].Value = dr.recordIdField;
                cmd.Parameters["@p9"].Value = dr.totalSurfaceField;
                cmd.Parameters["@p10"].Value = dr.sizeField;
                cmd.Parameters["@p11"].Value = dr.palletNumField;
                cmd.Parameters["@p12"].Value = true;
                cmd.Parameters["@p13"].Value = dr.gradeField;
                cmd.Parameters["@p14"].Value = dr.shadeField;
                cmd.Parameters["@p15"].Value = dr.caliberField;
                cmd.Parameters["@p16"].Value = dr.isApprovedBySLField;
                cmd.Parameters["@p17"].Value = dr.totalPiecesOnPalletField;
                cmd.Parameters["@p18"].Value = dr.boxesOnPalletField;
                cmd.Parameters["@p19"].Value = dr.whLocationIdField;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                adapter.SelectCommand = cmd;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this._connection.Close();
            }

        }

        #endregion

        #region App Settings

        public void GetSettings()
        {
            SqlCeCommand cmd = new SqlCeCommand();


            string sqlString = "SELECT top (1) * FROM Options;";
            try
            {
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();

                SqlCeDataReader result = cmd.ExecuteReader();
                if (result != null && result.Read())
                {
                    Options.NumberOfRowsUpload = short.Parse(result["NumRowsToUpload"].ToString());
                    Options.IsSlApprovalReq = bool.Parse(result["IsSLApprovalRequired"].ToString());
                    Options.IsLocationRequired = bool.Parse(result["IsLocationRequired"].ToString());
                    Options.HasManualEntryAllowed = bool.Parse(result["HasManualEntryAllowed"].ToString());
                }
            }
            finally
            {
                this._connection.Close();
            }
        }

        public bool SaveSettings(int numRow, bool isSlApproveReq, bool isLocationRequired, bool hasManualEntryAllowed)
        {
            SqlCeDataAdapter adapter = new SqlCeDataAdapter();
            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "UPDATE Options SET NumRowsToUpload=@p1, IsSLApprovalRequired=@p2, IsLocationRequired=@p3, HasManualEntryAllowed=@p4;";
            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.TinyInt);
                cmd.Parameters["@p1"].Value = numRow;

                cmd.Parameters.Add("@p2", SqlDbType.Bit);
                cmd.Parameters["@p2"].Value = isSlApproveReq;

                cmd.Parameters.Add("@p3", SqlDbType.Bit);
                cmd.Parameters["@p3"].Value = isLocationRequired;

                cmd.Parameters.Add("@p4", SqlDbType.Bit);
                cmd.Parameters["@p4"].Value = hasManualEntryAllowed;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    return true;
            }
            finally
            {
                this._connection.Close();
            }
            return false;
        }

        #endregion

        #region Device Messages Ops

        public static bool CheckInternet()
        {
            
            bool connected;
            string NetworkDefaultIP = "127.0.0.1";
            try
            {
                IPHostEntry thisHost = Dns.GetHostEntry(AppVariables.DeviceName);
                string localIpAddress = thisHost.AddressList[0].ToString();

                connected = localIpAddress != IPAddress.Parse(NetworkDefaultIP).ToString();
                //connected = Microsoft.WindowsMobile.Status.SystemState.WiFiStateConnected &&
                //    Microsoft.WindowsMobile.Status.SystemState.ConnectionsCount > 0;

                

            }
            catch (Exception)
            {
                connected = false;
            }

            return connected;
        }

        public void SubmitMessage(string message, string methodName, string parameters)
        {
            DeviceMessage msg = new DeviceMessage()
            {
                Message = message,
                MethodName = methodName,
                DeviceName = string.IsNullOrEmpty(AppVariables.DeviceName) ? "" : AppVariables.DeviceName,
                Username = string.IsNullOrEmpty(AppVariables.UpdatedBy) ? "" : AppVariables.UpdatedBy,
                Parameters = parameters,
                DateOccur = DateTime.Now,
                DateOccurSpecified = true,
                DeviceIP = AppVariables.DeviceIP,
                ProjectName = AppVariables.ProjectName,
                IsSavedSpecified = false
            };

            /*if (DBClass.CheckInternet()) //If network is ready.
            {
                DeviceOps client = new DeviceOps();
                client.BeginSaveMessage(msg, new AsyncCallback(DeviceMsgCallback), client);
            }
            else
            {*/
                MessageDevice(msg, true);
            //}
        }

        static void DeviceMsgCallback(IAsyncResult result)
        {
            DeviceOps client = (DeviceOps)result.AsyncState;
            try
            {
                bool isSaveMsgResult = false, isPinged = false;
                if (client != null && result.IsCompleted)
                {
                    client.EndSaveMessage(result, out isSaveMsgResult, out isPinged);
                }
            }
            catch //(Exception exp)
            {
                //new DBClass().SubmitMessage(exp.Message, "DeviceMsgCallback", exp.StackTrace);

                //MessageBox.Show(exp.Message);
            }
        }
        
        public bool MessageDevice(DeviceMessage message, bool closeConn)
        {
            try
            {
                string sqlString = "INSERT INTO DeviceMessage (Username, DeviceName, MsgDate, DeviceIP, Message, MethodName, Parameter, IsSent) " +
                    "VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8);";

                SqlCeCommand cmd = new SqlCeCommand();

                cmd.Parameters.Add("@p1", SqlDbType.NVarChar,50);
                cmd.Parameters.Add("@p2", SqlDbType.NVarChar,50);
                cmd.Parameters.Add("@p3", SqlDbType.DateTime);
                cmd.Parameters.Add("@p4", SqlDbType.NVarChar,30);
                cmd.Parameters.Add("@p5", SqlDbType.NVarChar,1000);
                cmd.Parameters.Add("@p6", SqlDbType.NVarChar,100);
                cmd.Parameters.Add("@p7", SqlDbType.NVarChar, 4000);
                cmd.Parameters.Add("@p8", SqlDbType.Bit);

                cmd.Parameters["@p1"].Value = message.Username;
                cmd.Parameters["@p2"].Value = message.DeviceName;
                cmd.Parameters["@p3"].Value = message.DateOccur;
                cmd.Parameters["@p4"].Value = AppVariables.DeviceIP;
                cmd.Parameters["@p5"].Value = message.Message;
                cmd.Parameters["@p6"].Value = message.MethodName;
                cmd.Parameters["@p7"].Value = message.Parameters;
                cmd.Parameters["@p8"].Value = false;

                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlString;

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                    return true;
            }
            finally
            {
                if (closeConn)
                {
                    if (_connection.State != ConnectionState.Closed)
                        _connection.Close();
                }
            }

            return false;
        }

        public bool SendMessagesAgain()
        {
            List<DeviceMessage> messages = new List<DeviceMessage>();

            SqlCeCommand cmd = new SqlCeCommand();
            string sqlString = "SELECT top (25) * FROM DeviceMessage WHERE IsSent=@p1 ORDER BY MsgDate;";
            try
            {
                cmd.Parameters.Add("@p1", SqlDbType.Bit);
                cmd.Parameters["@p1"].Value = false;

                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = this._connection;

                if (this._connection.State != ConnectionState.Open)
                    this._connection.Open();
                SqlCeDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DeviceMessage message = new DeviceMessage()
                    {
                        DeviceName = dr["DeviceName"].ToString(),
                        Username = dr["UserName"].ToString(),
                        DateOccur = DateTime.Parse(dr["MsgDate"].ToString()),
                        DateOccurSpecified = true,
                        Message = dr["Message"].ToString(),
                        MethodName = dr["MethodName"].ToString(),
                        DeviceIP = AppVariables.DeviceIP,
                        ProjectName = AppVariables.ProjectName,
                        Parameters = dr["Parameter"].ToString(),
                        ID = long.Parse(dr["ID"].ToString()),
                        IDSpecified = true
                    };
                    messages.Add(message);
                }
            }
            finally
            {
                this._connection.Close();
            }

            if (messages.Count > 0)
            {
                DeviceOps client = new DeviceOps();
                client.BeginSaveMessages(messages.ToArray(), new AsyncCallback(DeviceMessagesCallback), client);
            }
            return true;
        }
        static void DeviceMessagesCallback(IAsyncResult result)
        {
            DeviceOps client = (DeviceOps)result.AsyncState;
            try
            {
                if (client != null && result.IsCompleted)
                {
                    var messages = client.EndSaveMessages(result);
                    if (messages != null && messages.Count() > 0)
                    {
                        DBClass db = new DBClass();
                        foreach (DeviceMessage message in messages)
                        {
                            db.UpdateMessage(message, false);
                        }
                    }

                }
            }
            catch (Exception exp)
            {
                new DBClass().SubmitMessage(exp.Message, "DeviceMessagesCallback", exp.StackTrace);

                //MessageBox.Show(exp.Message);
            }
        }

        public bool UpdateMessage(DeviceMessage message, bool closeConn)
        {
            try
            {
                string sqlString = "UPDATE DeviceMessage SET IsSent=@p1 WHERE ID=@p2;";
                SqlCeCommand cmd = new SqlCeCommand();

                cmd.Parameters.Add("@p1", SqlDbType.Bit);
                cmd.Parameters.Add("@p2", SqlDbType.BigInt);

                cmd.Parameters["@p1"].Value = true;
                cmd.Parameters["@p2"].Value = message.ID;

                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlString;

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                    return true;
            }
            finally
            {
                if (closeConn)
                {
                    if (_connection.State != ConnectionState.Closed)
                        _connection.Close();
                }
            }

            return false;
        }

        #endregion
    }
}
