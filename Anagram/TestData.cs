using System;

namespace Anagram
{
    class TestData
    {
        public int TestNumber { get; private set; }
        public bool ExpectedResult { get; private set; }
        public bool ReturnedResult { get; set; }
        public string HintPhrase { get; private set; }
        public string MD5HashKey { get; private set; }
        public string InputFile { get; private set; }
        public string ExpectedSecretPhrase { get; private set; }
        public string ReturnedSecretPhrase { get; set; }
        public TimeSpan TotalTime { get; set; }
        public int WordsFiltered { get; set; }
        public int NodesAdded { get; set; }

        public bool TestPassed
        {
            get
            {
                return ExpectedResult == ReturnedResult && ExpectedSecretPhrase == ReturnedSecretPhrase;
            }
        }

        public TestData(int testNumber, bool expectedResult, string hintPhrase, string md5HashKey, string inputFile, string expectedSecretPhrase)
        {
            TestNumber = testNumber;
            ExpectedResult = expectedResult;
            HintPhrase = hintPhrase;
            MD5HashKey = md5HashKey;
            InputFile = inputFile;
            ExpectedSecretPhrase = expectedSecretPhrase;
        }
    }
}
