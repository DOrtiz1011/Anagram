using System;
using System.Collections.Generic;

namespace Anagram
{
    internal class Tree
    {
        /// <summary>
        /// The starting point of the tree. All of its properties will be null;
        /// </summary>
        public Node RootNode { get; private set; }

        private IEnumerable<string> DistinctWords { get; set; }

        private int NumWords { get; set; }

        private Anagram _Anagram { get; set; }

        public Tree(int numWords, IEnumerable<string> distinctWords, Anagram anagram)
        {
            NumWords = numWords;
            DistinctWords = distinctWords;
            _Anagram = anagram;
            RootNode = new Node(null, null, 0);
            _Anagram.NumNodes++;

            _Anagram.AddNodesStartTime = DateTime.Now;

            AddNodes(RootNode);

            _Anagram.AddNodesEndTime = DateTime.Now;
        }

        public void AddNodes(Node node)
        {
            if (!_Anagram.SecretPhraseFound && node != null && node.WordNumber <= NumWords)
            {
                foreach (var word in DistinctWords)
                {
                    var newNode = node.AddAdjacentNode(word, _Anagram);

                    if (!_Anagram.SecretPhraseFound && newNode != null)
                    {
                        AddNodes(newNode);
                    }
                }
            }
        }
    }
}
