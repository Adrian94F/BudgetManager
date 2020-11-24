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
    class BillingPeriodGridCreator
    {
        Window window;
        Grid header, categories, expenses, summary;
        BillingPeriod period;

        public BillingPeriodGridCreator(Window w)
        {
            window = w;
        }

        public void SetGrids(Grid h, Grid v, Grid e, Grid s)
        {
            header = h;
            categories = v;
            expenses = e;
            summary = s;
        }

        public void SetPeriod(BillingPeriod bp)
        {
            period = bp;
        }

        public void CreateSummary()
        {
            var net = period.netIncome;
            var add = period.additionalIncome;
            var incSum = period.netIncome + period.additionalIncome;
            var expSum = period.GetSumOfExpenses();
            var savings = period.plannedSavings;
            var balance = incSum - expSum - savings;
            var daysLeft = (period.endDate - DateTime.Today).Days;
            var estimatedExpense = daysLeft > 0 ? Math.Round(balance / daysLeft, 2) : Math.Round(balance, 2);

            foreach (var child in summary.Children)
            {
                if (child.GetType() == typeof(TextBlock) && ((TextBlock)child).Name != "")
                {
                    var textBlock = ((TextBlock)child);
                    switch (textBlock.Name)
                    {
                        case "NetIncomeTextBlock":
                            textBlock.Text = net.ToString("F") + " zł";
                            break;
                        case "AddIncomeTextBlock":
                            textBlock.Text = add.ToString("F") + " zł";
                            break;
                        case "IncomeSumTextBlock":
                            textBlock.Text = incSum.ToString("F") + " zł";
                            break;
                        case "ExpensesSumTextBlock":
                            textBlock.Text = expSum.ToString("F") + " zł";
                            break;
                        case "PlannedSavingsTextBlock":
                            textBlock.Text = savings.ToString("F") + " zł";
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
            }
        }

        public void CreateMultiGridTable()
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

        private void AddColumnDefinitionsForDays(Grid grid, int numOfDays)
        {
            for (var i = 0; i < numOfDays; i++)
            {
                // add column definition
                var colDef = new ColumnDefinition();
                colDef.MinWidth = 40;
                grid.ColumnDefinitions.Add(colDef);
            }
        }

        private void AddRowDefinitionsForCategories(Grid grid)
        {
            var numOfCategories = DataSet.expenseCategories.Count;
            for (var i = 0; i < numOfCategories; i++)
            {
                var rowDef = new RowDefinition();
                rowDef.MinHeight = 16;
                grid.RowDefinitions.Add(rowDef);
            }
        }

        private void CreateHeaderGrid(Grid grid, BillingPeriod period)
        {
            var numOfDays = (period.endDate - period.startDate).Days + 1;
            AddColumnDefinitionsForDays(grid, numOfDays);

            AddWeekendsRectangles(grid, period);
            AddTodayRectangle(grid, period);

            for (var i = 0; i < numOfDays; i++)
            {
                var date = period.startDate.AddDays(i);
                AddButtonToGrid(date.ToString("d.MM"), 0, i, grid, null, date);
            }

            AddStretchColumn(grid);
        }

        private void CreateVerticalGrid(Grid grid)
        {
            AddRowDefinitionsForCategories(grid);

            for (var i = 0; i < DataSet.expenseCategories.Count; i++)
            {
                var category = DataSet.expenseCategories.ElementAt(i);
                AddTextToGrid(category.name, i, 0, grid);
            }

            AddStretchRow(grid);
        }

        private void CreateExpensesDataGrid(Grid grid, BillingPeriod period)
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
                    var date = period.startDate.AddDays(j);
                    var sum = Decimal.Round(period.GetSumOfExpensesOfCategoryAndDate(category, date)).ToString();
                    AddButtonToGrid(sum, i, j, grid, category, date);
                }
            }

            AddStretchColumn(grid);
            AddStretchRow(grid);
        }

        private void AddStretchRow(Grid grid)
        {
            var rowDef = new RowDefinition
            {
                Height = new GridLength(100, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(rowDef);
        }

        private void AddStretchColumn(Grid grid)
        {
            var colDef = new ColumnDefinition
            {
                Width = new GridLength(100, GridUnitType.Star)
            };
            grid.ColumnDefinitions.Add(colDef);
        }

        private void AddTextToGrid(string text, int row, int col, Grid grid)
        {
            var textBlock = new TextBlock
            {
                Text = text
            };
            AddUIElementToGrid(textBlock, row, col, grid);
        }

        private void AddButtonToGrid(string text, int row, int col, Grid grid, ExpenseCategory category, DateTime date)
        {
            var btn = new Button
            {
                Content = text,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                MaxHeight = 16,
                MaxWidth = 40,
                MinWidth = 40,
                Padding = new Thickness(0)
            };
            btn.Click += (sender, e) => {
                DataSet.selectedCategory = category;
                DataSet.selectedDate = date;
                window.IsEnabled = false;
                var expWin = new ExpensesWindow();
                expWin.Closed += ExpWin_Closed;
                expWin.Show();
            };
            AddUIElementToGrid(btn, row, col, grid);
        }

        private void ExpWin_Closed(object sender, EventArgs e)
        {
            window.IsEnabled = true;
            CreateSummary();
            CreateMultiGridTable();
        }

        private void AddUIElementToGrid(UIElement obj, int row, int col, Grid grid)
        {

            Grid.SetRow(obj, row);
            Grid.SetColumn(obj, col);
            grid.Children.Add(obj);
        }

        private void AddRectangleAt(int row, int col, int rowSpan, int colSpan, Brush fill, double opacity, Grid grid)
        {
            if (row < 0 || col < 0)
            {
                return;
            }
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

        private void AddWeekendsRectangles(Grid grid, BillingPeriod period)
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

        private void AddTodayRectangle(Grid grid, BillingPeriod period)
        {
            if (DateTime.Today.Date > period.endDate.Date)
            {
                return;
            }
            var row = 0;
            var rowSpan = grid.RowDefinitions.Count;
            var col = (DateTime.Now - period.startDate).Days;
            var colSpan = 1;
            var fill = new SolidColorBrush(System.Windows.Media.Colors.LightGreen);
            AddRectangleAt(row, col, rowSpan, colSpan, fill, 0.3, grid);
        }
    }
}
