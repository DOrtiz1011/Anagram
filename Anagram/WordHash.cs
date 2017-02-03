using System;
using System.Collections.Generic;

namespace Anagram
{
    internal sealed class WordHash
    {
        public readonly Dictionary<int, Dictionary<string, List<string>>> MainHash = new Dictionary<int, Dictionary<string, List<string>>>();

        public void AddWord(string newWord)
        {
            if (!string.IsNullOrEmpty(newWord))
            {
                var newWordLength = newWord.Length;

                if (MainHash.ContainsKey(newWordLength))
                {
                    AddWordToSubHash(newWord, MainHash[newWordLength]);
                }
                else
                {
                    // add new length to the hash
                    var subHash = new Dictionary<string, List<string>>();

                    AddWordToSubHash(newWord, subHash);
                    MainHash.Add(newWordLength, subHash);
                }
            }
        }

        private static void AddWordToSubHash(string newWord, Dictionary<string, List<string>> subHash)
        {
            if (subHash == null)
            {
                throw new ArgumentNullException(nameof(subHash), "Sub hash is null.");
            }

            var newWordHashKey = GetHashKey(newWord);

            if (subHash.ContainsKey(newWordHashKey))
            {
                var list = subHash[newWordHashKey];

                if (!list.Contains(newWord))
                {
                    list.Add(newWord);
                }
            }
            else
            {
                subHash.Add(newWordHashKey, new List<string> { newWord });
            }
        }

        private static string GetHashKey(string word)
        {
            var charArray = word.ToCharArray();
            Array.Sort(charArray);
            return new string(charArray);
        }
    }
}
