using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    [Serializable]
    class ExpenseCategory
    {
        public string name;
        public ExpenseCategory parent;
    }
}
