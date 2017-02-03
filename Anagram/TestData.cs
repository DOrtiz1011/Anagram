using System;

namespace Anagram
{
    internal sealed class TestData
    {
        public int TestNumber { get; private set; }
        private bool ExpectedResult { get; }
        public bool ReturnedResult { private get; set; }
        public string HintPhrase { get; private set; }
        public string Md5HashKey { get; private set; }
        public string InputFile { get; private set; }
        private string ExpectedSecretPhrase { get; }
        public string ReturnedSecretPhrase { private get; set; }
        public TimeSpan? TotalTime { get; set; }
        public int WordsFiltered { get; set; }
        public int NodesAdded { get; set; }
        public int Md5Comparisons { get; set; }

        public bool TestPassed => ExpectedResult == ReturnedResult && ExpectedSecretPhrase == ReturnedSecretPhrase;

        public TestData(int testNumber, bool expectedResult, string hintPhrase, string md5HashKey, string inputFile, string expectedSecretPhrase)
        {
            TestNumber = testNumber;
            ExpectedResult = expectedResult;
            HintPhrase = hintPhrase;
            Md5HashKey = md5HashKey;
            InputFile = inputFile;
            ExpectedSecretPhrase = expectedSecretPhrase;
        }
    }
}
