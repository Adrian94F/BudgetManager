using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BudgetManager
{
    [Serializable]
    class BillingPeriod : IComparable
    {
        public DateTime startDate;
        public DateTime endDate;
        public decimal netIncome;
        public decimal additionalIncome;

        public HashSet<Expense> expenses;

        public int CompareTo(object obj)
        {
            return startDate.CompareTo(((BillingPeriod)obj).startDate);
        }



        private decimal GetSumOfExpensesOfCategoryAndDate(ExpenseCategory category, DateTime date)
        {
            var ret = Decimal.Zero;
            var day = date.Day;
            var month = date.Month;

            foreach (var expense in expenses)
            {
                var expDay = expense.date.Day;
                var expMonth = expense.date.Month;
                if (expense.category == category && day == expDay && month == expMonth)
                {
                    ret += expense.value;
                }
            }
            return ret;
        }



        private decimal GetSumOfExpensesOfDate(DateTime date)
        {
            var ret = Decimal.Zero;
            var day = date.Day;
            var month = date.Month;

            foreach (var expense in expenses)
            {
                var expDay = expense.date.Day;
                var expMonth = expense.date.Month;
                if (day == expDay && month == expMonth)
                {
                    ret += expense.value;
                }
            }
            return ret;
        }


        private decimal GetSumOfExpensesOfCategory(ExpenseCategory category)
        {
            var ret = Decimal.Zero;

            foreach (var expense in expenses)
            {
                if (expense.category == category)
                {
                    ret += expense.value;
                }
            }
            return ret;
        }


        private decimal GetSumOfExpenses()
        {
            var ret = Decimal.Zero;

            foreach (var expense in expenses)
            {
                ret += expense.value;
            }
            return ret;
        }


        private void AddTextToGrid(string text, int row, int col, Grid grid)
        {
            var textBlock = new TextBlock
            {
                Text = text
            };
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, col);
            grid.Children.Add(textBlock);
        }

        public void SetGrid(Grid grid)
        {
            SetGridText(grid);
            SetGridColors(grid);
        }

        private void SetGridColors(Grid grid)
        {

        }

        private void SetGridText(Grid grid)
        {
            // column definitions
            var numOfDays = (endDate - startDate).Days + 1;
            var numOfCols = numOfDays + 2;  // category, sum, days
            for (var i = 0; i < numOfCols; i++)
            {
                var colDef = new ColumnDefinition();
                colDef.MinWidth = 40;
                grid.ColumnDefinitions.Add(colDef);
            }

            // row definitions
            var numOfCategories = DataSet.expenseCategories.Count;
            var numOfRows = numOfCategories + 2;  // title row, categories, sum
            for (var i = 0; i < numOfRows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            // title row
            AddTextToGrid("Kategoria", 0, 0, grid);
            AddTextToGrid("Suma", 0, 1, grid);
            for (var i = 0; i < numOfDays; i++)
            {
                AddTextToGrid(startDate.AddDays(i).Day.ToString(), 0, i + 2, grid);
            }

            // categories
            for (var i = 0; i < numOfCategories; i++)
            {
                var category = DataSet.expenseCategories.ElementAt(i);

                // name
                AddTextToGrid(category.name, i + 1, 0, grid);

                // sum
                var catSum = GetSumOfExpensesOfCategory(category);
                AddTextToGrid(catSum.ToString(), i + 1, 1, grid);

                // expenses
                for (var j = 0; j < numOfDays; j++)
                {
                    var sum = GetSumOfExpensesOfCategoryAndDate(category, startDate.AddDays(j));
                    AddTextToGrid(sum.ToString(), i + 1, j + 2, grid);
                }
            }

            // sum row
            var sumRow = numOfRows - 1;
            AddTextToGrid("Suma", sumRow, 0, grid);
            AddTextToGrid(GetSumOfExpenses().ToString(), sumRow, 1, grid);
            for (var i = 0; i < numOfDays; i++)
            {
                var date = startDate.AddDays(i);
                var sum = GetSumOfExpensesOfDate(date);
                AddTextToGrid(sum.ToString(), sumRow, i + 2, grid);
            }

            // sum column

        }
    }
}
