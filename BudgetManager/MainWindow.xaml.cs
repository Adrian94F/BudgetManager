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
        private BillingPeriod currentPeriod = DataSet.billingPeriods.Last();


        public MainWindow()
        {
            InitializeComponent();
            PrintDataAsText();
            PrintBillingPeriodTable();
        }


        private void PrintBillingPeriodTable()
        {
            log("Wyświetlanie okresu rozliczeniowego w tabeli (początek: " + currentPeriod.startDate.ToString() + ")");
            dataGrid = currentPeriod.GetGrid();
        }

        private void PrintDataAsText()
        {
            var str = "";

            // print categories
            str += "Liczba kategorii: " + Convert.ToString(DataSet.expenseCategories.Count) + "\n";
            foreach (var category in DataSet.expenseCategories)
            {
                str += category.name + "   ";
            }
            str += "\n\n";

            // print periods
            str += "Liczba okresów rozliczeniowych: " + Convert.ToString(DataSet.billingPeriods.Count) + "\n";
            foreach (var period in DataSet.billingPeriods)
            {
                str += " " + period.startDate + ", dochód: " + period.netIncome.ToString() + "zł (+" + period.additionalIncome.ToString() + "zł)\n";
                foreach (var exp in period.expenses)
                {
                    str += "  " + exp.date + " " + exp.value.ToString() + "zł (" + exp.category.name + ")\n";
                }
            }

            log(str);
        }

        private void log(string txt)
        {
            debugLogTextBlock.Text += txt + "\n";
            logScrollViewer.ScrollToEnd();
        }
    }
}
