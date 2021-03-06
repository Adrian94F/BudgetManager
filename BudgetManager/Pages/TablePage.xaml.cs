﻿using System;
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
    /// Interaction logic for TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        public TablePage()
        {
            InitializeComponent();
            FillTable();
        }

        public void FillTable()
        {
            if (AppData.billingPeriods != null && AppData.billingPeriods.Count > 0)
            {
                var period = AppData.billingPeriods.ElementAt(AppData.currentPeriod);
                Table?.FillTable(period);
            }
        }

        private void TodayButton_OnClick(object sender, RoutedEventArgs e)
        {
            Table.ScrollToToday();
        }

        private void HeatMapToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            Table.ToggleHeatMap(((ToggleSwitch)sender).IsOn);
        }

        private void ScrollToLeftButton_OnClick(object sender, RoutedEventArgs e)
        {
            Table.ExpensesScrollViewer.ScrollToHorizontalOffset(0);
        }

        private void ScrollToRightButton_OnClick(object sender, RoutedEventArgs e)
        {
            Table.ExpensesScrollViewer.ScrollToHorizontalOffset(Table.ExpensesScrollViewer.ScrollableWidth);
        }
    }
}
