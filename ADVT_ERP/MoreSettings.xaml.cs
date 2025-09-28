using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for AgencyMaster.xaml
    /// </summary>
    public partial class MoreSettings : UserControl
    {
        public MoreSettings()
        {
            InitializeComponent();
            load_form_value();
            txtCGST.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string CGST = txtCGST.Text;
            string SGST = txtSGST.Text;

            IDictionary<string, string> col_val = new Dictionary<string, string>();

            col_val.Add("CGST", CGST);
            col_val.Add("SGST", SGST);

            string value =  JsonConvert.SerializeObject(col_val, Formatting.Indented);

            var dbo = new DataAccessLib();
            int rslt = 0;
            rslt = dbo.set_configuration("GSTConfig", value);

            if(rslt > 0)
            {
                MessageBox.Show("GST Tax Detail Saved Successfully", "Information !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("GST Tax Detail Could Not Be Save", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void load_form_value()
        {
            var dbo = new DataAccessLib();
            string configval = dbo.get_configuration("GSTConfig");
            if (configval.Length > 3)
            {
                IDictionary<string, string> config = new Dictionary<string, string>();
                config = JsonConvert.DeserializeObject<IDictionary<string, string>>(configval);

                txtCGST.Text = config["CGST"];
                txtSGST.Text = config["SGST"];
            }
        }
    }
}
