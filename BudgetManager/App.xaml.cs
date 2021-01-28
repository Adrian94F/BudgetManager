using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private FilesHandler filesHandler;
        private void LoadData()
        {
            filesHandler = new FilesHandler();
            filesHandler.ReadData();
        }

        private void CheckIfNewPeriodIsNeeded()
        {
            if (AppData.billingPeriods.Count == 0)
                return;
            var lastPeriodEndDate = AppData.billingPeriods.Last().endDate;
            var todayDate = DateTime.Today;
            if (lastPeriodEndDate < todayDate)
            {
                var res = MessageBox.Show("Ostatni miesiąc się zakończył. Czy chcesz automatycznie utworzyć nowy?", "Koniec miesiąca!", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        CreateNextBillingPeriod();
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        Shutdown(0);
                        break;
                }
            }
        }

        private void CreateNextBillingPeriod()
        {
            var lastPeriod = AppData.billingPeriods.Last();
            var startDate = lastPeriod.endDate.AddDays(1);
            var endDate = startDate.AddMonths(1);
            var typicalEndDay = (AppData.settings.TypicalBeginningOfPeriod - 1) % 31 + 1;
            var daysInMonth = DateTime.DaysInMonth(endDate.Year, endDate.Month);
            if (typicalEndDay > daysInMonth)
            {
                typicalEndDay = daysInMonth;
            }
            endDate = new DateTime(endDate.Year, endDate.Month, typicalEndDay);
            decimal[] incomesAndSavings = { decimal.Zero, decimal.Zero, decimal.Zero };
            decimal[] prevIncomesAndSavings = { lastPeriod.netIncome, lastPeriod.additionalIncome, lastPeriod.plannedSavings };
            string[] types = { "przychód netto", "dodatkowy przychód", "planowane oszczędności" };
            for (var i = 0; i < 3; i++)
            {
                var res = MessageBox.Show("Czy chcesz przenieść " + types[i] + "?", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    incomesAndSavings[i] = prevIncomesAndSavings[i];
                }
            }
            
            var period = new BillingPeriod()
            {
                startDate = startDate,
                endDate = endDate,
                netIncome = incomesAndSavings[0],
                additionalIncome = incomesAndSavings[1],
                plannedSavings = incomesAndSavings[2]
            };
            if (AppData.billingPeriods.Count > 0)
            {
                period.expenses = AppData.billingPeriods.Last().GetCopyOfMonthlyExpensesForNextPeriod();
            }
            AppData.billingPeriods.Add(period);
            AppData.isDataChanged = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoadData();
            CheckIfNewPeriodIsNeeded();
        }
    }
}
