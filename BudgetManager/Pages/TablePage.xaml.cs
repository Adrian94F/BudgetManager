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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BudgetManager.Pages
{
    /// <summary>
    /// Interaction logic for TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        public TablePage()
        {
            InitializeComponent();
            FillTable();
        }

        public void FillTable()
        {
            if (AppData.billingPeriods != null && AppData.billingPeriods.Count > 0)
            {
                var period = AppData.billingPeriods.ElementAt(AppData.currentPeriod);
                Table?.FillTable(period);
            }
        }
    }
}
