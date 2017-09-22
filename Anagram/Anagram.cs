using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Anagram
{
    public sealed class Anagram : IDisposable
    {
        private MD5 _md5Hash;
        private int[] _maxPhraseLengths;
        private int[] _minPhraseLengths;
        private readonly StringBuilder _stringBuilder = new StringBuilder(32);

        internal WordHash WordHash;

        private int SingleWordMaxLength { get; set; }
        private Dictionary<char, int> CharCountFromHint { get; set; }
        private DateTime? StartTime { get; set; }
        private DateTime? EndTime { get; set; }
        private string InputFile { get; set; }
        private HashSet<string> DistinctWords { get; set; }
        private int LongestWordLength { get; set; }

        public string Md5HashKey { get; private set; }
        public int NumMd5HashKeyComparisons { get; private set; }
        public string SecretPhrase { get; private set; }
        public bool SecretPhraseFound { get; private set; }
        public string HintPhrase { get; private set; }
        public int NumWords { get; private set; }
        public int NumNodes { get; set; }

        public TimeSpan? TotalTime => EndTime.HasValue && StartTime.HasValue ? EndTime.Value - StartTime.Value : default(TimeSpan?);

        public int WordsFiltered => DistinctWords.Count;

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
            Md5HashKey = md5HashKeyOfSolution;
            InputFile = inputFile;

            InitializeProperties();
            Tree.AddNodes(this);
            CleanUp();

            EndTime = DateTime.Now;
        }

        private void InitializeProperties()
        {
            ClearCollections();

            CharCountFromHint = GetCharCountFromString(HintPhrase);
            NumWords = (CharCountFromHint.ContainsKey(' ') ? CharCountFromHint[' '] : 0) + 1;
            SingleWordMaxLength = HintPhrase.Length - (NumWords - 1) - (CharCountFromHint.ContainsKey(' ') ? CharCountFromHint[' '] : 0);

            GetDistinctWords();
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
            DistinctWords = null;
            CharCountFromHint = null;
            WordHash = null;
            _maxPhraseLengths = null;
            _minPhraseLengths = null;
        }

        private void CreateWordHash()
        {
            WordHash = new WordHash();

            foreach (var word in DistinctWords)
            {
                WordHash.AddWord(word);
            }
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
                    minLength = HintPhrase.Length - (LongestWordLength + 1);
                }

                _minPhraseLengths[i] = minLength;
            }
        }

        private void CleanUp() => Dispose();

        public bool IsSubPhraseValid(string word)
        {
            var charCount = GetCharCountFromString(word);

            foreach (var keyValuePair in charCount)
            {
                if (!CharCountFromHint.ContainsKey(keyValuePair.Key) || CharCountFromHint[keyValuePair.Key] < keyValuePair.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsPhraseAnagram(string phrase)
        {
            var charCount = GetCharCountFromString(phrase);

            foreach (var keyValuePair in charCount)
            {
                if (!CharCountFromHint.ContainsKey(keyValuePair.Key) || CharCountFromHint[keyValuePair.Key] != keyValuePair.Value)
                {
                    return false;
                }
            }

            return true;
        }

        private static Dictionary<char, int> GetCharCountFromString(string str)
        {
            var charCount = new Dictionary<char, int>();

            foreach (var c in str)
            {
                if (charCount.ContainsKey(c))
                {
                    charCount[c]++;
                }
                else
                {
                    charCount.Add(c, 1);
                }
            }

            return charCount;
        }

        private void GetDistinctWords()
        {
            DistinctWords = new HashSet<string>();

            foreach (var word in File.ReadAllLines(InputFile))
            {
                if (IsSubPhraseValid(word))
                {
                    DistinctWords.Add(word);

                    if (word.Length > LongestWordLength)
                    {
                        LongestWordLength = word.Length;
                    }
                }
            }
        }

        private string GetMd5Hash(string input)
        {
            _stringBuilder.Clear();

            foreach (var t in _md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input)))
            {
                _stringBuilder.Append(t.ToString("x2"));
            }

            return _stringBuilder.ToString();
        }

        public void VerifyMd5Hash(string input)
        {
            NumMd5HashKeyComparisons++;

            if (0 != StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(input), Md5HashKey))
            {
                return;
            }

            SecretPhrase = input;
            SecretPhraseFound = true;
        }

        public bool CheckLength(int newWordLength, int wordNumber, int currentPhraseLength)
        {
            var validLength = false;

            if (wordNumber == 1)
            {
                validLength = true;
            }
            else
            {
                var phraseLength = newWordLength + currentPhraseLength;

                if (wordNumber > 1 && wordNumber < NumWords)
                {
                    validLength = phraseLength <= _maxPhraseLengths[wordNumber] && phraseLength >= _minPhraseLengths[wordNumber];
                }
                else if (wordNumber == NumWords)
                {
                    validLength = phraseLength == HintPhrase.Length;
                }
            }

            return validLength;
        }

        public void Dispose() => ((IDisposable)_md5Hash).Dispose();
    }
}
