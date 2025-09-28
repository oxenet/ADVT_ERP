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
    public partial class BillClearance : UserControl
    {
        private static bool firstrun = true;
        public BillClearance()
        {
            firstrun = true;
            InitializeComponent();
            firstrun = false;            
            load_district_list();
        }

        public void refreshGrid(string agencyid)
        {
            string qry = @"select * from (SELECT pr.ReceiptNo, pr.ReceiptDate, pr.ClientName, pr.Amount, coalesce(sum(irm.SetteledAmount),0) as Cleared, (pr.Amount - coalesce(sum(irm.SetteledAmount),0)) as Remaining 
                            from PaymentReceipt pr left join InvoiceReceiptMapping irm on irm.ReceiptNo = pr.ReceiptNo 
                            where pr.AgencyId = '" + agencyid + "' and pr.IsSettled = 'N' and pr.IsCanceled = 'N' " +
                            "group by pr.ReceiptNo, pr.ReceiptDate, pr.ClientName, pr.Amount) abc where abc.Remaining > 0 order by ReceiptDate desc";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdPaymentList.ItemsSource = data.DefaultView;
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

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string AgencyId = "";
            string AgencyName = "";

            try 
            { 
                AgencyId = drpAgencyList.SelectedValue.ToString();
                AgencyName = drpAgencyList.Text;
            } catch { }

            if (AgencyId != "")
            {
                txtAgencyId_hidden.Text = AgencyId;
                txtAgencyName.Text = AgencyName;
                refreshGrid(AgencyId);                
            }
            else
            {
                MessageBox.Show("Select Agency", "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 
        
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView dataRowView = (DataRowView)((Button)e.Source).DataContext;
                String ReceiptNo = dataRowView[0].ToString();

                txtReceiptNo.Text = ReceiptNo;
                txtReceiptDate.Text = Convert.ToDateTime(dataRowView[1]).ToString("dd-MM-yyyy");
                txtReceiptTxnAmount.Text = dataRowView[3].ToString();
                txtToBeClearAmount.Text = dataRowView[5].ToString();
                txtSelectedInvoiceAmount.Text = "0";
                load_pending_invoice();
                popupInvoices.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnCloseBillPopup_Click(object sender, RoutedEventArgs e)
        {
            popupInvoices.Visibility = Visibility.Hidden;
        }

        public void load_pending_invoice()
        {
            string InvoiceFrom = "";
            string InvoiceTo = "";
            string AgencyId = txtAgencyId_hidden.Text;

            if (dpkrInvoiceFrom.Text != "")
            {
                Convert.ToDateTime(dpkrInvoiceFrom.Text).ToString("yyyy-MM-dd");
            }

            if (dpkrInvoiceTo.Text != "")
            {
                Convert.ToDateTime(dpkrInvoiceTo.Text).ToString("yyyy-MM-dd");
            }

            string qry = @"select * from (select sum(0) as SelectedAmount, iv.InvoiceNo, FORMAT(iv.PublishDate,'dd/MM/yyyy') as PublishDate, FORMAT(iv.InvoiceDate,'dd/MM/yyyy') as InvoiceDate, iv.RONum, iv.ClientName, iv.AdSpace, iv.Rate, iv.Amount, iv.AgreedAmount, iv.CGST, iv.SGST, iv.NetAmount,
                            coalesce(sum(irm.SetteledAmount),0) as SetteledAmount, (iv.NetAmount - coalesce(sum(irm.SetteledAmount),0)) as OutStanding
                            from Invoice iv left join InvoiceReceiptMapping irm on irm.InvoiceNo = iv.InvoiceNo
                            where iv.AgencyId = '" + AgencyId+ "' and iv.IsCanceled = 'N' group by iv.InvoiceNo, iv.PublishDate, iv.InvoiceDate, iv.RONum, iv.ClientName, iv.AdSpace, iv.Rate, iv.Amount, iv.AgreedAmount, iv.CGST, iv.SGST, iv.NetAmount) abc where abc.OutStanding > 0 ";

            if (InvoiceFrom != "" && InvoiceTo != "")
            {
                qry += " and InvoiceDate >= '" + InvoiceFrom + "' and InvoiceDate <= '" + InvoiceTo + "'";
            }

            qry += " order by InvoiceDate asc";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdPendingInvoices.ItemsSource = data.DefaultView;
                grdPendingInvoices.Columns[1].IsReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnInvoiceFilter_Click(object sender, RoutedEventArgs e)
        {
            load_pending_invoice();
        }

        private void SelectInvoice_Checked(object sender, RoutedEventArgs e)
        {            
            DataRowView dataRowView = (DataRowView)((CheckBox)e.Source).DataContext;            
            dataRowView[0] = dataRowView[14];
            float SelectedAmount = 0, ToBeClearAmount = 0, SelectedInvoiceAmount = 0;
            try { float.TryParse(dataRowView[0].ToString(), out SelectedAmount); } catch { }
            try { float.TryParse(txtToBeClearAmount.Text, out ToBeClearAmount); } catch { }
            try { float.TryParse(txtSelectedInvoiceAmount.Text, out SelectedInvoiceAmount); } catch { }

            SelectedInvoiceAmount = sum_selected_invoice_amount();

            if (SelectedInvoiceAmount > ToBeClearAmount)
            {
                dataRowView[0] = 0;
                MessageBox.Show("Amount is More then to Require for Clear");
            }
            else
            {
                txtSelectedInvoiceAmount.Text = SelectedInvoiceAmount.ToString();
            }
        }

        private void SelectInvoice_Unchecked(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = (DataRowView)((CheckBox)e.Source).DataContext;            
            float SelectedAmount = 0, SelectedInvoiceAmount = 0;
            try { float.TryParse(dataRowView[0].ToString(), out SelectedAmount); } catch { }
            try { float.TryParse(txtSelectedInvoiceAmount.Text, out SelectedInvoiceAmount); } catch { }
            dataRowView[0] = 0;

            SelectedInvoiceAmount = sum_selected_invoice_amount();

            txtSelectedInvoiceAmount.Text = SelectedInvoiceAmount.ToString();
        }

        private void grdPendingInvoices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var currentRowIndex = grdPendingInvoices.Items.IndexOf(grdPendingInvoices.CurrentItem);
                txtRowIndex_hidden.Text = currentRowIndex.ToString();
                foreach (DataRowView row in grdPendingInvoices.SelectedItems)
                {
                    DataRow MyRow = row.Row;

                    float OutStanding = 0, ToBeClearAmount = 0, SelectedInvoiceAmount = 0, RequireAmount = 0;
                    try { float.TryParse(MyRow["OutStanding"].ToString(), out OutStanding); } catch { }
                    try { float.TryParse(txtToBeClearAmount.Text, out ToBeClearAmount); } catch { }
                    try { float.TryParse(txtSelectedInvoiceAmount.Text, out SelectedInvoiceAmount); } catch { }

                    RequireAmount = ToBeClearAmount - SelectedInvoiceAmount;

                    if(RequireAmount <= OutStanding)
                    {
                        txtAmountToClearEdit.Text = RequireAmount.ToString();
                        txtAmount_hidden.Text = RequireAmount.ToString();
                    }
                    else
                    {
                        txtAmountToClearEdit.Text = MyRow["SelectedAmount"].ToString();
                        txtAmount_hidden.Text = MyRow["SelectedAmount"].ToString();
                    }
                    
                    popupCellEdit.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCloseCellEditPopup_Click(object sender, RoutedEventArgs e)
        {
            popupCellEdit.Visibility = Visibility.Hidden;
        }

        private void btnUpdateAmoutToClear_Click(object sender, RoutedEventArgs e)
        {
            float SelectedAmount = 0, ToBeClearAmount = 0, SelectedInvoiceAmount = 0, Amount_hidden = 0;
            try { float.TryParse(txtAmountToClearEdit.Text, out SelectedAmount); } catch { }
            try { float.TryParse(txtAmount_hidden.Text, out Amount_hidden); } catch { }
            try { float.TryParse(txtToBeClearAmount.Text, out ToBeClearAmount); } catch { }
            try { float.TryParse(txtSelectedInvoiceAmount.Text, out SelectedInvoiceAmount); } catch { }

            SelectedInvoiceAmount = sum_selected_invoice_amount() - Amount_hidden;

            if (SelectedInvoiceAmount + SelectedAmount > ToBeClearAmount)
            {                
                MessageBox.Show("Amount is More then to Require for Clear", "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                popupCellEdit.Visibility = Visibility.Hidden;
                int currentRowIndex = Convert.ToInt32(txtRowIndex_hidden.Text);
                DataRowView rowView = (grdPendingInvoices.Items[currentRowIndex] as DataRowView);
                rowView.BeginEdit();
                rowView[0] = txtAmountToClearEdit.Text;
                rowView.EndEdit();
                //grdPendingInvoices.Items.Refresh();
                SelectedInvoiceAmount = sum_selected_invoice_amount();
                txtSelectedInvoiceAmount.Text = SelectedInvoiceAmount.ToString();
            }            
        }

        public float sum_selected_invoice_amount()
        {
            float sum = 0, temp = 0;

            foreach (DataRowView dr in grdPendingInvoices.ItemsSource)
            {
                try { float.TryParse(dr[0].ToString(), out temp); } catch { temp = 0; }
                sum += temp;
            }

            return sum;
        }

        private void btnSaveInvoiceClearance_Click(object sender, RoutedEventArgs e)
        {
            string ReceiptNo = txtReceiptNo.Text;
            var dbo = new DataAccessLib();

            foreach (DataRowView dr in grdPendingInvoices.ItemsSource)
            {
                string InvoiceNo = dr[1].ToString(); 
                float InvoiceAmount = 0, SetteledAmount = 0;
                try { float.TryParse(dr[12].ToString(), out InvoiceAmount); } catch { InvoiceAmount = 0; }
                try { float.TryParse(dr[0].ToString(), out SetteledAmount); } catch { SetteledAmount = 0; }

                if(SetteledAmount > 0)
                {
                    IDictionary<string, string> col_val = new Dictionary<string, string>();

                    col_val.Add("InvoiceNo", InvoiceNo);
                    col_val.Add("ReceiptNo", ReceiptNo);
                    col_val.Add("InvoiceAmount", InvoiceAmount.ToString());
                    col_val.Add("SetteledAmount", SetteledAmount.ToString());

                    dbo.InsertSingleRow("InvoiceReceiptMapping", col_val);
                }
            }

            load_pending_invoice();

            MessageBox.Show("Data Saved", "Information !", MessageBoxButton.OK, MessageBoxImage.Information);

            popupInvoices.Visibility = Visibility.Hidden;

            string AgencyId = txtAgencyId_hidden.Text;

            refreshGrid(AgencyId);
        }
    }
}
