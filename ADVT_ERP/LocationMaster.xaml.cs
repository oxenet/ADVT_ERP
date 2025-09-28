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
    public partial class LocationMaster : UserControl
    {
        public LocationMaster()
        {
            InitializeComponent();
            load_district_list();
            refreshGrid();
            drpDistrictList.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string LocationName = txtLocationName.Text;
            string PK = txtPK_hidden.Text;
            string DistrictId = drpDistrictList.SelectedValue.ToString();
            string IsActive = "";
            if (rdoIsActive_A.IsChecked == true)
            {
                IsActive = "A";
            }
            else if (rdoIsActive_D.IsChecked == true)
            {
                IsActive = "D";
            }

            if (LocationName.Length >= 2  && DistrictId != "0" && DistrictId != "")
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("LocationName", LocationName);
                col_val.Add("DistrictId", DistrictId);
                col_val.Add("IsActive", IsActive);

                var dbo = new DataAccessLib();
                int rslt = 0;

                if (PK.Length > 0)
                {
                    List<String> whr_cls = new List<String>();
                    whr_cls.AddRange(new string[3] { "Id", "=", PK });
                    rslt = dbo.UpdateRecords("Master_Location", col_val, whr_cls);
                }
                else
                {
                    dbo.InsertSingleRow("Master_Location", col_val);
                }

                refreshGrid();

                txtLocationName.Text = "";
                txtPK_hidden.Text = "";
                drpDistrictList.SelectedIndex = 0;
                rdoIsActive_A.IsChecked = true;
                rdoIsActive_D.IsChecked = false;
                btnNew.IsEnabled = false;
                btnSave.Content = "Save";
            }
            else
            {
                MessageBox.Show("Location Name Should Be Contain atleast 2 Characters and State, District Selection is Compulsary", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void refreshGrid()
        {
            string qry = @"SELECT Master_Location.Id, Master_Location.DistrictId, Master_Location.LocationName, Master_District.DistrictName, Master_Location.IsActive from Master_Location join Master_District on Master_District.Id = Master_Location.DistrictId";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdLocationList.ItemsSource = data.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                grdLocationList.Columns[0].Visibility = Visibility.Hidden;
                grdLocationList.Columns[1].Visibility = Visibility.Hidden;
                //grdLocationList.Columns[2].Visibility = Visibility.Hidden;
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

        private void grdLocationList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                foreach (DataRowView row in grdLocationList.SelectedItems)
                {
                    DataRow MyRow = row.Row;
                    txtLocationName.Text = MyRow["LocationName"].ToString();
                    txtPK_hidden.Text = MyRow["Id"].ToString();
                    drpDistrictList.SelectedValue = MyRow["DistrictId"].ToString();

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
            txtLocationName.Text = "";
            txtPK_hidden.Text = "";
            drpDistrictList.SelectedIndex = 0;
            rdoIsActive_A.IsChecked = true;
            rdoIsActive_D.IsChecked = false;
            btnNew.IsEnabled = false;
            btnSave.Content = "Save";
        }
    }
}
