using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Linq;

namespace BitPoolMiner.Utils
{
    /// <summary>
    /// Fill in any data points that are missing with ZERO to allow proper charting
    /// </summary>
    public class PaymentChartDataBackFill
    {
        /// <summary>
        /// Chart data is currently showing data points at an interval of 1 day
        /// </summary>
        const int datePointTimeIntervalDays = 1;

        /// <summary>
        /// Chart data is currently using a 30 day window for historical data points from the current time backwards
        /// </summary>
        const int datePointTimeWindow = 30;

        /// <summary>
        /// Public entry point
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ChartValues<DateTimePoint> BackFillList(ChartValues<DateTimePoint> list)
        {
            list.ToList().ForEach(x => x.DateTime = RoundDown(x.DateTime, TimeSpan.FromMinutes(datePointTimeIntervalDays)));
            ChartValues<DateTimePoint> completeList = GetListAllDates(list);
            return SortList(completeList);
        }

        /// <summary>
        /// Sort list based on date
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private ChartValues<DateTimePoint> SortList(ChartValues<DateTimePoint> list)
        {
            var sortedList = list.OrderBy(x => x.DateTime).ToList();
            ChartValues<DateTimePoint> completeListSorted = new ChartValues<DateTimePoint>();

            foreach (DateTimePoint dateTimePoint in sortedList)
            {
                completeListSorted.Add(dateTimePoint);
            }

            return completeListSorted;
        }

        /// <summary>
        /// Identify and missing data points and add a new ZERO datapoint using the appropriate datetime
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private ChartValues<DateTimePoint> GetListAllDates(ChartValues<DateTimePoint> list)
        {
            DateTime tmpInterval = RoundDown(DateTime.UtcNow.ToLocalTime().Subtract(TimeSpan.FromDays(datePointTimeWindow - datePointTimeIntervalDays)), TimeSpan.FromDays(datePointTimeIntervalDays));
            var upperBound = DateTime.UtcNow.ToLocalTime();

            while (tmpInterval <= upperBound)
            {
                if (list.Any(x => RoundDown(x.DateTime, TimeSpan.FromDays(datePointTimeIntervalDays)) == tmpInterval) == false)
                {
                    DateTimePoint dateTimePoint = new DateTimePoint
                    {
                        Value = 0,
                        DateTime = tmpInterval
                    };
                    list.Add(dateTimePoint);
                }
                tmpInterval = tmpInterval.Add(TimeSpan.FromDays(datePointTimeIntervalDays));
            };

            return list;
        }

        private DateTime RoundDown(DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }
    }
}
