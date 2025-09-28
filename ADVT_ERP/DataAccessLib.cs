using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace ADVT_ERP
{
    class DataAccessLib
    {        
        String ConnStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection dbcon = new SqlConnection();

        //Constructor Function
        public DataAccessLib()
        {
            dbcon = new SqlConnection(ConnStr);
        }

        //Check Connection Status
        public bool check_connection()
        {
            bool returnval = false;

            try
            {
                dbcon.Open();
                returnval = true;
                dbcon.Close();
            }
            catch (Exception ex)
            {
                returnval = false;
            }

            return returnval;
        }

        public DataTable GetDataTable(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                dbcon.Open();
                SqlCommand readCommand = new SqlCommand(query, dbcon);
                SqlDataAdapter da = new SqlDataAdapter(readCommand);
                da.Fill(dt);
                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return dt;
        }

        public int InsertRow(string table, List<List<string>> col_val)
        {
            int returnval = 0;

            try
            {
                string query = "insert into " + table + "(";
                string query_val = " values(";
                int cnt = 0;

                foreach (var item in col_val)
                {
                    if (cnt > 0)
                    {
                        query += ",";
                        query_val += ",";
                    }
                    

                    query += item[0];

                    if (item[2] == "dat")
                    {
                        query_val += "'" + item[1] + "'";
                    }
                    if (item[2] == "num")
                    {
                        query_val += item[1];
                    }
                    else
                    {
                        query_val += "'" + item[1] + "'";
                    }

                    cnt++;
                } 

                query += ")" + query_val + ")";

                SqlCommand Command = new SqlCommand(query, dbcon);
                dbcon.Open();
                returnval = Command.ExecuteNonQuery();
                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return returnval;
        }

        public int InsertSingleRow(string table, IDictionary<string, string> col_val)
        {
            int returnval = 0;

            try
            {                
                string query = "insert into "+table+"(";
                string query_val = " values(";
                int cnt = 0;
                

                foreach (KeyValuePair<string, string> entry in col_val)
                {
                    if(cnt > 0)
                    {
                        query += ",";
                        query_val += ",";
                    }

                    query += entry.Key;
                    query_val += "'"+entry.Value+"'";
                    cnt++;
                }

                query += ")" + query_val + ")";

                SqlCommand Command = new SqlCommand(query, dbcon);
                dbcon.Open();
                returnval = Command.ExecuteNonQuery();
                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return returnval;
        }

        ///Update Records
        public int UpdateRecords(string table, IDictionary<string, string> col_val, List<String> whr_cls)
        {
            int returnval = 0;

            try
            {
                string query = "update " + table + " set ";
                int cnt = 0;

                foreach (KeyValuePair<string, string> entry in col_val)
                {
                    if (cnt > 0)
                    {
                        query += ",";
                    }

                    query += entry.Key + "='" + entry.Value + "'";
                    cnt++;
                }

                if (whr_cls.Count > 0)
                {
                    query += " where ";
                    cnt = 0;

                    foreach (var itm in whr_cls)
                    {
                        cnt++;

                        if (cnt == 4)
                        {
                            query += " and ";
                            cnt = 1;
                        }

                        if (cnt == 1 || cnt == 2)
                        {
                            query += itm;
                        }
                        else if (cnt == 3)
                        {
                            query += "'" + itm + "'";
                        }
                    }
                }

                SqlCommand Command = new SqlCommand(query, dbcon);
                dbcon.Open();
                returnval = Command.ExecuteNonQuery();
                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return returnval;
        }

        ///////////////////////
        
        public IDictionary<int, string> get_district_list(string state_selected = "")
        {
            IDictionary<int, string> district_list = new Dictionary<int, string>();

            district_list.Add(0, "Select District");

            string query = @"select top 150 Id, DistrictName from Master_District where IsActive = 'A' order by DistrictName";

            try
            {
                dbcon.Open();
                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        district_list.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return district_list;
        }


        ///////////////////////

        public IDictionary<int, string> get_location_list(string DistrictId = "0", string state_selected = "")
        {
            IDictionary<int, string> location_list = new Dictionary<int, string>();

            location_list.Add(0, "Select Location");

            string query = @"select top 500 Id,LocationName from Master_Location where DistrictId = '" + DistrictId + "' and IsActive = 'A' order by LocationName";

            try
            {
                dbcon.Open();
                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        location_list.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return location_list;
        }

        ///////////
        ///get single row
        public string get_configuration(string configkey)
        {
            string configval = "";

            string query = @"select top 1 * from Configuration where ConfigKey = '"+ configkey + "'";

            try
            {
                dbcon.Open();

                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        configval =  reader.GetString(2);
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return configval;
        }

        ////
        public int set_configuration(string key, string value)
        {
            int result = 0;

            string query = @"update Configuration set ConfigValue = '"+ value + "' where ConfigKey = '" + key + "'";

            try
            {
                SqlCommand Command = new SqlCommand(query, dbcon);
                dbcon.Open();
                result = Command.ExecuteNonQuery();
                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }            

            return result;
        }

        ////
        public int get_last_invoice_id()
        {
            int invoice_id = 0, monthday = 0;
            int year = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            int month = Convert.ToInt32(DateTime.Now.ToString("MM"));
            if (month == 6 || month == 11 || month == 9 || month == 4) { monthday = 30; }
            else if (month == 2) { if (year % 4 == 0) { monthday = 29; } else { monthday = 28; } }
            else { monthday = 31; }
            string fromdate = Convert.ToDateTime("01-" + month.ToString() + "-" + year.ToString()).ToString("yyyy-MM-dd");
            string todate = Convert.ToDateTime(monthday.ToString() + "-" + month.ToString() + "-" + year.ToString()).ToString("yyyy-MM-dd");

            string query = @"select count(*) from Invoice where InvoiceDate between '" + fromdate + "' and '" + todate + "'";

            try
            {
                dbcon.Open();

                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try { invoice_id = reader.GetInt32(0); } catch { }
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return invoice_id;
        }

        //////

        public int get_last_receipt_id()
        {
            int receipt_id = 0;

            string query = @"select max(id) from PaymentReceipt";

            try
            {
                dbcon.Open();

                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try { receipt_id = reader.GetInt32(0); } catch { }
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return receipt_id;
        }

        //////

        public int get_last_cr_note_id()
        {
            int id = 0;

            string query = @"select max(id) from CreditNote";

            try
            {
                dbcon.Open();

                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try { id = reader.GetInt32(0); } catch { }
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return id;
        }


        public int get_last_dr_note_id()
        {
            int id = 0;

            string query = @"select max(id) from DebitNote";

            try
            {
                dbcon.Open();

                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try { id = reader.GetInt32(0); } catch { }
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return id;
        }

        /////////////

        public IDictionary<int, string> get_agency_list(string LocationId = "0", string AgencyType = "", string state_selected = "")
        {
            IDictionary<int, string> agency_list = new Dictionary<int, string>();

            agency_list.Add(0, "Select Agency");

            string query = @"select top 500 Id, AgencyName from Master_Agency where LocationId = '" + LocationId + "' and IsActive = 'A' ";

            if(AgencyType != "")
            {
                query += " and AgencyType = '" + AgencyType + "'";
            }

            query += " order by AgencyName";

            try
            {
                dbcon.Open();
                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        agency_list.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return agency_list;
        }

        ////

        public IDictionary<int, string> get_executive_list(string state_selected = "")
        {
            IDictionary<int, string> executive_list = new Dictionary<int, string>();

            executive_list.Add(0, "Select Executive");

            string query = @"select top 500 Id, EmpId, EmpName from Master_Executive where IsActive = 'A' order by EmpId, EmpName";

            try
            {
                dbcon.Open();
                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        executive_list.Add(reader.GetInt32(0), reader.GetString(1) +" - "+ reader.GetString(2));
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return executive_list;
        }


        public IDictionary<string, string> get_invoice_detail(string InvoiceNo)
        {
            IDictionary<string, string> InvoiceDetail = new Dictionary<string, string>();

            string qry = @"SELECT top 1 iv.InvoiceNo, iv.PublishDate, iv.InvoiceDate, iv.RONum, lc.LocationName, ag.AgencyName, iv.ClientName, iv.AdHeight, iv.AdWidth, iv.AdSpace, iv.Rate, iv.AgreedAmount, iv.CGST, iv.SGST, iv.NetAmount, ag.GSTN as AgencyGSTN, iv.Remark, iv.Amount, ISNULL(iv.PackageName,'') as PackageName, ISNULL(iv.ColourScheme, '') as ColourScheme from Invoice iv left join Master_Agency ag on iv.AgencyId = ag.Id left join Master_Location lc on iv.LocationId = lc.Id where iv.InvoiceNo = '" + InvoiceNo +"'";

            try
            {
                dbcon.Open();

                SqlCommand readCommand = new SqlCommand(qry, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        InvoiceDetail.Add("InvoiceNo", reader.GetString(0));
                        InvoiceDetail.Add("PublishDate", reader.GetDateTime(1).ToString("dd/MM/yyyy"));
                        InvoiceDetail.Add("InvoiceDate", reader.GetDateTime(2).ToString("dd/MM/yyyy"));
                        InvoiceDetail.Add("RONum", reader.GetString(3));
                        InvoiceDetail.Add("LocationName", reader.GetString(4));
                        InvoiceDetail.Add("AgencyName", reader.GetString(5));
                        InvoiceDetail.Add("ClientName", reader.GetString(6));
                        InvoiceDetail.Add("AdHeight", reader.GetDouble(7).ToString());
                        InvoiceDetail.Add("AdWidth", reader.GetDouble(8).ToString());
                        InvoiceDetail.Add("AdSpace", reader.GetDouble(9).ToString());
                        InvoiceDetail.Add("Rate", reader.GetDouble(10).ToString("0.00"));
                        InvoiceDetail.Add("AgreedAmount", reader.GetDouble(11).ToString("0.00"));
                        InvoiceDetail.Add("CGST", reader.GetDouble(12).ToString("0.00"));
                        InvoiceDetail.Add("SGST", reader.GetDouble(13).ToString("0.00"));                        
                        InvoiceDetail.Add("NetAmount", reader.GetDouble(14).ToString("0.00"));
                        InvoiceDetail.Add("AgencyGSTN", reader.GetString(15));
                        InvoiceDetail.Add("Remark", reader.GetString(16));
                        InvoiceDetail.Add("Amount", reader.GetDouble(17).ToString("0.00"));
                        InvoiceDetail.Add("PackageName", reader.GetString(18));
                        InvoiceDetail.Add("ColourScheme", reader.GetString(19));
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return InvoiceDetail;
        }

        //////

        public IDictionary<string, string> get_recipt_detail(string ReceiptNo)
        {
            IDictionary<string, string> RecieptDetail = new Dictionary<string, string>();

            string qry = @"SELECT top 1 rcv.ReceiptNo, rcv.ReceiptDate, rcv.ClientName, rcv.PymentMode, rcv.PymentStatus, rcv.ChequeNoTxnId, rcv.ChequeTxnDate, rcv.ChequeTxnBank, rcv.Amount, rcv.IsCanceled, rcv.IsSettled, lc.LocationName, ag.AgencyName, rcv.AgencyId, rcv.LocationId, rcv.DistrictId from PaymentReceipt rcv left join Master_Agency ag on rcv.AgencyId = ag.Id left join Master_Location lc on rcv.LocationId = lc.Id where rcv.ReceiptNo = '" + ReceiptNo + "'";

            try
            {
                dbcon.Open();

                SqlCommand readCommand = new SqlCommand(qry, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecieptDetail.Add("ReceiptNo", reader.GetString(0));
                        RecieptDetail.Add("ReceiptDate", reader.GetDateTime(1).ToString("dd/MM/yyyy"));
                        RecieptDetail.Add("ClientName", reader.GetString(2));
                        RecieptDetail.Add("PymentMode", reader.GetString(3));
                        RecieptDetail.Add("PymentStatus", reader.GetString(4));
                        RecieptDetail.Add("ChequeNoTxnId", reader.GetString(5));                        
                        RecieptDetail.Add("ChequeTxnDate", reader.GetDateTime(6).ToString("dd/MM/yyyy"));
                        RecieptDetail.Add("ChequeTxnBank", reader.GetString(7).ToString());
                        RecieptDetail.Add("Amount", reader.GetDouble(8).ToString("0.00"));
                        RecieptDetail.Add("IsCanceled", reader.GetString(9));
                        RecieptDetail.Add("IsSettled", reader.GetString(10));
                        RecieptDetail.Add("LocationName", reader.GetString(11));
                        RecieptDetail.Add("AgencyName", reader.GetString(12));
                        RecieptDetail.Add("AgencyId", reader.GetInt32(13).ToString());
                        RecieptDetail.Add("LocationId", reader.GetInt32(14).ToString());
                        RecieptDetail.Add("DistrictId", reader.GetInt32(15).ToString());
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return RecieptDetail;
        }

        /////

        public IDictionary<int, string> get_cr_note_purpose_list(string selected = "")
        {
            IDictionary<int, string> purpose_list = new Dictionary<int, string>();

            purpose_list.Add(0, "Select Purpose");

            string query = @"select top 150 Id, Reson from OptionMaster where ResonFor = 'CrNote' and IsActive = 'A' order by Reson";

            try
            {
                dbcon.Open();
                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        purpose_list.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return purpose_list;
        }

        public IDictionary<string, string> get_user_detail(string userid, string password)
        {
            IDictionary<string, string> userdetail = new Dictionary<string, string>();

            string query = @"select top 1 UserId, Password, UserType, UserName, IsActive from MasterUsers where UserId = '"+ userid + "' and Password = '"+ password + "' and IsActive = 'A'";

            try
            {
                dbcon.Open();
                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userdetail.Add("UserId", reader.GetString(0));
                        userdetail.Add("Password", reader.GetString(1));
                        userdetail.Add("UserType", reader.GetString(2));
                        userdetail.Add("UserName", reader.GetString(3));
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return userdetail;
        }

        public IDictionary<int, string> get_dr_note_purpose_list(string selected = "")
        {
            IDictionary<int, string> purpose_list = new Dictionary<int, string>();

            purpose_list.Add(0, "Select Purpose");

            string query = @"select top 150 Id, Reson from OptionMaster where ResonFor = 'DrNote' and IsActive = 'A' order by Reson";

            try
            {
                dbcon.Open();
                SqlCommand readCommand = new SqlCommand(query, dbcon);

                using (SqlDataReader reader = readCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        purpose_list.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                }

                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return purpose_list;
        }

        /////
        ///
        public int cancel_invoice(string InvoiceNo, string CancelRemark)
        {
            int rtn = 0;

            try { 

                string query = @"update Invoice set IsCanceled = 'Y', CancelRemark = '"+ CancelRemark + "', CanceledOn = getdate() where InvoiceNo = '" + InvoiceNo + "'";

                SqlCommand Command = new SqlCommand(query, dbcon);
                dbcon.Open();
                rtn = Command.ExecuteNonQuery();
                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return rtn;
        }


        ////

        public int debit_note_for_payment_receipt(string ReceiptNo, int DebitNoteId, string CancelRemark)
        {
            int rtn = 0;

            try
            {

                string query = @"update PaymentReceipt set IsCanceled = 'Y', CancelRemark = '" + CancelRemark + "', CanceledOn = getdate(), DebitNoteId = '"+ DebitNoteId + "' where ReceiptNo = '" + ReceiptNo + "'";

                SqlCommand Command = new SqlCommand(query, dbcon);
                dbcon.Open();
                rtn = Command.ExecuteNonQuery();
                dbcon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return rtn;
        }

        /////
        
        public void BackupDatabase()
        {
            try
            {
                string sqlStmt = string.Format("backup database [" + System.Windows.Forms.Application.StartupPath + "\\advtData.mdf] to disk='{0}'", System.Windows.Forms.Application.StartupPath + "\\db_copy\\advtData.bak");
                using (SqlCommand bu2 = new SqlCommand(sqlStmt, dbcon))
                {
                    dbcon.Open();
                    bu2.ExecuteNonQuery();
                    dbcon.Close();

                    MessageBox.Show("Backup Created Sucessfully");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        ////////////////////////////////////
    }
}
