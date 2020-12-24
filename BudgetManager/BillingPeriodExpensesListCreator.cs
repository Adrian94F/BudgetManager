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
    class BillingPeriodExpensesListCreator<W>
    {
        readonly Grid listGrid;
        readonly List<Expense> expensesList;
        W parentWindow;

        public BillingPeriodExpensesListCreator(Grid grid, List<Expense> expList, W window)
        {
            listGrid = grid;
            expensesList = expList;
            parentWindow = window;
            CreateExpensesListGrid();
        }

        private void CreateExpensesListGrid()
        {
            listGrid.Children.Clear();
            listGrid.RowDefinitions.Clear();
            listGrid.ColumnDefinitions.Clear();

            expensesList.Sort(delegate (Expense x, Expense y)
            {
                return -DateTime.Compare(x.date, y.date);
            });

            GridLength[] colWidths = {
                new GridLength(50, GridUnitType.Pixel),
                new GridLength(70, GridUnitType.Pixel),
                new GridLength(200, GridUnitType.Pixel),
                new GridLength(1, GridUnitType.Star)
            };
            HorizontalAlignment[] horizontalAlignments =
            {
                HorizontalAlignment.Left,
                HorizontalAlignment.Right,
                HorizontalAlignment.Left,
                HorizontalAlignment.Left
            };
            foreach (var expense in expensesList)
            {
                listGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Auto)
                });
                var buttonGrid = new Grid();
                for (var i = 0; i < colWidths.Length; i++)
                {
                    var cd = new ColumnDefinition
                    {
                        Width = colWidths[i]
                    };
                    buttonGrid.ColumnDefinitions.Add(cd);
                }
                var comment = expense.comment;
                var monthlyExpense = expense.monthlyExpense ? "wydatek stały" : "";
                string[] a = { comment, monthlyExpense };
                var appendix = String.Join(", ", a.Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s)));
                appendix = !string.IsNullOrEmpty(appendix) && !string.IsNullOrWhiteSpace(appendix) ? appendix : " ";
                string[] textBlocksValues =
                {
                    expense.date.ToString("dd.MM"),
                    expense.value.ToString("F") + " zł",
                    expense.category.name,
                    appendix
                };
                for (var i = 0; i < textBlocksValues.Length; i++)
                {
                    var textColor = (i == 1 && textBlocksValues[i][0] == '-') ? Brushes.MediumSeaGreen : Brushes.Black;
                    var fontStyle = i == 3 ? FontStyles.Italic : FontStyles.Normal;
                    var textBlock = new TextBlock
                    {
                        Text = textBlocksValues[i],
                        Padding = new Thickness(3, 0, 10, 0),
                        HorizontalAlignment = horizontalAlignments[i],
                        Foreground = textColor,
                        FontStyle = fontStyle
                    };
                    Grid.SetColumn(textBlock, i);
                    buttonGrid.Children.Add(textBlock);
                }
                var button = new Button
                {
                    Content = buttonGrid,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    MaxHeight = 16,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };
                button.Click += (sender, e) => {
                    AppData.selectedExpense = expense;
                    var expWinTuple = Utilities.OpenNewOrRestoreWindowAndCheckIfNew<ExpenseWindow>();
                    if (expWinTuple.Item2)
                    {
                        expWinTuple.Item1.Closed += ExpWin_Closed;
                    }
                };
                AddUIElementToGrid(button, listGrid.RowDefinitions.Count - 1, 0, listGrid);
            }
        }

        private void ExpWin_Closed(object sender, EventArgs e)
        {
            if (parentWindow is MainWindow mainWindow)
            {
                mainWindow.FillExpensesListTable();
                mainWindow.FillSummaryGrid();
            }
            else if (parentWindow is ExpensesWindow expensesWindow)
            {
                expensesWindow.FillWithExpenses();
            }
        }

        private void AddUIElementToGrid(UIElement obj, int row, int col, Grid grid)
        {
            Grid.SetRow(obj, row);
            Grid.SetColumn(obj, col);
            grid.Children.Add(obj);
        }
    }
}
