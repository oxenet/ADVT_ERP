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
    public partial class CompanyProfile : UserControl
    {
        public CompanyProfile()
        {
            InitializeComponent();
            load_form_value();
            txtCompanyName.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string CompanyName = txtCompanyName.Text;
            string Address = txtAddress.Text;
            string ContactNum = txtContactNum.Text;
            string EmailId = txtEmailId.Text;
            string DavpCode = txtDavpCode.Text;
            string DprCode = txtDprCode.Text;
            string PAN = txtPAN.Text;
            string GSTN = txtGSTN.Text;
            string ACHolder = txtACHolder.Text;
            string BankName = txtBankName.Text;
            string IFSC = txtIFSC.Text;
            string ACNum = txtACNum.Text;
            string LogoPathTemp = txtLogoTempPath_hidden.Text;
            string LogoPath = txtLogoPath_hidden.Text;

            IDictionary<string, string> col_val = new Dictionary<string, string>();

            col_val.Add("CompanyName", CompanyName);
            col_val.Add("Address", Address);
            col_val.Add("ContactNum", ContactNum);
            col_val.Add("EmailId", EmailId);
            col_val.Add("DavpCode", DavpCode);
            col_val.Add("DprCode", DprCode);
            col_val.Add("PAN", PAN);
            col_val.Add("GSTN", GSTN);
            col_val.Add("ACHolder", ACHolder);
            col_val.Add("BankName", BankName);
            col_val.Add("IFSC", IFSC);
            col_val.Add("ACNum", ACNum);

            if (LogoPathTemp.Length > 3)
            {
                try
                {
                    Random res = new Random();
                    String str = "abcdefghijklmnopqrstuvwxyz0123456789";
                    int size = 8;
                    String randomstring = "";
                    for (int i = 0; i < size; i++)
                    {
                        int x = res.Next(str.Length);
                        randomstring = randomstring + str[x];
                    }
                    string ext = Path.GetExtension(LogoPathTemp);
                    string lp = @"assets\\Logo_" + randomstring + ext;
                    File.Copy(LogoPathTemp, lp);
                    LogoPath = lp;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            col_val.Add("LogoPath", LogoPath);

            string value =  JsonConvert.SerializeObject(col_val, Formatting.Indented);

            var dbo = new DataAccessLib();
            int rslt = 0;
            rslt = dbo.set_configuration("CompanyProfile", value);

            if(rslt > 0)
            {
                MessageBox.Show("Company Profile Saved Successfully", "Information !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Company Profile Could Not Be Save", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void load_form_value()
        {
            var dbo = new DataAccessLib();
            string configval = dbo.get_configuration("CompanyProfile");
            if (configval.Length > 3)
            {
                IDictionary<string, string> config = new Dictionary<string, string>();
                config = JsonConvert.DeserializeObject<IDictionary<string, string>>(configval);

                txtCompanyName.Text = config["CompanyName"];
                txtAddress.Text = config["Address"];
                txtContactNum.Text = config["ContactNum"];
                txtEmailId.Text = config["EmailId"];
                txtDavpCode.Text = config["DavpCode"];
                txtDprCode.Text = config["DprCode"];
                txtPAN.Text = config["PAN"];
                txtGSTN.Text = config["GSTN"];
                txtACHolder.Text = config["ACHolder"];
                txtBankName.Text = config["BankName"];
                txtIFSC.Text = config["IFSC"];
                txtACNum.Text = config["ACNum"];
                txtLogoPath_hidden.Text = config["LogoPath"];

                if (File.Exists(config["LogoPath"]))
                {
                    FileInfo LogoPath = new FileInfo(config["LogoPath"]);
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri(LogoPath.FullName);
                    logo.EndInit();
                    imgLogo.Source = logo;
                }
            }
        }

        private void btnBrowseLogo_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "Image Files |*.png;*.jpeg;*.jpg";
            openFileDlg.Multiselect = false;
            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {
                txtLogoTempPath_hidden.Text = openFileDlg.FileName;
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(openFileDlg.FileName);
                logo.EndInit();
                imgLogo.Source = logo;
            }
        }

        private void btnRemoveLogo_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show("Are you sure to Remove Logo ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                txtLogoPath_hidden.Text = "";
                imgLogo.Source = null;
            }
        }
    }
}
