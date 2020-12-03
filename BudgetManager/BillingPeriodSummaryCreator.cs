using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BudgetManager
{
    class BillingPeriodSummaryCreator
    {
        Grid summary;
        BillingPeriod period;

        public BillingPeriodSummaryCreator(BillingPeriod bp, Grid s)
        {
            period = bp;
            summary = s;
            CreateSummary();
        }

        private void CreateSummary()
        {
            var net = period.netIncome;
            var add = period.additionalIncome;
            var incSum = period.netIncome + period.additionalIncome;
            var expSum = period.GetSumOfExpenses();
            var monthlyExpSum = period.GetSumOfMonthlyExpenses();
            var dailyExpSum = expSum - monthlyExpSum;
            var savings = period.plannedSavings;
            var balance = incSum - expSum - savings;
            var isActualBillingPeriod = (DateTime.Today - period.startDate).Days >= 0 && (period.endDate - DateTime.Today).Days >= 0;
            var daysLeft = (period.endDate - DateTime.Today).Days;
            var estimatedExpense = isActualBillingPeriod ? Math.Round(balance / daysLeft, 2) : Math.Round(balance, 2);

            foreach (var child in summary.Children)
            {
                if (child.GetType() == typeof(TextBlock) && ((TextBlock)child).Name != "")
                {
                    var textBlock = ((TextBlock)child);
                    switch (textBlock.Name)
                    {
                        case "NetIncomeTextBlock":
                            textBlock.Text = net.ToString("F") + " zł";
                            break;
                        case "AddIncomeTextBlock":
                            textBlock.Text = add.ToString("F") + " zł";
                            break;
                        case "IncomeSumTextBlock":
                            textBlock.Text = incSum.ToString("F") + " zł";
                            break;
                        case "IncomeSumTextBlock2":
                            textBlock.Text = incSum.ToString("F") + " zł";
                            break;
                        case "DailyExpensesSumTextBlock":
                            textBlock.Text = dailyExpSum.ToString("F") + " zł";
                            break;
                        case "MonthlyExpensesSumTextBlock":
                            textBlock.Text = monthlyExpSum.ToString("F") + " zł";
                            break;
                        case "ExpensesSumTextBlock":
                            textBlock.Text = expSum.ToString("F") + " zł";
                            break;
                        case "ExpensesSumTextBlock2":
                            textBlock.Text = expSum.ToString("F") + " zł";
                            break;
                        case "PlannedSavingsTextBlock":
                            textBlock.Text = savings.ToString("F") + " zł";
                            break;
                        case "BalanceTextBlock":
                            textBlock.Text = (balance > 0 ? "+" : "") + balance.ToString("F") + " zł";
                            break;
                        case "DaysLeftTextBlock":
                            textBlock.Text = isActualBillingPeriod ? daysLeft.ToString() : "-";
                            break;
                        case "EstimatedDailyExpenseTextBlock":
                            textBlock.Text = isActualBillingPeriod ? estimatedExpense.ToString("F") + " zł" : "-";
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
