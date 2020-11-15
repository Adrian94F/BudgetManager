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
        public MainWindow()
        {
            InitializeComponent();
            SetupVariables();
            SetupGridCreator();
            PrintDataAsText();
            FillSummaryTab();
            SetupButtons();
            this.Closing += MainWindow_Closing;
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
            gridCreator.SetGrids(HeaderDaysGrid, VerticalDataGrid, ExpensesGrid, SummaryGrid);
        }

        private void SetupButtons()
        {
            if (DataSet.currentPeriod > 0)
            {
                EnableButton(BtnPrev);
            }
            else
            {
                DisableButton(BtnPrev);
            }
            if (DataSet.currentPeriod < DataSet.billingPeriods.Count - 1)
            {
                EnableButton(BtnNext);
            }
            else
            {
                DisableButton(BtnNext);
            }
        }

        private void EnableButton(Button btn)
        {
            btn.IsEnabled = true;
        }

        private void DisableButton(Button btn)
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


        public void FillExpensesTable()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                Log("Wyświetlanie okresu rozliczeniowego w tabeli (początek: " + DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).startDate.ToString() + ")");
                gridCreator.SetPeriod(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod));
                gridCreator.CreateMultiGridTable();
            }
            else
            {
                Log("nie znaleziono okresu rozliczeniowego");
            }
        }

        public void FillSummaryTable()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                Log("Wyświetlanie podsumowania dla okresu rozliczeniowego w tabeli (początek: " + DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).startDate.ToString() + ")");
                gridCreator.SetPeriod(DataSet.billingPeriods.ElementAt(DataSet.currentPeriod));
                gridCreator.CreateSummary();
            }
            else
            {
                Log("nie znaleziono okresu rozliczeniowego");
            }
        }

        private void PrintDataAsText()
        {
            var str = "";

            // print categories
            str += "Liczba kategorii: " + Convert.ToString(DataSet.expenseCategories.Count) + "\n";
            foreach (var category in DataSet.expenseCategories)
            {
                str += " [" + category.name + "]";
            }
            str += "\n\n";

            // print periods
            str += "Liczba okresów rozliczeniowych: " + Convert.ToString(DataSet.billingPeriods.Count) + "\n";
            foreach (var period in DataSet.billingPeriods)
            {
                str += " " + period.startDate.ToShortDateString() + "-" + period.endDate.ToShortDateString() + " (" + ((period.endDate - period.startDate).Days + 1) + " dni), dochód: " + period.netIncome.ToString() + "zł (+" + period.additionalIncome.ToString() + "zł)\n  ";
                if (period.expenses != null)
                {
                    foreach (var exp in period.expenses)
                    {
                        //str += " [" + exp.date.ToShortDateString() + ", " + exp.value.ToString() + "zł, " + exp.category.name + "]";
                        str += ".";
                    }
                    str += "\n";
                }
            }

            Log(str);
        }

        public void Log(string txt)
        {
            //debugLogTextBlock.Text += txt + "\n";
            //logScrollViewer.ScrollToEnd();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            DataSet.currentPeriod--;
            if (DataSet.currentPeriod == 0)
            {
                DisableButton(BtnPrev);
            }
            if (!BtnNext.IsEnabled)
            {
                EnableButton(BtnNext);
            }
            FillSummaryTab();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            DataSet.currentPeriod++;
            if (DataSet.currentPeriod == DataSet.billingPeriods.Count - 1)
            {
                DisableButton(BtnNext);
            }
            if (!BtnPrev.IsEnabled)
            {
                EnableButton(BtnPrev);
            }
            FillSummaryTab();
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
            FillSummaryTable();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            DataSet.selectedCategory = null;
            DataSet.selectedDate = DateTime.Now;
            DataSet.selectedExpense = null;
            this.IsEnabled = false;
            var expenseWindow = new ExpenseWindow();
            expenseWindow.Closed += ExpenseWindow_Closed;
            expenseWindow.Show();
        }

        private void ExpenseWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            FillSummaryTab();
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var categoriesWindow = new CategoriesWindow();
            categoriesWindow.Closed += CategoriesWindow_Closed;
            categoriesWindow.Show();
        }

        private void CategoriesWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            FillSummaryTab();
        }

        private void BtnPeriods_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var periodsWindow = new BillingPeriodsWindow();
            periodsWindow.Closed += PeriodsWindow_Closed;
            periodsWindow.Show();
        }

        private void PeriodsWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            FillSummaryTab();
            SetupButtons();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Czy chcesz zapisać wprowadzone zmiany?", "Wyjście", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    var fh = new FilesHandler();
                    fh.SaveData();
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
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
    }
}
