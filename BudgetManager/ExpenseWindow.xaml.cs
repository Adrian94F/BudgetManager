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

        public void ValueTextBox_ChangedOrLostFocus(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            var strValue = Regex.Replace(txtBox.Text, "[^0-9,]", "");
            try
            {
                var value = decimal.Parse(strValue, NumberStyles.AllowCurrencySymbol | NumberStyles.Number);
                txtBox.Text = value.ToString("F");
            }
            catch (Exception)
            {
                // TODO
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (DataSet.selectedExpense != null)
            {
                DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).expenses.Remove(DataSet.selectedExpense);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataSet.selectedExpense == null)
            {
                // new expense
                var newExp = new Expense();

                // ...

                DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).expenses.Add(newExp);
            }
            else
            {
                // edited expense

                // ...
            }
            this.Close();
        }
    }
}
