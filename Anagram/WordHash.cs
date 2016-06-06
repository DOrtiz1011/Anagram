using System;
using System.Collections.Generic;

namespace Anagram
{
    public class WordHash
    {
        public WordHash(IEnumerable<string> distinctWordList)
        {
            CreateDistinctWordsDitionary(distinctWordList);
        }

        public Dictionary<string, List<string>> Hash { get; private set; }

        private void CreateDistinctWordsDitionary(IEnumerable<string> distinctWordList)
        {
            if (Hash != null)
            {
                Hash.Clear();
                Hash = null;
            }

            Hash = new Dictionary<string, List<string>>();

            foreach (var word in distinctWordList)
            {
                var key = GetHashKey(word);

                if (Hash.ContainsKey(key))
                {
                    Hash[key].Add(word);
                }
                else
                {
                    Hash.Add(key, new List<string>() { word });
                }
            }
        }

        private string GetHashKey(string word)
        {
            var charArray = word.ToCharArray();
            Array.Sort(charArray);
            return new string(charArray);
        }
    }
}
