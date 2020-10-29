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
        public MainWindow()
        {
            InitializeComponent();
            PrintDataAsText();
        }

        private void PrintDataAsText()
        {
            var str = "";

            // print categories
            str += "Liczba kategorii: " + Convert.ToString(DataSet.expenseCategories.Count) + "\n";
            var count = 0;
            foreach (var category in DataSet.expenseCategories)
            {
                str += "    " + Convert.ToString(++count) + ". " + category.name + "\n";
            }

            // print periods
            str += "\nLiczba okresów rozliczeniowych: " + Convert.ToString(DataSet.billingPeriods.Count) + "\n";
            count = 0;
            foreach (var period in DataSet.billingPeriods)
            {
                str += "    " + Convert.ToString(++count) + ". od " + period.startDate + ", dochód: " + period.netIncome.ToString() + "zł (+" + period.additionalIncome.ToString() + "zł)\n";
                foreach (var exp in period.expenses)
                {
                    str += "            " + exp.date + " " + exp.value.ToString() + "zł (" + exp.category.name + ")\n";
                }
            }

            debug_log_text_block.Text += str;
        }
    }
}
