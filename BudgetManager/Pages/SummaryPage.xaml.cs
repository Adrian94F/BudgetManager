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

namespace BudgetManager.Pages
{
    /// <summary>
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SummaryPage : Page
    {
        public SummaryPage()
        {
            InitializeComponent();
            FillPage();
        }

        public void FillPage()
        {
            if (AppData.billingPeriods != null && AppData.billingPeriods.Count > 0)
            {
                var period = AppData.billingPeriods.ElementAt(AppData.currentPeriod);
                FillSummaryGrid(period);
                FillFrame(period);
            }
        }

        private void FillSummaryGrid(BillingPeriod period)
        {
            var startDate = period.startDate.ToString("dd.MM.yyyy");
            var endDate = period.endDate.ToString("dd.MM.yyyy");
            var net = period.netIncome;
            var add = period.additionalIncome;
            var incSum = period.netIncome + period.additionalIncome;
            var expSum = period.GetSumOfExpenses();
            var monthlyExpSum = period.GetSumOfMonthlyExpenses();
            var dailyExpSum = expSum - monthlyExpSum;
            var savings = period.plannedSavings;
            var balance = incSum - expSum - savings;
            var isActualBillingPeriod = (DateTime.Today - period.startDate).Days >= 0 && (period.endDate - DateTime.Today).Days >= 0;
            var daysLeft = (period.endDate - DateTime.Today).Days + 1;
            var estimatedExpense = isActualBillingPeriod ? Math.Round(balance / daysLeft, 2) : Math.Round(balance, 2);

            PeriodDatesTextBlock.Text = "Podsumowanie dla " + startDate + "-" + endDate + ":";
            NetIncomeTextBlock.Text = net.ToString("F") + " zł";
            AddIncomeTextBlock.Text = add.ToString("F") + " zł";
            IncomeSumTextBlock.Text = incSum.ToString("F") + " zł";
            IncomeSumTextBlock2.Text = incSum.ToString("F") + " zł";
            DailyExpensesSumTextBlock.Text = dailyExpSum.ToString("F") + " zł";
            MonthlyExpensesSumTextBlock.Text = monthlyExpSum.ToString("F") + " zł"; 
            ExpensesSumTextBlock.Text = expSum.ToString("F") + " zł";
            ExpensesSumTextBlock2.Text = expSum.ToString("F") + " zł";
            PlannedSavingsTextBlock.Text = savings.ToString("F") + " zł";
            BalanceTextBlock.Text = (balance > 0 ? "+" : "") + balance.ToString("F") + " zł";
            DaysLeftTextBlock.Text = isActualBillingPeriod ? daysLeft.ToString() : "-";
            EstimatedDailyExpenseTextBlock.Text = isActualBillingPeriod ? estimatedExpense.ToString("F") + " zł" : "-";
        }

        private void FillFrame(BillingPeriod period)
        {
            var page = new BurndownPage(true);
            Frame.Navigate(page);
        }
    }
}
