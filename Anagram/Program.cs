using System;
using System.Collections.Generic;
using System.Linq;

namespace Anagram
{
    internal static class Program
    {
#if DEBUG
        private const string Mode = "Debug";
#else
        private const string Mode = "Release";
#endif
        private const string InputFile = "WordList.txt";
        //private const string InputFile = "WordListAllEnglishWords.txt";

        private static readonly List<TestData> TestDataList = new List<TestData>
        {
            new TestData("poultry outwits ants", "e4820b45d2277f3844eac66c903e84be"),
            new TestData("poultry outwits ants", "23170acc097c24edb98fc5488ab033fe"),
            new TestData("poultry outwits ants", "665e5bcb0c20062fe8abaaf4628bb154")
        };

        private static void Main()
        {
            Console.WriteLine($"*** Tests Started {DateTime.Now} - {Mode} Mode ***");
            Console.WriteLine();

            var anagram = new Anagram();

            foreach (var testData in TestDataList)
            {
                anagram.FindSecretPhrase(testData.HintPhrase, testData.Md5HashKey, InputFile);

                var secretPhrase = !string.IsNullOrEmpty(anagram.SecretPhrase) ? anagram.SecretPhrase : "** NOT FOUND **";

                PrintTestResults(secretPhrase, anagram);
                testData.TotalTime = anagram.TotalTime;
            }

            PrintTimeStats();

            Console.WriteLine($"*** Tests Ended {DateTime.Now} - {Mode} Mode ***");
            Console.ReadLine();
        }

        private static void PrintTestResults(string secretPhrase, Anagram anagram)
        {
            Console.WriteLine($"Secret Phrase = {secretPhrase}");
            Console.WriteLine($"Total Time    = {anagram.TotalTime}");
            Console.WriteLine($"Hint Phrase   = {anagram.HintPhrase}");
            Console.WriteLine($"MD5 Hash Key  = {anagram.Md5HashKey}");
            Console.WriteLine($"Words         = {anagram.WordsFiltered:n0}");
            Console.WriteLine($"Nodes         = {anagram.NumNodes:n0}");
            Console.WriteLine($"Comparisons   = {anagram.NumMd5HashKeyComparisons:n0}");
            Console.WriteLine();
        }

        private static void PrintTimeStats()
        {
            Console.WriteLine();
            Console.WriteLine($"Min Time     = {new TimeSpan(Convert.ToInt64(TestDataList.Min(timeSpan => timeSpan.TotalTime?.Ticks)))}");
            Console.WriteLine($"Max Time     = {new TimeSpan(Convert.ToInt64(TestDataList.Max(timeSpan => timeSpan.TotalTime?.Ticks)))}");
            Console.WriteLine($"Total Time   = {new TimeSpan(Convert.ToInt64(TestDataList.Sum(timeSpan => timeSpan.TotalTime?.Ticks)))}");
            Console.WriteLine($"Average Time = {new TimeSpan(Convert.ToInt64(TestDataList.Average(timeSpan => timeSpan.TotalTime?.Ticks)))}");
            Console.WriteLine();
        }
    }
}
