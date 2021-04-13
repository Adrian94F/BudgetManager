using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
