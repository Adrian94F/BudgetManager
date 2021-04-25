using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernWpf.Controls;
using Page = System.Windows.Controls.Page;

namespace BudgetManager.Pages
{
    /// <summary>
    /// Interaction logic for BillingPeriodPage.xaml
    /// </summary>
    public partial class BillingPeriodPage : Page
    {
        private Flyout parent;
        private BillingPeriod period;

        public BillingPeriodPage(Flyout fylout, BillingPeriod p)
        {
            InitializeComponent();
            parent = fylout;
            period = p;

            if (p != null)
            {
                StartDatePicker.SelectedDate = p.startDate;
                EndDatePicker.SelectedDate = p.endDate;
                NetIncomeTextBox.Text = p.netIncome.ToString("F");
                AddIncomeTextBox.Text = p.additionalIncome.ToString("F");
                PlannedSavingsTextBox.Text = p.plannedSavings.ToString("F");
            }
        }
    }
}
