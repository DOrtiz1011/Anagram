using System.Collections.Generic;
using System.Text;

namespace Anagram
{
    internal class GraphNode
    {
        public string Word { get; private set; }
        //public string ParentPhrase { get; private set; }
        public int WordNumber { get; private set; }
        public Dictionary<string, GraphNode> AdjacencyHash { get; private set; }
        public GraphNode ParentGraphNode { get; private set; }
        private bool foundSecretPhrase = false;

        public GraphNode(string word, GraphNode parentGraphNode, int wordNumber, string parentPhrase)
        {
            Word = word;
            AdjacencyHash = new Dictionary<string, GraphNode>();
            ParentGraphNode = parentGraphNode;
            WordNumber = wordNumber;
            //ParentPhrase = parentPhrase;
        }

        public void AddAdjacentNode(string word, AnagramUtilities anagramUtilities)
        {
            if (!foundSecretPhrase && !AdjacencyHash.ContainsKey(word))
            {
                var wordNumber   = WordNumber + 1;
                var parentPhrase = GetParentPhrase(this);

                if (!anagramUtilities.ExcludeByNumWords(string.Format("{0} {1}", parentPhrase, word), wordNumber, out foundSecretPhrase))
                {
                    var newNode = new GraphNode(word, this, wordNumber, parentPhrase);
                    anagramUtilities.NumNodes++;
                    AdjacencyHash.Add(newNode.Word, newNode);
                }
            }
        }

        public string GetParentPhrase(GraphNode graphNode)
        {
            if (graphNode.ParentGraphNode == null || graphNode.ParentGraphNode.Word == null)
            {
                return graphNode.Word;
            }

            return string.Format("{0} {1}", GetParentPhrase(graphNode.ParentGraphNode), graphNode.Word);
        }
    }
}
