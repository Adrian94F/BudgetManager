﻿using System;
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
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using Frame = System.Windows.Controls.Frame;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Type startPageType = typeof(SummaryPage);

        public MainWindow()
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

        public void RefreshPage()
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

        public void SaveData()
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

        public void ChangeBillingPeriod()
        {

            NavigateToSelectedPage();

            StatusBar.Refresh();
        }

        private void WindowElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SaveCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveData();
        }

        private void AddCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DateTime? date = DateTime.Today;
            ExpenseCategory category = null;
            FrameworkElement objectToShowAt = PagesList;
            FlyoutPlacementMode placement = FlyoutPlacementMode.Right;

            // new expense flyout
            var listFrame = new Frame();
            var flyout = new Flyout
            {
                Content = listFrame,
                ShowMode = FlyoutShowMode.Standard,
                Placement = placement
            };
            flyout.Closed += (s, o) => RefreshPage();

            switch (PagesList.SelectedIndex)
            {
                case 1:  // table view
                    var listPageInTablePage = (ListPage)((TablePage)RootFrame.Content).Table.listPageElement;
                    if (listPageInTablePage != null)
                    {
                        date = listPageInTablePage.ExpensesDatePicker.SelectedDate;
                        if (date == null)
                        {
                            date = DateTime.Today;
                        }
                        category = (ExpenseCategory)listPageInTablePage.CategoriesComboBox.SelectedItem;
                        objectToShowAt = listPageInTablePage;

                        flyout.Closed += (s, o) => listPageInTablePage.FillPage();
                    }
                    else
                    {
                        date = DateTime.Today;
                    }

                    break;
                case 2: // list view --> new expense for selected date/category
                    var listPage = (ListPage)RootFrame.Content;
                    date = listPage.ExpensesDatePicker.SelectedDate;
                    category = (ExpenseCategory)listPage.CategoriesComboBox.SelectedItem;
                    objectToShowAt = listPage.AddButton;
                    placement = FlyoutPlacementMode.Bottom;
                    break;
            }

            Expense expense = null;
            var expensePage = new ExpensePage(flyout, expense, category, date);
            listFrame.Navigate(expensePage);
            flyout.ShowAt(objectToShowAt);
        }

        private void ChangePageToNCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var index = Convert.ToInt32(e.Parameter);
            index %= PagesList.Items.Count;
            PagesList.SelectedIndex = index;
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
