﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BudgetManager.Commands
{
    public class Commands
    {
        public static RoutedUICommand Save = new RoutedUICommand("Save", "Save", typeof(Commands));
        public static RoutedUICommand Reload = new RoutedUICommand("Reload", "Reload", typeof(Commands));
        public static RoutedUICommand Close = new RoutedUICommand("Close", "Close", typeof(Commands));
        public static RoutedUICommand Add = new RoutedUICommand("Add", "Add", typeof(Commands));
        public static RoutedUICommand Previous = new RoutedUICommand("Previous", "Previous", typeof(Commands));
        public static RoutedUICommand Next = new RoutedUICommand("Next", "Next", typeof(Commands));


        public static RoutedUICommand Settings = new RoutedUICommand("Settings", "Settings", typeof(Commands));

        public static RoutedUICommand BillingPeriods = new RoutedUICommand("BillingPeriods", "BillingPeriods", typeof(Commands));
        public static RoutedUICommand Categories = new RoutedUICommand("Categories", "Categories", typeof(Commands));

        public static RoutedUICommand ChangePageToN = new RoutedUICommand("ChangeView", "ChangeView", typeof(Commands));

        public static RoutedUICommand About = new RoutedUICommand("About", "About", typeof(Commands));
        public static RoutedUICommand Help = new RoutedUICommand("Help", "Help", typeof(Commands));
    }
}
