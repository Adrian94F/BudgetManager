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
    /// Interaction logic for BillingPeriodsWindow.xaml
    /// </summary>
    public partial class BillingPeriodsWindow : Window
    {
        public BillingPeriodsWindow()
        {
            InitializeComponent();
            FillWithPeriods();
        }

        private void FillWithPeriods()
        {
            PeriodsGrid.Children.Clear();
            PeriodsGrid.RowDefinitions.Clear();

            foreach (var period in AppData.billingPeriods.Reverse())
            {
                var dateFormat = "dd.MM.yy";
                var content = period.startDate.ToString(dateFormat) + "-" + period.endDate.ToString(dateFormat);
                var periodBtn = new Button()
                {
                    Content = content,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Padding = new Thickness(5, 1, 5, 1),
                    Margin = new Thickness(0, 0, 0, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                periodBtn.Click += (sender, e) =>
                {
                    AppData.selectedPeriod = period;
                    BtnAdd_Click(sender, e);
                };

                PeriodsGrid.RowDefinitions.Add(new RowDefinition());
                var nOfRows = PeriodsGrid.RowDefinitions.Count;
                Grid.SetRow(periodBtn, nOfRows - 1);
                PeriodsGrid.Children.Add(periodBtn);
            }
            AddStretchRow(PeriodsGrid);
        }

        private void AddStretchRow(Grid grid)
        {
            var rowDef = new RowDefinition
            {
                Height = new GridLength(100, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(rowDef);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var bpw = new BillingPeriodWindow();
            bpw.Closed += BillingPeriodWindow_Closed;
            bpw.Show();
            
        }

        private void BillingPeriodWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            AppData.selectedPeriod = null;
            FillWithPeriods();
        }
    }
}
