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
using System.Windows.Shapes;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window
    {
        ExpenseCategory selectedCategory = AppData.selectedCategory;
        
        public CategoryWindow()
        {
            InitializeComponent();
            LoadData();
            Loaded += CategoryWindow_Loaded;
        }

        private void CategoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
        }

        private void LoadData()
        {
            if (selectedCategory != null)
            {
                NameTextBox.Text = selectedCategory.name;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text;
            if (selectedCategory == null)
            {
                var category = new ExpenseCategory()
                {
                    name = name
                };
                AppData.expenseCategories.Add(category);
            }
            else
            {
                AppData.selectedCategory.name = name;
            }
            AppData.isDataChanged = true;
            this.Close();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCategory != null)
            {
                if (CategoryNotUsed(selectedCategory))
                {
                    AppData.expenseCategories.Remove(AppData.selectedCategory);
                }
                else
                {
                    MessageBox.Show("Nie można usunąć kategorii, gdyż jest używana. Usuń wydatki lub zmień ich kategorię, a nastepnie spróbuj ponownie.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            AppData.isDataChanged = true;
            this.Close();
        }

        private bool CategoryNotUsed(ExpenseCategory category)
        {
            foreach (var period in AppData.billingPeriods)
            {
                foreach (var expense in period.expenses)
                {
                    if (expense.category == category)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
