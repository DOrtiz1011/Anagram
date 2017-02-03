﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Anagram
{
    internal sealed class Anagram : IDisposable
    {
        #region Fields

        /// <summary>
        /// MD5 object instance
        /// </summary>
        private MD5 _md5Hash;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The MD5 hash key that represents the secret phrase. It is used to veryfy that the phrase was found.
        /// </summary>
        private string Md5HashKeyOfSolution { get; set; }

        /// <summary>
        /// Number of time the MD5 has was used in a comparison.
        /// </summary>
        private int NumMd5HashKeyComparisons { get; set; }

        /// <summary>
        /// String that will hold the secret phrase if it is found.
        /// </summary>
        public string SecretPhrase { get; private set; }

        /// <summary>
        /// Bool that will be true if the secret phrase was found
        /// </summary>
        public bool SecretPhraseFound { get; private set; }

        /// <summary>
        /// Anagram of the secret phrase used to filter the list of words.
        /// </summary>
        public string HintPhrase { get; private set; }

        /// <summary>
        /// Number of words in the hint phrase.
        /// </summary>
        public int NumWords { get; private set; }

        /// <summary>
        /// The longest a single word can be in the solution.
        /// </summary>
        private int SingleWordMaxLength { get; set; }

        /// <summary>
        /// Hash that uses the chars in the hint phrase as a key and the number of times that char appears as the value.
        /// </summary>
        private Dictionary<char, int> CharCountFromHint { get; set; }

        /// <summary>
        /// Start time of the entire process.
        /// </summary>
        private DateTime? StartTime { get; set; }

        /// <summary>
        /// End time of the entire process.
        /// </summary>
        private DateTime? EndTime { get; set; }

        /// <summary>
        /// Start time of the creation of the distinct word list.
        /// </summary>
        private DateTime? DistinctListStartTime { get; set; }

        /// <summary>
        /// End time of the creation of the distinct word list.
        /// </summary>
        private DateTime? DistinctListEndTime { get; set; }

        /// <summary>
        /// Start time of adding nodes to the tree.
        /// </summary>
        private DateTime? AddNodesStartTime { get; set; }

        /// <summary>
        /// End time of adding noded to the tree
        /// </summary>
        private DateTime? AddNodesEndTime { get; set; }

        /// <summary>
        /// Full or relative path to the input file. If it is just the file name then it assumes the current working directory.
        /// </summary>
        private string InputFile { get; set; }

        /// <summary>
        /// Distinct list of words based on the criteria set by the hint phrase.
        /// </summary>
        private List<string> DistinctWordList { get; set; }

        public int WordsFiltered => DistinctWordList.Count;

        public Dictionary<string, HashSet<string>> WordHash { get; private set; }

        private int[] _maxPhraseLengths;
        private int[] _minPhraseLengths;

        internal TimeSpan? TotalTime
        {
            get
            {
                if (EndTime != null && StartTime != null)
                {
                    return EndTime.Value - StartTime.Value;
                }

                return null;
            }
        }

        /// <summary>
        /// Total nodes added to the tree
        /// </summary>
        public int NumNodes { get; set; }

        #endregion Properties

        public void FindSecretPhrase(string hintPhrase, string md5HashKeyOfSolution, string inputFile)
        {
            StartTime = DateTime.Now;

            if (string.IsNullOrEmpty(hintPhrase))
            {
                throw new ArgumentNullException(nameof(hintPhrase), "hintPhrase is null or empty.");
            }

            if (string.IsNullOrEmpty(md5HashKeyOfSolution))
            {
                throw new ArgumentNullException(nameof(md5HashKeyOfSolution), "md5HashKeyOfSolution is null or empty.");
            }

            if (string.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentNullException(nameof(inputFile), "inputFile is null or empty.");
            }

            HintPhrase = hintPhrase;
            Md5HashKeyOfSolution = md5HashKeyOfSolution;
            InputFile = inputFile;

            InitializeProperties();
            TreeSearch();
            CleanUp();

            EndTime = DateTime.Now;
        }

        private void TreeSearch()
        {
            AddNodesStartTime = DateTime.Now;

            Tree.AddNodes(this);

            AddNodesEndTime = DateTime.Now;
        }

        private void InitializeProperties()
        {
            ClearCollections();

            CharCountFromHint = GetCharCountFromString(HintPhrase);
            NumWords = (CharCountFromHint.ContainsKey(' ') ? CharCountFromHint[' '] : 0) + 1;
            SingleWordMaxLength = HintPhrase.Length - (NumWords - 1) - (CharCountFromHint.ContainsKey(' ') ? CharCountFromHint[' '] : 0);

            MakeDistinctWordList();
            CreateWordHash();
            GetMaxPhraseLengths();
            GetMinPhraseLengths();

            _md5Hash = MD5.Create();
            SecretPhrase = null;
            SecretPhraseFound = false;
            NumNodes = 0;
            NumMd5HashKeyComparisons = 0;
        }

        private void ClearCollections()
        {
            DistinctWordList = null;
            CharCountFromHint = null;
            WordHash = null;
            _maxPhraseLengths = null;
            _minPhraseLengths = null;
        }

        private void CreateWordHash()
        {
            WordHash = new Dictionary<string, HashSet<string>>();

            foreach (var word in DistinctWordList)
            {
                var key = GetHashKey(word);

                if (WordHash.ContainsKey(key))
                {
                    WordHash[key].Add(word);
                }
                else
                {
                    WordHash.Add(key, new HashSet<string> { word });
                }
            }
        }

        private static string GetHashKey(string word)
        {
            var charArray = word.ToCharArray();
            Array.Sort(charArray);
            return new string(charArray);
        }

        private void GetMaxPhraseLengths()
        {
            _maxPhraseLengths = new int[NumWords + 1];

            for (var i = 1; i < _maxPhraseLengths.Length; i++)
            {
                _maxPhraseLengths[i] = SingleWordMaxLength + (i - 1) * 2;
            }
        }

        private void GetMinPhraseLengths()
        {
            _minPhraseLengths = new int[NumWords + 1];

            for (var i = NumWords; i > 0; i--)
            {
                var minLength = 0;

                if (i == NumWords)
                {
                    minLength = HintPhrase.Length;
                }
                else if (i == NumWords - 1)
                {
                    minLength = HintPhrase.Length - (DistinctWordList.Any() ? DistinctWordList.Max(x => x.Length) + 1 : 0);
                }

                _minPhraseLengths[i] = minLength;
            }
        }

        private void CleanUp() => Dispose();

        public bool IsPhraseValid(string word, int wordNumber)
        {
            var isValid = false;

            if (wordNumber == NumWords)
            {
                isValid = IsPhraseAnagram(word);
            }
            else if (wordNumber > 1)
            {
                // wordNumber != 1 => words have already be individually filtered so no need to do it again
                isValid = IsSubPhraseValid(word);
            }
            else if (wordNumber == 1)
            {
                isValid = true;
            }

            return isValid;
        }

        /// <summary>
        /// Uses the hash table created from the hint phrase to exclude single words.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool IsSubPhraseValid(string word)
        {
            var invalid = true;

            // word must be shorter than or equal to the length of the hint phrase
            if (CheckLength(word))
            {
                if (GetCharCountFromString(word).Any(keyValuePair => !CharCountFromHint.ContainsKey(keyValuePair.Key) || CharCountFromHint[keyValuePair.Key] < keyValuePair.Value))
                {
                    invalid = false;
                }
            }
            else
            {
                // word is too long
                invalid = false;
            }

            return invalid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool CheckLength(string word)
        {
            bool valid;
            var numWordsInString = word.Count(x => x == ' ') + 1;

            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            if (numWordsInString == NumWords)
            {
                valid = word.Length == HintPhrase.Length;
            }
            else if (numWordsInString == 1)
            {
                valid = word.Length <= SingleWordMaxLength;
            }
            else if (numWordsInString == NumWords - 1)
            {
                valid = word.Length >= _minPhraseLengths[numWordsInString] && word.Length <= _maxPhraseLengths[numWordsInString];
            }
            else
            {
                valid = word.Length <= _maxPhraseLengths[numWordsInString];
            }

            return valid;
        }

        /// <summary>
        /// Uses the hash table created from the hint phrase to exclude phrases.
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        private bool IsPhraseAnagram(string phrase)
        {
            var isAnagram = true;

            // phrase must exactly match the length of the hint phrase
            if (phrase.Length == HintPhrase.Length)
            {
                if (GetCharCountFromString(phrase).Any(keyValuePair => !CharCountFromHint.ContainsKey(keyValuePair.Key) || CharCountFromHint[keyValuePair.Key] != keyValuePair.Value))
                {
                    isAnagram = false;
                }
            }
            else
            {
                isAnagram = false;
            }

            return isAnagram;
        }

        /// <summary>
        /// Creates a hast table (Dictionary) from a string using the chars as keys and the number of times they appear as the value.
        /// These hashes are used for filtering words and phrases.
        /// </summary>
        /// <param name="stringToCount"></param>
        /// <returns></returns>
        private static Dictionary<char, int> GetCharCountFromString(string stringToCount)
        {
            var countDictionary = new Dictionary<char, int>();

            foreach (var t in stringToCount)
            {
                if (countDictionary.ContainsKey(t))
                {
                    countDictionary[t]++;
                }
                else
                {
                    countDictionary.Add(t, 1);
                }
            }

            return countDictionary;
        }

        private void MakeDistinctWordList()
        {
            DistinctListStartTime = DateTime.Now;
            DistinctWordList = new List<string>();

            foreach (var wordLower in File.ReadAllLines(InputFile).ToList().Select(word => word.Trim().ToLower()).Where(IsSubPhraseValid))
            {
                DistinctWordList.Add(wordLower);
            }

            DistinctWordList = DistinctWordList.OrderByDescending(x => x.Length).ThenBy(x => x).Distinct().ToList();
            DistinctListEndTime = DateTime.Now;
        }

        /// <summary>
        /// Takes a string and returns its MD5 hash as a string.
        /// Method taken from: https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx
        /// </summary>
        /// <param name="input">String to be converted to a MD5 hash</param>
        /// <returns>MD5 hash key of the input string</returns>
        private string GetMd5Hash(string input)
        {
            var byteArray = _md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var stringBuilder = new StringBuilder();

            foreach (var t in byteArray)
            {
                stringBuilder.Append(t.ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns true if the input string matches the MD5 has of the secret phrase.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public void VerifyMd5Hash(string input)
        {
            NumMd5HashKeyComparisons++;

            if (0 != StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(input), Md5HashKeyOfSolution))
            {
                return;
            }

            SecretPhrase = input;
            SecretPhraseFound = true;
        }

        public void Dispose() => ((IDisposable)_md5Hash).Dispose();

        internal void PrintFullStats()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(" =========================================================");
            stringBuilder.AppendLine($" | Hint Phrase:          {HintPhrase}");
            stringBuilder.AppendLine($" | MD5 Hash Key:         {Md5HashKeyOfSolution}");
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine($" | Total Time:           {TotalTime}");
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine($" | Word Filter Time:     {DistinctListEndTime - DistinctListStartTime}");
            stringBuilder.AppendLine($" | Words Filted:         {WordsFiltered:n0}");
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine($" | Node Adding Time:     {AddNodesEndTime - AddNodesStartTime}");
            stringBuilder.AppendLine($" | Nodes Added:          {NumNodes:n0}");
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine($" | MD5 Comparisons:      {NumMd5HashKeyComparisons:n0}");
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine(SecretPhraseFound ? $" | Secret Phrase:        {SecretPhrase}"
                : " | Secret phrase was not found.");
            stringBuilder.AppendLine(" =========================================================");

            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
