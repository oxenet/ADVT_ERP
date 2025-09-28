using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for AgencyMaster.xaml
    /// </summary>
    public partial class AgencyLedger : UserControl
    {
        private static bool firstrun = true;
        private string usertype = "";
        public AgencyLedger(string usertype)
        {
            firstrun = true;
            InitializeComponent();
            firstrun = false;
            this.usertype = usertype;
            load_district_list();
            refreshGrid();
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string fromdate = "";
            string todate = "";
            string agency = "";
            try
            {
                todate = Convert.ToDateTime(dpkrInvoiceDateTo.Text).ToString("yyyy-MM-dd");
                fromdate = Convert.ToDateTime(dpkrInvoiceDateFrom.Text).ToString("yyyy-MM-dd");
                agency = drpAgencyList.SelectedValue.ToString();
            }
            catch { }

            refreshGrid(fromdate, todate, agency);
        }

        public string build_query(string fromdate = "", string todate = "", string agency = "")
        {
            string qry = @"select DocDate, ClientName, Mode, DocType, DocNo, DrAmount, CrAmount, SUM(DrAmount - CrAmount) OVER (ORDER BY DocDate ROWS UNBOUNDED PRECEDING) AS Balance  from (";

            qry += "select FORMAT(iv.InvoiceDate,'dd/MM/yyyy') as DocDate, iv.ClientName, '' as Mode, 'Bill' as DocType, iv.InvoiceNo as DocNo, iv.NetAmount as DrAmount, 0 as CrAmount from Invoice iv where iv.InvoiceDate between '" + fromdate + "' and '" + todate + "' and iv.AgencyId = '" + agency + "'";

            qry += " union all ";

            qry += "select FORMAT(pr.ReceiptDate,'dd/MM/yyyy') as DocDate, pr.ClientName, pr.PymentMode as Mode, 'RCR' as DocType, pr.ReceiptNo as DocNo, 0 as DrAmount, pr.Amount as CrAmount from PaymentReceipt pr where pr.ReceiptDate between '" + fromdate + "' and '" + todate + "'  and pr.AgencyId = '" + agency + "'";

            qry += ") rpt  order by rpt.DocDate Asc";

            return qry;
        }

        public void refreshGrid(string fromdate = "", string todate = "", string agency = "")
        {
            if (fromdate.Length > 4 && todate.Length > 4 && agency != "")
            {
                string qry = build_query(fromdate, todate, agency);

                try
                {
                    var dbo = new DataAccessLib();
                    DataTable data = dbo.GetDataTable(qry);
                    grdInvoiceList.ItemsSource = data.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public void load_agency()
        {
            if (firstrun == false)
            {
                string location = "0";

                try { location = drpLocationList.SelectedValue.ToString(); } catch { }

                string agency_type = "";

                if (rdoAgencyType_G.IsChecked == true)
                {
                    agency_type = "GOVT";
                }
                else if (rdoAgencyType_C.IsChecked == true)
                {
                    agency_type = "COMM";
                }

                if (location != "0" && location != "" && agency_type != "")
                {
                    var dbo = new DataAccessLib();
                    IDictionary<int, string> agency_list = new Dictionary<int, string>();
                    agency_list = dbo.get_agency_list(location, agency_type);
                    drpAgencyList.ItemsSource = agency_list;
                    drpAgencyList.DisplayMemberPath = "Value";
                    drpAgencyList.SelectedValuePath = "Key";
                    drpAgencyList.SelectedIndex = 0;
                }
            }
        }

        public void load_district_list()
        {
            var dbo = new DataAccessLib();
            IDictionary<int, string> district_list = new Dictionary<int, string>();
            district_list = dbo.get_district_list();
            drpDistrictList.ItemsSource = district_list;
            drpDistrictList.DisplayMemberPath = "Value";
            drpDistrictList.SelectedValuePath = "Key";
            drpDistrictList.SelectedIndex = 0;
        }

        public void load_location_list(string districtid)
        {
            var dbo = new DataAccessLib();
            IDictionary<int, string> location_list = new Dictionary<int, string>();
            location_list = dbo.get_location_list(districtid);
            drpLocationList.ItemsSource = location_list;
            drpLocationList.DisplayMemberPath = "Value";
            drpLocationList.SelectedValuePath = "Key";
            drpLocationList.SelectedIndex = 0;
        }

        private void drpDistrictList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string DistrictId = drpDistrictList.SelectedValue.ToString();
                load_location_list(DistrictId);
            }
            catch { }
        }

        private void drpLocationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            load_agency();
        }

        private void rdoAgencyType_C_Checked(object sender, RoutedEventArgs e)
        {
            load_agency();
        }

        private void rdoAgencyType_G_Checked(object sender, RoutedEventArgs e)
        {
            load_agency();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string InvoiceNo = "";
            try
            {
                foreach (DataRowView row in grdInvoiceList.SelectedItems)
                {
                    DataRow MyRow = row.Row;
                    InvoiceNo = MyRow["InvoiceNo"].ToString();

                }

                var print = new ViewInvoice(InvoiceNo);

                print.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {

                DataTable dataTable = new DataTable();

                string fromdate = "";
                string todate = "";
                string agency = "";
                try
                {
                    todate = Convert.ToDateTime(dpkrInvoiceDateTo.Text).ToString("yyyy-MM-dd");
                    fromdate = Convert.ToDateTime(dpkrInvoiceDateFrom.Text).ToString("yyyy-MM-dd");
                    agency = drpAgencyList.SelectedValue.ToString();
                }
                catch { }

                string qry = build_query(fromdate, todate, agency);

                try
                {
                    var dbo = new DataAccessLib();
                    dataTable = dbo.GetDataTable(qry);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                StringBuilder csv = new StringBuilder();

                // Include column headers
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    csv.Append(dataTable.Columns[i].ColumnName.Replace(',', ':'));
                    if (i < dataTable.Columns.Count - 1)
                    {
                        csv.Append(",");
                    }
                }
                csv.AppendLine();

                // Include data rows
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        csv.Append(row[i].ToString().Replace(",", ";")); // simple handling of comma within text
                        if (i < dataTable.Columns.Count - 1)
                        {
                            csv.Append(",");
                        }
                    }
                    csv.AppendLine();
                }

                // Save csv to selected path
                File.WriteAllText(saveFileDialog.FileName, csv.ToString());
            }
        }

    }
}
