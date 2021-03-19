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
using BudgetManager.Pages;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for NewMainWindow.xaml
    /// </summary>
    public partial class NewMainWindow : Window
    {
        private Type _startPage = typeof(TablePage);

        public NewMainWindow()
        {
            InitializeComponent();
            SetStartPage();
            NavigateToSelectedPage();
        }

        private void SetStartPage()
        {
            RootFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            PagesList.SelectedItem = PagesList.Items.OfType<InfoDataItem>().FirstOrDefault(x => x.PageType == _startPage);
        }

        private void NavigateToSelectedPage()
        {
            if (PagesList.SelectedValue is Type type && RootFrame != null)
            {
                RootFrame.Content = Activator.CreateInstance(type);
                //RootFrame?.Navigate(type);
            }
        }

        private void PagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigateToSelectedPage();
        }
    }

    public class PagesData : List<InfoDataItem>
    {
        public PagesData()
        {
            AddPage(typeof(Pages.SummaryPage), "Podsumowanie");
            AddPage(typeof(Pages.TablePage), "Tabela");
            AddPage(typeof(Pages.ListPage), "Lista");
            AddPage(typeof(Pages.BurndownPage), "Wypalenie");
            AddPage(typeof(Pages.HistoryPage), "Historia");
            AddPage(typeof(Pages.BillingPeriodsPage), "Miesiące");
            AddPage(typeof(Pages.SettingsPage), "Ustawienia");
            AddPage(typeof(Pages.InfoPage), "Informacje");
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
