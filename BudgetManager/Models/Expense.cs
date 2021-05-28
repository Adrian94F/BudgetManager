using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    public class Expense
    {
        public DateTime date { get; set; }
        public decimal value { get; set; }
        public string comment { get; set; }
        public ExpenseCategory category { get; set; }
        public bool monthlyExpense { get; set; }

        public Expense()
        {
            date = DateTime.Today;
            monthlyExpense = false;
        }
    }
}
