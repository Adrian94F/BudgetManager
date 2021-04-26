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

        // settings
        public static Settings settings = new Settings();

        public static bool IsNotEmpty()
        {
            return billingPeriods.Count > 0;
        }
    }
}
