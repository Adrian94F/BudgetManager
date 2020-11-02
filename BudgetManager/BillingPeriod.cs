using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BudgetManager
{
    [Serializable]
    class BillingPeriod : IComparable
    {
        public DateTime startDate;
        public DateTime endDate;
        public decimal netIncome;
        public decimal additionalIncome;

        public HashSet<Expense> expenses;

        public int CompareTo(object obj)
        {
            return startDate.CompareTo(((BillingPeriod)obj).startDate);
        }



        public decimal GetSumOfExpensesOfCategoryAndDate(ExpenseCategory category, DateTime date)
        {
            var ret = Decimal.Zero;
            var day = date.Day;
            var month = date.Month;

            foreach (var expense in expenses)
            {
                var expDay = expense.date.Day;
                var expMonth = expense.date.Month;
                if (expense.category == category && day == expDay && month == expMonth)
                {
                    ret += expense.value;
                }
            }
            return ret;
        }



        public decimal GetSumOfExpensesOfDate(DateTime date)
        {
            var ret = Decimal.Zero;
            var day = date.Day;
            var month = date.Month;

            foreach (var expense in expenses)
            {
                var expDay = expense.date.Day;
                var expMonth = expense.date.Month;
                if (day == expDay && month == expMonth)
                {
                    ret += expense.value;
                }
            }
            return ret;
        }


        public decimal GetSumOfExpensesOfCategory(ExpenseCategory category)
        {
            var ret = Decimal.Zero;

            foreach (var expense in expenses)
            {
                if (expense.category == category)
                {
                    ret += expense.value;
                }
            }
            return ret;
        }


        public decimal GetSumOfExpenses()
        {
            var ret = Decimal.Zero;

            foreach (var expense in expenses)
            {
                ret += expense.value;
            }
            return ret;
        }
    }
}
