using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace BudgetManager
{
    class BillingPeriodChartCreator
    {
        BillingPeriod period;
        Grid grid;

        public BillingPeriodChartCreator(BillingPeriod bp, Grid g)
        {
            period = bp;
            grid = g;
        }

        public void Plot()
        {
            var chart = new CartesianChart();

            // prepare data
            var nOfDays = (period.endDate - period.startDate).Days + 2;
            var values = new double[nOfDays];
            var incomeSum = (double)(period.netIncome + period.additionalIncome);
            var yesterdaySum = values[0] = incomeSum;
            var minValue = 0.0;
            for (var i = 1; i < nOfDays; i++)
            {
                values[i] = yesterdaySum - (double)period.GetSumOfExpensesOfDate(period.startDate.AddDays(i - 1));
                yesterdaySum = values[i];
                minValue = values[i] < minValue ? values[i] : minValue;
            }
            if (minValue < 0)
            {
                minValue = Math.Floor(minValue / 1000) * 1000;
            }
            var avgBurnValues = new double[nOfDays];
            var plannedSavings = (double)period.plannedSavings;
            for (var i = 0; i < nOfDays; i++)
            {
                var a = -(incomeSum - plannedSavings) / (nOfDays - 1);
                var b = incomeSum;
                avgBurnValues[i] = a * i + b;
                //avgBurnValues[i] = avgBurnValues[i - 1] - (incomeSum / (nOfDays - 1));
            }

            // labels
            var labels = new string[nOfDays];
            labels[0] = "start";
            for (var i = 1; i < nOfDays; i++)
            {
                labels[i] = period.startDate.AddDays(i-1).ToString("dd.MM");
            }

            // axis section
            var todayAxisSection = new AxisSection
            {
                Draggable = false,
                SectionOffset = 0.5 + (DateTime.Today - period.startDate).Days,
                SectionWidth = 1
            };

            // X axis
            chart.AxisX.Add(new Axis
            {
                Labels = labels,
                Separator = new LiveCharts.Wpf.Separator // force the separator step to 1, so it always display all labels
                {
                    Step = 1,
                    IsEnabled = false //disable it to make it invisible.
                },
                LabelsRotation = 60,
                Sections = { todayAxisSection }
            });

            // series
            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Pozostała suma",
                    Values = new ChartValues<double>(values),
                    PointGeometry = DefaultGeometries.Circle,
                    Fill = System.Windows.Media.Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Średnia (planowana) pozostała suma",
                    Values = new ChartValues<double>(avgBurnValues),
                    PointGeometry = DefaultGeometries.Circle,
                    Fill = System.Windows.Media.Brushes.Transparent
                }
            };

            chart.AxisY.Add(new Axis
            {
                LabelFormatter = value => Math.Round(value).ToString(),
                Separator = new LiveCharts.Wpf.Separator(),
                MinValue = minValue
            });

            // add to grid
            chart.Margin = new System.Windows.Thickness(0);
            grid.Children.Clear();
            grid.Children.Add(chart);


        }
    }
}
