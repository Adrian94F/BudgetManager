using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace BudgetManager
{
    class Utilities
    {
        public static string EmptyIfZero(Decimal value)
        {
            return value > Decimal.Zero ? value.ToString("F") : "";
        }

        public static decimal ParseDecimalString(string str)
        {
            str = Regex.Replace(str, "[^0-9-,]", "");
            decimal ret;
            try
            {
                ret = decimal.Parse(str, NumberStyles.AllowCurrencySymbol | NumberStyles.Number | NumberStyles.AllowLeadingSign);
            }
            catch (Exception)
            {
                ret = Decimal.Zero;
            }
            return ret;
        }

        public static Window OpenNewOrRestoreWindow<T>() where T : Window, new()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w is T)
                {
                    w.Activate();
                    return w;
                }
            }

            T newWindow = new T();
            newWindow.Show();
            return newWindow;
        }

        public static Tuple<Window, bool> OpenNewOrRestoreWindowAndCheckIfNew<T>() where T : Window, new()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w is T)
                {
                    w.Activate();
                    return new Tuple<Window, bool>(w, false);
                }
            }

            T newWindow = new T();
            newWindow.Show();
            return new Tuple<Window, bool>(newWindow, true);
        }
    }
}
