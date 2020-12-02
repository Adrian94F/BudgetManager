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
        int currentPeriod = DataSet.currentPeriod;
        ExpenseCategory selectedCategory = DataSet.selectedCategory;
        DateTime selectedDate = DataSet.selectedDate;

        public ExpensesWindow()
        {
            InitializeComponent();
            SetLabel();
            FillWithExpenses();
            BtnOk.IsDefault = true;
        }

        private void SetLabel()
        {
            var str = selectedDate != new DateTime() ? selectedDate.ToString("d.MM.yyyy") : DataSet.billingPeriods.ElementAt(currentPeriod).startDate.ToString("d.MM") + "-" + DataSet.billingPeriods.ElementAt(currentPeriod).endDate.ToString("d.MM");
            if (selectedCategory != null)
            {
                str += ", " + selectedCategory.name;
            }
            Label.Content = str;
        }

        private void FillWithExpenses()
        {
            ExpensesGrid.Children.Clear();
            ExpensesGrid.RowDefinitions.Clear();

            var expenses = selectedDate != new DateTime() ? DataSet.billingPeriods.ElementAt(currentPeriod).GetExpensesOfCategoryAndDate(selectedCategory, selectedDate) : DataSet.billingPeriods.ElementAt(currentPeriod).GetExpensesOfCategory(selectedCategory);
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
