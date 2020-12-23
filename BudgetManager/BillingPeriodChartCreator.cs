﻿using System;
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
            Plot();
        }

        private void Plot()
        {
            var chart = new CartesianChart();

            // prepare data
            var nOfDays = (period.endDate - period.startDate).Days + 2;
            var minValue = 0.0;

            // burndown
            var burnValues = new double[nOfDays];
            var incomeSum = (double)(period.netIncome + period.additionalIncome);
            var yesterdaySum = burnValues[0] = incomeSum;
            for (var i = 1; i < nOfDays; i++)
            {
                burnValues[i] = yesterdaySum - (double)period.GetSumOfAllExpensesOfDate(period.startDate.AddDays(i - 1));
                yesterdaySum = burnValues[i];
                minValue = burnValues[i] < minValue ? burnValues[i] : minValue;
            }

            // minimal value on chart
            if (minValue < 0)
            {
                minValue = Math.Floor(minValue / 1000) * 1000;
            }

            // burndown without monhly expenses
            var burnValuesWoMonthlyExp = new double[nOfDays];
            var incomeSumWoMonthlyExp = (double)(period.netIncome + period.additionalIncome - period.GetSumOfMonthlyExpenses());
            yesterdaySum = burnValuesWoMonthlyExp[0] = incomeSumWoMonthlyExp;
            for (var i = 1; i < nOfDays; i++)
            {
                burnValuesWoMonthlyExp[i] = yesterdaySum - (double)period.GetSumOfDailyExpensesOfDate(period.startDate.AddDays(i - 1));
                yesterdaySum = burnValuesWoMonthlyExp[i];
            }

            // minimal value on chart
            if (minValue < 0)
            {
                minValue = Math.Floor(minValue / 1000) * 1000;
            }

            // average burndown
            var avgBurnValues = new double[nOfDays];
            var plannedSavings = (double)period.plannedSavings;
            for (var i = 0; i < nOfDays; i++)
            {
                var a = -(incomeSum - plannedSavings) / (nOfDays - 1);
                var b = incomeSum;
                avgBurnValues[i] = a * i + b;
            }

            //average burndown without monthly expenses
            var avgBurnValuesWoMonthlyExp = new double[nOfDays];
            for (var i = 0; i < nOfDays; i++)
            {
                var a = -(incomeSumWoMonthlyExp - plannedSavings) / (nOfDays - 1);
                var b = incomeSumWoMonthlyExp;
                avgBurnValuesWoMonthlyExp[i] = a * i + b;
            }

            // daily expense
            var dailyExpenses = new double[nOfDays];
            dailyExpenses[0] = (double)period.GetSumOfMonthlyExpenses();
            for (var i = 1; i < nOfDays; i++)
            {
                dailyExpenses[i] = (double)period.GetSumOfDailyExpensesOfDate(period.startDate.AddDays(i - 1));
            }

            // labels
            var labels = new string[nOfDays];
            labels[0] = "start";
            for (var i = 1; i < nOfDays; i++)
            {
                labels[i] = period.startDate.AddDays(i - 1).ToString("dd");
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
                Sections = { todayAxisSection }
            });

            // series
            var lineSmoothness = 0;
            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Pozostało",
                    Values = new ChartValues<double>(burnValues),
                    PointGeometry = DefaultGeometries.None,
                    LineSmoothness = lineSmoothness,
                    Stroke = Brushes.DodgerBlue,
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Pozostało (plan)",
                    Values = new ChartValues<double>(avgBurnValues),
                    PointGeometry = DefaultGeometries.None,
                    LineSmoothness = lineSmoothness,
                    Stroke = Brushes.DodgerBlue,
                    Fill = Brushes.Transparent,
                    StrokeDashArray = new DoubleCollection {2}
                },
                new LineSeries
                {
                    Title = "Pozostało (bez wydatków stałych)",
                    Values = new ChartValues<double>(burnValuesWoMonthlyExp),
                    PointGeometry = DefaultGeometries.None,
                    LineSmoothness = lineSmoothness,
                    Stroke = Brushes.YellowGreen,
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Pozostało (bez wydatków stałych, plan)",
                    Values = new ChartValues<double>(avgBurnValuesWoMonthlyExp),
                    PointGeometry = DefaultGeometries.None,
                    LineSmoothness = lineSmoothness,
                    Stroke = Brushes.YellowGreen,
                    Fill = Brushes.Transparent,
                    StrokeDashArray = new DoubleCollection {2}
                },
                new ColumnSeries
                {
                    Title = "Dzienny wydatek",
                    Values = new ChartValues<double>(dailyExpenses),
                    DataLabels = true,
                    Fill = Brushes.Gold
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
            chart.LegendLocation = LegendLocation.Top;
            grid.Children.Clear();
            grid.Children.Add(chart);


        }
    }
}
