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
using System.Windows.Shapes;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for ExpensesWindow.xaml
    /// </summary>
    public partial class ExpensesWindow : Window
    {
        int currentPeriod = AppData.currentPeriod;
        ExpenseCategory selectedCategory = AppData.selectedCategory;
        DateTime selectedDate = AppData.selectedDate;

        public ExpensesWindow()
        {
            InitializeComponent();
            SetLabel();
            FillWithExpenses();
            BtnOk.IsDefault = true;
        }

        private void SetLabel()
        {
            var str = selectedDate != new DateTime() ? selectedDate.ToString("d.MM.yyyy") : AppData.billingPeriods.ElementAt(currentPeriod).startDate.ToString("d.MM") + "-" + AppData.billingPeriods.ElementAt(currentPeriod).endDate.ToString("d.MM");
            if (selectedCategory != null)
            {
                str += ", " + selectedCategory.name;
            }
            Label.Content = str;
        }

        public void FillWithExpenses()
        {
            var expenses = selectedDate != new DateTime() ?
                AppData.billingPeriods.ElementAt(currentPeriod).GetExpensesOfCategoryAndDate(selectedCategory, selectedDate) :
                AppData.billingPeriods.ElementAt(currentPeriod).GetExpensesOfCategory(selectedCategory);
            _ = new BillingPeriodExpensesListCreator<ExpensesWindow>(ExpensesGrid, new List<Expense>(expenses), this);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void ExpenseWindow_Closed(object sender, EventArgs e)
        {
            AppData.selectedExpense = null;
            FillWithExpenses();
        }

        private void Add()
        {
            var expenseWindow = Utilities.OpenNewOrRestoreWindow<ExpenseWindow>();
            expenseWindow.Closed += ExpenseWindow_Closed;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;
                case Key.N:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        Add();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
