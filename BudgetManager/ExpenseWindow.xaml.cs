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
    /// Interaction logic for ExpenseWindow.xaml
    /// </summary>
    public partial class ExpenseWindow : Window
    {
        Expense selectedExpense = AppData.selectedExpense;
        ExpenseCategory selectedCategory = AppData.selectedCategory;
        int currentPeriod = AppData.currentPeriod;

        public ExpenseWindow()
        {
            InitializeComponent();
            LoadData();
            Loaded += ExpenseWindow_Loaded;
        }

        private void ExpenseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ValueTextBox.Focus();
        }

        private void LoadData()
        {
            DatePicker.SelectedDate = AppData.selectedDate;

            var categories = AppData.expenseCategories;
            CategoriesComboBox.ItemsSource = categories;

            if (selectedExpense != null)
            {
                ValueTextBox.Text = selectedExpense.value.ToString("F");
                DatePicker.SelectedDate = selectedExpense.date;
                CategoriesComboBox.SelectedItem = selectedExpense.category;
                CommentTextBox.Text = selectedExpense.comment;
                MonthlyExpenseCheckBox.IsChecked = selectedExpense.monthlyExpense;
            }
            else if (selectedCategory != null)
            {
                CategoriesComboBox.SelectedItem = selectedCategory;
            }

            if (selectedExpense == null)
            {
                BtnRemove.IsEnabled = false;
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

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            Remove();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            var value = ParseDecimalString(ValueTextBox.Text);
            var category = (ExpenseCategory)CategoriesComboBox.SelectedItem;
            var date = (DateTime)DatePicker.SelectedDate;
            var comment = CommentTextBox.Text;
            var isMonthlyExpense = (bool)MonthlyExpenseCheckBox.IsChecked;
            if (value == 0 || category == null)
            {
                return;
            }
            Expense exp;
            if (selectedExpense == null)
            {
                // new expense
                exp = new Expense()
                {
                    value = value,
                    date = date,
                    category = category,
                    comment = comment,
                    monthlyExpense = isMonthlyExpense
                };
                AppData.billingPeriods.ElementAt(AppData.currentPeriod).expenses.Add(exp);
            }
            else
            {
                // edited expense
                selectedExpense.value = value;
                selectedExpense.date = date;
                selectedExpense.category = category;
                selectedExpense.comment = comment;
                selectedExpense.monthlyExpense = isMonthlyExpense;
            }
            AppData.isDataChanged = true;
            this.Close();
        }

        private void Remove()
        {
            if (selectedExpense != null)
            {
                AppData.billingPeriods.ElementAt(currentPeriod).expenses.Remove(selectedExpense);
            }
            AppData.isDataChanged = true;
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Save();
                    break;
                case Key.Escape:
                    this.Close();
                    break;
                default:
                    break;
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckDecimalValueAndComboBoxSelectionAndSetSaveButton();
        }

        private void CategoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckDecimalValueAndComboBoxSelectionAndSetSaveButton();
        }

        private void CheckDecimalValueAndComboBoxSelectionAndSetSaveButton()
        {
            var value = ParseDecimalString(ValueTextBox.Text);
            if (value == decimal.Zero || CategoriesComboBox.SelectedItem == null)
            {
                BtnSave.IsEnabled = false;
            }
            else
            {
                BtnSave.IsEnabled = true;
            }
        }
    }
}
