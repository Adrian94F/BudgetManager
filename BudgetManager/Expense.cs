using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    class Expense
    {
        public DateTime date;
        public decimal value;
        public string comment;
        public ExpenseCategory category;
    }
}
