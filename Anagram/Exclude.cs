using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Anagram
{
    internal class Exclude
    {

        /// <summary>
        /// MD5 object instance
        /// </summary>
        private MD5 MD5Hash;

        /// <summary>
        /// The MD5 hash key that represents the secret phrase. It is used to veryfy that the phrase was found.
        /// </summary>
        public string MD5HashKeyOfSolution { get; private set; }

        public string SecretPhrase { get; private set; }
        public string HintPhrase { get; private set; }
        public int NumWords { get; private set; }
        private Dictionary<char, int> CharCountFromHintDictionary { get; set; }

        public Exclude(string hintPhrase, string md5HashKeyOfSolution)
        {
            HintPhrase = hintPhrase;
            CharCountFromHintDictionary = GetCharCountFromString(HintPhrase);
            NumWords = CharCountFromHintDictionary[' '] + 1;
            MD5HashKeyOfSolution = md5HashKeyOfSolution;
            MD5Hash = MD5.Create();
        }

        public bool ExcludeByNumWords(string word, int wordNumber, out bool foundSecretPhrase)
        {
            var exclude = false;

            foundSecretPhrase = false;

            if (wordNumber == NumWords)
            {
                exclude = ExcludePhrase(word);

                if (!exclude)
                {
                    foundSecretPhrase = VerifyMd5Hash(word);
                }
            }
            else if (wordNumber != 1 && wordNumber < NumWords)
            {
                exclude = ExcludeWord(word);
            }

            return exclude;
        }

        /// <summary>
        /// Uses the hash table created from the hint phrase to exclude single words.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool ExcludeWord(string word)
        {
            var excludeWord = false;

            // word must be shorter than or equal to the length of the hint phrase
            if (word.Length <= HintPhrase.Length)
            {
                var wordCharCountDictionary = GetCharCountFromString(word);

                foreach (var keyValuePair in wordCharCountDictionary)
                {
                    if (!CharCountFromHintDictionary.ContainsKey(keyValuePair.Key) || CharCountFromHintDictionary[keyValuePair.Key] < keyValuePair.Value)
                    {
                        // if the hash table does have the char or the number of times the char appears is to large the word will be excluded.
                        excludeWord = true;
                        break;
                    }
                }
            }
            else
            {
                // word is too long
                excludeWord = true;
            }

            return excludeWord;
        }

        /// <summary>
        /// Uses the hash table created from the hint phrase to exclude phrases.
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public bool ExcludePhrase(string phrase)
        {
            var exclude = false;

            // phrase must exactly match the length of the hint phrase
            if (phrase.Length == HintPhrase.Length)
            {
                var wordCharCountDictionary = GetCharCountFromString(phrase);

                foreach (var keyValuePair in wordCharCountDictionary)
                {
                    if (!CharCountFromHintDictionary.ContainsKey(keyValuePair.Key) || CharCountFromHintDictionary[keyValuePair.Key] != keyValuePair.Value)
                    {
                        // if the hash table does have the char or the number of times the char appears is to large the word will be excluded.
                        exclude = true;
                        break;
                    }
                }
            }
            else
            {
                exclude = true;
            }

            return exclude;
        }

        /// <summary>
        /// Creates a hast table (Dictionary) from a string using the chars as keys and the number of times they appear as the value.
        /// These hashes are used for filtering words and phrases.
        /// </summary>
        /// <param name="stringToCount"></param>
        /// <returns></returns>
        private Dictionary<char, int> GetCharCountFromString(string stringToCount)
        {
            var countDictionary = new Dictionary<char, int>();
            var charList = stringToCount.Trim().ToCharArray();

            foreach (var character in charList)
            {
                if (countDictionary.ContainsKey(character))
                {
                    countDictionary[character]++;
                }
                else
                {
                    countDictionary.Add(character, 1);
                }
            }

            return countDictionary;
        }

        /// <summary>
        /// Takes a string and returns its MD5 hash as a string.
        /// Method taken from: https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx
        /// </summary>
        /// <param name="input">String to be converted to a MD5 hash</param>
        /// <returns>MD5 hash key of the input string</returns>
        private string GetMd5Hash(string input)
        {
            var byteArray = MD5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < byteArray.Length; i++)
            {
                stringBuilder.Append(byteArray[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns true if the input string matches the MD5 has of the secret phrase.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private bool VerifyMd5Hash(string input)
        {
            var found = false;

            if (0 == StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(input), MD5HashKeyOfSolution))
            {
                SecretPhrase = input;
                found = true;
            }

            return found;
        }
    }
}
