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
using System.Xml.Serialization;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace BudgetManager
{
    class FilesHandler
    {
        readonly static string pathToSettings = "settings.json";

        readonly static string categoriesKey = "categories";
        readonly static string billingPeriodsKey = "billingPeriods";
        readonly static string billingPeriodStartDateKey = "start";
        readonly static string billingPeriodEndDateKey = "end";
        readonly static string billingPeriodNetIncomeKey = "netIncome";
        readonly static string billingPeriodAdditionalIncomeKey = "additionalIncome";
        readonly static string billingPeriodPlannedSavingsKey = "plannedSavings";
        readonly static string billingPeriodExpensesKey = "expenses";
        readonly static string expenseValueKey = "value";
        readonly static string expenseDateKey = "date";
        readonly static string expenseCommentKey = "comment";
        readonly static string expenseMonthlyExpenseKey = "monthlyExpense";
        readonly static string expenseCategoryKey = "category";

        public static void ReadData()
        {
            ReadSettings();
            
            AppData.billingPeriods = new SortedSet<BillingPeriod>();
            AppData.expenseCategories = new HashSet<ExpenseCategory>();
            //ReadSerialized();
            ReadDataJsonDict();
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

        private static void ReadDataJsonDict()
        {
            var pathToDataSet = AppData.settings.PathToAppData + ".json";
            if (File.Exists(pathToDataSet))
            {
                var sr = new StreamReader(pathToDataSet);
                string jsonString = sr.ReadToEnd();

                var dataSet = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);

                // categories
                var categories = JsonSerializer.Deserialize<List<object>>(dataSet[categoriesKey].ToString());
                foreach (var cat in categories)
                {
                    var category = new ExpenseCategory()
                    {
                        name = cat.ToString()
                    };
                    AppData.expenseCategories.Add(category);
                }

                // billing periods
                var billingPeriods = JsonSerializer.Deserialize<List<object>>(dataSet[billingPeriodsKey].ToString());
                foreach (var bp in billingPeriods)
                {
                    var billingPeriodFromJson = JsonSerializer.Deserialize<Dictionary<string, object>>(bp.ToString());

                    var startDate = DateTime.Parse(billingPeriodFromJson[billingPeriodStartDateKey].ToString());
                    var endDate = DateTime.Parse(billingPeriodFromJson[billingPeriodEndDateKey].ToString());
                    var net = billingPeriodFromJson[billingPeriodNetIncomeKey];
                    var netIncome = Decimal.Parse(billingPeriodFromJson[billingPeriodNetIncomeKey].ToString());
                    var addIncome = Decimal.Parse(billingPeriodFromJson[billingPeriodAdditionalIncomeKey].ToString());
                    var plannedSavings = Decimal.Parse(billingPeriodFromJson[billingPeriodPlannedSavingsKey].ToString());

                    var billingPeriod = new BillingPeriod()
                    {
                        startDate = startDate,
                        endDate = endDate,
                        netIncome = netIncome,
                        additionalIncome = addIncome,
                        plannedSavings = plannedSavings
                    };

                    //expenses
                    var expenses = JsonSerializer.Deserialize<List<object>>(billingPeriodFromJson[billingPeriodExpensesKey].ToString());
                    foreach (var exp in expenses)
                    {
                        var expenseFromJson = JsonSerializer.Deserialize<Dictionary<string, string>>(exp.ToString());

                        var value = Decimal.Parse(expenseFromJson[expenseValueKey].ToString());
                        var date = DateTime.Parse(expenseFromJson[expenseDateKey].ToString());
                        var comment = expenseFromJson[expenseCommentKey].ToString();
                        var monthlyExpense = expenseFromJson[expenseMonthlyExpenseKey].ToString().Equals("True")
                            ? true
                            : false;
                        var categoryName = expenseFromJson[expenseCategoryKey].ToString();
                        var category = new ExpenseCategory()
                        {
                            name = categoryName
                        };
                        foreach (var cat in AppData.expenseCategories)
                            if (cat.Equals(category))
                            {
                                category = cat;
                                break;
                            }

                        var expense = new Expense()
                        {
                            value = value,
                            date = date,
                            comment = comment,
                            monthlyExpense = monthlyExpense,
                            category = category
                        };
                        billingPeriod.expenses.Add(expense);
                    }

                    AppData.billingPeriods.Add(billingPeriod);
                }
            }
        }

        public static void ReadSettings()
        {
            var jsonString = File.ReadAllText(pathToSettings);
            try
            {
                AppData.settings = JsonSerializer.Deserialize<Settings>(jsonString);
            }
            catch (Exception e)
            {
                AppData.settings = new Settings();
            }

            if (AppData.settings.PathToAppData == null)
                AppData.settings.PathToAppData = "dataset.data";
        }

        public static void SaveData()
        {
            SaveDataJsonDict();
            /*var pathToDataSet = AppData.settings.PathToAppData;
            var formatter = new BinaryFormatter();
            var dataSet = new Tuple<HashSet<ExpenseCategory>, SortedSet<BillingPeriod>>(AppData.expenseCategories, AppData.billingPeriods);
            var stream = new FileStream(pathToDataSet, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, dataSet);
            stream.Close();*/
        }

        public static void SaveDataJsonDict()
        {
            var output = new Dictionary<string, object>();

            // categories
            var categories = new List<object>();
            foreach (var cat in AppData.expenseCategories)
            {
                // category
                categories.Add(cat.name);
            }
            output.Add(categoriesKey, categories);

            // billing periods
            var billingPeriods = new List<object>();
            foreach (var bp in AppData.billingPeriods)
            {
                var period = new Dictionary<string, object>();
                // fields
                period.Add(billingPeriodStartDateKey, bp.startDate);
                period.Add(billingPeriodEndDateKey, bp.endDate);
                period.Add(billingPeriodNetIncomeKey, bp.netIncome.ToString());
                period.Add(billingPeriodAdditionalIncomeKey, bp.additionalIncome.ToString());
                period.Add(billingPeriodPlannedSavingsKey, bp.plannedSavings.ToString());

                // expenses
                var expenses = new List<Dictionary<string, object>>();
                foreach (var exp in bp.expenses)
                {
                    var expense = new Dictionary<string, object>();
                    // fields
                    expense.Add(expenseValueKey, exp.value.ToString());
                    expense.Add(expenseDateKey, exp.date);
                    expense.Add(expenseCommentKey, exp.comment);
                    expense.Add(expenseMonthlyExpenseKey, exp.monthlyExpense.ToString());
                    expense.Add(expenseCategoryKey, exp.category.name);

                    expenses.Add(expense);
                }
                period.Add(billingPeriodExpensesKey, expenses);

                billingPeriods.Add(period);
            }
            output.Add(billingPeriodsKey, billingPeriods);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(output, options);
            File.WriteAllBytes(AppData.settings.PathToAppData + ".json", jsonUtf8Bytes);
        }

        public static void SaveSettings()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(AppData.settings, options);
            File.WriteAllBytes(pathToSettings, jsonUtf8Bytes);
        }
    }
}
