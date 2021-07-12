using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Models
{
    public class Income
    {
        public enum IncomeType
        {
            Salary,
            Additional
        };

        public decimal value;
        public IncomeType type;
        public string comment;
    }
}
