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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernWpf.Controls;
using Page = System.Windows.Controls.Page;

namespace BudgetManager.Pages
{
    /// <summary>
    /// Interaction logic for ExpensePage.xaml
    /// </summary>
    public partial class ExpensePage : Page
    {
        public Flyout parent;
        public ExpenseCategory category;
        public DateTime? date;
        public Expense expense;

        public ExpensePage(Flyout parentFlyout, Expense exp, ExpenseCategory cat, DateTime? datetime)
        {
            InitializeComponent();

            parent = parentFlyout;
            expense = exp;
            if (expense != null)
            {
                category = expense.category;
                date = expense.date;
                DeleteButton.IsEnabled = true;
                TitleTextBlock.Text = "Edycja wydatku";
            }
            else
            {
                category = cat;
                date = datetime;
                TitleTextBlock.Text = "Nowy wydatek";
                DeleteButton.Visibility = Visibility.Collapsed;
            }

            LoadCategories();
            SetupDatePicker();
            FillWithCorrectValues();

            ValueTextBox.Focus();
        }

        private void FillWithCorrectValues()
        {
            if (expense != null)
            {
                ValueTextBox.Text = expense.value.ToString("F");
                ExpenseDatePicker.SelectedDate = expense.date;
                CategoriesComboBox.SelectedItem = expense.category;
                MonthlyExpenseCheckBox.IsChecked = expense.monthlyExpense;
                CommentTextBox.Text = expense.comment;
            }
            else
            {
                CategoriesComboBox.SelectedItem = category;
                ExpenseDatePicker.SelectedDate = date;
            }
        }

        private void LoadCategories()
        {
            var categories = AppData.expenseCategories;
            var selectedIndex = 0;
            foreach (var category in categories)
            {
                CategoriesComboBox.Items.Add(category);
                if (category == this.category)
                {
                    selectedIndex = CategoriesComboBox.Items.Count - 1;
                }
            }
            CategoriesComboBox.SelectedIndex = selectedIndex;
        }

        private void SetSelectedDate()
        {
            ExpenseDatePicker.SelectedDate = date;
        }

        private void SetupDatePicker()
        {
            var selectedPeriod = AppData.billingPeriods?.ElementAt(AppData.currentPeriod);
            var begin = selectedPeriod?.startDate;
            var end = selectedPeriod?.endDate;
            var today = DateTime.Today.Date;
            ExpenseDatePicker.DisplayDateStart = begin;
            ExpenseDatePicker.DisplayDateEnd = end;
            ExpenseDatePicker.SelectedDate = date;
            SetSelectedDate();
        }

        private decimal ParseDecimalString(string str)
        {
            str = Regex.Replace(str, "[^0-9-,]", "");
            decimal ret;
            try
            {
                ret = decimal.Parse(str, NumberStyles.AllowCurrencySymbol | NumberStyles.Number | NumberStyles.AllowLeadingSign);
            }
            catch (Exception)
            {
                ret = Decimal.Zero;
            }
            return ret;
        }

        private bool IsAnyOfExpenseValuesDifferent()
        {
            if (expense == null || ExpenseDatePicker == null)
            {
                return false;
            }

            var value = ParseDecimalString(ValueTextBox.Text);
            var category = (ExpenseCategory)CategoriesComboBox.SelectedItem;
            var date = (DateTime?)ExpenseDatePicker.SelectedDate;
            var comment = CommentTextBox.Text;
            var isMonthlyExpense = (bool)MonthlyExpenseCheckBox.IsChecked;

            return value != expense.value ||
                   category != expense.category ||
                   date != expense.date ||
                   comment != expense.comment ||
                   isMonthlyExpense != expense.monthlyExpense;
        }

        private bool AreRequiredValuesExisting()
        {
            var value = ParseDecimalString(ValueTextBox.Text);
            var category = (ExpenseCategory)CategoriesComboBox.SelectedItem;
            var date = (DateTime?)ExpenseDatePicker.SelectedDate;

            return value != 0 &&
                   category != null &&
                   date != null;
        }

        private void OnSomethingChanged()
        {
            if (expense != null)
                SaveButton.IsEnabled = IsAnyOfExpenseValuesDifferent();
            else
                SaveButton.IsEnabled = AreRequiredValuesExisting();
        }

        private void ValueTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ValueTextBox.Text = ParseDecimalString(ValueTextBox.Text).ToString("F");
            OnSomethingChanged();
        }

        private void ValueTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OnSomethingChanged();
        }

        private void ExpenseDatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSomethingChanged();
        }

        private void CategoriesComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSomethingChanged();
        }

        private void MonthlyExpenseCheckBox_OnCheckedUnchecked(object sender, RoutedEventArgs e)
        {
            OnSomethingChanged();
        }

        private void CommentTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OnSomethingChanged();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (expense == null)
            {
                // new expense
                var exp = new Expense
                {
                    value = ParseDecimalString(ValueTextBox.Text),
                    date = ExpenseDatePicker.SelectedDate ?? DateTime.Today,
                    category = (ExpenseCategory) CategoriesComboBox.SelectedItem,
                    monthlyExpense = MonthlyExpenseCheckBox.IsChecked ?? false,
                    comment = CommentTextBox.Text
                };
                AppData.billingPeriods.ElementAt(AppData.currentPeriod).expenses.Add(exp);
            }
            else
            {
                // existing expense
                expense.value = ParseDecimalString(ValueTextBox.Text);
                expense.date = ExpenseDatePicker.SelectedDate ?? DateTime.Today;
                expense.category = (ExpenseCategory) CategoriesComboBox.SelectedItem;
                expense.monthlyExpense = MonthlyExpenseCheckBox.IsChecked ?? false;
                expense.comment = CommentTextBox.Text;
            }

            AppData.isDataChanged = true;

            parent.Hide();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            AppData.billingPeriods.ElementAt(AppData.currentPeriod).expenses.Remove(expense);
            parent.Hide();
        }
    }
}
