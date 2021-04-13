using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for BillingPeriodsPage.xaml
    /// </summary>
    public partial class BillingPeriodsPage : Page
    {
        public BillingPeriodsPage()
        {
            InitializeComponent();
            FillTable();
        }

        public void FillTable()
        {
            var periods = AppData.billingPeriods.Reverse();
            var periodsCollection = new ObservableCollection<BillingPeriodDataItem>();
            
            foreach (var period in periods)
            {
                periodsCollection.Add(new BillingPeriodDataItem(period));
            }

            ExpensesDataGrid.ItemsSource = periodsCollection;
        }
    }

    class BillingPeriodDataItem
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string netIncome { get; set; }
        public string addIncome { get; set; }
        public string plannedSavings { get; set; }
        public BillingPeriod originalBillingPeriod;

        public BillingPeriodDataItem(BillingPeriod bp)
        {
            startDate = bp.startDate;
            endDate = bp.endDate;
            netIncome = Utilities.EmptyIfZero(bp.netIncome);
            addIncome = Utilities.EmptyIfZero(bp.additionalIncome);
            plannedSavings = Utilities.EmptyIfZero(bp.plannedSavings);
            originalBillingPeriod = bp;
        }
    }
}
