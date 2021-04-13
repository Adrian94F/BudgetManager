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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BudgetManager.Pages;
using ModernWpf.Controls;
using Frame = System.Windows.Controls.Frame;

namespace BudgetManager.User_controls
{
    /// <summary>
    /// Interaction logic for ExpensesTable.xaml
    /// </summary>
    public partial class ExpensesTable : UserControl
    {
        public ExpensesTable()
        {
            InitializeComponent();
        }

        private double rowHeight;
        private double fontSize;
        private BillingPeriod billingPeriod;

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                ScrollViewer[] horizontalScrollViewers = { HeaderDaysScrollViewer, DaySumsScrollViewer, DataScrollViewer };
                foreach (var viewer in horizontalScrollViewers)
                {
                    viewer.ScrollToHorizontalOffset(e.HorizontalOffset);
                }
            }

            if (e.VerticalChange != 0)
            {
                ScrollViewer[] verticalScrollViewers = { CategoriesScrollViewer, CategorySumsScrollViewer, DataScrollViewer };
                foreach (var viewer in verticalScrollViewers)
                {
                    viewer.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }
        }

        public void FillTable(BillingPeriod period = null)
        {
            if (period != null)
                billingPeriod = period;
            else if (billingPeriod == null)
                return;

            foreach (var grid in new Grid[] { DaysGrid, DaySumsGrid, CategoriesGrid, CategorySumsGrid, ExpensesGrid })
            {
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                grid.ColumnDefinitions.Clear();
            }
            FillDaysGrid(DaysGrid, billingPeriod);
            FillDaySumsGrid(DaySumsGrid, billingPeriod);
            FillCategoriesGrid(CategoriesGrid);
            FillCategorySumsGrid(CategorySumsGrid, billingPeriod);
            FillExpensesGrid(ExpensesGrid, billingPeriod);
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
            var numOfCategories = AppData.expenseCategories.Count;
            for (var i = 0; i < numOfCategories; i++)
            {
                var rowDef = new RowDefinition();
                rowDef.MinHeight = 16;
                grid.RowDefinitions.Add(rowDef);
            }
        }

        private void FillDaysGrid(Grid grid, BillingPeriod period)
        {
            var numOfDays = (period.endDate - period.startDate).Days + 1;
            AddColumnDefinitionsForDays(grid, numOfDays);

            AddWeekendsRectangles(grid, period);
            AddTodayRectangle(grid, period);

            for (var i = 0; i < numOfDays; i++)
            {
                var date = period.startDate.AddDays(i);
                var btn = new Button
                {
                    Content = date.ToString("d.MM"),
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    MaxWidth = 40,
                    MinWidth = 40,
                    Padding = new Thickness(0)
                };
                btn.Click += (sender, e) => {
                    _ = OpenExpensesListDialog(null, date);
                };
                AddUIElementToGrid(btn, 0, i, grid);

                if (i == 0)
                {
                    SetupTextBlocksBasedOnButton(btn);
                }
            }

            AddStretchColumn(grid);
        }

        private void SetupTextBlocksBasedOnButton(Button btn)
        {
            rowHeight = btn.Height;
            fontSize = btn.FontSize;

            TextBlock[] tbs = { TextBlock01, TextBlock10, TextBlock11 };
            foreach (var tb in tbs)
            {
                tb.Height = rowHeight;
                tb.FontSize = fontSize;
            }
        }

        private void FillDaySumsGrid(Grid grid, BillingPeriod period)
        {
            var numOfDays = (period.endDate - period.startDate).Days + 1;
            AddColumnDefinitionsForDays(grid, numOfDays);

            AddWeekendsRectangles(grid, period);
            AddTodayRectangle(grid, period);

            for (var i = 0; i < numOfDays; i++)
            {
                var date = period.startDate.AddDays(i);
                var sum = period.GetSumOfDailyExpensesOfDate(date);
                var sumStr = sum != 0 ? Decimal.Round(sum).ToString() : "";
                var txtBlock = new TextBlock
                {
                    Text = sumStr,
                    Height = rowHeight,
                    FontSize = fontSize,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                AddUIElementToGrid(txtBlock, 0, i, grid);
            }

            AddStretchColumn(grid);
        }

        private void FillCategoriesGrid(Grid grid)
        {
            AddRowDefinitionsForCategories(grid);

            for (var i = 0; i < AppData.expenseCategories.Count; i++)
            {
                var category = AppData.expenseCategories.ElementAt(i);
                var btn = new Button
                {
                    Content = category.name,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    MaxWidth = 300,
                    MinWidth = 100,
                    Padding = new Thickness(3, 0, 0, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                btn.Click += (sender, e) =>
                {
                    _ = OpenExpensesListDialog(category, null);
                };
                AddUIElementToGrid(btn, i, 0, grid);
            }
            
            AddStretchRow(grid);
        }

        private void FillCategorySumsGrid(Grid grid, BillingPeriod period)
        {
            AddRowDefinitionsForCategories(grid);

            for (var i = 0; i < AppData.expenseCategories.Count; i++)
            {
                var category = AppData.expenseCategories.ElementAt(i);
                var sum = period.GetSumOfExpensesOfCategory(category);
                var sumStr = sum != 0 ? Decimal.Round(sum).ToString() : "";
                var txtBlock = new TextBlock
                {
                    Text = sumStr,
                    Height = rowHeight,
                    FontSize = fontSize,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                AddUIElementToGrid(txtBlock, i, 0, grid);
            }
            
            AddStretchRow(grid);
        }

        private void FillExpensesGrid(Grid grid, BillingPeriod period)
        {
            var numOfDays = (period.endDate - period.startDate).Days + 1;
            AddColumnDefinitionsForDays(grid, numOfDays);
            AddRowDefinitionsForCategories(grid);

            AddWeekendsRectangles(grid, period);
            AddTodayRectangle(grid, period);

            var numOfCategories = AppData.expenseCategories.Count;
            for (var i = 0; i < numOfCategories; i++)
            {
                var category = AppData.expenseCategories.ElementAt(i);
                for (var j = 0; j < numOfDays; j++)
                {
                    var date = period.startDate.AddDays(j);
                    var sum = period.GetSumOfExpensesOfCategoryAndDate(category, date);
                    var sumStr = sum != 0 ? Decimal.Round(sum).ToString() : "";
                    var btn = new Button
                    {
                        Content = sumStr,
                        BorderThickness = new Thickness(0),
                        Background = Brushes.Transparent,
                        MaxWidth = 40,
                        MinWidth = 40,
                        Padding = new Thickness(0)
                    };
                    btn.Click += (sender, e) => {
                        _ = OpenExpensesListDialog(category, date);
                    };
                    AddUIElementToGrid(btn, i, j, grid);
                }
            }

            AddStretchColumn(grid);
            AddStretchRow(grid);
        }

        private void AddStretchRow(Grid grid)
        {
            var rowDef = new RowDefinition
            {
                Height = new GridLength(100, GridUnitType.Star),
                MinHeight = 16
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
                AppData.selectedCategory = category;
                AppData.selectedDate = date;
                var expWinTuple = Utilities.OpenNewOrRestoreWindowAndCheckIfNew<ExpensesWindow>();
                if (expWinTuple.Item2)
                {
                    expWinTuple.Item1.Closed += ExpWin_Closed;
                }
            };
            AddUIElementToGrid(btn, row, col, grid);
        }

        private void ExpWin_Closed(object sender, EventArgs e)
        {
            FillTable();
        }

        private void AddUIElementToGrid(UIElement obj, int row, int col, Grid grid)
        {

            Grid.SetRow(obj, row);
            Grid.SetColumn(obj, col);
            grid.Children.Add(obj);
        }

        private void AddRectangleAt(int row, int col, int rowSpan, int colSpan, Brush fill, Grid grid)
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
            Grid.SetRow(rect, row);
            Grid.SetRowSpan(rect, rowSpan);
            Grid.SetColumn(rect, col);
            Grid.SetColumnSpan(rect, colSpan);

            grid.Children.Add(rect);
        }

        private void AddWeekendsRectangles(Grid grid, BillingPeriod period)
        {
            var row = 0;
            var rowSpan = grid.RowDefinitions.Count + 1;
            var col = 0;
            var colSpan = 1;
            var fill = (Brush)FindResource("Alpha-Gray-2");

            var day = period.startDate;
            while (day.Date <= period.endDate.Date)
            {
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    AddRectangleAt(row, col, rowSpan, colSpan, fill, grid);
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
            var rowSpan = grid.RowDefinitions.Count + 1;
            var col = (DateTime.Now - period.startDate).Days;
            var colSpan = 1;
            var fill = (Brush)FindResource("Alpha-Green");
            AddRectangleAt(row, col, rowSpan, colSpan, fill, grid);
        }

        private async Task OpenExpensesListDialog(ExpenseCategory cat, DateTime? date)
        {
            var listPage = new ListPage(cat, date);
            var listFrame = new Frame();
            listFrame.Navigate(listPage);
            var dialog = new ContentDialog
            {
                Title = "Lista wydatków",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary,
                Content = listFrame
            };
            dialog.Closed += (sender, args) => FillTable();
            var result = await dialog.ShowAsync();
        }
    }
}
