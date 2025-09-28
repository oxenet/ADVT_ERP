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
    /// Interaction logic for AgencyMaster.xaml
    /// </summary>
    public partial class ExecutiveMaster : UserControl
    {
        public ExecutiveMaster()
        {
            InitializeComponent();
            refreshGrid();
            txtEmpId.Focus();
            this.DataContext = this;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string EmpId = txtEmpId.Text;
            string EmpName = txtEmpName.Text;
            string FatherName = txtFatherName.Text;
            string Address = txtAddress.Text;
            string EmailId = txtEmailId.Text;
            string ContactNum = txtContactNum.Text;
            string PAN = txtPAN.Text;
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


            if (EmpId.Length >= 2 && EmpName != "" && ContactNum != "")
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("EmpId", EmpId);
                col_val.Add("EmpName", EmpName);
                col_val.Add("FatherName", FatherName);
                col_val.Add("Address", Address);
                col_val.Add("PAN", PAN);
                col_val.Add("EmailId", EmailId);
                col_val.Add("ContactNum", ContactNum);
                col_val.Add("IsActive", IsActive);

                var dbo = new DataAccessLib();
                int rslt = 0;

                if (PK.Length > 0)
                {
                    List<String> whr_cls = new List<String>();
                    whr_cls.AddRange(new string[3] { "Id", "=", PK });
                    rslt = dbo.UpdateRecords("Master_Executive", col_val, whr_cls);
                }
                else
                {
                    dbo.InsertSingleRow("Master_Executive", col_val);
                }

                refreshGrid();

                txtEmpId.Text = "";
                txtEmpName.Text = "";
                txtFatherName.Text = "";
                txtAddress.Text = "";
                txtEmailId.Text = "";
                txtContactNum.Text = "";
                txtPAN.Text = "";
                rdoIsActive_A.IsChecked = true;
                rdoIsActive_D.IsChecked = false;
                btnNew.IsEnabled = false;
                btnSave.Content = "Save";
            }
            else
            {
                MessageBox.Show("Employee ID Should Be Contain atleast 2 Characters District, Location Selection is Compulsary", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void refreshGrid()
        {
            string qry = @"SELECT Master_Executive.Id, Master_Executive.EmpId, Master_Executive.EmpName, Master_Executive.FatherName, Master_Executive.Address, Master_Executive.ContactNum, Master_Executive.EmailId, Master_Executive.PAN, Master_Executive.IsActive from Master_Executive";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdExecutiveList.ItemsSource = data.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                grdExecutiveList.Columns[0].Visibility = Visibility.Hidden;
            }
            catch { }
        }

        private void grdAgencyList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                foreach (DataRowView row in grdExecutiveList.SelectedItems)
                {
                    DataRow MyRow = row.Row;
                    txtEmpId.Text = MyRow["EmpId"].ToString();
                    txtEmpName.Text = MyRow["EmpName"].ToString();
                    txtFatherName.Text = MyRow["FatherName"].ToString();
                    txtAddress.Text = MyRow["Address"].ToString();
                    txtEmailId.Text = MyRow["EmailId"].ToString();
                    txtContactNum.Text = MyRow["ContactNum"].ToString();
                    txtPAN.Text = MyRow["PAN"].ToString();
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
            txtEmpId.Text = "";
            txtEmpName.Text = "";
            txtFatherName.Text = "";
            txtAddress.Text = "";
            txtEmailId.Text = "";
            txtContactNum.Text = "";
            txtPAN.Text = "";
            rdoIsActive_A.IsChecked = true;
            rdoIsActive_D.IsChecked = false;
            btnNew.IsEnabled = false;
            btnSave.Content = "Save";
        }

    }
}
