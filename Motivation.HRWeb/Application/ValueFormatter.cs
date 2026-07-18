using System.Globalization;

namespace Motivation.HRWeb.Application;

public static class ValueFormatter
{
    private static readonly NumberFormatInfo NumberFormatWithSpace = new CultureInfo("ru-RU").NumberFormat;
    public static string FormatValue(object? value)
    {
        return value switch
        {
            null => "-",
            decimal d => d.ToString("N1", NumberFormatWithSpace),
            double d => d.ToString("N1", NumberFormatWithSpace),
            float f => f.ToString("N1", NumberFormatWithSpace),
            int i => i.ToString("N1", NumberFormatWithSpace),
            long l => l.ToString("N1", NumberFormatWithSpace),
            string s => s,
            _ => value.ToString() ?? "-"
        };
    }
}
