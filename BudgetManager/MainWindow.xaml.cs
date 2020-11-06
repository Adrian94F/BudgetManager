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
        private int currentPeriod;


        public MainWindow()
        {
            InitializeComponent();
            SetupVariables();
            PrintDataAsText();
            FillExpensesTable();
            FillSummaryTable();
            SetupButtons();
        }

        private void SetupButtons()
        {
            if (currentPeriod > 0)
            {
                EnableButton(BtnPrev);
            }
        }

        private void EnableButton(Button btn)
        {
            btn.IsEnabled = true;
            btn.Opacity = 1;
        }

        private void DisableButton(Button btn)
        {
            btn.IsEnabled = false;
            btn.Opacity = 0.5;
        }

        private void SetupVariables()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                currentPeriod = DataSet.billingPeriods.Count - 1;
            }
        }

        private void DataScrolViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            HeaderDaysScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            VerticalScrolViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }


        private void FillExpensesTable()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                Log("Wyświetlanie okresu rozliczeniowego w tabeli (początek: " + DataSet.billingPeriods.ElementAt(currentPeriod).startDate.ToString() + ")");
                BillingPeriodGridCreator.CreateMultiGridTable(HeaderDaysGrid, VerticalDataGrid, ExpensesGrid, DataSet.billingPeriods.ElementAt(currentPeriod));
            }
            else
            {
                Log("nie znaleziono okresu rozliczeniowego");
            }
        }

        private void FillSummaryTable()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                Log("Wyświetlanie podsumowania dla okresu rozliczeniowego w tabeli (początek: " + DataSet.billingPeriods.ElementAt(currentPeriod).startDate.ToString() + ")");
                BillingPeriodGridCreator.CreateSummary(SummaryGrid, DataSet.billingPeriods.ElementAt(currentPeriod));
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
            debugLogTextBlock.Text += txt + "\n";
            logScrollViewer.ScrollToEnd();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            currentPeriod--;
            if (currentPeriod == 0)
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
            currentPeriod++;
            if (currentPeriod == DataSet.billingPeriods.Count - 1)
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

        private void IncomeTextBox_ChangedOrLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            var strValue = Regex.Replace(txtBox.Text, "[^0-9,]", "");
            try
            {
                var value = decimal.Parse(strValue, NumberStyles.AllowCurrencySymbol | NumberStyles.Number);
                if (txtBox.Name == "NetIncomeTextBox")
                {
                    DataSet.billingPeriods.ElementAt(currentPeriod).netIncome = value;
                }
                else if (txtBox.Name == "AddIncomeTextBox")
                {
                    DataSet.billingPeriods.ElementAt(currentPeriod).additionalIncome = value;
                }

            } catch (Exception) {}
            FillSummaryTable();
        }
    }
}
