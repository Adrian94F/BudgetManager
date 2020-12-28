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
    class BillingPeriodsHistoryChartCreator
    {
        SortedSet<BillingPeriod> periods;
        Grid grid;

        public BillingPeriodsHistoryChartCreator(SortedSet<BillingPeriod> p, Grid g)
        {
            periods = p;
            grid = g;
            Plot();
        }

        private void Plot()
        {
            var chart = new CartesianChart();

            // prepare data
            var nOfPeriods = periods.Count - 1; // omit last (current) period

            // incomes, expenses and balances
            var periodIncomes = new double[nOfPeriods];
            var periodExpenses = new double[nOfPeriods];
            var periodBalances = new double[nOfPeriods];
            for (var i = 0; i < nOfPeriods; i++)
            {
                periodIncomes[i] = (double)(periods.ElementAt(i).netIncome + periods.ElementAt(i).additionalIncome);
                periodExpenses[i] = (double)periods.ElementAt(i).GetSumOfExpenses();
                periodBalances[i] = periodIncomes[i] - periodExpenses[i];
            }

            // labels
            var labels = new string[periods.Count];
            for (var i = 0; i < nOfPeriods; i++)
            {
                labels[i] = periods.ElementAt(i).startDate.ToString("dd.MM") + "-" + periods.ElementAt(i).endDate.ToString("dd.MM");
            }

            // X axis
            chart.AxisX.Add(new Axis
            {
                Labels = labels,
                Separator = new LiveCharts.Wpf.Separator // force the separator step to 1, so it always display all labels
                {
                    Step = 1,
                    IsEnabled = false //disable it to make it invisible.
                },
                //LabelsRotation = 60
            });

            // series
            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Suma dochodów",
                    Values = new ChartValues<double>(periodIncomes),
                    Stroke = Brushes.YellowGreen,
                    Fill = Brushes.Transparent,
                    Width = 10,
                    DataLabels = true
                },
                new LineSeries
                {
                    Title = "Suma wydatków",
                    Values = new ChartValues<double>(periodExpenses),
                    Stroke = Brushes.Tomato,
                    Fill = Brushes.Transparent,
                    DataLabels = true
                },
                new ColumnSeries
                {
                    Title = "Bilans",
                    Values = new ChartValues<double>(periodBalances),
                    Fill = Brushes.DodgerBlue,
                    DataLabels = true
                }
            };

            chart.AxisY.Add(new Axis
            {
                LabelFormatter = value => Math.Round(value).ToString(),
                Separator = new LiveCharts.Wpf.Separator(),
            });

            // add to grid
            chart.Margin = new System.Windows.Thickness(0);
            chart.LegendLocation = LegendLocation.Top;
            grid.Children.Clear();
            grid.Children.Add(chart);

        }
    }
}