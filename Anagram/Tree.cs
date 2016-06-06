using System;
using System.Collections.Generic;
using System.Text;

namespace Anagram
{
    internal class Tree
    {
        /// <summary>
        /// The starting point of the tree. All of its properties will be null;
        /// </summary>
        private Node RootNode { get; set; }

        private Anagram _Anagram { get; set; }

        public void TreeSearch(int numWords, IEnumerable<string> distinctWords, Anagram anagram)
        {
            _Anagram = anagram;

            _Anagram.AddNodesStartTime = DateTime.Now;
            RootNode = new Node(null, null, 0);

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
                var newWordNumber = currentNode.WordNumber + 1;
                var currentPhrase = currentNode.GetFullPhrase();
                var stringBuilder = new StringBuilder(_Anagram.HintPhrase.Length);

                foreach (var keyWordListPair in _Anagram._WordHash.Hash)
                {
                    stringBuilder.Clear().Append(currentPhrase).Append(" ").Append(keyWordListPair.Key);

                    if (!_Anagram.ExcludeByNumWords(stringBuilder.ToString().Trim(), newWordNumber))
                    {
                        foreach (var word in keyWordListPair.Value)
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
