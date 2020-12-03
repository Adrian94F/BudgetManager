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
            this.Closing += MainWindow_Closing;
        }

        public void RefreshTabControlContentAndSummary(bool includeHistory = true)
        {
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
            _ = new BillingPeriodChartCreator(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod), BurndownChartGrid);
        }

        private void FillHistoryTab()
        {
            _ = new BillingPeriodsHistoryChartCreator(DataSet.billingPeriods, HistoryChartGrid);
        }

        private void SetupButtons()
        {
            if (DataSet.currentPeriod > 0)
            {
                EnablMenuItem(MenuItemPrev);
            }
            else
            {
                DisableMenuItem(MenuItemPrev);
            }
            if (DataSet.currentPeriod < DataSet.billingPeriods.Count - 1)
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
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                DataSet.currentPeriod = DataSet.billingPeriods.Count - 1;
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
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                _ = new BillingPeriodGridCreator(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod), this, HeaderDaysGrid, VerticalDataGrid, ExpensesGrid);
            }
        }

        public void FillExpensesListTable()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                _ = new BillingPeriodExpensesListCreator(ExpensesListGrid, new List<Expense>(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).expenses));
            }
        }

        public void FillSummaryGrid()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                _ = new BillingPeriodSummaryCreator(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod), SummaryGrid);
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
            MessageBox.Show("Pomyslnie zapisano wydatki!", "Zapis", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
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

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = Utilities.OpenNewOrRestoreWindow<SettingsWindow>();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = (TabControl)sender;
            if (BurndownTabItem.IsSelected)
            {
                FillBurndownTab();
            }
        }

        private void Add()
        {
            DataSet.selectedCategory = null;
            DataSet.selectedDate = DateTime.Now;
            DataSet.selectedExpense = null;
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
            DataSet.currentPeriod++;
            if (DataSet.currentPeriod >= DataSet.billingPeriods.Count - 1)
            {
                DataSet.currentPeriod = DataSet.billingPeriods.Count - 1;
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
            DataSet.currentPeriod--;
            if (DataSet.currentPeriod <= 0)
            {
                DataSet.currentPeriod = 0;
                DisableMenuItem(MenuItemPrev);
            }
            if (!MenuItemNext.IsEnabled)
            {
                EnablMenuItem(MenuItemNext);
            }
            RefreshTabControlContentAndSummary(false);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        Add();
                        break;
                    case Key.K:
                        Categories();
                        break;
                    case Key.M:
                        BillingPeriods();
                        break;
                    case Key.W:
                        SetViewTab(0);
                        break;
                    case Key.L:
                        SetViewTab(1);
                        break;
                    case Key.B:
                        SetViewTab(2);
                        break;
                    case Key.H:
                        SetViewTab(3);
                        break;
                    case Key.S:
                        SaveData();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        this.Close();
                        break;
                    case Key.PageDown:
                        NextBillingPeriod();
                        break;
                    case Key.PageUp:
                        PreviousBillingPeriod();
                        break;
                    default:
                        break;
                }
            }
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItems = ViewMenuItem.Items;

            foreach (var item in menuItems)
            {
                if (item != null && item.GetType() == typeof(MenuItem))
                {
                    ((MenuItem)item).IsChecked = sender == item;
                }
            }

            if (ViewTabControl == null)
            {
                return;
            }

            if (sender == ExpensesTableMenuItem)
            {
                ViewTabControl.SelectedIndex = 0;
            }
            else if (sender == ExpensesListMenuItem)
            {
                ViewTabControl.SelectedIndex = 1;
            }
            else if (sender == BurndownMenuItem)
            {
                ViewTabControl.SelectedIndex = 2;
            }
            else if (sender == HistoryMenuItem)
            {
                ViewTabControl.SelectedIndex = 3;
            }
        }

        private void SetViewTab(int n)
        {
            if (n > ViewTabControl.Items.Count - 1)
            {
                throw new Exception();
            }
            ViewTabControl.SelectedIndex = n;

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
