using System;
using System.Collections.Generic;
using System.Linq;

namespace Anagram
{
    class Program
    {
        private const string inputFile = "WordList.txt";
        //private const string inputFile = "WordListAllEnglishWords.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("*** Tests Started {0} ***\n\n", DateTime.Now);

            var anagram = new Anagram();
            var testsToRun = testDataList;                                  // All Tests
            //var testsToRun = testDataList.Where(x => x.TestNumber == 1);    // Specific Test
            //var testsToRun = testDataList.Where(x => x.ExpectedResult);     // Positive Tests
            //var testsToRun = testDataList.Where(x => !x.ExpectedResult);    // Negative Tests

            foreach (var testData in testsToRun)
            {
                Console.WriteLine("{0}\tSTARTING TEST {1}\n", DateTime.Now, testData.TestNumber);

                anagram.FindSecretPhrase(testData.HintPhrase, testData.MD5HashKey, testData.InputFile);
                anagram.PrintFullStats();

                testData.ReturnedResult = anagram.SecretPhraseFound;
                testData.ReturnedSecretPhrase = anagram.SecretPhrase;
                testData.TotalTime = anagram.TotalTime;
                testData.WordsFiltered = anagram.WordsFiltered;
                testData.NodesAdded = anagram.NumNodes;
            }

            if (!testsToRun.Any(x => !x.TestPassed))
            {
                Console.WriteLine("ALL TESTS PASSED\n");
            }

            var headerRow = new string[] { "Test #", "Result", "Total Time", "Words Filtered", "Nodes Added" };

            TablePrinter.PrintLine();
            TablePrinter.PrintRow(headerRow);
            TablePrinter.PrintLine();

            foreach (var testData in testsToRun)
            {
                TablePrinter.PrintRow(new string[]
                {
                    testData.TestNumber.ToString(),
                    testData.TestPassed ? " Passed " : "*Failed*",
                    testData.TotalTime.ToString(),
                    testData.WordsFiltered.ToString(),
                    testData.NodesAdded.ToString()
                });
            }

            TablePrinter.PrintLine();

            var doubleAverageTicks = testsToRun.Average(timeSpan => timeSpan.TotalTime.Ticks);
            var longAverageTicks = Convert.ToInt64(doubleAverageTicks);
            var averageTime = new TimeSpan(longAverageTicks);

            Console.WriteLine(string.Format("\nAverage Time = {0}", averageTime));

            Console.WriteLine("\n\n*** Tests Ended {0} ***\n", DateTime.Now);
            Console.ReadLine();
        }

        private static List<TestData> testDataList = new List<TestData>()
        {
            new TestData( 1, true,  "poultry outwits ants",    "4624d200580677270a54ccff86b9610e", inputFile, "pastils turnout towy"),
            new TestData( 2, true,  "poultry outwits ants",    "4a9f51db2c7eba0c724499f749d3176a", inputFile, "trustpilot wants you"),
            new TestData( 3, true,  "academicians outwits ac", "6c98e6e707c180420e358075e5d93b81", inputFile, "academicians outwits ac"),
            new TestData( 4, true,  "a",                       "0cc175b9c0f1b6a831c399e269772661", inputFile, "a"),
            new TestData( 5, true,  "a b c",                   "fa5124f324ffee2030948de7d4bb78ad", inputFile, "c b a"),
            new TestData( 6, true,  "a b c d",                 "a74caae2896653023ae0013f88249f91", inputFile, "d c b a"),
            new TestData( 7, true,  "a b c d a b c d",         "430530dae271684171f86612d00935ad", inputFile, "d c b a d c b a"),
            new TestData( 8, true,  "a b c d e f g h",         "a878170edc8cad2b11adef4c872be6be", inputFile, "a b c d e f g h"),
            new TestData( 9, false, "a b c d a b c d",         "430530dae271684171f86612d00935ac", inputFile, null),
            new TestData(10, false, "1 3 2",                   "0cc174b9e0f1baa831c399e269772661", inputFile, null),
            new TestData(11, false, "a b c d e f g h i j",     "fa5124f324ffAA2030948de7d4bb78ad", inputFile, null),
            new TestData(12, false, "poultry outwits ants",    "4624d200580677270a54ccff86b9611e", inputFile, null),
            new TestData(13, true,  "trustpilot",              "f5ad76ee64505e3ae887b634e05f30be", inputFile, "trustpilot"),
        };
    }
}
