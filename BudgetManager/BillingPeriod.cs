using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
        public decimal netIncome = decimal.Zero;
        public decimal additionalIncome = decimal.Zero;
        public decimal plannedSavings = decimal.Zero;

        public HashSet<Expense> expenses = new HashSet<Expense>();

        public int CompareTo(object obj)
        {
            return startDate.CompareTo(((BillingPeriod)obj).startDate);
        }

        public decimal GetSumOfExpensesOfCategoryAndDate(ExpenseCategory category, DateTime date)
        {
            var ret = Decimal.Zero;
            var day = date.Day;
            var month = date.Month;

            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    var expDay = expense.date.Day;
                    var expMonth = expense.date.Month;
                    if (expense.category == category && day == expDay && month == expMonth)
                    {
                        ret += expense.value;
                    }
                }
            }
            return ret;
        }

        public HashSet<Expense> GetExpensesOfDayAndCategory(ExpenseCategory category, DateTime date)
        {
            var ret = new HashSet<Expense>();
            var day = date.Day;
            var month = date.Month;
            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    var expDay = expense.date.Day;
                    var expMonth = expense.date.Month;

                    if (day == expDay && month == expMonth)
                    {
                        if (category == null)
                        {
                            ret.Add(expense);
                        }
                        else if (expense.category == category)
                        {
                            ret.Add(expense);
                        }
                    }
                }
            }
            return ret;
        }

        public decimal GetSumOfExpensesOfDate(DateTime date)
        {
            var ret = Decimal.Zero;
            var day = date.Day;
            var month = date.Month;
            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    var expDay = expense.date.Day;
                    var expMonth = expense.date.Month;
                    if (day == expDay && month == expMonth)
                    {
                        ret += expense.value;
                    }
                }
            }
            return ret;
        }

        public decimal GetSumOfExpensesOfCategory(ExpenseCategory category)
        {
            var ret = Decimal.Zero;
            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    if (expense.category == category)
                    {
                        ret += expense.value;
                    }
                }
            }
            return ret;
        }

        public decimal GetSumOfExpenses()
        {
            var ret = Decimal.Zero;
            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    ret += expense.value;
                }
            }
            return ret;
        }
    }
}
