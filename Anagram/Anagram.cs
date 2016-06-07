using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Anagram
{
    public class Anagram : IDisposable
    {
        #region Fields

        /// <summary>
        /// MD5 object instance
        /// </summary>
        private MD5 MD5Hash;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The MD5 hash key that represents the secret phrase. It is used to veryfy that the phrase was found.
        /// </summary>
        public string MD5HashKeyOfSolution { get; private set; }

        /// <summary>
        /// Number of time the MD5 has was used in a comparison.
        /// </summary>
        public int NumMD5HashKeyComparisons { get; private set; }

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
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// End time of the entire process.
        /// </summary>
        public DateTime? EndTime { get; private set; }

        /// <summary>
        /// Start time of the creation of the distinct word list.
        /// </summary>
        public DateTime? DistinctListStartTime { get; private set; }

        /// <summary>
        /// End time of the creation of the distinct word list.
        /// </summary>
        public DateTime? DistinctListEndTime { get; private set; }

        /// <summary>
        /// Start time of adding nodes to the tree.
        /// </summary>
        public DateTime? AddNodesStartTime { get; private set; }

        /// <summary>
        /// End time of adding noded to the tree
        /// </summary>
        public DateTime? AddNodesEndTime { get; private set; }

        /// <summary>
        /// Full or relative path to the input file. If it is just the file name then it assumes the current working directory.
        /// </summary>
        public string InputFile { get; private set; }

        /// <summary>
        /// Distinct list of words based on the criteria set by the hint phrase.
        /// </summary>
        private List<string> DistinctWordList { get; set; }

        public Dictionary<string, List<string>> WordHash { get; private set; }

        private int[] MaxPhraseLengths;
        private int[] MinPhraseLengths;

        public TimeSpan TotalTime { get { return EndTime.Value - StartTime.Value; } }

        /// <summary>
        /// Total nodes added to the tree
        /// </summary>
        public int NumNodes { get; set; }

        #endregion Properties

        public bool FindSecretPhrase(string hintPhrase, string md5HashKeyOfSolution, string inputFile)
        {
            StartTime = DateTime.Now;

            if (string.IsNullOrEmpty(hintPhrase))
            {
                throw new ArgumentNullException("hintPhrase", "hintPhrase is null or empty.");
            }

            if (string.IsNullOrEmpty(md5HashKeyOfSolution))
            {
                throw new ArgumentNullException("md5HashKeyOfSolution", "md5HashKeyOfSolution is null or empty.");
            }

            if (string.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentNullException("inputFile", "inputFile is null or empty.");
            }

            HintPhrase = hintPhrase;
            MD5HashKeyOfSolution = md5HashKeyOfSolution;
            InputFile = inputFile;

            InitializeProperties();
            TreeSearch();
            CleanUp();

            EndTime = DateTime.Now;

            return SecretPhraseFound;
        }

        private void TreeSearch()
        {
            AddNodesStartTime = DateTime.Now;

            new Tree().TreeSearch(this);

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

            MD5Hash = MD5.Create();
            SecretPhrase = null;
            SecretPhraseFound = false;
            NumNodes = 0;
            NumMD5HashKeyComparisons = 0;
        }

        private void ClearCollections()
        {
            if (DistinctWordList != null)
            {
                DistinctWordList.Clear();
                DistinctWordList = null;
            }

            if (CharCountFromHint != null)
            {
                CharCountFromHint.Clear();
                CharCountFromHint = null;
            }

            if (WordHash != null)
            {
                WordHash.Clear();
                WordHash = null;
            }

            if (MaxPhraseLengths != null)
            {
                MaxPhraseLengths = null;
            }

            if (MinPhraseLengths != null)
            {
                MinPhraseLengths = null;
            }
        }

        private void CreateWordHash()
        {
            WordHash = new Dictionary<string, List<string>>();

            foreach (var word in DistinctWordList)
            {
                var key = GetHashKey(word);

                if (WordHash.ContainsKey(key))
                {
                    WordHash[key].Add(word);
                }
                else
                {
                    WordHash.Add(key, new List<string>() { word });
                }
            }
        }

        private string GetHashKey(string word)
        {
            var charArray = word.ToCharArray();
            Array.Sort(charArray);
            return new string(charArray);
        }

        private void GetMaxPhraseLengths()
        {
            MaxPhraseLengths = new int[NumWords + 1];

            for (var i = 1; i < MaxPhraseLengths.Length; i++)
            {
                MaxPhraseLengths[i] = SingleWordMaxLength + ((i - 1) * 2);
            }
        }

        private void GetMinPhraseLengths()
        {
            MinPhraseLengths = new int[NumWords + 1];

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

                MinPhraseLengths[i] = minLength;
            }
        }

        private void CleanUp()
        {
            Dispose();
        }

        public bool IsPhraseValid(string word, int wordNumber)
        {
            var isValid = false;

            if (wordNumber == NumWords)
            {
                isValid = IsPhraseAnagram(word);
            }
            else
            {
                // wordNumber != 1 => words have already be individually filtered so no need to do it again
                isValid = IsSubPhraseValid(word);
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
                var wordCharCountDictionary = GetCharCountFromString(word);

                foreach (var keyValuePair in wordCharCountDictionary)
                {
                    if (!CharCountFromHint.ContainsKey(keyValuePair.Key) || CharCountFromHint[keyValuePair.Key] < keyValuePair.Value)
                    {
                        // if the hash table does have the char or the number of times the char appears is to large the word will be excluded.
                        invalid = false;
                        break;
                    }
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
            var valid = false;
            var numWordsInString = word.Count(x => x == ' ') + 1;

            if (!string.IsNullOrEmpty(word))
            {
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
                    valid = word.Length >= MinPhraseLengths[numWordsInString] && word.Length <= MaxPhraseLengths[numWordsInString];
                }
                else
                {
                    valid = word.Length <= MaxPhraseLengths[numWordsInString];
                }
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
                var wordCharCount = GetCharCountFromString(phrase);

                foreach (var keyValuePair in wordCharCount)
                {
                    if (!CharCountFromHint.ContainsKey(keyValuePair.Key) || CharCountFromHint[keyValuePair.Key] != keyValuePair.Value)
                    {
                        // if the hash table does have the char or the number of times the char appears is to large the word will be excluded.
                        isAnagram = false;
                        break;
                    }
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
        private Dictionary<char, int> GetCharCountFromString(string stringToCount)
        {
            var countDictionary = new Dictionary<char, int>();

            foreach (var character in stringToCount.Trim().ToCharArray())
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

        private void MakeDistinctWordList()
        {
            DistinctListStartTime = DateTime.Now;
            DistinctWordList = new List<string>();

            foreach (var word in File.ReadAllLines(InputFile).ToList())
            {
                var wordLower = word.Trim().ToLower();

                if (IsSubPhraseValid(wordLower))
                {
                    DistinctWordList.Add(wordLower);
                }
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
            var byteArray = MD5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < byteArray.Length; i++)
            {
                stringBuilder.Append(byteArray[i].ToString("x2"));
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
            NumMD5HashKeyComparisons++;

            if (0 == StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(input), MD5HashKeyOfSolution))
            {
                SecretPhrase = input;
                SecretPhraseFound = true;
            }
        }

        public void Dispose()
        {
            ((IDisposable)MD5Hash).Dispose();
        }

        public void PrintFullStats()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(" =========================================================");
            stringBuilder.AppendLine(string.Format(" | Hint Phrase:          {0}", HintPhrase));
            stringBuilder.AppendLine(string.Format(" | MD5 Hash Key:         {0}", MD5HashKeyOfSolution));
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine(string.Format(" | Total Time:           {0}", TotalTime));
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine(string.Format(" | Word Filter Time:     {0}", DistinctListEndTime - DistinctListStartTime));
            stringBuilder.AppendLine(string.Format(" | Words Filted:         {0:n0}", DistinctWordList.Count));
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine(string.Format(" | Node Adding Time:     {0}", AddNodesEndTime - AddNodesStartTime));
            stringBuilder.AppendLine(string.Format(" | Nodes Added:          {0:n0}", NumNodes));
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine(string.Format(" | MD5 Comparisons:      {0:n0}", NumMD5HashKeyComparisons));
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine(SecretPhraseFound ? string.Format(" | Secret Phrase:        {0}", SecretPhrase) : " | Secret phrase was not found.");
            stringBuilder.AppendLine(" =========================================================");

            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
