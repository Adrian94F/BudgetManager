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
