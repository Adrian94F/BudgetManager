﻿using System;
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

        BillingPeriod selectedPeriod = AppData.selectedPeriod;

        private void LoadData()
        {
            if (AppData.selectedPeriod != null)  // existing billing period
            {
                StartDatePicker.SelectedDate = selectedPeriod.startDate;
                EndDatePicker.SelectedDate = selectedPeriod.endDate;
                NetIncomeTextBox.Text = selectedPeriod.netIncome.ToString("F");
                AddIncomeTextBox.Text = selectedPeriod.additionalIncome.ToString("F");
                PlannedSavingsTextBox.Text = selectedPeriod.plannedSavings.ToString("F");
            }
            else  // new billing period
            {
                var startDate = AppData.billingPeriods.Last().endDate.AddDays(1);
                StartDatePicker.SelectedDate = startDate;
                var endDate = startDate.AddMonths(1);
                var typicalEndDay = (AppData.settings.TypicalBeginningOfPeriod - 1) % 31 + 1;
                var daysInMonth = DateTime.DaysInMonth(endDate.Year, endDate.Month);
                if (typicalEndDay > daysInMonth)
                {
                    typicalEndDay = daysInMonth;
                }
                endDate = new DateTime(endDate.Year, endDate.Month, typicalEndDay);
                EndDatePicker.SelectedDate = endDate;
                if (AppData.billingPeriods.Count > 0)
                {
                    NetIncomeTextBox.Text = AppData.billingPeriods.Last().netIncome.ToString("F");
                    PlannedSavingsTextBox.Text = AppData.billingPeriods.Last().plannedSavings.ToString("F");
                }
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

        public void ValueTextBox_LostFocus(object sender, EventArgs e)
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
            var plannedSavings = ParseDecimalString(PlannedSavingsTextBox.Text);
            
            if (AppData.selectedPeriod == null)  // new period
            {
                var period = new BillingPeriod()
                {
                    startDate = startDate,
                    endDate = endDate,
                    netIncome = netIncome,
                    additionalIncome = addIncome,
                    plannedSavings = plannedSavings
                };
                if (AppData.billingPeriods.Count > 0)
                {
                    period.expenses = AppData.billingPeriods.Last().GetCopyOfMonthlyExpensesForNextPeriod();
                }
                AppData.billingPeriods.Add(period);
            }
            else  // existing period
            {
                AppData.selectedPeriod.startDate = startDate;
                AppData.selectedPeriod.endDate = endDate;
                AppData.selectedPeriod.netIncome = netIncome;
                AppData.selectedPeriod.additionalIncome = addIncome;
                AppData.selectedPeriod.plannedSavings = plannedSavings;
            }
            AppData.isDataChanged = true;
            this.Close();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPeriod != null)
            {
                var result = MessageBox.Show("Czy na pewno chcesz usunąć ten okres rozliczeniowy? Nie będzie można cofnąć tej operacji.", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    result = MessageBox.Show("Czy jesteś pewien? Nie będzie można cofnąć tej operacji.", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        AppData.billingPeriods.Remove(selectedPeriod);
                        AppData.currentPeriod = AppData.billingPeriods.Count - 1;
                    }
                }
            }
            AppData.isDataChanged = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
