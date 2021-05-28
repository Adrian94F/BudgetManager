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
                FillForExistingPeriod(p);
            }
            else
            {
                FillForNewPeriod();
            }
            SetupButtons();
        }

        private bool IsSaveable()
        {
            var startDate = StartDatePicker.SelectedDate;
            var endDate = EndDatePicker.SelectedDate;
            var netIncome = Utilities.ParseDecimalString(NetIncomeTextBox.Text);
            var addIncome = Utilities.ParseDecimalString(AddIncomeTextBox.Text);
            var plannedSavings = Utilities.ParseDecimalString(PlannedSavingsTextBox.Text);

            return (period != null &&
                    (startDate != period.startDate ||
                     endDate != period.endDate ||
                     netIncome != period.netIncome ||
                     addIncome != period.additionalIncome ||
                     plannedSavings != period.plannedSavings)) ||
                   (startDate != null && endDate != null);
        }

        private void SetupButtons()
        {
            DeleteButton.IsEnabled = period != null;
            SaveButton.IsEnabled = IsSaveable();
        }

        private void FillForExistingPeriod(BillingPeriod p)
        {
            StartDatePicker.SelectedDate = p.startDate;
            EndDatePicker.SelectedDate = p.endDate;
            NetIncomeTextBox.Text = p.netIncome.ToString("F");
            AddIncomeTextBox.Text = p.additionalIncome.ToString("F");
            PlannedSavingsTextBox.Text = p.plannedSavings.ToString("F");
        }

        private void FillForNewPeriod()
        {
            if (AppData.billingPeriods.Count > 0)
            {
                StartDatePicker.SelectedDate = AppData.billingPeriods.Last().endDate.AddDays(1);
                EndDatePicker.SelectedDate = AppData.billingPeriods.Last().NewPeriodEndDate();
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (period != null)
            {
                period.startDate = (DateTime) StartDatePicker.SelectedDate;
                period.endDate = (DateTime) EndDatePicker.SelectedDate;
                period.netIncome = Utilities.ParseDecimalString(NetIncomeTextBox.Text);
                period.additionalIncome = Utilities.ParseDecimalString(AddIncomeTextBox.Text);
                period.plannedSavings = Utilities.ParseDecimalString(PlannedSavingsTextBox.Text);

            }
            else
            {
                var period = new BillingPeriod()
                {
                    startDate = (DateTime) StartDatePicker.SelectedDate,
                    endDate = (DateTime) EndDatePicker.SelectedDate,
                    netIncome = Utilities.ParseDecimalString(NetIncomeTextBox.Text),
                    additionalIncome = Utilities.ParseDecimalString(AddIncomeTextBox.Text),
                    plannedSavings = Utilities.ParseDecimalString(PlannedSavingsTextBox.Text)
                };
                AppData.billingPeriods.Add(period);
            }

            AppData.isDataChanged = true;
            parent.Hide();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (period.IsEmpty())
            {
                AppData.billingPeriods.Remove(period);
                AppData.isDataChanged = true;
            }
            else
            {
                MessageBox.Show("Nie można usunąć miesiąca, gdyż nie jest pusty. Usuń wszystkie wydatki, a nastepnie spróbuj ponownie.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            parent.Hide();
        }

        private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetupButtons();
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            var value = Utilities.ParseDecimalString(tb.Text);
            tb.Text = value.ToString("F");
            SetupButtons();
        }
    }
}
