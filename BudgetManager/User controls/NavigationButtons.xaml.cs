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
            SetupButtons();
        }

        public void SetupButtons()
        {
            if (AppData.IsNotEmpty())
            {
                var min = 0;
                var current = AppData.currentPeriod;
                var max = AppData.billingPeriods.Count - 1;

                PrevButton.IsEnabled = current > min;
                NextButton.IsEnabled = current < max;
            }
            else
            {
                PrevButton.IsEnabled = NextButton.IsEnabled = false;
            }
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SaveData();
        }

        private void ButtonPrev_OnClick(object sender, RoutedEventArgs e)
        {
            AppData.currentPeriod--;
            SetupButtons();
            ((MainWindow)Window.GetWindow(this)).ChangeBillingPeriod();
        }

        private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
        {
            AppData.currentPeriod++;
            SetupButtons();
            ((MainWindow)Window.GetWindow(this)).ChangeBillingPeriod();
        }

        private void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            FilesHandler.ReadData();
            ((MainWindow)Window.GetWindow(this)).RefreshPage();
        }
    }
}
