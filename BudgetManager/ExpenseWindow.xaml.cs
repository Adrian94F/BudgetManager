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
        public ExpenseWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            DatePicker.SelectedDate = DataSet.selectedDate;

            var categories = DataSet.expenseCategories;
            CategoriesComboBox.ItemsSource = categories;

            if (DataSet.selectedExpense != null)
            {
                ValueTextBox.Text = DataSet.selectedExpense.value.ToString("F");
                DatePicker.SelectedDate = DataSet.selectedExpense.date;
                CategoriesComboBox.SelectedItem = DataSet.selectedExpense.category;
                CommentTextBox.Text = DataSet.selectedExpense.comment;
                MonthlyExpenseCheckBox.IsChecked = DataSet.selectedExpense.monthlyExpense;
            }
            else if (DataSet.selectedCategory != null)
            {
                CategoriesComboBox.SelectedItem = DataSet.selectedCategory;
            }

            if (DataSet.selectedExpense == null)
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
            if (DataSet.selectedExpense != null)
            {
                DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).expenses.Remove(DataSet.selectedExpense);
            }
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
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
            if (DataSet.selectedExpense == null)
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
                DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).expenses.Add(exp);
            }
            else
            {
                // edited expense
                DataSet.selectedExpense.value = value;
                DataSet.selectedExpense.date = date;
                DataSet.selectedExpense.category = category;
                DataSet.selectedExpense.comment = comment;
                DataSet.selectedExpense.monthlyExpense = isMonthlyExpense;
            }
            this.Close();
        }
    }
}
