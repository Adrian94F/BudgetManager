﻿using System;
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
    /// Interaction logic for EmptyAppDataPlaceholder.xaml
    /// </summary>
    public partial class EmptyAppDataPlaceholder : UserControl
    {
        public EmptyAppDataPlaceholder()
        {
            InitializeComponent();
            CheckIfNeeded();
        }

        private void CheckIfNeeded()
        {
            if (AppData.IsNotEmpty())
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }
    }
}
