using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Linq;

namespace BitPoolMiner.Utils
{
    /// <summary>
    /// Fill in any data points that are missing with ZERO to allow proper charting
    /// </summary>
    public class ChartDataBackFill
    {
        /// <summary>
        /// Chart data is currently showing data points at an interval of 5 minutes
        /// </summary>
        const int datePointTimeIntervalMins = 5;

        /// <summary>
        /// Chart data is currently using a 24 hour window for historical data points from the current time backwards
        /// </summary>
        const int datePointTimeWindow = 1440;

        /// <summary>
        /// Public entry point
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ChartValues<DateTimePoint> BackFillList(ChartValues<DateTimePoint> list)
        {
            list.ToList().ForEach(x => x.DateTime = RoundDown(x.DateTime, TimeSpan.FromMinutes(5)));
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
            DateTime tmpInterval = RoundDown(DateTime.UtcNow.ToLocalTime().Subtract(TimeSpan.FromMinutes(datePointTimeWindow - datePointTimeIntervalMins)), TimeSpan.FromMinutes(datePointTimeIntervalMins));
            var upperBound = DateTime.UtcNow.ToLocalTime().Subtract(TimeSpan.FromMinutes(datePointTimeIntervalMins));

            while (tmpInterval <= upperBound)
            {
                if (list.Any(x => RoundDown(x.DateTime, TimeSpan.FromMinutes(datePointTimeIntervalMins)) == tmpInterval) == false)
                {
                    DateTimePoint dateTimePoint = new DateTimePoint
                    {
                        Value = 0,
                        DateTime = tmpInterval
                    };
                    list.Add(dateTimePoint);
                }
                tmpInterval = tmpInterval.Add(TimeSpan.FromMinutes(datePointTimeIntervalMins));
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
