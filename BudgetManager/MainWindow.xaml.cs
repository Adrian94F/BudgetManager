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
        BillingPeriodGridCreator gridCreator;
        BillingPeriodChartCreator chartCreator;
        public MainWindow()
        {
            InitializeComponent();
            SetupVariables();
            SetupGridCreator();
            RefreshTabs();
            SetupButtons();
            this.Closing += MainWindow_Closing;
        }

        private void FillBurndownTab()
        {
            chartCreator = new BillingPeriodChartCreator(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod), BurndownChartGrid);
            chartCreator.Plot();
        }

        public void RefreshTabs()
        {
            FillSummaryTab();
            FillBurndownTab();
        }

        private void FillSummaryTab()
        {
            FillExpensesTable();
            FillSummaryTable();
            PrintBillingPeriodDates();
        }

        private void PrintBillingPeriodDates()
        {
            var start = DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).startDate.ToString("dd.MM.yyyy");
            var end = DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).endDate.ToString("dd.MM.yyyy");
            PeriodDatesTextBlock.Text = start + "-" + end;
        }

        private void SetupGridCreator()
        {
            gridCreator = new BillingPeriodGridCreator(this);
            gridCreator.SetGrids(HeaderDaysGrid, VerticalDataGrid, ExpensesGrid, SummaryGrid, ExpensesListGrid);
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
                gridCreator.SetPeriod(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod));
                gridCreator.CreateMultiGridTable();
            }
        }

        public void FillSummaryTable()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                gridCreator.SetPeriod(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod));
                gridCreator.CreateSummary();
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

        private void IncomeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            var strValue = Regex.Replace(txtBox.Text, "[^0-9,]", "");
            try
            {
                var value = decimal.Parse(strValue, NumberStyles.AllowCurrencySymbol | NumberStyles.Number);
                if (txtBox.Name == "NetIncomeTextBox")
                {
                    DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).netIncome = value;
                }
                else if (txtBox.Name == "AddIncomeTextBox")
                {
                    DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).additionalIncome = value;
                }

            } catch (Exception) {
                txtBox.Text = "!!!" + txtBox.Text;
            }
            RefreshTabs();
        }

        private void AddExpenseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void ExpenseWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            RefreshTabs();
        }

        private void CategoriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Categories();
        }

        private void CategoriesWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            RefreshTabs();
        }

        private void PeriodsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            BillingPeriods();
        }

        private void PeriodsWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            RefreshTabs();
            SetupButtons();
        }

        private void SaveData()
        {
            var fh = new FilesHandler();
            fh.SaveData();
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
            this.IsEnabled = false;
            var settingsWindow = new SettingsWindow();
            settingsWindow.Closed += SettingsWindow_Closed;
            settingsWindow.Show();
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
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
            this.IsEnabled = false;
            var expenseWindow = new ExpenseWindow();
            expenseWindow.Closed += ExpenseWindow_Closed;
            expenseWindow.Show();
        }

        private void Categories()
        {
            this.IsEnabled = false;
            var categoriesWindow = new CategoriesWindow();
            categoriesWindow.Closed += CategoriesWindow_Closed;
            categoriesWindow.Show();
        }

        private void BillingPeriods()
        {
            this.IsEnabled = false;
            var periodsWindow = new BillingPeriodsWindow();
            periodsWindow.Closed += PeriodsWindow_Closed;
            periodsWindow.Show();
        }

        private void NextBillingPeriod()
        {
            DataSet.currentPeriod++;
            if (DataSet.currentPeriod == DataSet.billingPeriods.Count - 1)
            {
                DisableMenuItem(MenuItemNext);
            }
            if (!MenuItemPrev.IsEnabled)
            {
                EnablMenuItem(MenuItemPrev);
            }
            RefreshTabs();
        }

        private void PreviousBillingPeriod()
        {
            DataSet.currentPeriod--;
            if (DataSet.currentPeriod == 0)
            {
                DisableMenuItem(MenuItemPrev);
            }
            if (!MenuItemNext.IsEnabled)
            {
                EnablMenuItem(MenuItemNext);
            }
            RefreshTabs();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;
                case Key.N:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        Add();
                    }
                    break;
                case Key.K:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        Categories();
                    }
                    break;
                case Key.M:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        BillingPeriods();
                    }
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

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var helpWindow = new HelpWindow();
            helpWindow.Closed += PeriodsWindow_Closed;
            helpWindow.Show();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var aboutWindow = new AboutWindow();
            aboutWindow.Closed += PeriodsWindow_Closed;
            aboutWindow.Show();
        }
    }
}
