using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for AgencyMaster.xaml
    /// </summary>
    public partial class MonthWiseSummary : UserControl
    {
        private static bool firstrun = true;
        private string usertype = "";
        public MonthWiseSummary(string usertype)
        {
            firstrun = true;
            InitializeComponent();
            firstrun = false;
            this.usertype = usertype;
            load_district_list();
            set_fyears();
            int fyear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            refreshGrid(fyear);
        }

        public void set_fyears()
        {
            IDictionary<int, string> fyears = new Dictionary<int, string>();
            for(int i = 2023; i <= Convert.ToInt32(DateTime.Now.ToString("yyyy")); i++)
            {
                fyears.Add(i, i.ToString());
            }

            drpFyears.ItemsSource = fyears;
            drpFyears.DisplayMemberPath = "Value";
            drpFyears.SelectedValuePath = "Key";
            drpFyears.SelectedIndex = 0;
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            int fyear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));

            string agency = "";

            try
            {                
                agency = drpAgencyList.SelectedValue.ToString();
                fyear = Convert.ToInt32(drpFyears.SelectedValue);
            }
            catch { }

            refreshGrid(fyear, agency);
        }

        public void refreshGrid(int fyear = 0, string agency = "")
        {
            string extra_qry = "";

            if (agency != "" && agency != "0")
            {
                extra_qry += " and AgencyId = '" + agency + "'";
            }

            string qry = @"select Month, sum(a.bill) as Invoice, sum(a.payment) as Receipt, sum(a.drn) as DrNote, sum(a.crn) as CrNote, (sum(a.bill) - sum(a.payment)) as Balance  from ( ";

            IDictionary<int, string> months = new Dictionary<int, string>();
            months.Add(1, "January");
            months.Add(2, "Fabruary");
            months.Add(3, "March");
            months.Add(4, "April");
            months.Add(5, "May");
            months.Add(6, "June");
            months.Add(7, "July");
            months.Add(8, "August");
            months.Add(9, "September");
            months.Add(10, "October");
            months.Add(11, "November");
            months.Add(12, "December");

            string todate = "", fromdate = "";
            int year = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            int monthday = 0;
            
            for (int i = 1; i <= 12; i++)
            {
                if(i < 4) { year = fyear + 1; } else { year = fyear; }
                if(i == 6 || i == 11 || i == 9 || i == 4) { monthday = 30; }
                else if(i == 2) { if (fyear % 4 == 0) { monthday = 29; } else { monthday = 28; } }
                else { monthday = 31; }

                fromdate = Convert.ToDateTime("01-"+ i.ToString() +"-"+ fyear.ToString()).ToString("yyyy-MM-dd");
                todate = Convert.ToDateTime(monthday.ToString()+"-" + i.ToString() + "-"+ fyear.ToString()).ToString("yyyy-MM-dd");
                
                if (i > 1)
                {
                    qry += " union all ";
                }

                qry += " select '"+ months[i] + "' as Month, coalesce(sum(i.NetAmount),0) as bill, 0 as payment, 0 as drn, 0 as crn from Invoice i where i.InvoiceDate between '" + fromdate + "' and '" + todate + "'" + extra_qry;

                qry += " union all ";

                qry += " select '" + months[i] + "' as Month, 0 as bill, coalesce(sum(p.Amount),0) as payment, 0 as drn, 0 as crn from PaymentReceipt p where p.ReceiptDate  between '" + fromdate + "' and '" + todate + "'" + extra_qry;

                qry += " union all ";

                qry += " select '" + months[i] + "' as Month, 0 as bill, 0 as payment, coalesce(sum(d.Amount),0) as drn, 0 as crn from DebitNote d where d.DrNoteDate between '" + fromdate + "' and '" + todate + "'" + extra_qry;

                qry += " union all ";

                qry += " select '" + months[i] + "' as Month, 0 as bill, 0 as payment, 0 as drn, coalesce(sum(c.Amount),0) as crn from CreditNote c where c.CrNoteDate between '" + fromdate + "' and '" + todate + "'" + extra_qry;

                
            }

            qry += " ) a group by Month";
            
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
