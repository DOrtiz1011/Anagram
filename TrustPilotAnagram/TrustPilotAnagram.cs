using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TrustPilotAnagram
{
    /// <summary>
    /// Daniel Ortiz
    /// DOrtiz1011@gmail.com
    /// 08-Jan-2016
    /// 
    /// Anagram programming challege for TrustPilot
    /// http://followthewhiterabbit.trustpilot.com/cs/step3.html
    /// </summary>
    public class TrustPilotAnagram
    {
        #region Members

        /// <summary>
        /// MD5 object instance
        /// </summary>
        private MD5 MD5Hash;

        #endregion

        #region Properties

        /// <summary>
        /// List of all the words from the input file that meet all the restrictions based on the hint phrase
        /// </summary>
        private List<string> FilteredWordList { get; set; }

        /// <summary>
        /// Anagram of the secret phrase used to filter the list of words.
        /// </summary>
        public string HintPhrase { get; private set; }

        /// <summary>
        /// The MD5 hash key that represents the secret phrase. It is used to veryfy that the phrase was found.
        /// </summary>
        public string MD5HashKeyOfSolution { get; private set; }

        /// <summary>
        /// Full or relative path to the input file. If it is just the file name then it assumes the current working directory.
        /// </summary>
        public string InputFile { get; private set; }

        /// <summary>
        /// Start time of the search and filtering.
        /// </summary>
        public DateTime? StartDateTime { get; private set; }

        /// <summary>
        /// Time when the search finishes.
        /// </summary>
        public DateTime? EndDateTime { get; private set; }

        /// <summary>
        /// Number of times a the MD5 hash key of the secret phrase was used in a comparison.
        /// </summary>
        public int NumberOfPhraseComparisons { get; private set; }

        /// <summary>
        /// Duration of the search.
        /// </summary>
        public TimeSpan? SearchTimeSpan
        {
            get
            {
                var searchTimeSpan = default(TimeSpan?);

                if (StartDateTime.HasValue || EndDateTime.HasValue)
                {
                    searchTimeSpan = EndDateTime - StartDateTime;
                }

                return searchTimeSpan;
            }
        }

        /// <summary>
        /// Hash that uses the chars in the hint phrase as a key and the number of times that char appears as the value.
        /// </summary>
        private Dictionary<char, int> CharCountFromHintDictionary { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Entry point for the search.
        /// </summary>
        /// <param name="hintPhrase">Anagram of the secret phrase to find</param>
        /// <param name="md5HashKeyOfSolution">MD5 hash key used to verify the secret phrase was found</param>
        /// <param name="inputFile">Full or relative path to the input file. If it is just the file name then it assumes the current working directory.</param>
        /// <returns></returns>
        public string FindSecretPhrase(string hintPhrase, string md5HashKeyOfSolution, string inputFile)
        {
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

            // Initialize
            HintPhrase = hintPhrase.Trim().ToLower();
            MD5HashKeyOfSolution = md5HashKeyOfSolution;
            InputFile = inputFile.Trim();
            NumberOfPhraseComparisons = 0;
            MD5Hash = MD5.Create();
            FilteredWordList = new List<string>();

            StartDateTime = DateTime.Now;
            Console.WriteLine("Started search:\t\t" + StartDateTime.Value.ToString());

            CharCountFromHintDictionary = GetCharCountFromString(HintPhrase);
            ReadInputFile();
            var secretPhrase = SearchPhrases();

            EndDateTime = DateTime.Now;

            // clean up
            MD5Hash.Dispose();
            CharCountFromHintDictionary.Clear();
            CharCountFromHintDictionary = null;
            FilteredWordList.Clear();
            FilteredWordList = null;

            Console.WriteLine("Ended search:\t\t" + EndDateTime.Value.ToString());
            Console.WriteLine("Time elapsed:\t\t" + SearchTimeSpan.ToString());
            Console.WriteLine("Number of comparisons:\t" + NumberOfPhraseComparisons);

            if (!string.IsNullOrEmpty(secretPhrase))
            {
                Console.WriteLine("\nThe secret phrase is:\t{0}\n\n\n", secretPhrase);
            }
            else
            {
                Console.WriteLine("\nThe secret phrase was not found.\n\n\n");
            }

            return secretPhrase;
        }

        /// <summary>
        /// Loops through all the words in the input file and only adds words that meet the criteria to filteredWordList
        /// </summary>
        private void ReadInputFile()
        {
            var start = DateTime.Now;

            foreach (var word in File.ReadAllLines(InputFile).ToList())
            {
                if (!ExcludeWord(word.Trim().ToLower()))
                {
                    FilteredWordList.Add(word.ToLower());
                }
            }

            // Sort the filtered words first by length then alphabetically. This will make the search alot faster later.
            FilteredWordList = FilteredWordList.OrderByDescending(x => x.Length).ThenBy(x => x).Distinct().ToList();

            Console.WriteLine("Word filtering time:\t{0}", (DateTime.Now - start).ToString());
            Console.WriteLine("Words filtered:\t\t{0}", FilteredWordList.Count);
        }

        /// <summary>
        /// Creates a hash (Dictionary) using the word length as a key and a List<string> as the value.
        /// This list has all filtered words that are of the length of the key
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, List<string>> CreateWordLengthsDictionary()
        {
            var wordLengthsDictionary = new Dictionary<int, List<string>>();

            foreach (var word in FilteredWordList)
            {
                if (wordLengthsDictionary.ContainsKey(word.Length))
                {
                    var lengthList = wordLengthsDictionary[word.Length];
                    lengthList.Add(word);
                }
                else
                {
                    var lengthList = new List<string>();
                    lengthList.Add(word);

                    wordLengthsDictionary.Add(word.Length, lengthList);
                }
            }

            return wordLengthsDictionary;
        }

        /// <summary>
        /// Compares three word phrases with the MD5 hash key of the secret phrase.
        /// </summary>
        /// <returns>The secret phrase or null if not found</returns>
        private string SearchPhrases()
        {
            // must be three words because the spaces count in the MD5 hash key
            // words cannot have spaces
            // assumes that no leading or trail white space exists
            var wordLengthsDictionary = CreateWordLengthsDictionary();
            var lengthWithoutSpaces = HintPhrase.Length - CharCountFromHintDictionary[' '];

            // Get three word sets that have combined lengthWithoutSpaces chars. The ints reprent indexes in the wordLengthsDictionary.
            var lengthTuplesList = new List<Tuple<int, int, int>>();

            // create tuples of words whos lengths add up to the length of the hint phrase minus the spaces
            for (var first = 0; first < wordLengthsDictionary.Count; first++)
            {
                for (var second = 0; second < wordLengthsDictionary.Count; second++)
                {
                    for (var third = 0; third < wordLengthsDictionary.Count; third++)
                    {
                        var totalLength = wordLengthsDictionary.ElementAt(first).Key + wordLengthsDictionary.ElementAt(second).Key + wordLengthsDictionary.ElementAt(third).Key;

                        if (totalLength == lengthWithoutSpaces)
                        {
                            lengthTuplesList.Add(new Tuple<int, int, int> (first, second, third));
                        }
                    }
                }
            }

            var phrase = new StringBuilder(HintPhrase.Length);

            // use the tuple to make only phrases of length 20
            foreach (var tuple in lengthTuplesList)
            {
                foreach (var word1 in wordLengthsDictionary.ElementAt(tuple.Item1).Value)
                {
                    foreach (var word2 in wordLengthsDictionary.ElementAt(tuple.Item2).Value)
                    {
                        phrase.Clear().Append(word1).Append(" ").Append(word2);

                        // Its possible for a pair to be invalid on their own by violating the cardinality of any char. This saves alot of time.
                        // The sorting by length decending shows its benifit by excluding many two word phrases at this point.
                        if (!ExcludeWord(phrase.ToString()))
                        {
                            foreach (var word3 in wordLengthsDictionary.ElementAt(tuple.Item3).Value)
                            {
                                phrase.Clear().Append(word1).Append(" ").Append(word2).Append(" ").Append(word3);

                                if (!ExcludePhrase(phrase.ToString()) && VerifyMd5Hash(phrase.ToString()))
                                {
                                    return phrase.ToString();
                                }
                            }
                        }
                    }
                }
            }

            return null;
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
            var charList = stringToCount.Trim().ToCharArray().OrderBy(x => x).ToList();

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
        /// Uses the hash table created from the hint phrase to exclude single words.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool ExcludeWord(string word)
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
        private bool ExcludePhrase(string phrase)
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
            NumberOfPhraseComparisons++;
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(input), MD5HashKeyOfSolution);
        }

        #endregion Methods
    }
}
