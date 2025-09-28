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
    public partial class BillReport : UserControl
    {
        private static bool firstrun = true;
        private string usertype = "";
        public BillReport(string usertype)
        {
            firstrun = true;
            InitializeComponent();
            firstrun = false;
            this.usertype = usertype;
            load_district_list();
            load_executive_list();
            refreshGrid();
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string fromdate = "";
            string todate = "";
            string district = "";
            string location = "";
            string agency = "";
            string executive = "";
            try
            {
                todate = Convert.ToDateTime(dpkrInvoiceDateTo.Text).ToString("yyyy-MM-dd");
                fromdate = Convert.ToDateTime(dpkrInvoiceDateFrom.Text).ToString("yyyy-MM-dd");
                district = drpDistrictList.SelectedValue.ToString();
                location = drpLocationList.SelectedValue.ToString();
                agency = drpAgencyList.SelectedValue.ToString();
                executive = drpExecutiveList.SelectedValue.ToString();
            }
            catch { }

            refreshGrid(fromdate, todate, executive, district, location, agency);
        }

        public void refreshGrid(string fromdate = "", string todate = "", string executive = "", string district = "", string location = "", string agency = "")
        {
            string qry = @"SELECT top 2000 iv.InvoiceNo, FORMAT(iv.PublishDate, 'dd/MM/yyyy') as PublishDate, FORMAT(iv.InvoiceDate,'dd/MM/yyyy') as InvoiceDate, iv.RONum, lc.LocationName, ag.AgencyName, iv.ClientName, iv.ExecutiveId, iv.AdHeight, iv.AdWidth, iv.AdSpace, iv.Rate, iv.AgreedAmount, iv.CGST, iv.SGST, iv.NetAmount, case when iv.PaymentStatus = 'P' then 'Partial' else case when iv.PaymentStatus = 'F' then 'Paid' else 'Un-Paid' end end as PayStatus from Invoice iv left join Master_Agency ag on iv.AgencyId = ag.Id left join Master_Location lc on iv.LocationId = lc.Id where iv.IsCanceled = 'N' ";

            if(fromdate.Length > 2 && todate.Length > 2)
            {
                qry += " and iv.InvoiceDate between '"+ fromdate + "' and '"+ todate +"' ";
            }

            if(executive != "" && executive != "0")
            {
                qry += " and iv.ExecutiveId = '"+ executive + "'";
            }

            if(district != "" && district != "0")
            {
                qry += " and iv.DistrictId = '" + district + "'";
            }

            if (location != "" && location != "0")
            {
                qry += " and iv.LocationId = '" + location + "'";
            }

            if (agency != "" && agency != "0")
            {
                qry += " and iv.AgencyId = '" + agency + "'";
            }

            qry += " order by iv.Id Desc";

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

        public void load_executive_list()
        {
            var dbo = new DataAccessLib();
            IDictionary<int, string> executive_list = new Dictionary<int, string>();
            executive_list = dbo.get_executive_list();
            drpExecutiveList.ItemsSource = executive_list;
            drpExecutiveList.DisplayMemberPath = "Value";
            drpExecutiveList.SelectedValuePath = "Key";
            drpExecutiveList.SelectedIndex = 0;
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
                string fromdate = "";
                string todate = "";
                string district = "";
                string location = "";
                string agency = "";
                string executive = "";
                try
                {
                    todate = Convert.ToDateTime(dpkrInvoiceDateTo.Text).ToString("yyyy-MM-dd");
                    fromdate = Convert.ToDateTime(dpkrInvoiceDateFrom.Text).ToString("yyyy-MM-dd");
                    district = drpDistrictList.SelectedValue.ToString();
                    location = drpLocationList.SelectedValue.ToString();
                    agency = drpAgencyList.SelectedValue.ToString();
                    executive = drpExecutiveList.SelectedValue.ToString();
                }
                catch { }

                string qry = @"SELECT top 1000 iv.InvoiceNo, FORMAT(iv.PublishDate, 'dd/MM/yyyy') as PublishDate, FORMAT(iv.InvoiceDate,'dd/MM/yyyy') as InvoiceDate, iv.RONum, lc.LocationName, ag.AgencyName, iv.ClientName, iv.ExecutiveId, iv.AdHeight, iv.AdWidth, iv.AdSpace, iv.Rate, iv.AgreedAmount, iv.CGST, iv.SGST, iv.NetAmount, case when iv.PaymentStatus = 'P' then 'Partial' else case when iv.PaymentStatus = 'F' then 'Paid' else 'Un-Paid' end end as PayStatus from Invoice iv left join Master_Agency ag on iv.AgencyId = ag.Id left join Master_Location lc on iv.LocationId = lc.Id where iv.IsCanceled = 'N' ";

                if (fromdate.Length > 2 && todate.Length > 2)
                {
                    qry += " and iv.InvoiceDate between '" + fromdate + "' and '" + todate + "' ";
                }

                if (executive != "" && executive != "0")
                {
                    qry += " and iv.ExecutiveId = '" + executive + "'";
                }

                if (district != "" && district != "0")
                {
                    qry += " and iv.DistrictId = '" + district + "'";
                }

                if (location != "" && location != "0")
                {
                    qry += " and iv.LocationId = '" + location + "'";
                }

                if (agency != "" && agency != "0")
                {
                    qry += " and iv.AgencyId = '" + agency + "'";
                }

                DataTable dataTable = new DataTable();

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
                    csv.Append(dataTable.Columns[i].ColumnName.Replace(',',':'));
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if(this.usertype == "ADMIN")
            {
                string InvoiceNo = "";
                try
                {
                    foreach (DataRowView row in grdInvoiceList.SelectedItems)
                    {
                        DataRow MyRow = row.Row;
                        InvoiceNo = MyRow["InvoiceNo"].ToString();
                    }

                    var cancel = new CancelInvoice(InvoiceNo);

                    cancel.ShowDialog();

                    ButtonAutomationPeer peer = new ButtonAutomationPeer(btnApplyFilter);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("You Are Not Authorised for this Action.", "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
