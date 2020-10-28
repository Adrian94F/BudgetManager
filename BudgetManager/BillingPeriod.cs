using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    class BillingPeriod
    {
        public DateTime startDate;
        public decimal netIncome;
        public decimal additionalIncome;

        public HashSet<Expense> expenses;
    }
}
