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
    public partial class DistrictMaster : UserControl
    {
        public DistrictMaster()
        {
            InitializeComponent();
            refreshGrid();
            txtDistrictName.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string DistrictName = txtDistrictName.Text;
            string PK = txtPK_hidden.Text;
            string IsActive = "";
            if (rdoIsActive_A.IsChecked == true)
            {
                IsActive = "A";
            }
            else if (rdoIsActive_D.IsChecked == true)
            {
                IsActive = "D";
            }

            if (DistrictName.Length >= 2)
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("DistrictName",DistrictName);
                col_val.Add("IsActive", IsActive);

                var dbo = new DataAccessLib();
                int rslt = 0;

                if (PK.Length > 0)
                {
                    List<String> whr_cls = new List<String>();
                    whr_cls.AddRange(new string[3] { "Id", "=", PK });
                    rslt = dbo.UpdateRecords("Master_District", col_val, whr_cls);
                }
                else
                {
                    dbo.InsertSingleRow("Master_District", col_val);
                }

                refreshGrid();

                txtDistrictName.Text = "";
                txtPK_hidden.Text = "";
                rdoIsActive_A.IsChecked = true;
                rdoIsActive_D.IsChecked = false;
                btnNew.IsEnabled = false;
                btnSave.Content = "Save";
            }
            else
            {
                MessageBox.Show("District Name Should Be Contain atleast 2 Characters and State Selection is Compulsary","Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void refreshGrid()
        {
            string qry = @"SELECT Master_District.Id, Master_District.DistrictName, Master_District.IsActive from Master_District";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdDistList.ItemsSource = data.DefaultView;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                grdDistList.Columns[0].Visibility = Visibility.Hidden;
               // grdDistList.Columns[1].Visibility = Visibility.Hidden;
            }
            catch { }
        }

        private void grdDistList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {               
                foreach (DataRowView row in grdDistList.SelectedItems)
                {
                    DataRow MyRow = row.Row;
                    txtDistrictName.Text = MyRow["DistrictName"].ToString();
                    txtPK_hidden.Text = MyRow["Id"].ToString();

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
            txtDistrictName.Text = "";
            txtPK_hidden.Text = "";
            rdoIsActive_A.IsChecked = true;
            rdoIsActive_D.IsChecked = false;
            btnNew.IsEnabled = false;
            btnSave.Content = "Save";
        }
    }
}
