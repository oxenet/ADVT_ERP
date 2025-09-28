using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for AgencyMaster.xaml
    /// </summary>
    public partial class AgencyMaster : UserControl
    {
        public AgencyMaster()
        {
            InitializeComponent();
            load_district_list();
            refreshGrid();
            drpDistrictList.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string AgencyName = txtAgencyName.Text;
            string CommitionPercent = txtCommitionPercent.Text;
            string Address = txtAddress.Text;
            string OwnerName = txtOwnerName.Text;
            string EmailId = txtEmailId.Text;
            string ContactNum1 = txtContactNum1.Text;
            string ContactNum2 = txtContactNum2.Text;
            string PAN = txtPAN.Text;
            string GSTN = txtGSTN.Text;
            string PK = txtPK_hidden.Text;
            string DistrictId = drpDistrictList.SelectedValue.ToString();
            string LocationId = drpLocationList.SelectedValue.ToString();
            string IsActive = "";
            if (rdoIsActive_A.IsChecked == true)
            {
                IsActive = "A";
            }
            else if (rdoIsActive_D.IsChecked == true)
            {
                IsActive = "D";
            }

            string AgencyType = "";
            if (rdoAgencyType_G.IsChecked == true)
            {
                AgencyType = "GOVT";
            }
            else if (rdoAgencyType_C.IsChecked == true)
            {
                AgencyType = "COMM";
            }

            if (AgencyName.Length >= 2 && DistrictId != "0" && DistrictId != "" && LocationId != "0" && LocationId != "" && ContactNum1 != "")
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("AgencyName", AgencyName);
                col_val.Add("AgencyType", AgencyType);
                col_val.Add("OwnerName", OwnerName);
                col_val.Add("CommitionPercent", CommitionPercent);
                col_val.Add("Address", Address);
                col_val.Add("DistrictId", DistrictId);
                col_val.Add("LocationId", LocationId);
                col_val.Add("PAN", PAN);
                col_val.Add("GSTN", GSTN);
                col_val.Add("EmailID", EmailId);
                col_val.Add("ContactNum1", ContactNum1);
                col_val.Add("ContactNum2", ContactNum2);
                col_val.Add("IsActive", IsActive);

                var dbo = new DataAccessLib();
                int rslt = 0;

                if (PK.Length > 0)
                {
                    List<String> whr_cls = new List<String>();
                    whr_cls.AddRange(new string[3] { "Id", "=", PK });
                    rslt = dbo.UpdateRecords("Master_Agency", col_val, whr_cls);
                }
                else
                {
                    dbo.InsertSingleRow("Master_Agency", col_val);
                }

                refreshGrid();
                
                txtAgencyName.Text = "";
                txtCommitionPercent.Text = "";
                txtPK_hidden.Text = "";
                txtOwnerName.Text = "";
                txtEmailId.Text = "";
                txtContactNum1.Text = "";
                txtContactNum2.Text = "";
                txtAddress.Text = "";
                txtPAN.Text = "";
                txtGSTN.Text = "";
                drpDistrictList.SelectedIndex = 0;
                drpLocationList.SelectedIndex = 0;
                rdoIsActive_A.IsChecked = true;
                rdoIsActive_D.IsChecked = false;
                rdoAgencyType_C.IsChecked = true;
                rdoAgencyType_G.IsChecked = false;
                btnNew.IsEnabled = false;
                btnSave.Content = "Save";
            }
            else
            {
                MessageBox.Show("Agency Name Should Be Contain atleast 2 Characters and State, District, Location Selection is Compulsary", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void refreshGrid()
        {
            string qry = @"SELECT Master_Agency.Id, Master_Agency.DistrictId, Master_Agency.LocationId, Master_Agency.AgencyName, Master_Agency.AgencyType, Master_District.DistrictName, Master_Location.LocationName, Master_Agency.OwnerName, Master_Agency.Address, Master_Agency.PAN, Master_Agency.GSTN, Master_Agency.EmailID, Master_Agency.ContactNum1, Master_Agency.ContactNum2, Master_Agency.IsActive from Master_Agency join Master_District on Master_District.Id = Master_Agency.DistrictId join Master_Location on Master_Location.Id = Master_Agency.LocationId";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdAgencyList.ItemsSource = data.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                grdAgencyList.Columns[0].Visibility = Visibility.Hidden;
                grdAgencyList.Columns[1].Visibility = Visibility.Hidden;
                grdAgencyList.Columns[2].Visibility = Visibility.Hidden;
               // grdAgencyList.Columns[3].Visibility = Visibility.Hidden;
            }
            catch { }
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

        private void grdAgencyList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                foreach (DataRowView row in grdAgencyList.SelectedItems)
                {
                    DataRow MyRow = row.Row;
                    txtAgencyName.Text = MyRow["AgencyName"].ToString();
                    txtOwnerName.Text = MyRow["OwnerName"].ToString();
                    txtEmailId.Text = MyRow["EmailID"].ToString();
                    txtContactNum1.Text = MyRow["ContactNum1"].ToString();
                    txtContactNum2.Text = MyRow["ContactNum2"].ToString();
                    txtAddress.Text = MyRow["Address"].ToString();
                    txtPAN.Text = MyRow["PAN"].ToString();
                    txtGSTN.Text = MyRow["GSTN"].ToString();
                    txtPK_hidden.Text = MyRow["Id"].ToString();
                    drpDistrictList.SelectedValue = MyRow["DistrictId"].ToString();
                    try { load_location_list(MyRow["DistrictId"].ToString()); } catch { }
                    drpLocationList.SelectedValue = MyRow["LocationId"].ToString();

                    if (MyRow["IsActive"].ToString() == "A")
                    {
                        rdoIsActive_A.IsChecked = true;
                        rdoIsActive_D.IsChecked = false;
                    }
                    else if (MyRow["IsActive"].ToString() == "D")
                    {
                        rdoIsActive_A.IsChecked = false;
                        rdoIsActive_D.IsChecked = true;
                    }
                    else
                    {
                        rdoIsActive_A.IsChecked = false;
                        rdoIsActive_D.IsChecked = false;
                    }

                    ///////////-------
                    
                    if (MyRow["AgencyType"].ToString() == "GOVT")
                    {
                        rdoAgencyType_G.IsChecked = true;
                        rdoAgencyType_C.IsChecked = false;
                    }
                    else if (MyRow["AgencyType"].ToString() == "COMM")
                    {
                        rdoAgencyType_G.IsChecked = false;
                        rdoAgencyType_C.IsChecked = true;
                    }
                    else
                    {
                        rdoAgencyType_G.IsChecked = false;
                        rdoAgencyType_C.IsChecked = false;
                    }
                }

                btnNew.IsEnabled = true;
                btnSave.Content = "Update";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            txtAgencyName.Text = "";
            txtPK_hidden.Text = "";
            txtOwnerName.Text = "";
            txtEmailId.Text = "";
            txtContactNum1.Text = "";
            txtContactNum2.Text = "";
            txtAddress.Text = "";
            txtPAN.Text = "";
            txtGSTN.Text = "";
            drpDistrictList.SelectedIndex = 0;
            drpLocationList.SelectedIndex = 0;
            rdoIsActive_A.IsChecked = true;
            rdoIsActive_D.IsChecked = false;
            rdoAgencyType_C.IsChecked = true;
            rdoAgencyType_G.IsChecked = false;
            btnNew.IsEnabled = false;
            btnSave.Content = "Save";
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
    }
}
