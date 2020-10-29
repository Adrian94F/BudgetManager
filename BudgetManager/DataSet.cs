using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    [Serializable]
    static class DataSet
    {
        public static SortedSet<BillingPeriod> billingPeriods = new SortedSet<BillingPeriod>();
        public static HashSet<ExpenseCategory> expenseCategories = new HashSet<ExpenseCategory>();
    }
}
