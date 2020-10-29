using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BudgetManager
{
    [Serializable]
    class BillingPeriod : IComparable
    {
        public DateTime startDate;
        public decimal netIncome;
        public decimal additionalIncome;

        public HashSet<Expense> expenses;

        public int CompareTo(object obj)
        {
            return startDate.CompareTo(((BillingPeriod)obj).startDate);
        }

        public Grid GetGrid()
        {
            var grid = new Grid();

            var categoryColumnDefinition = new ColumnDefinition();
            grid.ColumnDefinitions.Add(categoryColumnDefinition);

            var sumColumnDefinition = new ColumnDefinition();
            grid.ColumnDefinitions.Add(sumColumnDefinition);

            var numberOfdays = 31;
            for (var i = 0; i < numberOfdays; i++)
            {
                var dayColumn = new ColumnDefinition();
                grid.ColumnDefinitions.Add(dayColumn);
            }




            return grid;
        }
    }
}
