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

            if (DataSet.selectedCategory != null)
            {
                CategoriesComboBox.SelectedItem = DataSet.selectedCategory;
            }

            //DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).expenses.Remove();
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var expense = new Expense();
            

            DataSet.billingPeriods.ElementAt(DataSet.currentPeriod).expenses.Add(expense);
        }
    }
}
