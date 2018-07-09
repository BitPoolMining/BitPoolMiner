using BitPoolMiner.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BitPoolMiner.Utils.CommandConverter
{
    public class WorkerParameters
    {
        public string WorkerName { get; set; }
        public CoinType CoinType { get; set; }
    }

    public class Converter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CoinType coinType = (CoinType)Enum.Parse(typeof(CoinType), values[1].ToString());
            return new WorkerParameters() { WorkerName = values[0] as string, CoinType = coinType };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
