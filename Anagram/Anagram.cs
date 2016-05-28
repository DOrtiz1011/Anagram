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
        /// Length of the hint phrase minus the spaces.
        /// </summary>
        public int LengthWithoutSpaces { get; private set; }

        /// <summary>
        /// The longest a single word can be in the solution.
        /// </summary>
        public int SingleWordMaxLength { get; private set; }

        /// <summary>
        /// Hash that uses the chars in the hint phrase as a key and the number of times that char appears as the value.
        /// </summary>
        private Dictionary<char, int> CharCountFromHint { get; set; }

        /// <summary>
        /// Start time of the entire process.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// End time of the entire process.
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Start time of the creation of the distinct word list.
        /// </summary>
        public DateTime? DistinctListStartTime { get; set; }

        /// <summary>
        /// End time of the creation of the distinct word list.
        /// </summary>
        public DateTime? DistinctListEndTime { get; set; }

        /// <summary>
        /// Start time of adding nodes to the tree.
        /// </summary>
        public DateTime? AddNodesStartTime { get; set; }

        /// <summary>
        /// End time of adding noded to the tree
        /// </summary>
        public DateTime? AddNodesEndTime { get; set; }

        /// <summary>
        /// Full or relative path to the input file. If it is just the file name then it assumes the current working directory.
        /// </summary>
        public string InputFile { get; private set; }

        /// <summary>
        /// Distinct list of words based on the criteria set by the hint phrase.
        /// </summary>
        private List<string> DistinctWordList { get; set; }

        /// <summary>
        /// Tree used to find the secret phrase.
        /// </summary>
        //private Tree SearchTree { get; set; }

        /// <summary>
        /// Total nodes added to the tree
        /// </summary>
        internal int NumNodes { get; set; }

        #endregion Properties

        public bool FindSecrethPhrase(string hintPhrase, string md5HashKeyOfSolution, string inputFile)
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

            // initialize
            HintPhrase = hintPhrase;
            MD5HashKeyOfSolution = md5HashKeyOfSolution;
            InputFile = inputFile;

            Initialize();

            new Tree(NumWords, DistinctWordList, this);

            EndTime = DateTime.Now;

            PrintStats();
            CleanUp();

            return SecretPhraseFound;
        }

        private void Initialize()
        {
            CharCountFromHint = GetCharCountFromString(HintPhrase);
            NumWords = (CharCountFromHint.ContainsKey(' ') ? CharCountFromHint[' '] : 0) + 1;
            LengthWithoutSpaces = HintPhrase.Length - (CharCountFromHint.ContainsKey(' ') ? CharCountFromHint[' '] : 0);
            SingleWordMaxLength = HintPhrase.Length - (NumWords - 1) - (CharCountFromHint.ContainsKey(' ') ? CharCountFromHint[' '] : 0);
            MD5Hash = MD5.Create();
            SecretPhrase = null;
            SecretPhraseFound = false;

            MakeDistinctWordList();
        }

        private void CleanUp()
        {
            Dispose();
            
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
        }

        public bool ExcludeByNumWords(string word, int wordNumber)
        {
            var exclude = false;

            if (wordNumber > NumWords)
            {
                exclude = true;
            }
            else if (wordNumber == NumWords)
            {
                exclude = IsAnagram(word);

                if (!exclude)
                {
                    VerifyMd5Hash(word);
                }
            }
            else if (wordNumber != 1 && wordNumber < NumWords)
            {
                // wordNumber != 1 => words have already be individually filtered so no need to do it again
                exclude = IsWordValid(word);
            }

            return exclude;
        }

        /// <summary>
        /// Uses the hash table created from the hint phrase to exclude single words.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool IsWordValid(string word)
        {
            var isWordValid = false;

            // word must be shorter than or equal to the length of the hint phrase
            if (CheckLength(word))
            {
                var wordCharCountDictionary = GetCharCountFromString(word);

                foreach (var keyValuePair in wordCharCountDictionary)
                {
                    if (!CharCountFromHint.ContainsKey(keyValuePair.Key) || CharCountFromHint[keyValuePair.Key] < keyValuePair.Value)
                    {
                        // if the hash table does have the char or the number of times the char appears is to large the word will be excluded.
                        isWordValid = true;
                        break;
                    }
                }
            }
            else
            {
                // word is too long
                isWordValid = true;
            }

            return isWordValid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool CheckLength(string word)
        {
            var valid            = false;
            var numWordsInString = word.Count(x => x == ' ') + 1;

            if (numWordsInString == NumWords)
            {
                valid = word.Length == HintPhrase.Length;
            }
            else if (numWordsInString == 1)
            {
                valid = word.Length <= SingleWordMaxLength;
            }
            else
            {
                valid = word.Length <= GetMaxLength(numWordsInString);
            }

            return valid;
        }

        public int GetMaxLength(int numWordsInString)
        {
            return SingleWordMaxLength + ((numWordsInString - 1) * 2);
        }

        public bool CheckPhraseLength(Node node, int nextWordLength)
        {
            return node.GetParentPhrase().Length + nextWordLength <= GetMaxLength(node.WordNumber + 1);
        }

        /// <summary>
        /// Uses the hash table created from the hint phrase to exclude phrases.
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public bool IsAnagram(string phrase)
        {
            var isAnagram = false;

            // phrase must exactly match the length of the hint phrase
            if (CheckLength(phrase))
            {
                var wordCharCount = GetCharCountFromString(phrase);

                foreach (var keyValuePair in wordCharCount)
                {
                    if (!CharCountFromHint.ContainsKey(keyValuePair.Key) || CharCountFromHint[keyValuePair.Key] != keyValuePair.Value)
                    {
                        // if the hash table does have the char or the number of times the char appears is to large the word will be excluded.
                        isAnagram = true;
                        break;
                    }
                }
            }
            else
            {
                isAnagram = true;
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
            DistinctWordList      = new List<string>();

            foreach (var word in File.ReadAllLines(InputFile).ToList())
            {
                var wordLower = word.Trim().ToLower();

                if (!IsWordValid(wordLower))
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
        /// <returns></returns>
        private void VerifyMd5Hash(string input)
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

        public void PrintStats()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(" =========================================================");
            stringBuilder.AppendLine(string.Format(" | Hint Phrase:          {0}", HintPhrase));
            stringBuilder.AppendLine(string.Format(" | MD5 Hash Key:         {0}", MD5HashKeyOfSolution));
            stringBuilder.AppendLine(" |");
            stringBuilder.AppendLine(string.Format(" | Total Time:           {0}", EndTime - StartTime));
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
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
