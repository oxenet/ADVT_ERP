using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ADVT_ERP
{
    /// <summary>
    /// Interaction logic for AgencyMaster.xaml
    /// </summary>
    public partial class MonthWiseReceipt : UserControl
    {
        private static bool firstrun = true;
        private string usertype = "";
        public MonthWiseReceipt(string usertype)
        {
            firstrun = true;
            InitializeComponent();
            firstrun = false;
            this.usertype = usertype;
            set_fyears();
            int fyear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            refreshGrid(fyear);
        }

        public void set_fyears()
        {
            IDictionary<int, string> fyears = new Dictionary<int, string>();
            for (int i = 2023; i <= Convert.ToInt32(DateTime.Now.ToString("yyyy")); i++)
            {
                fyears.Add(i, i.ToString());
            }

            drpFyears.ItemsSource = fyears;
            drpFyears.DisplayMemberPath = "Value";
            drpFyears.SelectedValuePath = "Key";
            drpFyears.SelectedIndex = 0;
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            int fyear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));

            try
            {
                fyear = Convert.ToInt32(drpFyears.SelectedValue);
            }
            catch { }

            refreshGrid(fyear);
        }

        public string build_query(int fyear = 0)
        {
            int febday = 0;

            string qry = @"select lc.LocationName, ag.AgencyName, sum(Apr) as April, sum(May) as May, sum(Jun) as June, sum(Jul) as July, sum(Aug) as Aug, sum(Sep) as September, sum(Oct) as October, sum(Nov) as Novomber, sum(Dece) as December, sum(Jan) as January, sum(Feb) as February, sum(Mar) as March  from (";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, coalesce(sum(Amount),0) as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-04-01' and '" + fyear + "-04-30' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, coalesce(sum(Amount),0) as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-05-01' and '" + fyear + "-05-31' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, coalesce(sum(Amount),0) as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-06-01' and '" + fyear + "-06-30' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, coalesce(sum(Amount),0) as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-07-01' and '" + fyear + "-07-31' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, coalesce(sum(Amount),0) as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-08-01' and '" + fyear + "-08-31' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, coalesce(sum(Amount),0) as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-09-01' and '" + fyear + "-09-30' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, coalesce(sum(Amount),0) as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-10-01' and '" + fyear + "-10-31' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, coalesce(sum(Amount),0) as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-11-01' and '" + fyear + "-11-30' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, coalesce(sum(Amount),0) as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-12-01' and '" + fyear + "-12-31' group by LocationId, AgencyId";

            qry += " union all ";

            fyear = fyear + 1;

            if (fyear % 4 == 0) { febday = 29; } else { febday = 28; }

            qry += "select LocationId, AgencyId, coalesce(sum(Amount),0) as Jan, 0 as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-01-01' and '" + fyear + "-01-31' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, coalesce(sum(Amount),0) as Feb, 0 as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-02-01' and '" + fyear + "-02-" + febday + "' group by LocationId, AgencyId";

            qry += " union all ";

            qry += "select LocationId, AgencyId, 0 as Jan, 0 as Feb, coalesce(sum(Amount),0) as Mar, 0 as Apr, 0 as May, 0 as Jun, 0 as Jul, 0 as Aug, 0 as Sep, 0 as Oct, 0 as Nov, 0 as Dece from PaymentReceipt where ReceiptDate between '" + fyear + "-03-01' and '" + fyear + "-03-31' group by LocationId, AgencyId";

            qry += ") iv left join Master_Agency ag on iv.AgencyId = ag.Id left join Master_Location lc on iv.LocationId = lc.Id group by lc.LocationName, ag.AgencyName";

            return qry;
        }

        public void refreshGrid(int fyear = 0)
        {
            if (fyear > 0)
            {
                string qry = build_query(fyear);

                try
                {
                    var dbo = new DataAccessLib();
                    DataTable data = dbo.GetDataTable(qry);
                    grdInvoiceList.ItemsSource = data.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {

                DataTable dataTable = new DataTable();

                int fyear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));

                try
                {
                    fyear = Convert.ToInt32(drpFyears.SelectedValue);
                }
                catch { }

                string qry = build_query(fyear);

                try
                {
                    var dbo = new DataAccessLib();
                    dataTable = dbo.GetDataTable(qry);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error !", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                StringBuilder csv = new StringBuilder();

                // Include column headers
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    csv.Append(dataTable.Columns[i].ColumnName.Replace(',', ':'));
                    if (i < dataTable.Columns.Count - 1)
                    {
                        csv.Append(",");
                    }
                }
                csv.AppendLine();

                // Include data rows
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        csv.Append(row[i].ToString().Replace(",", ";")); // simple handling of comma within text
                        if (i < dataTable.Columns.Count - 1)
                        {
                            csv.Append(",");
                        }
                    }
                    csv.AppendLine();
                }

                // Save csv to selected path
                File.WriteAllText(saveFileDialog.FileName, csv.ToString());
            }
        }        
    }
}
