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
            anagram.NumNodes++;

            anagram.AddNodesStartTime = DateTime.Now;

            AddNodes(RootNode);

            anagram.AddNodesEndTime = DateTime.Now;
        }

        public void AddNodes(Node node)
        {
            if (node.WordNumber <= NumWords && string.IsNullOrEmpty(_Anagram.SecretPhrase))
            {
                foreach (var word in DistinctWords)
                {
                    var newNode = node.AddAdjacentNode(word, _Anagram);

                    if (!string.IsNullOrEmpty(_Anagram.SecretPhrase))
                    {
                        break;
                    }

                    if (newNode != null)
                    {
                        AddNodes(newNode);
                    }
                }
            }
        }
    }
}
