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
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using Frame = System.Windows.Controls.Frame;
using Page = System.Windows.Controls.Page;

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

        private void ShowBillingPeriodFlyout(FrameworkElement objectToShowOn)
        {
            if (objectToShowOn == null)
                return;
            var periodFrame = new Frame();
            var flyout = new Flyout
            {
                Content = periodFrame,
                ShowMode = FlyoutShowMode.Standard,
                Placement = FlyoutPlacementMode.Bottom
            };
            var period = objectToShowOn.GetType() == typeof(DataGridRow)
                ? ((BillingPeriodDataItem)((DataGridRow)objectToShowOn).Item).originalBillingPeriod  // DataGridRow
                : null;  // Button etc.
            var periodPage = new BillingPeriodPage(flyout, period);
            periodFrame.Navigate(periodPage);
            flyout.Closed += (sender, o) => FillTable();
            flyout.ShowAt(objectToShowOn);
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowBillingPeriodFlyout((Button)sender);
        }

        private void CategoriesDataGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                e.OriginalSource as DependencyObject) as DataGridRow;
            ShowBillingPeriodFlyout(row);
        }

        private void CategoriesDataGrid_OnTouchUp(object sender, TouchEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                e.OriginalSource as DependencyObject) as DataGridRow;
            ShowBillingPeriodFlyout(row);
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
