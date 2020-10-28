using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Data;

namespace BudgetManager
{
    class FilesHandler
    {
        readonly string pathToHistoricalFolder = "..\\..\\import data";
        public void ReadHistoricalData()
        {
            var dictionary = this.ReadDictionary();
            var files = Directory.GetFiles(pathToHistoricalFolder);
            foreach (var file in files)
            {
                if (Path.GetFileNameWithoutExtension(file) != "dictionary")
                {
                    ReadHistoricalFile(file, dictionary);
                }
            }
        }

        void ReadHistoricalFile(string path, KeyValuePair<string, string> dictionary)
        {
            var dayRow = 1;
            var dataStartRow = 2;
            var categoryColumn = 1;
            var dataStartColumn = 5;

            // add billing period based on filename
            var name = Path.GetFileNameWithoutExtension(path);
            Console.Write(path);
            DateTime date = Convert.ToDateTime(name);
            var period = new BillingPeriod();
            period.startDate = date;

            DataSet.billingPeriods.Add(period);

            // open the file
            // csv to table of strings
            // for every row with category
            // check if category exists - if not, create (based on dictionary) new one
            // read expenses based on day in the top row
        }

        KeyValuePair<string, string> ReadDictionary()
        {
            var dict = new KeyValuePair<string, string>();

            // TODO

            return dict;
        }

        public void ReadSavedDatabase()
        { }

        public void SaveDatabase()
        { }
    }
}
