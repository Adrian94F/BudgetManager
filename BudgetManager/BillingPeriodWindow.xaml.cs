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
using System.Windows.Shapes;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for BillingPeriodWindow.xaml
    /// </summary>
    public partial class BillingPeriodWindow : Window
    {
        public BillingPeriodWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (DataSet.selectedPeriod != null)  // existing billing period
            {
                StartDatePicker.SelectedDate = DataSet.selectedPeriod.startDate;
                EndDatePicker.SelectedDate = DataSet.selectedPeriod.endDate;
                NetIncomeTextBox.Text = DataSet.selectedPeriod.netIncome.ToString("F");
                AddIncomeTextBox.Text = DataSet.selectedPeriod.additionalIncome.ToString("F");
            }
            else  // new billing period
            {
                var startDate = DataSet.billingPeriods.Last().endDate.AddDays(1);
                StartDatePicker.SelectedDate = startDate;
                var endDate = startDate.AddMonths(1);
                var typicalEndDay = 18;
                endDate = new DateTime(endDate.Year, endDate.Month, typicalEndDay);
                EndDatePicker.SelectedDate = endDate;
            }
        }

        private decimal ParseDecimalString(string str)
        {
            str = Regex.Replace(str, "[^0-9,]", "");
            decimal ret;
            try
            {
                ret = decimal.Parse(str, NumberStyles.AllowCurrencySymbol | NumberStyles.Number);
            }
            catch (Exception)
            {
                ret = Decimal.Zero;
            }
            return ret;
        }

        public void IncomeTextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            txtBox.Text = ParseDecimalString(txtBox.Text).ToString("F");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var startDate = (DateTime)StartDatePicker.SelectedDate;
            var endDate = (DateTime)EndDatePicker.SelectedDate;
            var netIncome = ParseDecimalString(NetIncomeTextBox.Text);
            var addIncome = ParseDecimalString(AddIncomeTextBox.Text);
            
            if (DataSet.selectedPeriod == null)  // new period
            {
                var period = new BillingPeriod()
                {
                    startDate = startDate,
                    endDate = endDate,
                    netIncome = netIncome,
                    additionalIncome = addIncome
                };
                DataSet.billingPeriods.Add(period);
            }
            else  // existing period
            {
                DataSet.selectedPeriod.startDate = startDate;
                DataSet.selectedPeriod.endDate = endDate;
                DataSet.selectedPeriod.netIncome = netIncome;
                DataSet.selectedPeriod.additionalIncome = addIncome;
            }
            this.Close();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (DataSet.selectedPeriod != null)
            {
                var result = MessageBox.Show("Czy na pewno chcesz usunąć ten okres rozliczeniowy? Nie będzie można cofnąć tej operacji.", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    result = MessageBox.Show("Czy jesteś pewien? Nie będzie można cofnąć tej operacji.", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        DataSet.billingPeriods.Remove(DataSet.selectedPeriod);
                        DataSet.currentPeriod = DataSet.billingPeriods.Count - 1;
                    }
                }
            }
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
