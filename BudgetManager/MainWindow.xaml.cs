using System;
using System.Collections.Generic;
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
            FillExpensesTable();
            FillSummaryTable();
            SetupButtons();
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
                foreach (var exp in period.expenses)
                {
                    //str += " [" + exp.date.ToShortDateString() + ", " + exp.value.ToString() + "zł, " + exp.category.name + "]";
                    str += ".";
                }
                str += "\n";
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
            FillExpensesTable();
            FillSummaryTable();
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
            FillExpensesTable();
            FillSummaryTable();
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
            FillExpensesTable();
            FillSummaryTable();
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
            FillExpensesTable();
            FillSummaryTable();
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
            FillExpensesTable();
            FillSummaryTable();
        }
    }
}
