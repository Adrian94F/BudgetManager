using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    class Settings
    {
        public int TypicalBeginningOfPeriod { get; set; }
        public string PathToAppData { get; set; }
        public decimal BigExpenseThreshold { get; set; }

        public Settings()
        {
            BigExpenseThreshold = (Decimal)300.0;
        }
    }
}
