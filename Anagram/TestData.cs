using System;

namespace Anagram
{
    internal sealed class TestData
    {
        public string HintPhrase { get; private set; }
        public string Md5HashKey { get; private set; }
        public TimeSpan? TotalTime { get; set; }

        public TestData(string hintPhrase, string md5HashKey)
        {
            HintPhrase = hintPhrase;
            Md5HashKey = md5HashKey;
        }
    }
}
