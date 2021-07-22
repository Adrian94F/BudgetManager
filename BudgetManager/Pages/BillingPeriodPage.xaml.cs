using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BudgetManager.Models;
using ModernWpf.Controls;
using Page = System.Windows.Controls.Page;

namespace BudgetManager.Pages
{
    /// <summary>
    /// Interaction logic for BillingPeriodPage.xaml
    /// </summary>
    public partial class BillingPeriodPage : Page
    {
        private ContentDialog parent;
        private BillingPeriod period;

        public BillingPeriodPage(ContentDialog dialog, BillingPeriod p)
        {
            InitializeComponent();
            parent = dialog;
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

        private void SetupButtons()
        {
            DeleteButton.IsEnabled = period != null;
        }

        private void FillTable(BillingPeriod p)
        {
            var incomesCollection = new ObservableCollection<IncomeDataItem>();
            foreach (var inc in p.incomes)
            {
                incomesCollection.Add(new IncomeDataItem(inc));
            }
            IncomesDataGrid.ItemsSource = incomesCollection;
        }

        private void FillForExistingPeriod(BillingPeriod p)
        {
            StartDatePicker.SelectedDate = p.startDate;
            EndDatePicker.SelectedDate = p.endDate;
            PlannedSavingsTextBox.Text = p.plannedSavings.ToString("F");
            FillTable(p);
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
            bool newPeriod = period == null;

            if (newPeriod)
                period = new BillingPeriod();

            period.startDate = (DateTime)StartDatePicker.SelectedDate;
            period.endDate = (DateTime)EndDatePicker.SelectedDate;
            period.plannedSavings = Utilities.ParseDecimalString(PlannedSavingsTextBox.Text);

            var incomes = new HashSet<Income>();
            foreach (var inc in IncomesDataGrid.Items)
            {
                incomes.Add(new Income()
                {
                    value = Convert.ToDecimal(((IncomeDataItem)inc).value),
                    type = ((IncomeDataItem)inc).isSalary
                        ? Income.IncomeType.Salary
                        : Income.IncomeType.Additional,
                    comment = ((IncomeDataItem)inc).comment
                });
            }
            period.incomes = incomes;

            if (newPeriod)
                AppData.billingPeriods.Add(period);

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

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            parent.Hide();
        }

        private void AddIncomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var newIncome = new IncomeDataItem(new Income());
            ObservableCollection<IncomeDataItem> data = (ObservableCollection<IncomeDataItem>)IncomesDataGrid.ItemsSource;
            data.Add(newIncome);
            IncomesDataGrid.ItemsSource = data;
        }

        private void DeleteIncomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedIdx = IncomesDataGrid.SelectedIndex;
            if (selectedIdx >= 0)
            {
                var row = (IncomeDataItem)IncomesDataGrid.SelectedItems[0];

                ObservableCollection<IncomeDataItem> data = (ObservableCollection<IncomeDataItem>)IncomesDataGrid.ItemsSource;
                data.Remove(row);
                IncomesDataGrid.ItemsSource = data;
            }
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

    class IncomeDataItem
    {
        public double value { get; set; }
        public bool isSalary { get; set; }
        public string comment { get; set; }

        public IncomeDataItem(Income inc)
        {
            value = Convert.ToDouble(inc.value);
            isSalary = inc.type == Income.IncomeType.Salary;
        }
    }
}
