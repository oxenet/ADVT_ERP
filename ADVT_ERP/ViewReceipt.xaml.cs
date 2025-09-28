using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for ViewInvoice.xaml
    /// </summary>
    public partial class ViewReceipt : Window
    {
        public ViewReceipt(string InvoiceNo = "")
        {
            InitializeComponent();

            set_invoice_data(InvoiceNo);
        }

        public void set_invoice_data(string InvoiceNo)
        {
            var dbo = new DataAccessLib();
            string configval = dbo.get_configuration("CompanyProfile");
            if (configval.Length > 3)
            {
                IDictionary<string, string> config = new Dictionary<string, string>();
                config = JsonConvert.DeserializeObject<IDictionary<string, string>>(configval);

                CompanyName.Inlines.Add(config["CompanyName"]);
                CompanyNameSeal.Inlines.Add(config["CompanyName"]);
                CompanyAddress.Inlines.Add(config["Address"]);
                CompanyContacts.Inlines.Add(config["ContactNum"]+", "+ config["EmailId"]);
                DAVPCode.Inlines.Add(config["DavpCode"]);
                DPRCode.Inlines.Add(config["DprCode"]);
                PAN.Inlines.Add(config["PAN"]);
                GSTN.Inlines.Add(config["GSTN"]);
                ACHolder.Inlines.Add(config["ACHolder"]);
                BankName.Inlines.Add(config["BankName"]);
                IFSC.Inlines.Add(config["IFSC"]);
                ACNum.Inlines.Add(config["ACNum"]);

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

            IDictionary<string, string> InvoiceDetail = dbo.get_invoice_detail(InvoiceNo);

            AgencyName.Inlines.Add(InvoiceDetail["AgencyName"]);
            InvoiceNum.Inlines.Add(InvoiceDetail["InvoiceNo"]);
            PublishDate.Inlines.Add(InvoiceDetail["PublishDate"]);
            InvoiceDate.Inlines.Add(InvoiceDetail["InvoiceDate"]);
            RONum.Inlines.Add(InvoiceDetail["RONum"]);
            LocationName.Inlines.Add(InvoiceDetail["LocationName"]);
            ClientName.Inlines.Add(InvoiceDetail["ClientName"]);
            AgencyGSTN.Inlines.Add(InvoiceDetail["AgencyGSTN"]);
            AdWidth.Inlines.Add(InvoiceDetail["AdWidth"]);
            AdHeight.Inlines.Add(InvoiceDetail["AdHeight"]);
            AdSpace.Inlines.Add(InvoiceDetail["AdSpace"]);
            Rate.Inlines.Add(InvoiceDetail["Rate"]);
            AgreedAmount.Inlines.Add(InvoiceDetail["AgreedAmount"]);
            Remark.Inlines.Add("Remark: " + InvoiceDetail["Remark"]);
            Amount.Inlines.Add(InvoiceDetail["Amount"]);
            CGST.Inlines.Add(InvoiceDetail["CGST"]);
            SGST.Inlines.Add(InvoiceDetail["SGST"]);
            NetAmount.Inlines.Add(InvoiceDetail["NetAmount"]);
            PackageName.Inlines.Add(InvoiceDetail["PackageName"]);
            
            if(InvoiceDetail["ColourScheme"] == "B")
            {
                ColourScheme.Inlines.Add("B/W");
            }
            else if(InvoiceDetail["ColourScheme"] == "C")
            {
                ColourScheme.Inlines.Add("Colour");
            }
            else
            {
                ColourScheme.Inlines.Add("");
            }

            float AmountNumber = 0;

            try { float.TryParse(InvoiceDetail["NetAmount"], out AmountNumber); } catch { }

            string wrd = NumberToWords.ConvertAmount(AmountNumber);

            AmountWord.Inlines.Add("Amount in Words : "+wrd);

        }

        private void btnPrintInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = true;
                btnPrintInvoice.Visibility = Visibility.Hidden;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(ToBePrint, "Invoice");
                }
                btnPrintInvoice.Visibility = Visibility.Visible;
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }
}
