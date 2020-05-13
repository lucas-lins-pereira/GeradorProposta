using ExcelDataReader;
using ExcelNumberFormat;
using System;
using System.Globalization;

namespace ProposalGenerator.Extensions
{
    public static class Extensions
    {
        public static string GetFormattedValue(this IExcelDataReader reader, int columnIndex, CultureInfo culture = null)
        {
            var value = reader.GetValue(columnIndex);
            var formatString = reader.GetNumberFormatString(columnIndex);
            if (formatString != null)
            {
                var format = new NumberFormat(formatString);
                return format.Format(value, culture ?? CultureInfo.InvariantCulture);
            }
            return Convert.ToString(value, culture ?? CultureInfo.InvariantCulture);
        }
    }
}
