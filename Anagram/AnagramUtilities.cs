using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Anagram
{
    public class AnagramUtilities : IDisposable
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
        /// String that will hold the secret phrase if it is found.
        /// </summary>
        public string SecretPhrase { get; private set; }

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
        /// Hash that uses the chars in the hint phrase as a key and the number of times that char appears as the value.
        /// </summary>
        private Dictionary<char, int> CharCountFromHintDictionary { get; set; }

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
        /// Start time of adding nodes to the graph.
        /// </summary>
        public DateTime? AddNodesStartTime { get; set; }

        /// <summary>
        /// End time of adding noded to the graph
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
        /// Graph used to find the secret phrase.
        /// </summary>
        private AnagramGraph anagramGraph { get; set; }

        /// <summary>
        /// Total nodes added to the graph not including the root.
        /// </summary>
        internal int NumNodes { get; set; }

        #endregion Properties

        public AnagramUtilities(string hintPhrase, string md5HashKeyOfSolution, string inputFile)
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
            CharCountFromHintDictionary = GetCharCountFromString(HintPhrase);
            NumWords = CharCountFromHintDictionary[' '] + 1;
            LengthWithoutSpaces = HintPhrase.Length - CharCountFromHintDictionary[' '];
            MD5Hash = MD5.Create();

            MakeDistinctWordList();

            anagramGraph = new AnagramGraph(NumWords, DistinctWordList, this);

            EndTime = DateTime.Now;

            PrintStats();
            CleanUp();
        }

        private void CleanUp()
        {
            Dispose();
            
            if (DistinctWordList != null)
            {
                DistinctWordList.Clear();
                DistinctWordList = null;
            }

            if (CharCountFromHintDictionary != null)
            {
                CharCountFromHintDictionary.Clear();
                CharCountFromHintDictionary = null;
            }
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
            if (word.Length <= LengthWithoutSpaces)
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

        private void MakeDistinctWordList()
        {
            DistinctListStartTime = DateTime.Now;
            DistinctWordList      = new List<string>();

            foreach (var word in File.ReadAllLines(InputFile).ToList())
            {
                var wordLower = word.Trim().ToLower();

                if (!ExcludeWord(wordLower))
                {
                    DistinctWordList.Add(wordLower);
                }
            }

            // Sort the filtered words first by length then alphabetically. This will make the search alot faster later.
            //DistinctWordList = DistinctWordList.OrderByDescending(x => x.Length).ThenBy(x => x).Distinct().ToList();
            DistinctWordList = DistinctWordList.Distinct().ToList();

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

        public void Dispose()
        {
            ((IDisposable)MD5Hash).Dispose();
        }

        public void PrintStats()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(string.Format("Start Time:           {0}", StartTime));
            stringBuilder.AppendLine(string.Format("End Time:             {0}", EndTime));
            stringBuilder.AppendLine(string.Format("Total Time:           {0}", EndTime - StartTime));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(string.Format("Word Filter Time:     {0}", DistinctListEndTime - DistinctListStartTime));
            stringBuilder.AppendLine(string.Format("Words Filted:         {0:n0}", DistinctWordList.Count));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(string.Format("Node Adding Time:     {0}", AddNodesEndTime - AddNodesStartTime));
            stringBuilder.AppendLine(string.Format("Nodes Added:          {0:n0}", NumNodes));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(SecretPhrase == null ? "Secret phrase was not found." : string.Format("Secret Phrase Phrase: {0}", SecretPhrase));

            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
