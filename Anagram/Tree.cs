using System;
using System.Collections.Generic;

namespace Anagram
{
    internal class Tree
    {
        /// <summary>
        /// The starting point of the tree. All of its properties will be null;
        /// </summary>
        private Node RootNode { get; set; }

        private Dictionary<int, List<string>> DistinctWords = new Dictionary<int, List<string>>();

        private int NumWords { get; set; }

        private Anagram _Anagram { get; set; }

        public void TreeSearch(int numWords, IEnumerable<string> distinctWords, Anagram anagram)
        {
            NumWords = numWords;
            _Anagram = anagram;

            _Anagram.AddNodesStartTime = DateTime.Now;
            RootNode = new Node(null, null, 0);

            _Anagram.NumNodes++;

            CreateDistinctWordsDitionary(distinctWords);
            AddNodes();

            _Anagram.AddNodesEndTime = DateTime.Now;
        }

        private void CreateDistinctWordsDitionary(IEnumerable<string> distinctWords)
        {
            foreach (var word in distinctWords)
            {
                if (DistinctWords.ContainsKey(word.Length))
                {
                    DistinctWords[word.Length].Add(word);
                }
                else
                {
                    var list = new List<string>();
                    list.Add(word);

                    DistinctWords.Add(word.Length, list);
                }
            }
        }

        private void AddNodes()
        {
            var queue = new Queue<Node>();
            queue.Enqueue(RootNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                foreach (var keyValuePair in DistinctWords)
                {
                    var nextWordLength = keyValuePair.Key;

                    if (_Anagram.CheckPhraseLength(currentNode, nextWordLength))
                    {
                        var wordList = keyValuePair.Value;   // all words of length nextWordLength

                        foreach (var word in wordList)
                        {
                            var newNode = currentNode.AddAdjacentNode(word, _Anagram);

                            if (!_Anagram.SecretPhraseFound && newNode != null && newNode.WordNumber < _Anagram.NumWords)
                            {
                                queue.Enqueue(newNode);
                            }
                            else if (_Anagram.SecretPhraseFound)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
