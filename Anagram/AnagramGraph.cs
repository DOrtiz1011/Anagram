using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Anagram
{
    internal class AnagramGraph
    {
        public GraphNode RootNode { get; private set; }
        public string HintPhrase { get; private set; }
        internal Exclude Excluder { get; private set; }

        /// <summary>
        /// The MD5 hash key that represents the secret phrase. It is used to veryfy that the phrase was found.
        /// </summary>
        public string MD5HashKeyOfSolution { get; private set; }

        /// <summary>
        /// Full or relative path to the input file. If it is just the file name then it assumes the current working directory.
        /// </summary>
        public string InputFile { get; private set; }

        private List<string> DistinctWordList { get; set; }

        public AnagramGraph(string hintPhrase, string md5HashKey, string inputFile)
        {
            HintPhrase = hintPhrase;
            MD5HashKeyOfSolution = md5HashKey;
            InputFile = inputFile;
            Excluder = new Exclude(HintPhrase, md5HashKey);
            RootNode = new GraphNode(null, null, 0, /*NumWords,*/ null);

            MakeDistinctWordList();

            var start = DateTime.Now;
            Console.WriteLine("Node adding Start:\t{0}", start);
            AddNodes(RootNode);
            Console.WriteLine("Node adding time:\t{0}", (DateTime.Now - start).ToString());
        }

        private void MakeDistinctWordList()
        {
            var start = DateTime.Now;

            DistinctWordList = new List<string>();

            foreach (var word in File.ReadAllLines(InputFile).ToList())
            {
                var wordLower = word.Trim().ToLower();

                if (!Excluder.ExcludeWord(wordLower))
                {
                    DistinctWordList.Add(wordLower);
                }
            }

            // Sort the filtered words first by length then alphabetically. This will make the search alot faster later.
            DistinctWordList = DistinctWordList.OrderByDescending(x => x.Length).ThenBy(x => x).Distinct().ToList();

            Console.WriteLine("Word filtering time:\t{0}", (DateTime.Now - start).ToString());
            Console.WriteLine("Words filtered:\t\t{0}", DistinctWordList.Count);
        }

        public void AddNodes(GraphNode graphNode)
        {
            if (graphNode.WordNumber <= Excluder.NumWords)
            {
                foreach (var word in DistinctWordList)
                {
                    graphNode.AddAdjacentNode(word, Excluder);

                    if (!string.IsNullOrEmpty(Excluder.SecretPhrase))
                    {
                        break;
                    }
                }

                if (string.IsNullOrEmpty(Excluder.SecretPhrase))
                {
                    foreach (var keyValuePair in graphNode.AdjacencyList)
                    {
                        AddNodes(keyValuePair.Value);
                    }
                }
            }
        }
    }
}
