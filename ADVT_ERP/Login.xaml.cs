using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {        
        public Login()
        {
            InitializeComponent();
            load_default_val();
            MouseDown += Window_MouseDown;
        }

        public void load_default_val()
        {
            var dbo = new DataAccessLib();
            string configval = dbo.get_configuration("CompanyProfile");
            if (configval.Length > 3)
            {
                IDictionary<string, string> config = new Dictionary<string, string>();
                config = JsonConvert.DeserializeObject<IDictionary<string, string>>(configval);

                lblCompanyName.Content = config["CompanyName"];

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

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            string userid = txtLoginId.Text;

            string password = txtLoginPass.Password;

            lblWait.Visibility = Visibility.Visible;

            btnSignIn.Visibility = Visibility.Hidden;

            if (userid != "" && password != "")
            {
                                
                try
                {
                    var dbo = new DataAccessLib();

                    IDictionary<string, string> userdetail = dbo.get_user_detail(userid, password);

                    if (userdetail.ContainsKey("UserId") && userdetail.ContainsKey("Password") && userdetail.ContainsKey("UserType") && userdetail.ContainsKey("UserName"))
                    {
                        var mainWindow = new MainWindow(userdetail["UserType"], userdetail["UserId"], userdetail["Password"]);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Userid and Password.");
                    }
                }
                catch (WebException ex)
                {
                    MessageBox.Show("Exception: " + ex.Message, "Error");
                }

                /////////////////    
            }
            else
            {
                MessageBox.Show("Please Provide Userid and Password");
            }

            lblWait.Visibility = Visibility.Hidden;

            btnSignIn.Visibility = Visibility.Visible;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = System.Windows.MessageBox.Show("Are you sure to Close ?", "Close Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
                
        }
    }
}
