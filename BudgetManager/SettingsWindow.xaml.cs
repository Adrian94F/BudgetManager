using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            TypicalStartDayTextBox.Text = AppData.settings.TypicalBeginningOfPeriod.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var typicalStartDay = ParseIntegerString(TypicalStartDayTextBox.Text);
            TypicalStartDayTextBox.Text = typicalStartDay.ToString();
            if (typicalStartDay < 1 || typicalStartDay > 31)
            {
                MessageBox.Show("Wpisano zły domyślny dzień rozpoczęcia okresu rozliczeniowego. Podaj wartość z przedziału 1-31.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppData.settings.TypicalBeginningOfPeriod = typicalStartDay;

            var fh = new FilesHandler();
            fh.SaveSettings();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int ParseIntegerString(string str)
        {
            str = Regex.Replace(str, "[^0-9]", "");
            int ret = 0;
            try
            {
                ret = Convert.ToInt32(str);
            }
            catch (Exception) {}
            return ret;
        }

        private void NumberTextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            txtBox.Text = ParseIntegerString(txtBox.Text).ToString();
        }
    }
}
