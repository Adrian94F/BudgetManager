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
    /// Interaction logic for CategoriesWindow.xaml
    /// </summary>
    public partial class CategoriesWindow : Window
    {
        public CategoriesWindow()
        {
            InitializeComponent();
            FillWithCategories();
        }

        private void FillWithCategories()
        {
            CategoriesGrid.Children.Clear();
            CategoriesGrid.RowDefinitions.Clear();

            var periodNumber = AppData.currentPeriod;
            var categories = AppData.expenseCategories;
            foreach (var category in categories)
            {
                var content = category.name;
                var categoryBtn = new Button()
                {
                    Content = content,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Padding = new Thickness(5, 1, 5, 1),
                    Margin = new Thickness(0, 0, 0, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                categoryBtn.Click += (sender, e) =>
                {
                    AppData.selectedCategory = category;
                    BtnAdd_Click(sender, e);
                };

                CategoriesGrid.RowDefinitions.Add(new RowDefinition());
                var nOfRows = CategoriesGrid.RowDefinitions.Count;
                Grid.SetRow(categoryBtn, nOfRows - 1);
                CategoriesGrid.Children.Add(categoryBtn);
            }
            AddStretchRow();
        }

        private void AddStretchRow()
        {
            var rowDef = new RowDefinition
            {
                Height = new GridLength(100, GridUnitType.Star)
            };
            CategoriesGrid.RowDefinitions.Add(rowDef);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var categoryWindow = new CategoryWindow();
            categoryWindow.Closed += CategoryWindow_Closed;
            categoryWindow.Show();
        }

        private void CategoryWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            AppData.selectedCategory = null;
            FillWithCategories();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
