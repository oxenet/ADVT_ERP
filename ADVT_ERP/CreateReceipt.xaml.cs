using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for AddEmp.xaml
    /// </summary>
    public partial class CreateReceipt : UserControl
    {
        private static bool firstrun = true;
        public CreateReceipt()
        {
            firstrun = true;
            InitializeComponent();
            firstrun = false;            
            load_district_list();
            refreshGrid();
        }

        public void set_default_values()
        {
            var dbo = new DataAccessLib();
            int receipt_id = dbo.get_last_receipt_id();
            receipt_id++;
            txtReceiptNo.Text = "CWR"+DateTime.Now.ToString("\\/yyyy\\/").ToUpper() + receipt_id.ToString().PadLeft(3,'0');

            dpkrReceiptDate.SelectedDate = DateTime.Today;
            dpkrChequeTxnDate.SelectedDate = DateTime.Today;

            IDictionary<string, string> pm_list = new Dictionary<string, string>();
            pm_list.Add("Select Payment Mode", "Select Payment Mode");
            pm_list.Add("Cash", "Cash");
            pm_list.Add("Cheque", "Cheque");
            pm_list.Add("NEFT/RTGS", "NEFT/RTGS");
            pm_list.Add("UPI", "UPI");
            drpPymentMode.ItemsSource = pm_list;
            drpPymentMode.DisplayMemberPath = "Value";
            drpPymentMode.SelectedValuePath = "Key";
            drpPymentMode.SelectedIndex = 0;
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string ReceiptNo = txtReceiptNo.Text;
            string ReceiptDate = Convert.ToDateTime(dpkrReceiptDate.Text).ToString("yyyy-MM-dd");
            string AgencyId = ""; 
            string DistrictId = ""; 
            string LocationId = ""; 
            string PymentMode = "";
            try
            {
                AgencyId = drpAgencyList.SelectedValue.ToString();
                DistrictId = drpDistrictList.SelectedValue.ToString();
                LocationId = drpLocationList.SelectedValue.ToString();
                PymentMode = drpPymentMode.SelectedValue.ToString();
            }
            catch { }

            string ClientName = txtClientName.Text;
            string ChequeNoTxnId = txtChequeNoTxnId.Text;
            string ChequeTxnDate = Convert.ToDateTime(dpkrChequeTxnDate.Text).ToString("yyyy-MM-dd");
            string ChequeTxnBank = txtChequeTxnBank.Text;
            string Remark = txtRemark.Text;            
            string Amount = txtAmount.Text;            

            if (ReceiptNo.Length >= 2 && PymentMode.Length > 0 && Amount.Length > 0 && DistrictId != "0" && DistrictId != "" && LocationId != "0" && LocationId != "" && AgencyId != "0" && AgencyId != "")
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("ReceiptNo", ReceiptNo);
                col_val.Add("ReceiptDate", ReceiptDate);
                col_val.Add("AgencyId", AgencyId);
                col_val.Add("DistrictId", DistrictId);
                col_val.Add("LocationId", LocationId);
                col_val.Add("PymentMode", PymentMode);
                col_val.Add("ClientName", ClientName);
                col_val.Add("ChequeNoTxnId", ChequeNoTxnId);
                col_val.Add("ChequeTxnDate", ChequeTxnDate);
                col_val.Add("ChequeTxnBank", ChequeTxnBank);
                col_val.Add("Amount", Amount);
                col_val.Add("Remark", Remark);

                var dbo = new DataAccessLib();

                dbo.InsertSingleRow("PaymentReceipt", col_val);
                
                txtReceiptNo.Text = "";
                dpkrReceiptDate.Text = "";
                txtClientName.Text = "";
                txtChequeNoTxnId.Text = "";
                drpDistrictList.SelectedIndex = 0;
                drpLocationList.SelectedIndex = 0;
                drpAgencyList.SelectedIndex = 0;
                drpPymentMode.SelectedIndex = 0;
                txtClientName.Text = "";
                dpkrChequeTxnDate.Text = "";
                txtChequeTxnBank.Text = "";
                txtRemark.Text = "";
                txtAmount.Text = "";

                refreshGrid();
            }
            else
            {
                MessageBox.Show("Please Fill Compulsary Fields", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void refreshGrid()
        {
            string qry = @"SELECT top 50  FORMAT(pr.ReceiptDate,'dd/MM/yyyy') as ReceiptDate, pr.ReceiptNo, pr.PymentMode, lc.LocationName, ag.AgencyName, pr.ClientName, pr.Amount from PaymentReceipt pr left join Master_Agency ag on pr.AgencyId = ag.Id left join Master_Location lc on pr.LocationId = lc.Id order by pr.Id desc";

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

            try
            {
                grdInvoiceList.Columns[0].Visibility = Visibility.Hidden;
                // grdInvoiceList.Columns[1].Visibility = Visibility.Hidden;
                // grdInvoiceList.Columns[2].Visibility = Visibility.Hidden;
                // grdAgencyList.Columns[3].Visibility = Visibility.Hidden;
            }
            catch { }

            set_default_values();
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

        private void txtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string rgx = ConfigurationManager.AppSettings["decimalNumbers"];

            var regex = new Regex(@rgx);

            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            txtReceiptNo.Text = "";
            dpkrReceiptDate.Text = "";
            txtClientName.Text = "";
            txtChequeNoTxnId.Text = "";
            drpDistrictList.SelectedIndex = 0;
            drpLocationList.SelectedIndex = 0;
            drpAgencyList.SelectedIndex = 0;
            drpPymentMode.SelectedIndex = 0;
            txtClientName.Text = "";
            dpkrChequeTxnDate.Text = "";
            txtChequeTxnBank.Text = "";
            txtRemark.Text = "";

            set_default_values();
        }
    }
}
