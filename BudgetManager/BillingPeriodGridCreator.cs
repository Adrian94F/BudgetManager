using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BudgetManager
{
    static class BillingPeriodGridCreator
    {
        public static void CreateSummary(Grid grid, BillingPeriod period)
        {
            var net = period.netIncome;
            var add = period.additionalIncome;
            var incSum = period.netIncome + period.additionalIncome;
            var expSum = period.GetSumOfExpenses();
            var balance = incSum - expSum;
            var daysLeft = (period.endDate - DateTime.Today).Days;
            var estimatedExpense = Math.Round(balance / daysLeft, 2);

            foreach (var child in grid.Children)
            {
                if (child.GetType() == typeof(TextBlock) && ((TextBlock)child).Name != "")
                {
                    var textBlock = ((TextBlock)child);
                    switch (textBlock.Name)
                    {
                        case "IncomeSumTextBlock":
                            textBlock.Text = incSum.ToString("F") + " zł";
                            break;
                        case "ExpensesSumTextBlock":
                            textBlock.Text = expSum.ToString("F") + " zł";
                            break;
                        case "BalanceTextBlock":
                            textBlock.Text = (balance > 0 ? "+" : "") + balance.ToString("F") + " zł";
                            break;
                        case "DaysLeftTextBlock":
                            textBlock.Text = daysLeft < 0 ? "" : daysLeft.ToString();
                            break;
                        case "EstimatedDailyExpenseTextBlock":
                            textBlock.Text = daysLeft < 0 ? "" : estimatedExpense.ToString("F") + " zł";
                            break;
                        default:
                            break;
                    }
                }
                if (child.GetType() == typeof(TextBox) && ((TextBox)child).Name != "")
                {
                    var textBox = ((TextBox)child);
                    switch (textBox.Name)
                    {
                        case "NetIncomeTextBox":
                            textBox.Text = net.ToString("F");
                            break;
                        case "AddIncomeTextBox":
                            textBox.Text = add.ToString("F");
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void CreateMultiGridTable(Grid header, Grid categories, Grid expenses, BillingPeriod period)
        {
            foreach (var grid in new Grid[] { header, categories, expenses })
            {
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                grid.ColumnDefinitions.Clear();
            }
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
                rowDef.MinHeight = 16;
                grid.RowDefinitions.Add(rowDef);
            }
        }

        private static void CreateHeaderGrid(Grid grid, BillingPeriod period)
        {
            var numOfDays = (period.endDate - period.startDate).Days + 1;
            AddColumnDefinitionsForDays(grid, numOfDays);

            AddWeekendsRectangles(grid, period);
            AddTodayRectangle(grid, period);

            for (var i = 0; i < numOfDays; i++)
            {
                AddTextToGrid(period.startDate.AddDays(i).ToString("d.MM"), 0, i, grid);
            }
        }

        private static void CreateVerticalGrid(Grid grid)
        {
            AddRowDefinitionsForCategories(grid);

            for (var i = 0; i < DataSet.expenseCategories.Count; i++)
            {
                var category = DataSet.expenseCategories.ElementAt(i);
                AddTextToGrid(category.name, i, 0, grid);
            }
        }

        private static void CreateExpensesDataGrid(Grid grid, BillingPeriod period)
        {
            var numOfDays = (period.endDate - period.startDate).Days + 1;
            AddColumnDefinitionsForDays(grid, numOfDays);
            AddRowDefinitionsForCategories(grid);

            AddWeekendsRectangles(grid, period);
            AddTodayRectangle(grid, period);

            var numOfCategories = DataSet.expenseCategories.Count;
            for (var i = 0; i < numOfCategories; i++)
            {
                var category = DataSet.expenseCategories.ElementAt(i);
                for (var j = 0; j < numOfDays; j++)
                {
                    var sum = period.GetSumOfExpensesOfCategoryAndDate(category, period.startDate.AddDays(j)).ToString();
                    AddButtonToGrid(sum, i, j, grid);
                }
            }
        }

        private static void AddTextToGrid(string text, int row, int col, Grid grid)
        {
            var textBlock = new TextBlock
            {
                Text = text
            };
            AddUIElementToGrid(textBlock, row, col, grid);
        }

        private static void AddButtonToGrid(string text, int row, int col, Grid grid)
        {
            var btn = new Button
            {
                Content = text,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                MaxHeight = 16,
                Padding = new Thickness(0)
            };
            AddUIElementToGrid(btn, row, col, grid);
        }

        private static void AddUIElementToGrid(UIElement obj, int row, int col, Grid grid)
        {

            Grid.SetRow(obj, row);
            Grid.SetColumn(obj, col);
            grid.Children.Add(obj);
        }

        private static void AddRectangleAt(int row, int col, int rowSpan, int colSpan, Brush fill, double opacity, Grid grid)
        {
            var rect = new Rectangle();
            rect.Fill = fill;
            if (rowSpan < 1)
            {
                rowSpan = 1;
            }
            if (colSpan < 1)
            {
                colSpan = 1;
            }
            rect.Opacity = opacity;
            Grid.SetRow(rect, row);
            Grid.SetRowSpan(rect, rowSpan);
            Grid.SetColumn(rect, col);
            Grid.SetColumnSpan(rect, colSpan);

            grid.Children.Add(rect);
        }

        private static void AddWeekendsRectangles(Grid grid, BillingPeriod period)
        {
            var row = 0;
            var rowSpan = grid.RowDefinitions.Count;
            var col = 0;
            var colSpan = 1;
            var fill = new SolidColorBrush(System.Windows.Media.Colors.LightGray);

            var day = period.startDate;
            while (day.Date <= period.endDate.Date)
            {
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    AddRectangleAt(row, col, rowSpan, colSpan, fill, 0.2, grid);
                }
                day = day.AddDays(1);
                col++;
            }
        }

        private static void AddTodayRectangle(Grid grid, BillingPeriod period)
        {
            if (DateTime.Today.Date > period.endDate.Date)
            {
                return;
            }
            var row = 0;
            var rowSpan = grid.RowDefinitions.Count;
            var col = (DateTime.Now - period.startDate).Days;
            var colSpan = 1;
            var fill = new SolidColorBrush(System.Windows.Media.Colors.LightBlue);
            AddRectangleAt(row, col, rowSpan, colSpan, fill, 0.2, grid);
        }
    }
}
