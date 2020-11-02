using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BudgetManager
{
    static class BillingPeriodGridCreator
    {
        public static void createGridForBillingPeriod(Grid grid, BillingPeriod period)
        {
            SetGridColors(grid, period);
            SetGridText(grid, period);
        }

        static private void SetGridColors(Grid grid, BillingPeriod period)
        {

        }

        static private void SetGridText(Grid grid, BillingPeriod period)
        {
            // column definitions
            var numOfDays = (period.endDate - period.startDate).Days + 1;
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
                var rowDef = new RowDefinition();
                rowDef.MinHeight = 16;
                grid.RowDefinitions.Add(rowDef);
            }

            // title row
            AddTextToGrid("Kategoria", 0, 0, grid);
            AddTextToGrid("Suma", 0, 1, grid);
            for (var i = 0; i < numOfDays; i++)
            {
                AddTextToGrid(period.startDate.AddDays(i).Day.ToString(), 0, i + 2, grid);
            }

            // categories
            for (var i = 0; i < numOfCategories; i++)
            {
                var category = DataSet.expenseCategories.ElementAt(i);

                // name
                AddTextToGrid(category.name, i + 1, 0, grid);

                // sum
                var catSum = period.GetSumOfExpensesOfCategory(category);
                AddTextToGrid(catSum.ToString(), i + 1, 1, grid);

                // expenses
                for (var j = 0; j < numOfDays; j++)
                {
                    var sum = period.GetSumOfExpensesOfCategoryAndDate(category, period.startDate.AddDays(j));
                    AddTextToGrid(sum.ToString(), i + 1, j + 2, grid);
                }
            }

            // sum row
            var sumRow = numOfRows - 1;
            AddTextToGrid("Suma", sumRow, 0, grid);
            AddTextToGrid(period.GetSumOfExpenses().ToString(), sumRow, 1, grid);
            for (var i = 0; i < numOfDays; i++)
            {
                var date = period.startDate.AddDays(i);
                var sum = period.GetSumOfExpensesOfDate(date);
                AddTextToGrid(sum.ToString(), sumRow, i + 2, grid);
            }

            // sum column

        }


        static private void AddTextToGrid(string text, int row, int col, Grid grid)
        {
            var textBlock = new TextBlock
            {
                Text = text
            };
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, col);
            grid.Children.Add(textBlock);
        }
    }
}
