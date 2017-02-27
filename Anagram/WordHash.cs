using System;
using System.Collections.Generic;

namespace Anagram
{
    internal sealed class WordHash
    {
        public readonly Dictionary<int, Dictionary<string, HashSet<string>>> MainHash = new Dictionary<int, Dictionary<string, HashSet<string>>>();

        public void AddWord(string newWord)
        {
            if (!string.IsNullOrEmpty(newWord))
            {
                if (MainHash.ContainsKey(newWord.Length))
                {
                    AddWordToSubHash(newWord, MainHash[newWord.Length]);
                }
                else
                {
                    var subHash = new Dictionary<string, HashSet<string>>();

                    AddWordToSubHash(newWord, subHash);
                    MainHash.Add(newWord.Length, subHash);
                }
            }
        }

        private static void AddWordToSubHash(string newWord, Dictionary<string, HashSet<string>> subHash)
        {
            var newWordHashKey = GetHashKey(newWord);

            if (subHash.ContainsKey(newWordHashKey))
            {
                subHash[newWordHashKey].Add(newWord);
            }
            else
            {
                subHash.Add(newWordHashKey, new HashSet<string> { newWord });
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
