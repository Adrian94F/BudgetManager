using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    static class DataSet
    {
        public static HashSet<BillingPeriod> billingPeriods = new HashSet<BillingPeriod>();
        public static HashSet<ExpenseCategory> expenseCategories = new HashSet<ExpenseCategory>();
    }
}
