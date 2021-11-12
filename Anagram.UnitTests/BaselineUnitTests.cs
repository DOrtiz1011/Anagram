using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Anagram.UnitTests
{
    /// <summary>
    /// This class tests the output using the original word list, hash keys, and hint phrase.
    /// </summary>
    [TestClass]
    public class BaselineUnitTests
    {
        private const string InputFile    = "WordList.txt";
        private readonly Anagram _Anagram = new();

        [TestMethod]
        public void TestMethod1()
        {
            _Anagram.FindSecretPhrase("poultry outwits ants", "e4820b45d2277f3844eac66c903e84be", InputFile);

            _Anagram.SecretPhrase.ShouldBe("printout stout yawls");

            AssertValues(38100, 1527, 7);
        }

        [TestMethod]
        public void TestMethod2()
        {
            _Anagram.FindSecretPhrase("poultry outwits ants", "23170acc097c24edb98fc5488ab033fe", InputFile);

            _Anagram.SecretPhrase.ShouldBe("ty outlaws printouts");

            AssertValues(264761, 3487, 30);
        }

        [TestMethod]
        public void TestMethod3()
        {
            _Anagram.FindSecretPhrase("poultry outwits ants", "665e5bcb0c20062fe8abaaf4628bb154", InputFile);

            _Anagram.SecretPhrase.ShouldBeNull("This is a negative test that should not find a phrase.");

            AssertValues(591809, 4560, 40);
        }

        private void AssertValues(int numNodes, int numComparisons, int secondsToBeat)
        {
            _Anagram.WordsFiltered.ShouldBe(1659);
            _Anagram.NumNodes.ShouldBe(numNodes);
            _Anagram.NumMd5HashKeyComparisons.ShouldBe(numComparisons);
            _Anagram.TotalTime.ShouldNotBeNull();
            _Anagram.TotalTime.Value.ShouldBeLessThan(new TimeSpan(0, 0, secondsToBeat), $"Search took more than {secondsToBeat} seconds. Actual time was {_Anagram.TotalTime}. Be sure to run tests in release mode.");
        }
    }
}
