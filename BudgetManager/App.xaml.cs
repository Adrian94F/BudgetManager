using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BudgetManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private FilesHandler filesHandler;
        protected void LoadData()
        {
            filesHandler = new FilesHandler();
            filesHandler.ReadData();

        }
        protected void SaveData()
        {
            filesHandler.SaveData();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.LoadData();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.SaveData();
        }
    }
}
