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
            BtnOk.IsDefault = true;
        }

        private void SetLabel()
        {
            var str = DataSet.selectedDate != new DateTime() ? DataSet.selectedDate.ToString("d.MM.yyyy") : DataSet.billingPeriods.ElementAt(DataSet
                .currentPeriod).startDate.ToString("d.MM") + "-" + DataSet.billingPeriods.ElementAt(DataSet
                .currentPeriod).endDate.ToString("d.MM");
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
            var expenses = DataSet.selectedDate != new DateTime() ? DataSet.billingPeriods.ElementAt(periodNumber).GetExpensesOfCategoryAndDate(category, date) : DataSet.billingPeriods.ElementAt(periodNumber).GetExpensesOfCategory(category);
            var gridCreator = new BillingPeriodGridCreator(this);
            var expList = new List<Expense>(expenses);
            gridCreator.CreateExpensesListGrid(ExpensesGrid, expList);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void ExpenseWindow_Closed(object sender, EventArgs e)
        {
            DataSet.selectedExpense = null;
            this.IsEnabled = true;
            FillWithExpenses();
        }

        private void Add()
        {
            this.IsEnabled = false;
            var expenseWindow = new ExpenseWindow();
            expenseWindow.Closed += ExpenseWindow_Closed;
            expenseWindow.Show();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;
                case Key.N:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        Add();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
