using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    static class AppData
    {
        public static SortedSet<BillingPeriod> billingPeriods = new SortedSet<BillingPeriod>();
        public static HashSet<ExpenseCategory> expenseCategories = new HashSet<ExpenseCategory>();
        public static bool isDataChanged = false;

        // main window
        public static int currentPeriod = -1;

        // expense(s) window, list...
        public static BillingPeriod selectedPeriod;
        public static ExpenseCategory selectedCategory;
        public static DateTime selectedDate;
        public static Expense selectedExpense;

        // settings
        public static Settings settings = new Settings();
    }
}
