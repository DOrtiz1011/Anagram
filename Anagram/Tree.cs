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

        public Tree(int numWords, IEnumerable<string> distinctWords, Anagram anagramUtilities)
        {
            NumWords = numWords;
            DistinctWords = distinctWords;
            RootNode = new Node(null, null, 0);
            anagramUtilities.NumNodes++;

            anagramUtilities.AddNodesStartTime = DateTime.Now;

            AddNodes(RootNode, anagramUtilities);

            anagramUtilities.AddNodesEndTime = DateTime.Now;
        }

        public void AddNodes(Node node, Anagram anagramUtilities)
        {
            if (node.WordNumber <= NumWords && string.IsNullOrEmpty(anagramUtilities.SecretPhrase))
            {
                foreach (var word in DistinctWords)
                {
                    var newNode = node.AddAdjacentNode(word, anagramUtilities);

                    if (!string.IsNullOrEmpty(anagramUtilities.SecretPhrase))
                    {
                        break;
                    }

                    if (newNode != null)
                    {
                        AddNodes(newNode, anagramUtilities);
                    }
                }
            }
        }
    }
}
