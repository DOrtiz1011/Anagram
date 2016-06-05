namespace Anagram
{
    struct TestData
    {
        public int TestNumber { get; private set; }
        public bool ExpectedResult { get; private set; }
        public string HintPhrase { get; private set; }
        public string MD5HashKey { get; private set; }
        public string InputFile { get; private set; }
        public string ExpectedSecretPhrase { get; private set; }

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
