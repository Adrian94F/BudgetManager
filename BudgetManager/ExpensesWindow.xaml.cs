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
        int currentPeriod = DataSet.currentPeriod;
        ExpenseCategory selectedCategory = DataSet.selectedCategory;
        DateTime selectedDate = DataSet.selectedDate;

        public ExpensesWindow()
        {
            InitializeComponent();
            SetLabel();
            FillWithExpenses();
            BtnOk.IsDefault = true;
        }

        private void SetLabel()
        {
            var str = selectedDate != new DateTime() ? selectedDate.ToString("d.MM.yyyy") : DataSet.billingPeriods.ElementAt(currentPeriod).startDate.ToString("d.MM") + "-" + DataSet.billingPeriods.ElementAt(currentPeriod).endDate.ToString("d.MM");
            if (selectedCategory != null)
            {
                str += ", " + selectedCategory.name;
            }
            Label.Content = str;
        }

        private void FillWithExpenses()
        {
            var expenses = selectedDate != new DateTime() ?
                DataSet.billingPeriods.ElementAt(currentPeriod).GetExpensesOfCategoryAndDate(selectedCategory, selectedDate) :
                DataSet.billingPeriods.ElementAt(currentPeriod).GetExpensesOfCategory(selectedCategory);
            DataSet.expensesList = new List<Expense>(expenses);
            _ = new BillingPeriodExpensesListCreator(ExpensesGrid);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void ExpenseWindow_Closed(object sender, EventArgs e)
        {
            DataSet.selectedExpense = null;
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
