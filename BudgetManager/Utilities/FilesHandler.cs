﻿using System;
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
using BudgetManager.Models;
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
        readonly static string billingPeriodIncomesKey = "incomes";
        readonly static string incomeValueKey = "value";
        readonly static string incomeTypeKey = "type";
        readonly static string billingPeriodIncomeTypeSalary = "salary";
        readonly static string billingPeriodIncomeTypeAdditional = "additional";
        readonly static string incomeCommentKey = "comment";
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
            ReadDataJsonDict();
            SetupVariables();
        }

        private static void SetupVariables()
        {
            AppData.currentPeriod = AppData.billingPeriods.Count - 1;
        }

        private static void ReadDataJsonDict()
        {
            var pathToDataSet = AppData.settings.PathToAppData;
            if (File.Exists(pathToDataSet))
            {
                var sr = new StreamReader(pathToDataSet);
                string jsonString = sr.ReadToEnd();
                sr.Close();

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
                    var plannedSavings = Decimal.Parse(billingPeriodFromJson[billingPeriodPlannedSavingsKey].ToString(), CultureInfo.InvariantCulture);

                    var billingPeriod = new BillingPeriod()
                    {
                        startDate = startDate,
                        endDate = endDate,
                        plannedSavings = plannedSavings
                    };

                    // new incomes
                    if (billingPeriodFromJson.Keys.Contains(billingPeriodIncomesKey))
                    {
                        var incomes = JsonSerializer.Deserialize<List<object>>(billingPeriodFromJson[billingPeriodIncomesKey].ToString());
                        foreach (var inc in incomes)
                        {
                            var incomeFromJson = JsonSerializer.Deserialize<Dictionary<string, string>>(inc.ToString());
                            var value = Decimal.Parse(incomeFromJson[incomeValueKey].ToString(), CultureInfo.InvariantCulture);
                            if (value == 0)
                                continue;
                            var typeString = incomeFromJson[incomeTypeKey];
                            var type = typeString.Equals(billingPeriodIncomeTypeSalary)
                                ? Income.IncomeType.Salary
                                : Income.IncomeType.Additional;
                            var comment = incomeFromJson.Keys.Contains(incomeCommentKey)
                                ? incomeFromJson[incomeCommentKey]
                                : "";

                            var income = new Income()
                            {
                                value = value,
                                type = type,
                                comment = comment
                            };
                            billingPeriod.incomes.Add(income);
                        }
                    }
                    else
                    {
                        if (billingPeriodFromJson.Keys.Contains(billingPeriodNetIncomeKey))
                        {
                            var income = new Income()
                            {
                                value = Decimal.Parse(billingPeriodFromJson[billingPeriodNetIncomeKey].ToString(), CultureInfo.InvariantCulture),
                                type = Income.IncomeType.Salary
                            };
                            if (income.value > decimal.Zero)
                                billingPeriod.incomes.Add(income);
                        }
                        if (billingPeriodFromJson.Keys.Contains(billingPeriodAdditionalIncomeKey))
                        {
                            var income = new Income()
                            {
                                value = Decimal.Parse(billingPeriodFromJson[billingPeriodAdditionalIncomeKey].ToString(), CultureInfo.InvariantCulture),
                                type = Income.IncomeType.Additional
                            };
                            if (income.value > decimal.Zero)
                                billingPeriod.incomes.Add(income);
                        }
                    }

                    //expenses
                    var expenses = JsonSerializer.Deserialize<List<object>>(billingPeriodFromJson[billingPeriodExpensesKey].ToString());
                    foreach (var exp in expenses)
                    {
                        var expenseFromJson = JsonSerializer.Deserialize<Dictionary<string, string>>(exp.ToString());

                        var value = Decimal.Parse(expenseFromJson[expenseValueKey].ToString(), CultureInfo.InvariantCulture);
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
            if (File.Exists(pathToSettings))
            {
                var jsonString = File.ReadAllText(pathToSettings);
                AppData.settings = JsonSerializer.Deserialize<Settings>(jsonString);
            }
            else
            {
                AppData.settings = new Settings();
            }

            if (AppData.settings.PathToAppData == null)
                AppData.settings.PathToAppData = "dataset.json";
        }

        public static void SaveData()
        {
            SaveDataJsonDict();
        }

        public static byte[] SaveDataJsonDict()
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
                period.Add(billingPeriodPlannedSavingsKey, bp.plannedSavings);

                // backwards compatibility
                period.Add(billingPeriodNetIncomeKey, bp.GetSumOfIncomes(Income.IncomeType.Salary));
                period.Add(billingPeriodAdditionalIncomeKey, bp.GetSumOfIncomes(Income.IncomeType.Additional));

                // incomes
                var incomes = new List<Dictionary<string, object>>();

                foreach (var inc in bp.incomes)
                {
                    var income = new Dictionary<string, object>();
                    income.Add(incomeValueKey, inc.value);
                    var type = inc.type == Income.IncomeType.Salary
                        ? billingPeriodIncomeTypeSalary
                        : billingPeriodIncomeTypeAdditional;
                    income.Add(incomeTypeKey, type);
                    if (inc.comment != null)
                        income.Add(incomeCommentKey, inc.comment);
                    incomes.Add(income);
                }


                period.Add(billingPeriodIncomesKey, incomes);

                // expenses
                var expenses = new List<Dictionary<string, object>>();
                foreach (var exp in bp.expenses)
                {
                    var expense = new Dictionary<string, object>();
                    // fields
                    expense.Add(expenseValueKey, exp.value);
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
                WriteIndented = false,
                NumberHandling = JsonNumberHandling.WriteAsString
            };
            var jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(output, options);
            File.WriteAllBytes(AppData.settings.PathToAppData, jsonUtf8Bytes);
            
            return jsonUtf8Bytes;
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
