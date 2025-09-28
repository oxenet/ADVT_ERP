using System.Windows;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string usertype = "", userid = "", password = "";
        public MainWindow(string usertype, string userid, string password)
        {
            InitializeComponent();

            this.usertype = usertype;
            this.userid = userid;
            this.password = password;

            if(this.usertype != "ADMIN")
            {
                menuMastersDistrict.IsEnabled = false;
                menuMastersLocation.IsEnabled = false;
                menuMastersAgency.IsEnabled = false;
                menuMastersExecutive.IsEnabled = false;
                menuSettingsCompanyProfile.IsEnabled = false;
                menuSettingsDataSync.IsEnabled = false;
                menuSettingsMore.IsEnabled = false;
                menuManageUsers.IsEnabled = false;
            }            
        }

        private void mi_CreateBill_Click(object sender, RoutedEventArgs e)
        {
            CreateBill Child = new CreateBill(this.usertype);
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;      
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }
        
        private void mi_PaymentClearance_Click(object sender, RoutedEventArgs e)
        {
            BillClearance Child = new BillClearance();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;      
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }
        
        private void mi_CreateReceipt_Click(object sender, RoutedEventArgs e)
        {
            CreateReceipt Child = new CreateReceipt();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;      
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_ManageAgency_Click(object sender, RoutedEventArgs e)
        {
            AgencyMaster Child = new AgencyMaster();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_ManageDistrict_Click(object sender, RoutedEventArgs e)
        {
            DistrictMaster Child = new DistrictMaster();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_ManageLocation_Click(object sender, RoutedEventArgs e)
        {
            LocationMaster Child = new LocationMaster();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_ManageExecutive_Click(object sender, RoutedEventArgs e)
        {
            ExecutiveMaster Child = new ExecutiveMaster();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_DailyBillReport_Click(object sender, RoutedEventArgs e)
        {
            BillReport Child = new BillReport(this.usertype);
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        } 
        
        private void mi_DailyReceiptReport_Click(object sender, RoutedEventArgs e)
        {
            ReceiptReport Child = new ReceiptReport(this.usertype);
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        } 
        
        private void mi_AgencyLedger_Click(object sender, RoutedEventArgs e)
        {
            AgencyLedger Child = new AgencyLedger(this.usertype);
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        } 
        
        private void mi_MonthWiseSummary_Click(object sender, RoutedEventArgs e)
        {
            MonthWiseSummary Child = new MonthWiseSummary(this.usertype);
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }
        
        private void mi_MonthWiseInvoice_Click(object sender, RoutedEventArgs e)
        {
            MonthWiseInvoice Child = new MonthWiseInvoice(this.usertype);
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }
        
        private void mi_MonthWiseReceipt_Click(object sender, RoutedEventArgs e)
        {
            MonthWiseReceipt Child = new MonthWiseReceipt(this.usertype);
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_AboutADVTERP_Click(object sender, RoutedEventArgs e)
        {
            AboutADVTERP Child = new AboutADVTERP();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_UserManual_Click(object sender, RoutedEventArgs e)
        {
            UserManual Child = new UserManual();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_DatabaseSync_Click(object sender, RoutedEventArgs e)
        {
            DatabaseSync Child = new DatabaseSync();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_CompanyProfile_Click(object sender, RoutedEventArgs e)
        {
            CompanyProfile Child = new CompanyProfile();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }
        
        private void mi_MoreSettings_Click(object sender, RoutedEventArgs e)
        {
            MoreSettings Child = new MoreSettings();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }
        
        private void mi_DebitNote_Click(object sender, RoutedEventArgs e)
        {
            DebitNote Child = new DebitNote();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }
        
        private void mi_CreditNote_Click(object sender, RoutedEventArgs e)
        {
            CreditNote Child = new CreditNote();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_DrCrOptionMaster_Click(object sender, RoutedEventArgs e)
        {
            DrCrOptionMaster Child = new DrCrOptionMaster();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void mi_ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            ManageUsers Child = new ManageUsers();
            ContainerWindow.Children.Clear();
            object content = Child.Content;
            Child.Content = null;
            Child.Width = this.Width - 5;
            Child.Height = this.Height - 5;
            Child.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ContainerWindow.Children.Add(content as UIElement);
            btnClosePanel.Visibility = Visibility.Visible;
        }

        private void btnClosePanel_Click(object sender, RoutedEventArgs e)
        {
            ContainerWindow.Children.Clear();
            btnClosePanel.Visibility = Visibility.Hidden;
        }

    }
}
