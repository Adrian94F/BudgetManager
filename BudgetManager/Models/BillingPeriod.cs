using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BudgetManager.Models;

namespace BudgetManager
{
    public class BillingPeriod : IComparable
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public decimal plannedSavings { get; set; }

        public HashSet<Expense> expenses { get; set; }

        public HashSet<Income> incomes { get; set; }

        public BillingPeriod()
        {
            incomes = new HashSet<Income>();
            expenses = new HashSet<Expense>();
        }

        public DateTime NewPeriodEndDate()
        {
            var date = this.endDate.AddMonths(1);
            var endDay = AppData.settings.TypicalBeginningOfPeriod - 1;
            var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            if (endDay > daysInMonth)
            {
                endDay = daysInMonth;
            }
            date = new DateTime(date.Year, date.Month, endDay);
            return date;
        }

        public decimal GetSumOfIncomes(Income.IncomeType? type = null)
        {
            var sum = decimal.Zero;
            foreach (var income in incomes)
                if (type == null || income.type == type)
                    sum += income.value;
            return sum;
        }

        public HashSet<Expense> GetCopyOfMonthlyExpensesForNextPeriod()
        {
            var ret = new HashSet<Expense>();
            foreach (var exp in expenses)
            {
                if (exp.monthlyExpense)
                {
                    var newExp = new Expense()
                    {
                        value = exp.value,
                        date = exp.date.AddMonths(1),
                        comment = exp.comment,
                        category = exp.category,
                        monthlyExpense = true
                    };
                    ret.Add(newExp);
                }
            }
            return ret;
        }

        public bool IsEmpty()
        {
            return expenses.Count == 0;
        }

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

        public HashSet<Expense> GetExpensesOfCategoryAndDate(ExpenseCategory category, DateTime date)
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

        public HashSet<Expense> GetExpensesOfCategory(ExpenseCategory category)
        {
            var ret = new HashSet<Expense>();
            if (expenses != null)
            {
                foreach (var expense in expenses)
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
            return ret;
        }

        private decimal GetSumOfValidExpensesOfDate(DateTime date, Func<Expense, bool> validator)
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
                        if (validator(expense))
                        {
                            ret += expense.value;
                        }
                    }
                }
            }
            return ret;
        }

        public decimal GetSumOfAllExpensesOfDate(DateTime date)
        {
            return GetSumOfValidExpensesOfDate(date, exp => true);
        }

        public decimal GetSumOfDailyExpensesOfDate(DateTime date)
        {
            return GetSumOfValidExpensesOfDate(date, exp => !exp.monthlyExpense);
        }

        public decimal GetSumOfDailyNotBigExpensesOfDate(DateTime date)
        {
            return GetSumOfValidExpensesOfDate(date, exp => !exp.monthlyExpense && exp.value < AppData.settings.BigExpenseThreshold);
        }

        public decimal GetSumOfMonthlyExpensesOfDate(DateTime date)
        {
            return GetSumOfAllExpensesOfDate(date) - GetSumOfDailyExpensesOfDate(date);
        }

        public decimal GetSumOfMonthlyExpenses()
        {
            var ret = Decimal.Zero;
            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    if (expense.monthlyExpense)
                    {
                        ret += expense.value;
                    }
                }
            }
            return ret;
        }

        public decimal GetSumOfBigExpenses()
        {
            var ret = Decimal.Zero;
            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    if (!expense.monthlyExpense && expense.value >= AppData.settings.BigExpenseThreshold)
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
