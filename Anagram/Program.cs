﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Anagram
{
    class Program
    {
        const string inputFile = "wordlist.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("*** Tests Started {0} ***\n", DateTime.Now);

            var anagram = new Anagram();

            foreach (var testData in testDataList.Where(x => x.ExpectedResult))
            {
                anagram.FindSecrethPhrase(testData.HintPhrase, testData.MD5HashKey, testData.InputFile);

                if (anagram.SecretPhraseFound == testData.ExpectedResult && anagram.SecretPhrase == testData.ExpectedSecretPhrase)
                {
                    Console.WriteLine("Test {0} returned the expected result", testData.TestNumber);
                }
                else
                {
                    Console.WriteLine("Test {0} DID NOT return the expected result", testData.TestNumber);
                }

                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine("*** Tests Ended {0} ***\n", DateTime.Now);
            Console.ReadLine();
        }

        static List<TestData> testDataList = new List<TestData>()
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
            new TestData(10, false, "poultry outwits ants",    "4624d200580677270a54ccff86b9611e", inputFile, null),
        };
    }
}
