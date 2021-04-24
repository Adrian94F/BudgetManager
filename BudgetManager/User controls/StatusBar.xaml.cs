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
    /// Interaction logic for StatusBar.xaml
    /// </summary>
    public partial class StatusBar : UserControl
    {
        public StatusBar()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            ShowCurrentBillingPeriodDates();
        }

        private void ShowCurrentBillingPeriodDates()
        {
            if (AppData.billingPeriods.Count == 0)
            {
                SelectedPeriodTextBox.Text = "Brak danych o miesiącach";
                return;
            }

            var period = AppData.billingPeriods.ElementAt(AppData.currentPeriod);
            SelectedPeriodTextBox.Text = "Wybrany okres rozliczenowy: " +
                                         period.startDate.ToString("dd.MM.yyyy") +
                                         "-" +
                                         period.endDate.ToString("dd.MM.yyyy");
        }
    }
}
