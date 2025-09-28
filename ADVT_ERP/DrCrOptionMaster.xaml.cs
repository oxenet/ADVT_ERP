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
    public partial class DrCrOptionMaster : UserControl
    {
        public DrCrOptionMaster()
        {
            InitializeComponent();
            set_default_values();
            refreshGrid();
        }

        private void set_default_values()
        {
            IDictionary<string, string> ResonFor = new Dictionary<string, string>();
            ResonFor.Add("Select Reson For", "Select Reson For");
            ResonFor.Add("DrNote", "Debit Notes");
            ResonFor.Add("CrNote", "Credit Notes");
            drpResonFor.ItemsSource = ResonFor;
            drpResonFor.DisplayMemberPath = "Value";
            drpResonFor.SelectedValuePath = "Key";
            drpResonFor.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string Reson = txtReson.Text;
            string ResonFor = drpResonFor.SelectedValue.ToString();
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

            if (Reson.Length >= 2 && (ResonFor == "DrNote" || ResonFor == "CrNote"))
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("Reson", Reson);
                col_val.Add("ResonFor", ResonFor);
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
                    dbo.InsertSingleRow("OptionMaster", col_val);
                }

                refreshGrid();

                txtReson.Text = "";
                txtPK_hidden.Text = "";
                drpResonFor.SelectedIndex = 0;
                rdoIsActive_A.IsChecked = true;
                rdoIsActive_D.IsChecked = false;
                btnNew.IsEnabled = false;
                btnSave.Content = "Save";
            }
            else
            {
                MessageBox.Show("Reson Should Be Contain atleast 2 Characters and Reson For is Compulsary", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void refreshGrid()
        {
            string qry = @"SELECT * from OptionMaster";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdOptionList.ItemsSource = data.DefaultView;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
               // grdDistList.Columns[0].Visibility = Visibility.Hidden;
               // grdDistList.Columns[1].Visibility = Visibility.Hidden;
            }
            catch { }
        }

        private void grdDistList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {               
                foreach (DataRowView row in grdOptionList.SelectedItems)
                {
                    DataRow MyRow = row.Row;
                    txtReson.Text = MyRow["Reson"].ToString();
                    drpResonFor.SelectedValue = MyRow["ResonFor"].ToString();
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
            txtReson.Text = "";
            txtPK_hidden.Text = "";
            drpResonFor.SelectedIndex = 0;
            rdoIsActive_A.IsChecked = true;
            rdoIsActive_D.IsChecked = false;
            btnNew.IsEnabled = false;
            btnSave.Content = "Save";
        }
    }
}
