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

namespace BudgetManager.User_controls
{
    /// <summary>
    /// Interaction logic for NavigationButtons.xaml
    /// </summary>
    public partial class NavigationButtons : UserControl
    {
        public NavigationButtons()
        {
            InitializeComponent();
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SaveData();
        }

        private void ButtonPrev_OnClick(object sender, RoutedEventArgs e)
        {
            AppData.currentPeriod--;
            if (AppData.currentPeriod <= 0)
            {
                AppData.currentPeriod = 0;
                PrevButton.IsEnabled = false;
            }
            if (!NextButton.IsEnabled)
            {
                NextButton.IsEnabled = true;
            }
            ((MainWindow)Window.GetWindow(this)).ChangeBillingPeriod();
        }

        private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
        {
            AppData.currentPeriod++;
            if (AppData.currentPeriod >= AppData.billingPeriods.Count - 1)
            {
                AppData.currentPeriod = AppData.billingPeriods.Count - 1;
                NextButton.IsEnabled = false;
            }
            if (!PrevButton.IsEnabled)
            {
                PrevButton.IsEnabled = true;
            }
            ((MainWindow)Window.GetWindow(this)).ChangeBillingPeriod();
        }
    }
}
