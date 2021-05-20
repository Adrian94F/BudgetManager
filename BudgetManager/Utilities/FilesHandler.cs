using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BudgetManager
{
    class FilesHandler
    {
        readonly static string pathToSettings = "settings.json";

        public static void ReadData()
        {
            ReadSettings();
            
            AppData.billingPeriods = new SortedSet<BillingPeriod>();
            AppData.expenseCategories = new HashSet<ExpenseCategory>();
            //ReadHistoricalData();
            ReadSerialized();
            SetupVariables();
        }

        private static void SetupVariables()
        {
            AppData.currentPeriod = AppData.billingPeriods.Count - 1;
        }

        private static void ReadSerialized()
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

        public static void ReadSettings()
        {
            var jsonString = File.ReadAllText(pathToSettings);
            AppData.settings = JsonSerializer.Deserialize<Settings>(jsonString);

            if (AppData.settings.PathToAppData == null)
                AppData.settings.PathToAppData = "dataset.data";
        }

        public static void SaveData()
        {
            var pathToDataSet = AppData.settings.PathToAppData;
            var formatter = new BinaryFormatter();
            var dataSet = new Tuple<HashSet<ExpenseCategory>, SortedSet<BillingPeriod>>(AppData.expenseCategories, AppData.billingPeriods);
            var stream = new FileStream(pathToDataSet, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, dataSet);
            stream.Close();
        }

        public static void SaveSettings()
        {
            byte[] jsonUtf8Bytes;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(AppData.settings, options);
            File.WriteAllBytes(pathToSettings, jsonUtf8Bytes);
        }
    }
}
