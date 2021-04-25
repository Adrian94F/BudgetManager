using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for NewMainWindow.xaml
    /// </summary>
    public partial class NewMainWindow : Window
    {
        private Type startPageType = typeof(SummaryPage);

        public NewMainWindow()
        {
            InitializeComponent();
            SetStartPage();
            NavigateToSelectedPage();
            Closing += Window_Closing;
        }

        private void SetStartPage()
        {
            RootFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            PagesList.SelectedItem = PagesList.Items.OfType<InfoDataItem>().FirstOrDefault(x => x.PageType == startPageType);
        }

        private void NavigateToSelectedPage()
        {
            if (PagesList.SelectedValue is Type type && RootFrame != null)
            {
                RootFrame.Content = Activator.CreateInstance(type);
            }
        }

        private void RefreshPage()
        {
            if (RootFrame.Content.GetType() == typeof(SummaryPage))
            {
                ((SummaryPage)RootFrame.Content).FillPage();
            }
            else if (RootFrame.Content.GetType() == typeof(TablePage))
            {
                ((TablePage)RootFrame.Content).FillTable();
            }
            else if (RootFrame.Content.GetType() == typeof(ListPage))
            {
                ((ListPage) RootFrame.Content).FillPage();
            }
            else
            {
                NavigateToSelectedPage();
            }
        }

        private void PagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigateToSelectedPage();
        }

        private void SaveData()
        {
            FilesHandler.SaveData();
            AppData.isDataChanged = false;
            MessageBox.Show("Pomyslnie zapisano wydatki!", "Zapis", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ReloadData()
        {
            FilesHandler.ReadData();
            AppData.isDataChanged = false;
            RefreshPage();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (AppData.isDataChanged)
            {
                var result = MessageBox.Show("Czy chcesz zapisać wprowadzone zmiany?", "Wyjście", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveData();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void NextBillingPeriod()
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

            NavigateToSelectedPage();

            StatusBar.Refresh();
        }

        private void PreviousBillingPeriod()
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

            NavigateToSelectedPage();

            StatusBar.Refresh();
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void ButtonPrev_OnClick(object sender, RoutedEventArgs e)
        {
            PreviousBillingPeriod();
        }

        private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
        {
            NextBillingPeriod();
        }

        private void WindowElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }

    public class PagesData : List<InfoDataItem>
    {
        public PagesData()
        {
            AddPage(typeof(SummaryPage), "Podsumowanie");
            AddPage(typeof(TablePage), "Tabela");
            AddPage(typeof(ListPage), "Lista");
            AddPage(typeof(BurndownPage), "Wypalenie");
            AddPage(typeof(HistoryPage), "Historia");
            AddPage(typeof(BillingPeriodsPage), "Miesiące");
            AddPage(typeof(SettingsPage), "Ustawienia");
            AddPage(typeof(InfoPage), "Informacje");
        }

        private void AddPage(Type pageType, string displayName = null)
        {
            Add(new InfoDataItem(pageType, displayName));
        }
    }

    public class InfoDataItem
    {
        public InfoDataItem(Type pageType, string title = null)
        {
            PageType = pageType;
            Title = title ?? pageType.Name.Replace("Page", null);
        }

        public string Title { get; }

        public Type PageType { get; }

        public override string ToString()
        {
            return Title;
        }
    }

}
