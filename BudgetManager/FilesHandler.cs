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

namespace BudgetManager
{
    class FilesHandler
    {
        readonly string pathToHistoricalFolder = "..\\..\\import data";
        public void ReadHistoricalData()
        {
            var files = Directory.GetFiles(pathToHistoricalFolder);
            foreach (var file in files)
            {
                ReadHistoricalFile(file);
            }
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
            period.startDate = date;
            period.expenses = new HashSet<Expense>();


            // read csv to array
            var fileData = ReadCsvToArray(path);

            // for every row with category
            for (var row = dataStartRow; row < fileData.Count; row++)
            {
                // check if category exists - if not, create (based on dictionary) new one
                var categoryFromFile = fileData[row][categoryColumn];
                if (categoryFromFile == "")
                {
                    break;
                }
                ExpenseCategory category = null;
                foreach (var cat in DataSet.expenseCategories)
                {
                    if (cat.name == categoryFromFile)
                    {
                        category = cat;
                        break;
                    }
                }
                if (category == null)
                {
                    var newCategory = new ExpenseCategory();
                    newCategory.name = categoryFromFile;
                    DataSet.expenseCategories.Add(newCategory);
                }

                for (var col = dataStartColumn; col < fileData[row].Length; col++)
                {
                    // read expenses based on day in the top row


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
            DataSet.billingPeriods.Add(period);
        }


        public void ReadSavedDatabase()
        { }

        public void SaveDatabase()
        { }
    }
}
