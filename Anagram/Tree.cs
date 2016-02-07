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

        private Dictionary<int, List<string>> DistinctWords = new Dictionary<int, List<string>>();

        private int NumWords { get; set; }

        private Anagram _Anagram { get; set; }

        public Tree(int numWords, IEnumerable<string> distinctWords, Anagram anagram)
        {
            NumWords = numWords;
            //DistinctWords = distinctWords;
            _Anagram = anagram;
            RootNode = new Node(null, null, 0);
            _Anagram.NumNodes++;

            _Anagram.AddNodesStartTime = DateTime.Now;

            foreach (var s in distinctWords)
            {
                if (DistinctWords.ContainsKey(s.Length))
                {
                    DistinctWords[s.Length].Add(s);
                }
                else
                {
                    var list = new List<string>();
                    list.Add(s);

                    DistinctWords.Add(s.Length, list);
                }
            }

            AddNodes(RootNode);

            _Anagram.AddNodesEndTime = DateTime.Now;
        }

        public void AddNodes(Node node)
        {
            if (!_Anagram.SecretPhraseFound && node != null && node.WordNumber <= NumWords)
            {
                foreach (var keyValuePair in DistinctWords)
                {
                    if (_Anagram.IsPhraseTooLong(node, keyValuePair.Key))
                    {
                        foreach (var word in keyValuePair.Value)
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
    }
}
