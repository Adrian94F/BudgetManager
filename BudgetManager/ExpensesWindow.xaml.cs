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
using System.Windows.Shapes;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for ExpensesWindow.xaml
    /// </summary>
    public partial class ExpensesWindow : Window
    {
        public ExpensesWindow()
        {
            InitializeComponent();
            SetLabel();
            FillWithExpenses();
        }

        private void SetLabel()
        {
            var str = DataSet.selectedDate.ToString("d.MM.yyyy");
            if (DataSet.selectedCategory != null)
            {
                str += ", " + DataSet.selectedCategory.name;
            }
            Label.Content = str;
        }

        private void FillWithExpenses()
        {
            ExpensesGrid.Children.Clear();
            ExpensesGrid.RowDefinitions.Clear();

            var periodNumber = DataSet.currentPeriod;
            var category = DataSet.selectedCategory;
            var date = DataSet.selectedDate;
            var expenses = DataSet.billingPeriods.ElementAt(periodNumber).GetExpensesOfCategoryAndDate(category, date);
            foreach (var expense in expenses)
            {
                var content = expense.value.ToString("F");
                var categoryName = DataSet.selectedCategory == null ? expense.category.name : "";
                var comment = expense.comment;
                var monthlyExpense = expense.monthlyExpense ? "wydatek stały" : "";
                string[] a = { categoryName, comment, monthlyExpense };
                var appendix = String.Join(", ", a.Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s)));
                if (!string.IsNullOrEmpty(appendix) && !string.IsNullOrWhiteSpace(appendix))
                {
                    content += " (" + appendix + ")";
                }
                var expenseBtn = new Button()
                {
                    Content = content,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Padding = new Thickness(5,1,5,1),
                    Margin = new Thickness(0,0,0,0),
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                expenseBtn.Click += (sender, e) =>
                {
                    DataSet.selectedExpense = expense;
                    BtnAdd_Click(sender, e);
                };

                ExpensesGrid.RowDefinitions.Add(new RowDefinition());
                var nOfRows = ExpensesGrid.RowDefinitions.Count;
                Grid.SetRow(expenseBtn, nOfRows - 1);
                ExpensesGrid.Children.Add(expenseBtn);
            }
            AddStretchRow();
        }

        private void AddStretchRow()
        {
            var rowDef = new RowDefinition
            {
                Height = new GridLength(100, GridUnitType.Star)
            };
            ExpensesGrid.RowDefinitions.Add(rowDef);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var expenseWindow = new ExpenseWindow();
            expenseWindow.Closed += ExpenseWindow_Closed;
            expenseWindow.Show();
        }

        private void ExpenseWindow_Closed(object sender, EventArgs e)
        {
            DataSet.selectedExpense = null;
            this.IsEnabled = true;
            FillWithExpenses();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
