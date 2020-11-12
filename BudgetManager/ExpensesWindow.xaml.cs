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
            var expenses = DataSet.billingPeriods.ElementAt(periodNumber).GetExpensesOfDayAndCategory(category, date);
            foreach (var expense in expenses)
            {
                var content = expense.value.ToString("F");
                if (DataSet.selectedCategory == null || expense.comment != "")
                {
                    content += " (";
                }
                if (DataSet.selectedCategory == null)
                {
                    content += expense.category.name;
                }
                if (DataSet.selectedCategory == null && expense.comment != "")
                {
                    content += ", ";
                }
                if (expense.comment != "")
                {
                    content += expense.comment;
                }
                if (DataSet.selectedCategory == null || expense.comment != "")
                {
                    content += ")";
                }
                var expenseBtn = new Button()
                {
                    Content = content,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    MaxHeight = 16,
                    Padding = new Thickness(0),
                    Margin = new Thickness(10, 0, 0, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Left
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
            this.IsEnabled = true;
            FillWithExpenses();
        }
    }
}
