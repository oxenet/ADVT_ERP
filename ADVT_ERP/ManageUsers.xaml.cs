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
    public partial class ManageUsers : UserControl
    {
        public ManageUsers()
        {
            InitializeComponent();
            refreshGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string UserId = txtUserId.Text;
            string Password = txtPassword.Text;
            string UserName = txtUserName.Text;
            string UserMobile = txtUserMobile.Text;
            string UserEmail = txtUserEmail.Text;
            string PK = txtPK_hidden.Text;
            string IsActive = "", UserType = "";
            
            if (rdUserTypeAdmin.IsChecked == true)
            {
                UserType = "ADMIN";
            }
            else if (rdUserTypeOperator.IsChecked == true)
            {
                UserType = "OPERATOR";
            }

            if (rdoIsActive_A.IsChecked == true)
            {
                IsActive = "A";
            }
            else if (rdoIsActive_D.IsChecked == true)
            {
                IsActive = "D";
            }

            if (UserId.Length >= 5)
            {
                IDictionary<string, string> col_val = new Dictionary<string, string>();

                col_val.Add("UserId", UserId);
                col_val.Add("Password", Password);
                col_val.Add("UserName", UserName);
                col_val.Add("UserMobile", UserMobile);
                col_val.Add("UserEmail", UserEmail);
                col_val.Add("UserType", UserType);
                col_val.Add("IsActive", IsActive);

                var dbo = new DataAccessLib();
                int rslt = 0;

                if (PK.Length > 0)
                {
                    List<String> whr_cls = new List<String>();
                    whr_cls.AddRange(new string[3] { "Id", "=", PK });
                    rslt = dbo.UpdateRecords("MasterUsers", col_val, whr_cls);
                }
                else
                {
                    dbo.InsertSingleRow("MasterUsers", col_val);
                }

                refreshGrid();

                txtUserId.Text = "";
                txtPassword.Text = "";
                txtUserName.Text = "";
                txtUserMobile.Text = "";
                txtUserEmail.Text = "";
                txtPK_hidden.Text = "";
                rdoIsActive_A.IsChecked = true;
                rdoIsActive_D.IsChecked = false;
                rdUserTypeAdmin.IsChecked = true;
                rdUserTypeOperator.IsChecked = false;
                btnNew.IsEnabled = false;
                btnSave.Content = "Save";
            }
            else
            {
                MessageBox.Show("UserId Should Be Contain atleast 5 Characters", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void refreshGrid()
        {
            string qry = @"SELECT * from MasterUsers";

            try
            {
                var dbo = new DataAccessLib();
                DataTable data = dbo.GetDataTable(qry);
                grdUserList.ItemsSource = data.DefaultView;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void grdDistList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {               
                foreach (DataRowView row in grdUserList.SelectedItems)
                {
                    DataRow MyRow = row.Row;
                    txtUserId.Text = MyRow["UserId"].ToString();
                    txtPassword.Text = MyRow["Password"].ToString();
                    txtUserName.Text = MyRow["UserName"].ToString();
                    txtUserMobile.Text = MyRow["UserMobile"].ToString();
                    txtUserEmail.Text = MyRow["UserEmail"].ToString();
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
                    
                    if (MyRow["UserType"].ToString() == "ADMIN")
                    {
                        rdUserTypeAdmin.IsChecked = true;
                        rdUserTypeOperator.IsChecked = false;
                    }
                    else if (MyRow["UserType"].ToString() == "OPERATOR")
                    {
                        rdUserTypeAdmin.IsChecked = false;
                        rdUserTypeOperator.IsChecked = true;
                    }
                    else
                    {
                        rdUserTypeAdmin.IsChecked = false;
                        rdUserTypeOperator.IsChecked = false;
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
            txtUserId.Text = "";
            txtPassword.Text = "";
            txtUserName.Text = "";
            txtUserMobile.Text = "";
            txtUserEmail.Text = "";
            txtPK_hidden.Text = "";
            rdoIsActive_A.IsChecked = true;
            rdoIsActive_D.IsChecked = false;
            rdUserTypeAdmin.IsChecked = true;
            rdUserTypeOperator.IsChecked = false;
            btnNew.IsEnabled = false;
            btnSave.Content = "Save";
        }
    }
}
