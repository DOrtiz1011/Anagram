using System;
using System.Linq;

namespace Anagram
{
    internal static class TablePrinter
    {
        private const int TableWidth = 120;

        public static void PrintLine() => Console.WriteLine(new string('-', TableWidth));

        public static void PrintRow(params string[] columns) => Console.WriteLine(columns.Aggregate("|", (current, column) => current + AlignCentre(column, (TableWidth - columns.Length) / columns.Length) + "|"));

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            return string.IsNullOrEmpty(text) ? new string(' ', width) : text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }
    }
}
