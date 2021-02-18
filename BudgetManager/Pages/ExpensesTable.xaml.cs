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
    /// Interaction logic for ExpensesTablePage.xaml
    /// </summary>
    public partial class ExpensesTablePage : Page
    {
        Window parent;

        public ExpensesTablePage(Window parentWindow)
        {
            parent = parentWindow;
            InitializeComponent();
            FillTable();
        }

        private void FillTable()
        {
            if (AppData.billingPeriods != null && AppData.billingPeriods.Count > 0)
            {
                _ = new BillingPeriodGridCreator(AppData.billingPeriods.ElementAt(AppData.currentPeriod), HeaderDaysGrid, VerticalDataGrid, ExpensesGrid);
            }
        }

        private void DataScrolViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            HeaderDaysScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            VerticalScrolViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void VerticalScrolViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            VerticalScrolViewer.ScrollToVerticalOffset(e.VerticalOffset);
            DataScrolViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void CategoriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (parent.GetType() == typeof(MainWindow))
                ((MainWindow)parent).Categories();
        }
    }
}
