using System;

namespace Anagram
{
    internal sealed class TestData
    {
        public string HintPhrase { get; private set; }
        public string Md5HashKey { get; private set; }
        public string InputFile { get; private set; }
        public TimeSpan? TotalTime { get; set; }

        public TestData(string hintPhrase, string md5HashKey, string inputFile)
        {
            HintPhrase = hintPhrase;
            Md5HashKey = md5HashKey;
            InputFile = inputFile;
        }
    }
}
