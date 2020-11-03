using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BudgetManager
{
    static class BillingPeriodGridCreator
    {
        public static void CreateMultigridTable(Grid header, Grid categories, Grid expenses, BillingPeriod period)
        {
            CreateHeaderGrid(header, period);
            CreateVerticalGrid(categories);
            CreateExpensesDataGrid(expenses, period);
        }

        private static void AddColumnDefinitionsForDays(Grid grid, int numOfDays)
        {
            for (var i = 0; i < numOfDays; i++)
            {
                // add column definition
                var colDef = new ColumnDefinition();
                colDef.MinWidth = 40;
                grid.ColumnDefinitions.Add(colDef);
            }
        }

        private static void AddRowDefinitionsForCategories(Grid grid)
        {
            var numOfCategories = DataSet.expenseCategories.Count;
            for (var i = 0; i < numOfCategories; i++)
            {
                var rowDef = new RowDefinition();
                rowDef.MinHeight = 18;
                grid.RowDefinitions.Add(rowDef);
            }
        }

        private static void CreateHeaderGrid(Grid grid, BillingPeriod period)
        {
            // column definitions
            var numOfDays = (period.endDate - period.startDate).Days + 1;
            AddColumnDefinitionsForDays(grid, numOfDays);

            // text
            for (var i = 0; i < numOfDays; i++)
            {
                AddTextToGrid(period.startDate.AddDays(i).ToString("d.MM"), 0, i, grid);
            }
        }

        private static void CreateVerticalGrid(Grid grid)
        {
            // row definitions
            AddRowDefinitionsForCategories(grid);

            // text
            for (var i = 0; i < DataSet.expenseCategories.Count; i++)
            {
                var category = DataSet.expenseCategories.ElementAt(i);
                AddTextToGrid(category.name, i, 0, grid);
            }
        }

        private static void CreateExpensesDataGrid(Grid grid, BillingPeriod period)
        {
            // column & row definitions
            var numOfDays = (period.endDate - period.startDate).Days + 1;
            AddColumnDefinitionsForDays(grid, numOfDays);
            AddRowDefinitionsForCategories(grid);

            // text
            var numOfCategories = DataSet.expenseCategories.Count;
            for (var i = 0; i < numOfCategories; i++)
            {
                var category = DataSet.expenseCategories.ElementAt(i);
                for (var j = 0; j < numOfDays; j++)
                {
                    var sum = period.GetSumOfExpensesOfCategoryAndDate(category, period.startDate.AddDays(j));
                    AddTextToGrid(sum.ToString(), i, j, grid);
                }
            }
        }

        private static void AddTextToGrid(string text, int row, int col, Grid grid)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = 12
            };
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, col);
            grid.Children.Add(textBlock);
        }
    }
}
