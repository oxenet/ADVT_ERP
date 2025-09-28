using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for AddEmp.xaml
    /// </summary>
    public partial class DebitNote : UserControl
    {
        private static bool firstrun = true;
        public DebitNote()
        {
            firstrun = true;
            InitializeComponent();
            firstrun = false;
            refreshGrid();
            load_district_list();
            load_dr_note_purpose_list();
        }

        public void set_default_values()
        {
            var dbo = new DataAccessLib();
            int dr_note_id = dbo.get_last_dr_note_id();
            dr_note_id++;
            txtDrNoteNo.Text = "DRN" + DateTime.Now.ToString("\\/yyyy\\/").ToUpper() + dr_note_id.ToString().PadLeft(6, '0');

            dpkrDrNoteDate.SelectedDate = DateTime.Today;            
        }

        public void refreshGrid()
        {
            string qry = @"select top 50 * from DebitNote order by Id desc";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdDrNoteList.ItemsSource = data.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            set_default_values();

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

                if (location != "0" && location != "")
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
        
        public void load_dr_note_purpose_list()
        {
            var dbo = new DataAccessLib();
            IDictionary<int, string> purpose_list = new Dictionary<int, string>();
            purpose_list = dbo.get_dr_note_purpose_list();
            drpPurposeList.ItemsSource = purpose_list;
            drpPurposeList.DisplayMemberPath = "Value";
            drpPurposeList.SelectedValuePath = "Key";
            drpPurposeList.SelectedIndex = 0;
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

        private void btnCreteDebitNote_Click(object sender, RoutedEventArgs e)
        {
            string DrNoteNo = txtDrNoteNo.Text;
            string PaymentReceiptNo = txtRceiptNo.Text;
            string DrNoteDate = Convert.ToDateTime(dpkrDrNoteDate.Text).ToString("yyyy-MM-dd");
            string AgencyId = "";
            string Purpose = "";
            try
            {
                AgencyId = drpAgencyList.SelectedValue.ToString();
                Purpose = drpPurposeList.SelectedValue.ToString();
            }
            catch { }

            string Amount = txtAmount.Text;
            string Remark = txtRemark.Text;

            if (DrNoteNo.Length >= 2 && Purpose.Length > 0 && Amount.Length > 0 && AgencyId != "0" && AgencyId != "")
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("DrNoteNo", DrNoteNo);
                col_val.Add("DrNoteDate", DrNoteDate);
                col_val.Add("PaymentReceiptNo", PaymentReceiptNo);
                col_val.Add("AgencyId", AgencyId);
                col_val.Add("Purpose", Purpose);
                col_val.Add("Amount", Amount);
                col_val.Add("Remark", Remark);

                var dbo = new DataAccessLib();

                dbo.InsertSingleRow("DebitNote", col_val);

                if (PaymentReceiptNo.Length > 5)
                {

                    int dr_note_id = dbo.get_last_dr_note_id();

                    dbo.debit_note_for_payment_receipt(PaymentReceiptNo, dr_note_id, Remark);
                }

                txtDrNoteNo.Text = "";
                txtRceiptNo.Text = "";
                dpkrDrNoteDate.Text = "";
                txtAmount.Text = "";
                txtRemark.Text = "";
                drpDistrictList.SelectedIndex = 0;
                drpLocationList.SelectedIndex = 0;
                drpAgencyList.SelectedIndex = 0;
                drpPurposeList.SelectedIndex = 0;

                refreshGrid();
            }
            else
            {
                MessageBox.Show("Please Fill Compulsary Fields", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGetReceiptAmount_Click(object sender, RoutedEventArgs e)
        {
            string ReceiptNo = txtRceiptNo.Text;

            var dbo = new DataAccessLib();

            IDictionary<string, string> RecieptDetail = dbo.get_recipt_detail(ReceiptNo);

            if (RecieptDetail.ContainsKey("Amount"))
            {
                txtAmount.Text = RecieptDetail["Amount"];
                drpDistrictList.SelectedValue = RecieptDetail["DistrictId"];
                load_location_list(RecieptDetail["DistrictId"]);
                drpLocationList.SelectedValue = RecieptDetail["LocationId"];
                load_agency();
                drpAgencyList.SelectedValue = RecieptDetail["AgencyId"];
            }            
        }
    }
}
