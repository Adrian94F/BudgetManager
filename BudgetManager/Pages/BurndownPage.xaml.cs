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
using LiveCharts;
using LiveCharts.Wpf;

namespace BudgetManager.Pages
{
    /// <summary>
    /// Interaction logic for BurndownPage.xaml
    /// </summary>
    public partial class BurndownPage : Page
    {
        private BillingPeriod period;
        private int nOfDays;
        private double minValue = 0.0;
        private bool simplified = false;

        public BurndownPage()
        {
            InitializeComponent();
            FillPage();
        }

        public BurndownPage(bool isSimplified)
        {
            InitializeComponent();
            simplified = isSimplified;
            FillPage();
        }

        private void FillPage()
        {
            if (AppData.IsNotEmpty())
            {
                period = AppData.billingPeriods.ElementAt(AppData.currentPeriod);
                nOfDays = (period.endDate - period.startDate).Days + 2;
                Plot();
            }
        }

        private enum Series
        {
            Burndown,
            BurndownWithoutMonthlyExpenses,
            BurndownWithoutBigAndMonthlyExpenses,
            AverageBurndown,
            AverageBurndownWithoutMonthlyExpenses,
            AverageBurndownWithoutBigAndMonthlyExpenses,
            DailySums
        }

        private enum ExpensesType
        {
            Daily,
            DailyWithoutBig,
            All
        };

        private double GetIncomeSum(ExpensesType type)
        {
            var sum = (double)(period.netIncome + period.additionalIncome);
            switch (type)
            {
                case ExpensesType.Daily:
                    sum -= (double)period.GetSumOfMonthlyExpenses();
                    break;
                case ExpensesType.DailyWithoutBig:
                    sum -= (double)period.GetSumOfMonthlyExpenses();
                    sum -= (double)period.GetSumOfBigExpenses();
                    break;
                case ExpensesType.All:
                    break;
            }
            return sum;
        }

        private double[] GetBurndown(ExpensesType type)
        {
            var burnValues = new double[nOfDays];
            var incomeSum = GetIncomeSum(type);
            
            var yesterdaySum = burnValues[0] = incomeSum;
            for (var i = 1; i < nOfDays; i++)
            {
                burnValues[i] = yesterdaySum;
                switch (type)
                {
                    case ExpensesType.Daily:
                        burnValues[i] -= (double)period.GetSumOfDailyExpensesOfDate(period.startDate.AddDays(i - 1));
                        break;
                    case ExpensesType.DailyWithoutBig:
                        burnValues[i] -= (double)period.GetSumOfDailyNotBigExpensesOfDate(period.startDate.AddDays(i - 1)); ;
                        break;
                    case ExpensesType.All:
                        burnValues[i] -= (double)period.GetSumOfAllExpensesOfDate(period.startDate.AddDays(i - 1));
                        break;
                }
                yesterdaySum = burnValues[i];
            }
            return burnValues;
        }

        private void SetMinValue(double newValue)
        {
            if (newValue >= minValue)
                return;
            minValue = newValue;
            if (minValue < 0)
                minValue = Math.Floor(minValue / 500) * 500;
        }

        private double[] GetAverageBurndown(ExpensesType type)
        {
            var avgBurnValues = new double[nOfDays];
            var incomeSum = GetIncomeSum(type);
            var plannedSavings = (double)period.plannedSavings;
            for (var i = 0; i < nOfDays; i++)
            {
                var a = -(incomeSum - plannedSavings) / (nOfDays - 1);
                var b = incomeSum;
                avgBurnValues[i] = a * i + b;
                SetMinValue(avgBurnValues[i]);
            }
            return avgBurnValues;
        }

        private double[] GetDailyExpenses()
        {
            var dailyExpenses = new double[nOfDays];
            dailyExpenses[0] = (double)period.GetSumOfMonthlyExpenses();
            for (var i = 1; i < nOfDays; i++)
            {
                dailyExpenses[i] = (double)period.GetSumOfDailyExpensesOfDate(period.startDate.AddDays(i - 1));
                SetMinValue(dailyExpenses[i]);
            }
            return dailyExpenses;
        }
        

        private LineSeries GetLineSeries(Series series)
        {
            var title = "";
            var values = new double[]{};
            var stroke = Brushes.Black;
            var dashArray = new DoubleCollection();
            var fill = Brushes.Transparent;
            var pointGeometry = DefaultGeometries.None;
            var lineSmoothness = 0;
            switch (series)
            {
                case Series.Burndown:
                    title = "Wszystkie";
                    values = GetBurndown(ExpensesType.All);
                    stroke = Brushes.YellowGreen;
                    break;
                case Series.AverageBurndown:
                    title = "Wszystkie (plan)";
                    values = GetAverageBurndown(ExpensesType.All);
                    stroke = Brushes.YellowGreen;
                    dashArray = new DoubleCollection {2};
                    break;
                case Series.BurndownWithoutMonthlyExpenses:
                    title = "Codzienne z dużymi";
                    values = GetBurndown(ExpensesType.Daily);
                    stroke = Brushes.LightSeaGreen;
                    break;
                case Series.AverageBurndownWithoutMonthlyExpenses:
                    title = "Codzienne z dużymi (plan)";
                    values = GetAverageBurndown(ExpensesType.Daily);
                    stroke = Brushes.LightSeaGreen;
                    dashArray = new DoubleCollection { 2 };
                    break;
                case Series.BurndownWithoutBigAndMonthlyExpenses:
                    title = "Codzienne";
                    values = GetBurndown(ExpensesType.DailyWithoutBig);
                    stroke = Brushes.DodgerBlue;
                    break;
                case Series.AverageBurndownWithoutBigAndMonthlyExpenses:
                    title = "Codzienne (plan)";
                    values = GetAverageBurndown(ExpensesType.DailyWithoutBig);
                    stroke = Brushes.DodgerBlue;
                    dashArray = new DoubleCollection { 2 };
                    break;
            }

            return new LineSeries
            {
                Title = title,
                Values = new ChartValues<double>(values),
                PointGeometry = pointGeometry,
                LineSmoothness = lineSmoothness,
                Stroke = stroke,
                StrokeDashArray = dashArray,
                Fill = fill
            };
        }

        private ColumnSeries GetColumnSeries(Series series)
        {
            var title = "";
            var values = new double[] { };
            var stroke = Brushes.Black;
            var fill = Brushes.Transparent;
            var dataLabels = !simplified;
            switch (series)
            {
                case Series.DailySums:
                    title = "Dziennie";
                    values = GetDailyExpenses();
                    fill = Brushes.Gold;
                    break;
            }

            return new ColumnSeries
            {
                Title = title,
                Values = new ChartValues<double>(values),
                DataLabels = dataLabels,
                Fill = fill
            };
        }

        private SeriesCollection GetSeriesCollection(bool onlyDailyAndNotBigExpenses = false, bool averages = true)
        {
            var collection = new SeriesCollection();

            collection.Add(GetColumnSeries(Series.DailySums));

            if (!onlyDailyAndNotBigExpenses)
                collection.Add(GetLineSeries(Series.Burndown));
            if (averages && !onlyDailyAndNotBigExpenses)
                collection.Add(GetLineSeries(Series.AverageBurndown));

            if (!onlyDailyAndNotBigExpenses) 
                collection.Add(GetLineSeries(Series.BurndownWithoutMonthlyExpenses));
            if (averages && !onlyDailyAndNotBigExpenses)
                collection.Add(GetLineSeries(Series.AverageBurndownWithoutMonthlyExpenses));

            collection.Add(GetLineSeries(Series.BurndownWithoutBigAndMonthlyExpenses));
            if (averages)
                collection.Add(GetLineSeries(Series.AverageBurndownWithoutBigAndMonthlyExpenses));

            return collection;
        }

        private string[] GetLabels()
        {
            var labels = new string[nOfDays];
            labels[0] = "start";
            for (var i = 1; i < nOfDays; i++)
            {
                labels[i] = period.startDate.AddDays(i - 1).ToString("dd");
            }

            return labels;
        }

        private SectionsCollection GetSections(bool getToday, bool getWeekends)
        {
            var axisSections = new SectionsCollection();

            if (getToday)
            {
                var todayAxisSection = new AxisSection
                {
                    Draggable = false,
                    SectionOffset = 0.5 + (DateTime.Today - period.startDate).Days,
                    SectionWidth = 1,
                    Fill = Application.Current.Resources["Alpha-Green"] as SolidColorBrush
                };
                axisSections.Add(todayAxisSection);
            }

            if (getWeekends)
            {
                var start = period.startDate;
                int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)start.DayOfWeek + 7) % 7;
                var saturday = start.AddDays(daysUntilSaturday);
                while ((period.endDate - saturday).Days >= 0)
                {
                    var weekendSection = new AxisSection
                    {
                        Draggable = false,
                        SectionOffset = 0.5 + (saturday - period.startDate).Days,
                        SectionWidth = 2,
                        Fill = Application.Current.Resources["Alpha-Gray-1"] as SolidColorBrush
                    };
                    axisSections.Add(weekendSection);
                    saturday = saturday.AddDays(7);
                }
            }

            return axisSections;
        }

        private void Plot()
        {
            var chart = new CartesianChart();
            var grid = BurndownGrid;

            // prepare data
            var nOfDays = (period.endDate - period.startDate).Days + 2;

            // X axis
            chart.AxisX.Add(new Axis
            {
                Labels = GetLabels(),
                Separator = !simplified
                ? new LiveCharts.Wpf.Separator  // force the separator step to 1, so it always display all labels
                {
                    Step = 1,
                    IsEnabled = false
                }
                : new LiveCharts.Wpf.Separator
                {
                    IsEnabled = false
                },
                Sections = GetSections(true, true)
            });

            // series
            chart.Series = GetSeriesCollection(simplified);

            chart.AxisY.Add(new Axis
            {
                LabelFormatter = value => Math.Round(value).ToString(),
                Separator = new LiveCharts.Wpf.Separator(),
                MinValue = minValue
            });

            // add to grid
            chart.Margin = new System.Windows.Thickness(0);
            chart.LegendLocation = simplified
                ? LegendLocation.None
                : LegendLocation.Top;
            grid.Children.Clear();
            grid.Children.Add(chart);
        }
    }
}
