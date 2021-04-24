using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf.Controls;

namespace BudgetManager.User_controls
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class AppSettings : UserControl
    {
        public AppSettings()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            BillingPeriodTypicalBeginningNumberBox.Value = AppData.settings.TypicalBeginningOfPeriod;
            DataPathTextBox.Text = AppData.settings.PathToAppData;
        }

        private void BtnChangePathToDataSet_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = false,
                AllowNonFileSystemItems = true,
                Multiselect = false,
                DefaultFileName = "dataset.data",
                Title = "Wybierz lokalizację pliku z danymi aplikacji"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var directory = dialog.FileName;
                AppData.settings.PathToAppData = directory;
                DataPathTextBox.Text = directory;

                SaveSettings();
            }

            this.IsEnabled = true;
            this.Focus();
        }

        private void BillingPeriodTypicalBeginningNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            var typicalDay = (int)BillingPeriodTypicalBeginningNumberBox.Value;
            AppData.settings.TypicalBeginningOfPeriod = typicalDay;

            SaveSettings();
        }

        private void SaveSettings()
        {
            var fh = new FilesHandler();
            fh.SaveSettings();
        }
    }
}
