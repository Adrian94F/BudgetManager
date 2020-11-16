using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BudgetManager
{
    class BillingPeriodChartCreator
    {
        BillingPeriod period;
        Grid grid;

        public BillingPeriodChartCreator(BillingPeriod bp, Grid g)
        {
            period = bp;
            grid = g;
        }

        public void Plot()
        {

        }
    }
}
