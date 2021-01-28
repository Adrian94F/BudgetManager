using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupVariables();
            RefreshTabControlContentAndSummary();
            SetupButtons();
            Closing += MainWindow_Closing;
        }

        public void RefreshTabControlContentAndSummary(bool includeHistory = true)
        {
            if (AppData.billingPeriods.Count == 0)
                return;
            FillTables();
            FillBurndownTab();
            if (includeHistory)
            {
                FillHistoryTab();
            }
            FillSummaryGrid();
        }

        private void FillTables()
        {
            FillExpensesTable();
            FillExpensesListTable();
        }

        private void FillBurndownTab()
        {
            _ = new BillingPeriodChartCreator(AppData.billingPeriods.ElementAt(AppData.currentPeriod), BurndownChartGrid);
        }

        private void FillHistoryTab()
        {
            _ = new BillingPeriodsHistoryChartCreator(AppData.billingPeriods, HistoryChartGrid);
        }

        private void SetupButtons()
        {
            if (AppData.currentPeriod > 0)
            {
                EnablMenuItem(MenuItemPrev);
            }
            else
            {
                DisableMenuItem(MenuItemPrev);
            }
            if (AppData.currentPeriod < AppData.billingPeriods.Count - 1)
            {
                EnablMenuItem(MenuItemNext);
            }
            else
            {
                DisableMenuItem(MenuItemNext);
            }
        }

        private void EnablMenuItem(MenuItem btn)
        {
            btn.IsEnabled = true;
        }

        private void DisableMenuItem(MenuItem btn)
        {
            btn.IsEnabled = false;
        }

        private void SetupVariables()
        {
            if (AppData.billingPeriods != null && AppData.billingPeriods.Count > 0)
            {
                AppData.currentPeriod = AppData.billingPeriods.Count - 1;
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

        public void FillExpensesTable()
        {
            if (AppData.billingPeriods != null && AppData.billingPeriods.Count > 0)
            {
                _ = new BillingPeriodGridCreator(AppData.billingPeriods.ElementAt(AppData.currentPeriod), this, HeaderDaysGrid, VerticalDataGrid, ExpensesGrid);
            }
        }

        public void FillExpensesListTable()
        {
            if (AppData.billingPeriods != null && AppData.billingPeriods.Count > 0)
            {
                var expensesList = new List<Expense>(AppData.billingPeriods.ElementAt(AppData.currentPeriod).expenses);
                _ = new BillingPeriodExpensesListCreator<MainWindow>(ExpensesListGrid, expensesList, this);
            }
        }

        public void FillSummaryGrid()
        {
            if (AppData.billingPeriods != null && AppData.billingPeriods.Count > 0)
            {
                _ = new BillingPeriodSummaryCreator(AppData.billingPeriods.ElementAt(AppData.currentPeriod), SummaryGrid);
            }
        }

        private void PrevMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PreviousBillingPeriod();
        }

        private void NextPeriodMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NextBillingPeriod();
        }

        private void AddExpenseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void ExpenseWindow_Closed(object sender, EventArgs e)
        {
            RefreshTabControlContentAndSummary();
        }

        private void CategoriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Categories();
        }

        private void CategoriesWindow_Closed(object sender, EventArgs e)
        {
            RefreshTabControlContentAndSummary();
        }

        private void PeriodsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            BillingPeriods();
        }

        private void PeriodsWindow_Closed(object sender, EventArgs e)
        {
            RefreshTabControlContentAndSummary();
            SetupButtons();
        }

        private void SaveData()
        {
            var fh = new FilesHandler();
            fh.SaveData();
            AppData.isDataChanged = false;
            MessageBox.Show("Pomyslnie zapisano wydatki!", "Zapis", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (AppData.isDataChanged)
            {
                var result = MessageBox.Show("Czy chcesz zapisać wprowadzone zmiany?", "Wyjście", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveData();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _ = Utilities.OpenNewOrRestoreWindow<SettingsWindow>();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BurndownTabItem.IsSelected && AppData.billingPeriods.Count > 0)
            {
                FillBurndownTab();
            }
        }

        private void Add()
        {
            AppData.selectedCategory = null;
            AppData.selectedDate = DateTime.Now;
            AppData.selectedExpense = null;
            var expenseWindowTuple = Utilities.OpenNewOrRestoreWindowAndCheckIfNew<ExpenseWindow>();
            if (expenseWindowTuple.Item2)
            {
                expenseWindowTuple.Item1.Closed += ExpenseWindow_Closed;
            }
        }

        private void Categories()
        {
            var categoriesWindowTuple = Utilities.OpenNewOrRestoreWindowAndCheckIfNew<CategoriesWindow>();
            if (categoriesWindowTuple.Item2)
            {
                categoriesWindowTuple.Item1.Closed += CategoriesWindow_Closed;
            }
        }

        private void BillingPeriods()
        {
            var periodsWindowTuple = Utilities.OpenNewOrRestoreWindowAndCheckIfNew<BillingPeriodsWindow>();
            if (periodsWindowTuple.Item2)
            {
                periodsWindowTuple.Item1.Closed += PeriodsWindow_Closed;
            }
        }

        private void NextBillingPeriod()
        {
            AppData.currentPeriod++;
            if (AppData.currentPeriod >= AppData.billingPeriods.Count - 1)
            {
                AppData.currentPeriod = AppData.billingPeriods.Count - 1;
                DisableMenuItem(MenuItemNext);
            }
            if (!MenuItemPrev.IsEnabled)
            {
                EnablMenuItem(MenuItemPrev);
            }
            RefreshTabControlContentAndSummary(false);
        }

        private void PreviousBillingPeriod()
        {
            AppData.currentPeriod--;
            if (AppData.currentPeriod <= 0)
            {
                AppData.currentPeriod = 0;
                DisableMenuItem(MenuItemPrev);
            }
            if (!MenuItemNext.IsEnabled)
            {
                EnablMenuItem(MenuItemNext);
            }
            RefreshTabControlContentAndSummary(false);
        }

        private void ClearViewMenuItemsSelection()
        {
            foreach (var item in ViewMenuItem.Items)
            {
                if (item != null && item.GetType() == typeof(MenuItem))
                {
                    ((MenuItem)item).IsChecked = false;
                }
            }
        }

        private void SelectedPeriodTableView(object sender, ExecutedRoutedEventArgs e)
        {
            ClearViewMenuItemsSelection();
            SetTab(0);
            SetSelectedPeriodTab(0);
        }

        private void SelectedPeriodListView(object sender, ExecutedRoutedEventArgs e)
        {
            ClearViewMenuItemsSelection();
            SetTab(0);
            SetSelectedPeriodTab(1);
        }

        private void SelectedPeriodBurndownView(object sender, ExecutedRoutedEventArgs e)
        {
            ClearViewMenuItemsSelection();
            SetTab(0);
            SetSelectedPeriodTab(2);
        }

        private void HistoryView(object sender, ExecutedRoutedEventArgs e)
        {
            ClearViewMenuItemsSelection();
            SetTab(1);
        }

        private void SettingsView(object sender, ExecutedRoutedEventArgs e)
        {
            ClearViewMenuItemsSelection();
            SetTab(2);
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var helpWindowTuple = Utilities.OpenNewOrRestoreWindowAndCheckIfNew<HelpWindow>();
            if (helpWindowTuple.Item2)
            {
                helpWindowTuple.Item1.Closed += PeriodsWindow_Closed;
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindowTuple = Utilities.OpenNewOrRestoreWindowAndCheckIfNew<AboutWindow>();
            if (aboutWindowTuple.Item2)
            {
                aboutWindowTuple.Item1.Closed += PeriodsWindow_Closed;
            }
        }

        private void SetTab(int n)
        {
            if (n > ViewsTabControl.Items.Count - 1)
            {
                throw new Exception();
            }
            ViewsTabControl.SelectedIndex = n;

            foreach (var item in ViewMenuItem.Items)
            {
                if (item.GetType() == typeof(MenuItem))
                {
                    ((MenuItem)item).IsChecked = false;
                }
            }

            if (n == 1)
            {
                ((MenuItem)ViewMenuItem.Items[ViewMenuItem.Items.Count - 1]).IsChecked = true;
            }
        }

        private void SetSelectedPeriodTab(int n)
        {
            if (n > SelectedPeriodTabControl.Items.Count - 1)
            {
                throw new Exception();
            }
            SelectedPeriodTabControl.SelectedIndex = n;

            var i = 0;
            foreach (var item in ViewMenuItem.Items)
            {
                if (item.GetType() == typeof(MenuItem))
                {
                    ((MenuItem)item).IsChecked = i == n;
                    i++;
                }
            }
        }
    }
}
