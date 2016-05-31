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

        private int NumWords { get; set; }

        private Anagram _Anagram { get; set; }

        public void TreeSearch(int numWords, IEnumerable<string> distinctWords, Anagram anagram)
        {
            NumWords = numWords;
            _Anagram = anagram;

            _Anagram.AddNodesStartTime = DateTime.Now;
            RootNode = new Node(null, null, 0, 0);

            _Anagram.NumNodes++;
            AddNodes();

            _Anagram.AddNodesEndTime = DateTime.Now;
        }

        private void AddNodes()
        {
            var queue = new Queue<Node>();
            queue.Enqueue(RootNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                foreach (var wordsByLength in _Anagram.WordsByLength)
                {
                    var nextWordLength = wordsByLength.Key;

                    if (_Anagram.CheckPhraseLength(currentNode.FullPhraseLength, nextWordLength, currentNode.WordNumber))
                    {
                        var wordList = wordsByLength.Value;   // all words of length nextWordLength

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
