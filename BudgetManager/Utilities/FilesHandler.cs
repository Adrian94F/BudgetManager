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
        readonly string pathToHistoricalFolder = "..\\..\\import data";
        readonly string pathToSettings = "settings.data";
        public void ReadHistoricalData()
        {
            var files = Directory.GetFiles(pathToHistoricalFolder);
            foreach (var file in files)
            {
                ReadHistoricalFile(file);
            }
            SetAdditionalData();

        }

        List<string[]> ReadCsvToArray(string path)
        {
            var ret = new List<string[]>();
            var reader = new StreamReader(File.OpenRead(path));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var splitted = line.Split(';');
                ret.Add(splitted);
            }
            return ret;
        }


        void SetAdditionalData()
        {
            for (var i = 0; i < AppData.billingPeriods.Count - 1; i++)
            {
                AppData.billingPeriods.ElementAt(i).endDate = AppData.billingPeriods.ElementAt(i + 1).startDate - TimeSpan.FromDays(1);
            }
            var typicalEndDay = (AppData.settings.TypicalBeginningOfPeriod - 1) % 31 + 1;
            var endYear = AppData.billingPeriods.Last().startDate.Year;
            var endMonth = AppData.billingPeriods.Last().startDate.AddMonths(1).Month;
            AppData.billingPeriods.Last().endDate = new DateTime(endYear, endMonth, typicalEndDay);
        }

        void ReadHistoricalFile(string path)
        {
            var dataStartRow = 2;
            var categoryColumn = 1;
            var dataStartColumn = 5;
            var incomeCol = 4;
            var netIncomeRow = 48;
            var addIncomeRow = 49;

            // add billing period based on filename
            var period = new BillingPeriod();

            var name = Path.GetFileNameWithoutExtension(path);
            var date = Convert.ToDateTime(name);

            // set start date of current period
            period.startDate = date;
            period.expenses = new HashSet<Expense>();


            // read csv to array
            var fileData = ReadCsvToArray(path);

            // for every row with category
            for (var row = dataStartRow; row < fileData.Count; row++)
            {
                // check if category exists - if not, create (based on dictionary) new one
                var categoryFromFile = fileData[row][categoryColumn];
                if (categoryFromFile == "Podsumowanie")
                {
                    break;
                }
                ExpenseCategory category = null;
                foreach (var cat in AppData.expenseCategories)
                {
                    if (cat.name == categoryFromFile)
                    {
                        category = cat;
                        break;
                    }
                }
                if (category == null)
                {
                    category = new ExpenseCategory();
                    category.name = categoryFromFile;
                    AppData.expenseCategories.Add(category);
                }

                for (var col = dataStartColumn; col < fileData[row].Length; col++)
                {
                    // read expenses based on day in the top row
                    var value = fileData[row][col];
                    if (value == "" || value == "0")
                    {
                        continue;
                    }
                    var decimalValue = decimal.Parse(value);

                    var exp = new Expense();
                    exp.category = category;
                    exp.value = decimalValue;
                    exp.comment = "imported";

                    var dayOfPeriod = col - dataStartColumn;
                    exp.date = period.startDate.AddDays(dayOfPeriod);

                    period.expenses.Add(exp);
                }

                // read income
                var netIncome = fileData[netIncomeRow][incomeCol];
                netIncome = netIncome != "" ? netIncome : "0";
                var addIncome = fileData[addIncomeRow][incomeCol];
                addIncome = addIncome != "" ? addIncome : "0";
                period.netIncome = decimal.Parse(netIncome, NumberStyles.AllowCurrencySymbol | NumberStyles.Number);
                period.additionalIncome = decimal.Parse(addIncome, NumberStyles.AllowCurrencySymbol | NumberStyles.Number);
            }

            // add period
            AppData.billingPeriods.Add(period);
        }


        public void ReadData()
        {
            ReadSettings();
            
            AppData.billingPeriods = new SortedSet<BillingPeriod>();
            AppData.expenseCategories = new HashSet<ExpenseCategory>();
            //ReadHistoricalData();
            ReadSerialized();
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
