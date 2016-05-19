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

            AddNodes();

            _Anagram.AddNodesEndTime = DateTime.Now;
        }

        public void AddNodes2(Node node)
        {
            if (_Anagram.SecretPhraseFound || node == null || node.WordNumber > NumWords)
            {
                return;
            }

            foreach (var keyValuePair in DistinctWords)
            {
                //if (_Anagram.IsPhraseTooLong(node, keyValuePair.Key))
                {
                    foreach (var word in keyValuePair.Value)
                    {
                        var newNode = node.AddAdjacentNode(word, _Anagram);

                        if (!_Anagram.SecretPhraseFound && newNode != null)
                        {
                            AddNodes2(newNode);
                        }
                    }
                }
            }
        }

        private void AddNodes()
        {
            var queue = new Queue<Node>();
            queue.Enqueue(RootNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (node.WordNumber < _Anagram.NumWords)
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
                                    queue.Enqueue(newNode);
                                }
                                else if (_Anagram.SecretPhraseFound)
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
        }
    }
}
