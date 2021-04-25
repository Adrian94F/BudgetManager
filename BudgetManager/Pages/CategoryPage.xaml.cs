using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernWpf.Controls;
using Page = System.Windows.Controls.Page;

namespace BudgetManager.Pages
{
    /// <summary>
    /// Interaction logic for CategoryPage.xaml
    /// </summary>
    public partial class CategoryPage : Page
    {
        public Flyout parent;
        public ExpenseCategory category;

        public CategoryPage(Flyout parentFlyout, ExpenseCategory cat)
        {
            InitializeComponent();
            parent = parentFlyout;
            category = cat;

            if (category != null)
            {
                NameTextBox.Text = category.name;
                DeleteButton.IsEnabled = true;
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (category != null)
            {
                category.name = NameTextBox.Text;
                AppData.isDataChanged = true;
            }
            else
            {
                AppData.expenseCategories.Add(new ExpenseCategory()
                {
                    name = NameTextBox.Text
                });
            }
            AppData.isDataChanged = true;
            parent.Hide();
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

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (category != null)
            {
                if (CategoryNotUsed(category))
                {
                    AppData.expenseCategories.Remove(category);
                    AppData.isDataChanged = true;
                }
                else
                {
                    MessageBox.Show("Nie można usunąć kategorii, gdyż jest używana. Usuń wydatki lub zmień ich kategorię, a nastepnie spróbuj ponownie.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            parent.Hide();
        }
    }
}
