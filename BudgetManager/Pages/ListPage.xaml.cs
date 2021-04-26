using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using Frame = System.Windows.Controls.Frame;
using Page = System.Windows.Controls.Page;

namespace BudgetManager.Pages
{
    /// <summary>
    /// Interaction logic for ListPage.xaml
    /// </summary>
    public partial class ListPage : Page
    {
        public ExpenseCategory selectedCategory;

        public DateTime? selectedDate;

        private ObservableCollection<ExpenseDataItem> filteredExpenses;

        public ListPage()
        {
            InitializeComponent();
            FillPage();
        }

        public void HideTitleAndLabels()
        {
            TextBlock[] tbs = {TitleTextBlock, CategoryTextBlock, DateTextBlock};
            foreach (var tb in tbs)
            {
                tb.Visibility = Visibility.Collapsed;
            }
        }

        public ListPage(ExpenseCategory cat, DateTime? date)
        {
            InitializeComponent();
            selectedCategory = cat;
            selectedDate = date;
            FillPage();
        }

        public void FillPage()
        {
            LoadCategories();
            if (AppData.IsNotEmpty())
            {
                SetupDatePicker();
                FillDataGridWithExpenses();
            }
        }

        private void LoadCategories()
        {
            var categories = AppData.expenseCategories;
            foreach (var category in categories)
            {
                CategoriesComboBox.Items.Add(category);
                if (category == selectedCategory)
                {
                    CategoriesComboBox.SelectedIndex = CategoriesComboBox.Items.Count - 1;
                }
            }
        }

        private void SetupDatePicker()
        {
            var selectedPeriod = AppData.billingPeriods?.ElementAt(AppData.currentPeriod);
            var begin = selectedPeriod?.startDate;
            var end = selectedPeriod?.endDate;
            var today = DateTime.Today.Date;
            ExpensesDatePicker.DisplayDateStart = begin;
            ExpensesDatePicker.DisplayDateEnd = end;
            ExpensesDatePicker.SelectedDate = selectedDate;
            SetSelectedDate();
        }

        private void ClearSelectedDate()
        {
            selectedDate = null;
            SetSelectedDate();
        }

        private void SetSelectedDate()
        {
            ExpensesDatePicker.SelectedDate = selectedDate;
        }

        private void ButtonClearDate_OnClick(object sender, RoutedEventArgs e)
        {
            ClearSelectedDate();
        }

        private void FillDataGridWithExpenses()
        {
            var expenses = AppData.billingPeriods?.ElementAt(AppData.currentPeriod).expenses;
            filteredExpenses = new ObservableCollection<ExpenseDataItem>();

            var isCategorySelected = CategoriesComboBox.SelectedIndex >= 0;
            var category = CategoriesComboBox.SelectedItem;
            var date = ExpensesDatePicker.SelectedDate;
            var isDateSelected = date != null;

            foreach (var expense in expenses)
            {
                if ((expense.category == category || category == null) &&
                    ((isDateSelected && expense.date == date) || !isDateSelected))
                {
                    filteredExpenses.Add(new ExpenseDataItem(expense));
                }
            }

            ExpensesDataGrid.ItemsSource = filteredExpenses;
            SortWithDateAndCategory();
        }

        private void SortWithDateAndCategory()
        {
            ExpensesDataGrid.Items.SortDescriptions.Clear();
            var dateColumn = ExpensesDataGrid.Columns[0];
            var categoryColumn = ExpensesDataGrid.Columns[2];
            var dateSortDescription = new SortDescription(dateColumn.SortMemberPath, ListSortDirection.Descending);
            var categorySortDescription = new SortDescription(categoryColumn.SortMemberPath, ListSortDirection.Ascending);

            ExpensesDataGrid.Items.SortDescriptions.Add(dateSortDescription);
            ExpensesDataGrid.Items.SortDescriptions.Add(categorySortDescription);

            foreach (var col in ExpensesDataGrid.Columns)
            {
                col.SortDirection = null;
            }
            dateColumn.SortDirection = ListSortDirection.Descending;
            categoryColumn.SortDirection = ListSortDirection.Ascending;
            
            ExpensesDataGrid.Items.Refresh();
        }

        private void CategoriesComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillDataGridWithExpenses();
        }

        private void ExpensesDatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FillDataGridWithExpenses();
        }

        private void ShowAddExpenseFlyout(FrameworkElement objectToShowOn)
        {
            var listFrame = new Frame();
            var flyout = new Flyout
            {
                Content = listFrame,
                ShowMode = FlyoutShowMode.Standard,
                Placement = FlyoutPlacementMode.Bottom
            };
            var expense = objectToShowOn.GetType() == typeof(DataGridRow)
                ? ((ExpenseDataItem) ((DataGridRow) objectToShowOn).Item).originalExpense // DataGridRow
                : null;  // Button etc.
            var date = selectedDate != null ? selectedDate : DateTime.Today;
            var expensePage = new ExpensePage(flyout, expense, selectedCategory, date);
            listFrame.Navigate(expensePage);
            flyout.Closed += (sender, o) => FillDataGridWithExpenses();
            flyout.ShowAt(objectToShowOn);
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            ShowAddExpenseFlyout((FrameworkElement)sender);
        }

        private void ShowFlyoutOnRow(DataGridRow row)
        {
            if (row != null)
            {
                ShowAddExpenseFlyout(row);
            }
        }

        private void ExpensesDataGrid_OnMouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid) sender,
                e.OriginalSource as DependencyObject) as DataGridRow;
            ShowFlyoutOnRow(row);
        }

        private void ExpensesDataGrid_OnTouchUp(object sender, TouchEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                e.OriginalSource as DependencyObject) as DataGridRow;
            ShowFlyoutOnRow(row);
        }

        private void ButtonClearCategory_OnClick(object sender, RoutedEventArgs e)
        {
            CategoriesComboBox.SelectedIndex = -1;
        }
    }

    class ExpenseDataItem
    {
        public DateTime date { get; set; }
        public string value { get; set; }
        public string comment { get; set; }
        public string category { get; set; }
        public Boolean monthlyExpense { get; set; }
        public Expense originalExpense;

        public ExpenseDataItem(Expense exp)
        {
            date = exp.date;
            value = exp.value.ToString("F");
            comment = exp.comment;
            category = exp.category.name;
            monthlyExpense = exp.monthlyExpense;
            originalExpense = exp;
        }
    }
}
