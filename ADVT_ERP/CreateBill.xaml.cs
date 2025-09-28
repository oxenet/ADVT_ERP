using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for AddEmp.xaml
    /// </summary>
    public partial class CreateBill : UserControl
    {
        private static bool firstrun = true;
        private string usertype = "";
        public CreateBill(string usertype)
        {
            firstrun = true;
            InitializeComponent();
            firstrun = false;
            this.usertype = usertype;
            load_district_list();
            load_executive_list();
            dpkrInvoiceDate.Focus();
            refreshGrid();
        }

        public void set_default_values()
        {
            var dbo = new DataAccessLib();
            int invoice_id = 0;
            try { invoice_id = dbo.get_last_invoice_id(); } catch { }
            invoice_id++; // CWR/APR/23-24/001
            int month = Convert.ToInt32(DateTime.Now.ToString("MM"));
            int year = Convert.ToInt32(DateTime.Now.ToString("yy"));            
            if (month < 4) { year -= 1; }
            string fy = year.ToString() + "-" + (year + 1).ToString();

            txtInvoiceNo.Text = "CWR/"+DateTime.Now.ToString("MMM").ToUpper() + "/" + fy + "/" + invoice_id.ToString().PadLeft(3,'0');

            dpkrInvoiceDate.SelectedDate = DateTime.Today;
            dpkrPublishDate.SelectedDate = DateTime.Today;

            string configval = dbo.get_configuration("GSTConfig");

            if (configval.Length > 3)
            {
                IDictionary<string, string> config = new Dictionary<string, string>();
                config = JsonConvert.DeserializeObject<IDictionary<string, string>>(configval);

                lblCGST.Content = config["CGST"];
                lblSGST.Content = config["SGST"];
            }
        }

        public void refreshGrid()
        {
            string qry = @"SELECT top 50 iv.InvoiceNo, FORMAT(iv.PublishDate, 'dd/MM/yyyy') as PublishDate, FORMAT(iv.InvoiceDate,'dd/MM/yyyy') as InvoiceDate, iv.RONum, lc.LocationName, ag.AgencyName, iv.ClientName, iv.ExecutiveId, iv.AdHeight, iv.AdWidth, iv.AdSpace, iv.Rate, iv.AgreedAmount, iv.CGST, iv.SGST, iv.NetAmount, case when iv.PaymentStatus = 'P' then 'Partial' else case when iv.PaymentStatus = 'F' then 'Paid' else 'Un-Paid' end end as PayStatus from Invoice iv left join Master_Agency ag on iv.AgencyId = ag.Id left join Master_Location lc on iv.LocationId = lc.Id order by iv.Id desc";

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
                //grdInvoiceList.Columns[0].Visibility = Visibility.Hidden;
               // grdInvoiceList.Columns[1].Visibility = Visibility.Hidden;
               // grdInvoiceList.Columns[2].Visibility = Visibility.Hidden;
                // grdAgencyList.Columns[3].Visibility = Visibility.Hidden;
            }
            catch { }

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string InvoiceNo = txtInvoiceNo.Text;
            string PublishDate = Convert.ToDateTime(dpkrPublishDate.Text).ToString("yyyy-MM-dd");
            string InvoiceDate = Convert.ToDateTime(dpkrInvoiceDate.Text).ToString("yyyy-MM-dd");
            string RONum = txtRONum.Text;
            string DistrictId = "";
            string LocationId = "";
            string AgencyId = "";
            string ExecutiveId = "";
            try
            {
                DistrictId = drpDistrictList.SelectedValue.ToString();
                LocationId = drpLocationList.SelectedValue.ToString();
                AgencyId = drpAgencyList.SelectedValue.ToString();
                ExecutiveId = drpExecutiveList.SelectedValue.ToString();
            } 
            catch 
            {
                //MessageBox.Show("Please Fill Compulsary Fields", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            string ClientName = txtClientName.Text;            
            string AdHeight = txtAdHeight.Text;
            string AdWidth = txtAdWidth.Text;
            string AdSpace = txtAdSpace.Text;
            string Rate = txtRate.Text;
            string Amount = txtAmount.Text;
            string AgreedAmount = txtAgreedAmount.Text;
            string CGST = txtCGST.Text;
            string SGST = txtSGST.Text;
            string NetAmount = txtNetAmount.Text;
            string Remark = txtRemark.Text;

            if (InvoiceNo.Length >= 2 && AdHeight.Length > 0 && AdWidth.Length > 0 && AdSpace.Length > 0 && Rate.Length > 0 && Amount.Length > 0 && NetAmount.Length > 0 && DistrictId != "0" && DistrictId != "" && LocationId != "0" && LocationId != "" && AgencyId != "0" && AgencyId != "")
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("InvoiceNo", InvoiceNo);
                col_val.Add("PublishDate", PublishDate);
                col_val.Add("InvoiceDate", InvoiceDate);
                col_val.Add("RONum", RONum);
                col_val.Add("DistrictId", DistrictId);
                col_val.Add("LocationId", LocationId);
                col_val.Add("AgencyId", AgencyId);
                col_val.Add("ClientName", ClientName);
                col_val.Add("ExecutiveId", ExecutiveId);
                col_val.Add("AdHeight", AdHeight);
                col_val.Add("AdWidth", AdWidth);
                col_val.Add("AdSpace", AdSpace);
                col_val.Add("Rate", Rate);
                col_val.Add("Amount", Amount);
                col_val.Add("AgreedAmount", AgreedAmount);
                col_val.Add("CGST", CGST);
                col_val.Add("SGST", SGST);
                col_val.Add("NetAmount", NetAmount);
                col_val.Add("Remark", Remark);

                var dbo = new DataAccessLib();

                dbo.InsertSingleRow("Invoice", col_val);

                txtInvoiceNo.Text = "";
                dpkrInvoiceDate.SelectedDate = DateTime.Today;
                dpkrPublishDate.SelectedDate = DateTime.Today;
                txtRONum.Text = "";
                drpDistrictList.SelectedIndex = 0;
                drpLocationList.SelectedIndex = 0;
                drpAgencyList.SelectedIndex = 0;
                txtClientName.Text = "";
                drpExecutiveList.Text = "";
                txtAdHeight.Text = "";
                txtAdWidth.Text = "";
                txtAdSpace.Text = "";
                txtRate.Text = "";
                txtAmount.Text = "";
                txtAgreedAmount.Text = "";
                txtCGST.Text = "";
                txtSGST.Text = "";
                txtNetAmount.Text = "";
                txtRemark.Text = "";

                refreshGrid();
            }
            else
            {
                MessageBox.Show("Please Fill Compulsary Fields", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

        private void txtAgreedAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            //float AgreedAmount = float.Parse(txtAgreedAmount.Text, CultureInfo.InvariantCulture.NumberFormat);
            float AgreedAmount = 0, SGST = 0, CGST = 0, totalamt = 0;
            try { float.TryParse(txtAgreedAmount.Text, out AgreedAmount); } catch { }
            try { float.TryParse(lblSGST.Content.ToString(), out SGST); } catch { }
            try { float.TryParse(lblCGST.Content.ToString(), out CGST); } catch { }

            SGST = AgreedAmount * SGST / 100;
            CGST = AgreedAmount * CGST / 100;
            totalamt = AgreedAmount + CGST + SGST;

            txtCGST.Text = CGST.ToString("0.00");
            txtSGST.Text = SGST.ToString("0.00");
            txtNetAmount.Text = totalamt.ToString("0"); 
        }

        private void amt_change()
        {
            float AdHeight = 0, AdWidth = 0, AdSpace = 0, Rate = 0, Amount = 0;
            if (txtAdHeight.Text.Length > 0) { try { float.TryParse(txtAdHeight.Text, out AdHeight); } catch { } }
            if (txtAdWidth.Text.Length > 0) { try { float.TryParse(txtAdWidth.Text, out AdWidth); } catch { } }
            if (txtRate.Text.Length > 0) { try { float.TryParse(txtRate.Text, out Rate); } catch { } }

            AdSpace = AdHeight * AdWidth;
            Amount = AdSpace * Rate;
            txtAdSpace.Text = AdSpace.ToString();
            txtAmount.Text = Amount.ToString("0.00");
        }

        private void txtRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            amt_change();
        }

        private void txtAdHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            amt_change();
        }

        private void txtAdWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            amt_change();
        }

        private void txtAdHeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string rgx = ConfigurationManager.AppSettings["decimalNumbers"];

            var regex = new Regex(@rgx);

            e.Handled = !regex.IsMatch(e.Text);

            //if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
            //{
            //    e.Handled = false;
            //}
            //else
            //{
            //    e.Handled = true;
            //}
        }

        private void txtAdWidth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string rgx = ConfigurationManager.AppSettings["decimalNumbers"];

            var regex = new Regex(@rgx);

            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtRate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string rgx = ConfigurationManager.AppSettings["decimalNumbers"];

            var regex = new Regex(@rgx);

            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtAgreedAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string rgx = ConfigurationManager.AppSettings["decimalNumbers"];

            var regex = new Regex(@rgx);

            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            float AdHeight = 0, AdWidth = 0, AdSpace = 0, Rate = 0, Amount = 0;
            if (txtAdHeight.Text.Length > 0) { try { float.TryParse(txtAdHeight.Text, out AdHeight); } catch { } }
            if (txtAdWidth.Text.Length > 0) { try { float.TryParse(txtAdWidth.Text, out AdWidth); } catch { } }
            if (txtAmount.Text.Length > 0) { try { float.TryParse(txtAmount.Text, out Amount); } catch { } }

            AdSpace = AdHeight * AdWidth;

            if(Amount > 0 && AdSpace > 0)
            {
                Rate = Amount / AdSpace;
            }
            
            txtAdSpace.Text = AdSpace.ToString();
            txtRate.Text = Rate.ToString("0.00");
        }

        private void txtNetAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            float NetAmount = 0, SGST = 0, CGST = 0, SGST_PER = 0, CGST_PER = 0, AdHeight = 0, AdWidth = 0, AdSpace = 0, Rate = 0, Amount = 0;
            try { float.TryParse(txtNetAmount.Text, out NetAmount); } catch { }
            try { float.TryParse(lblSGST.Content.ToString(), out SGST_PER); } catch { }
            try { float.TryParse(lblCGST.Content.ToString(), out CGST_PER); } catch { }
            try { float.TryParse(txtAdHeight.Text, out AdHeight); } catch { }
            try { float.TryParse(txtAdWidth.Text, out AdWidth); } catch { } 
            try { float.TryParse(txtAmount.Text, out Amount); } catch { } 

            Amount = NetAmount * 100 / (100 + SGST_PER + CGST_PER);

            SGST = Amount * SGST_PER / 100;

            CGST = Amount * CGST_PER / 100;

            AdSpace = AdHeight * AdWidth;

            Rate = Amount / AdSpace;

            txtCGST.Text = CGST.ToString("0.00");
            txtSGST.Text = SGST.ToString("0.00");
            txtAmount.Text = Amount.ToString("0.00");
            txtAgreedAmount.Text = Amount.ToString("0");
            txtAdSpace.Text = AdSpace.ToString();
            txtRate.Text = Rate.ToString("0.00");
        }

        private void txtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string rgx = ConfigurationManager.AppSettings["decimalNumbers"];

            var regex = new Regex(@rgx);

            e.Handled = !regex.IsMatch(e.Text);
        }

        private void txtNetAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string rgx = ConfigurationManager.AppSettings["decimalNumbers"];

            var regex = new Regex(@rgx);

            e.Handled = !regex.IsMatch(e.Text);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            txtInvoiceNo.Text = "";
            dpkrPublishDate.Text = "";
            dpkrInvoiceDate.Text = "";
            txtRONum.Text = "";
            drpDistrictList.SelectedIndex = 0;
            drpLocationList.SelectedIndex = 0;
            drpAgencyList.SelectedIndex = 0;
            txtClientName.Text = "";
            drpExecutiveList.Text = "";
            txtAdHeight.Text = "";
            txtAdWidth.Text = "";
            txtAdSpace.Text = "";
            txtRate.Text = "";
            txtAmount.Text = "";
            txtAgreedAmount.Text = "";
            txtCGST.Text = "";
            txtSGST.Text = "";
            txtNetAmount.Text = "";
            txtRemark.Text = "";

            set_default_values();
        }

        private void grdInvoiceList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

                    refreshGrid();
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


        private void dpkrInvoiceDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var dbo = new DataAccessLib();
            int invoice_id = 0;
            try { invoice_id = dbo.get_last_invoice_id(); } catch { }
            invoice_id++; // CWR/APR/23-24/001
            string xyz = dpkrInvoiceDate.Text;
            int month = Convert.ToInt32(Convert.ToDateTime(dpkrInvoiceDate.Text).ToString("MM"));
            int year = Convert.ToInt32(Convert.ToDateTime(dpkrInvoiceDate.Text).ToString("yy"));
            if (month < 4) { year -= 1; }
            string fy = year.ToString() + "-" + (year + 1).ToString();

            txtInvoiceNo.Text = "CWR/" + Convert.ToDateTime(dpkrInvoiceDate.Text).ToString("MMM").ToUpper() + "/" + fy + "/" + invoice_id.ToString().PadLeft(3, '0');

        }
    }
}
