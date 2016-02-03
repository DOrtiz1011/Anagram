using System;
using System.Collections.Generic;

namespace Anagram
{
    internal class AnagramGraph
    {
        /// <summary>
        /// The starting point of the graph. All of its properties will be null;
        /// </summary>
        public GraphNode RootNode { get; private set; }

        private IEnumerable<string> DistinctWords { get; set; }

        private int NumWords { get; set; }

        public AnagramGraph(int numWords, IEnumerable<string> distinctWords, AnagramUtilities anagramUtilities)
        {
            NumWords = numWords;
            DistinctWords = distinctWords;
            RootNode = new GraphNode(null, null, 0);
            anagramUtilities.NumNodes++;

            anagramUtilities.AddNodesStartTime = DateTime.Now;

            AddNodes(RootNode, anagramUtilities);

            anagramUtilities.AddNodesEndTime = DateTime.Now;
        }

        public void AddNodes(GraphNode graphNode, AnagramUtilities anagramUtilities)
        {
            if (graphNode.WordNumber <= NumWords && string.IsNullOrEmpty(anagramUtilities.SecretPhrase))
            {
                foreach (var word in DistinctWords)
                {
                    graphNode.AddAdjacentNode(word, anagramUtilities);

                    if (!string.IsNullOrEmpty(anagramUtilities.SecretPhrase))
                    {
                        break;
                    }
                }

                if (string.IsNullOrEmpty(anagramUtilities.SecretPhrase))
                {
                    foreach (var keyValuePair in graphNode.AdjacencyHash)
                    {
                        AddNodes(keyValuePair.Value, anagramUtilities);
                    }
                }
            }
        }
    }
}
