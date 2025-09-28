using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for CancelInvoice.xaml
    /// </summary>
    public partial class CancelInvoice : Window
    {
        private string InvoiceNo = "";
        public CancelInvoice(string InvoiceNo)
        {
            InitializeComponent();
            this.InvoiceNo = InvoiceNo;
            set_invoice_data();
        }

        private void set_invoice_data()
        {
            var dbo = new DataAccessLib();

            IDictionary<string, string> InvoiceDetail = dbo.get_invoice_detail(this.InvoiceNo);

            lblInvoiceNo.Content = InvoiceDetail["InvoiceNo"];
            lblInvoiceDate.Content = InvoiceDetail["InvoiceDate"];
            lblPublishDate.Content = InvoiceDetail["PublishDate"];
            lblAgency.Content = InvoiceDetail["AgencyName"];
            lblAmount.Content = InvoiceDetail["NetAmount"];
            lblClient.Content = InvoiceDetail["ClientName"];
        }

        private void btnCancelInvoice_Click(object sender, RoutedEventArgs e)
        {
            string CancelRemark = new TextRange(txtCancelRemark.Document.ContentStart, txtCancelRemark.Document.ContentEnd).Text;
            
            if(CancelRemark.Length > 4)
            {
                var dbo = new DataAccessLib();

                int rslt = dbo.cancel_invoice(this.InvoiceNo, CancelRemark);

                if(rslt > 0)
                {
                    MessageBox.Show("Invoice Has Been Canceled", "Information !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invoice Could Not Be Cancele, Please Try Again", "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Remark is Compulsory - At Least 5 characters long", "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
