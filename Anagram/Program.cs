using System;
using System.Collections.Generic;
using System.Linq;

namespace Anagram
{
    internal static class Program
    {
        private const string InputFile = "WordList.txt";
        //private const string InputFile = "WordListAllEnglishWords.txt";

        private static void Main()
        {
            Console.WriteLine($"*** Tests Started {DateTime.Now} ***\n\n");

            var anagram = new Anagram();
            var testsToRun = TestDataList; // All Tests

            foreach (var testData in testsToRun)
            {
                Console.WriteLine($"{DateTime.Now}\tSTARTING TEST {testData.TestNumber}\n");

                anagram.FindSecretPhrase(testData.HintPhrase, testData.Md5HashKey, testData.InputFile);
                anagram.PrintFullStats();

                testData.ReturnedResult = anagram.SecretPhraseFound;
                testData.ReturnedSecretPhrase = anagram.SecretPhrase;
                testData.WordsFiltered = anagram.WordsFiltered;
                testData.NodesAdded = anagram.NumNodes;

                if (anagram.TotalTime != null)
                {
                    testData.TotalTime = anagram.TotalTime.Value;
                }
            }

            if (testsToRun.All(x => x.TestPassed))
            {
                Console.WriteLine("ALL TESTS PASSED\n");
            }

            var headerRow = new[] { "Test #", "Result", "Total Time", "Words Filtered", "Nodes Added" };

            TablePrinter.PrintLine();
            TablePrinter.PrintRow(headerRow);
            TablePrinter.PrintLine();

            foreach (var testData in testsToRun)
            {
                TablePrinter.PrintRow(
                    testData.TestNumber.ToString(),
                    testData.TestPassed ? " Passed " : "*Failed*",
                    testData.TotalTime.ToString(),
                    testData.WordsFiltered.ToString(),
                    testData.NodesAdded.ToString());
            }

            TablePrinter.PrintLine();

            var doubleAverageTicks = testsToRun.Average(timeSpan => timeSpan.TotalTime.Ticks);
            var longAverageTicks = Convert.ToInt64(doubleAverageTicks);
            var averageTime = new TimeSpan(longAverageTicks);

            Console.WriteLine($"\nAverage Time = {averageTime}");

            Console.WriteLine($"\n\n*** Tests Ended {DateTime.Now} ***\n");
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
