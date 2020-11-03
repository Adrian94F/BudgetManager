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

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BillingPeriod currentPeriod;


        public MainWindow()
        {
            InitializeComponent();
            SetupVariables();
            PrintDataAsText();
            PrintBillingPeriodTable();
        }


        private void SetupVariables()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                currentPeriod = DataSet.billingPeriods.Last();
            }
        }

        private void DataScrolViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            HeaderDaysScrolViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            VerticalScrolViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }


        private void PrintBillingPeriodTable()
        {
            if (DataSet.billingPeriods != null && DataSet.billingPeriods.Count > 0)
            {
                Log("Wyświetlanie okresu rozliczeniowego w tabeli (początek: " + currentPeriod.startDate.ToString() + ")");
                BillingPeriodGridCreator.CreateMultigridTable(HeaderDaysGrid, VerticalDataGrid, ExpensesGrid, currentPeriod);
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
    }
}
