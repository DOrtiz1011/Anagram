﻿using System;
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

        private static void Main()
        {
            Console.WriteLine($"*** Tests Started {DateTime.Now} - {Mode} Mode ***");
            Console.WriteLine();

            var anagram = new Anagram();

            foreach (var testData in TestDataList)
            {
                Console.WriteLine($"{DateTime.Now}\tSTARTING TEST {testData.TestNumber}/{TestDataList.Count}");
                Console.WriteLine();

                anagram.FindSecretPhrase(testData.HintPhrase, testData.Md5HashKey, testData.InputFile);
                anagram.PrintFullStats();

                testData.ReturnedResult = anagram.SecretPhraseFound;
                testData.ReturnedSecretPhrase = anagram.SecretPhrase;
                testData.WordsFiltered = anagram.WordsFiltered;
                testData.NodesAdded = anagram.NumNodes;
                testData.TotalTime = anagram.TotalTime;
                testData.Md5Comparisons = anagram.NumMd5HashKeyComparisons;
            }

            if (TestDataList.All(x => x.TestPassed))
            {
                Console.WriteLine("ALL TESTS PASSED");
                Console.WriteLine();
            }

            TablePrinter.PrintLine();
            TablePrinter.PrintRow("Test #", "Result", "Total Time", "Words Filtered", "Nodes Added", "MD5 Comparisons");
            TablePrinter.PrintLine();

            foreach (var testData in TestDataList)
            {
                TablePrinter.PrintRow(
                    testData.TestNumber.ToString(),
                    testData.TestPassed ? " Passed " : "*Failed*",
                    testData.TotalTime.ToString(),
                    testData.WordsFiltered.ToString("n0"),
                    testData.NodesAdded.ToString("n0"),
                    testData.Md5Comparisons.ToString("n0"));
            }

            TablePrinter.PrintLine();

            Console.WriteLine();
            Console.WriteLine($"Min Time     = {new TimeSpan(Convert.ToInt64(TestDataList.Min(timeSpan => timeSpan.TotalTime?.Ticks)))}");
            Console.WriteLine($"Max Time     = {new TimeSpan(Convert.ToInt64(TestDataList.Max(timeSpan => timeSpan.TotalTime?.Ticks)))}");
            Console.WriteLine($"Total Time   = {new TimeSpan(Convert.ToInt64(TestDataList.Sum(timeSpan => timeSpan.TotalTime?.Ticks)))}");
            Console.WriteLine($"Average Time = {new TimeSpan(Convert.ToInt64(TestDataList.Average(timeSpan => timeSpan.TotalTime?.Ticks)))}");
            Console.WriteLine();
            Console.WriteLine($"*** Tests Ended {DateTime.Now} - {Mode} Mode ***");
            Console.ReadLine();
        }

        private static readonly List<TestData> TestDataList = new List<TestData>
        {
            new TestData(1, true,  "poultry outwits ants", "e4820b45d2277f3844eac66c903e84be", InputFile, "printout stout yawls"),
            new TestData(2, true,  "poultry outwits ants", "23170acc097c24edb98fc5488ab033fe", InputFile, "ty outlaws printouts"),
            new TestData(3, false, "poultry outwits ants", "665e5bcb0c20062fe8abaaf4628bb154", InputFile, default(string))
        };
    }
}
