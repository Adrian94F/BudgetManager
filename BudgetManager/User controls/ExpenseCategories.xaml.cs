using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ModernWpf.Controls.Primitives;
using Frame = System.Windows.Controls.Frame;

namespace BudgetManager.User_controls
{
    /// <summary>
    /// Interaction logic for ExpenseCategories.xaml
    /// </summary>
    public partial class ExpenseCategories : UserControl
    {
        public ExpenseCategories()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categoriesCollection = new ObservableCollection<CategoryDataItem>();
            
            foreach (var category in AppData.expenseCategories)
            {
                var categoryDataItem = new CategoryDataItem(category);
                categoriesCollection.Add(categoryDataItem);
            }

            CategoriesDataGrid.ItemsSource = categoriesCollection;
        }

        private void ShowCategoryFlyout(FrameworkElement objectToShowOn)
        {
            var categoryFrame = new Frame();
            var flyout = new Flyout
            {
                Content = categoryFrame,
                ShowMode = FlyoutShowMode.Standard,
                Placement = FlyoutPlacementMode.Bottom
            };
            var category = objectToShowOn.GetType() == typeof(DataGridRow)
                ? ((CategoryDataItem)((DataGridRow)objectToShowOn).Item).originalCategory // DataGridRow
                : null;  // Button etc.
            var categoryPage = new CategoryPage(flyout, category);
            categoryFrame.Navigate(categoryPage);
            flyout.Closed += (sender, o) => LoadCategories();
            flyout.ShowAt(objectToShowOn);
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowCategoryFlyout((Button)sender);
        }

        private void CategoriesDataGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                e.OriginalSource as DependencyObject) as DataGridRow;
            ShowCategoryFlyout(row);
        }

        private void CategoriesDataGrid_OnTouchUp(object sender, TouchEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                e.OriginalSource as DependencyObject) as DataGridRow;
            ShowCategoryFlyout(row);
        }
    }

    class CategoryDataItem
    {
        public string name { get; set; }
        public ExpenseCategory originalCategory;

        public CategoryDataItem(ExpenseCategory category)
        {
            name = category.name;
            originalCategory = category;
        }
    }
}
