using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BudgetManager
{
    class FilesHandler
    {
        readonly string pathToSettings = "settings.data";

        public void ReadData()
        {
            ReadSettings();
            
            AppData.billingPeriods = new SortedSet<BillingPeriod>();
            AppData.expenseCategories = new HashSet<ExpenseCategory>();
            //ReadHistoricalData();
            ReadSerialized();
            SetupVariables();
        }

        private void SetupVariables()
        {
            AppData.currentPeriod = AppData.billingPeriods.Count - 1;
        }

        private void ReadSerialized()
        {
            var pathToDataSet = AppData.settings.PathToAppData;
            if (File.Exists(pathToDataSet))
            {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(pathToDataSet, FileMode.Open, FileAccess.Read);
                var dataSet = (Tuple<HashSet<ExpenseCategory>, SortedSet<BillingPeriod>>)formatter.Deserialize(stream);
                AppData.expenseCategories.UnionWith(dataSet.Item1);
                AppData.billingPeriods.UnionWith(dataSet.Item2);
                stream.Close();
            }
        }

        private void ReadSettings()
        {
            if (File.Exists(pathToSettings))
            {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(pathToSettings, FileMode.Open, FileAccess.Read);
                var settings = (Settings)formatter.Deserialize(stream);
                AppData.settings = settings;
                stream.Close();
            }
        }

        public void SaveData()
        {
            var pathToDataSet = AppData.settings.PathToAppData;
            var formatter = new BinaryFormatter();
            var dataSet = new Tuple<HashSet<ExpenseCategory>, SortedSet<BillingPeriod>>(AppData.expenseCategories, AppData.billingPeriods);
            var stream = new FileStream(pathToDataSet, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, dataSet);
            stream.Close();
        }

        public void SaveSettings()
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(pathToSettings, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, AppData.settings);
            stream.Close();
        }
    }
}
