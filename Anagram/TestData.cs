using System;

namespace Anagram
{
    internal sealed class TestData
    {
        public string HintPhrase { get; }
        public string Md5HashKey { get; }
        public TimeSpan? TotalTime { get; set; }

        public TestData(string hintPhrase, string md5HashKey)
        {
            HintPhrase = hintPhrase;
            Md5HashKey = md5HashKey;
        }
    }
}
