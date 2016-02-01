using System.Collections.Generic;
using System.Text;

namespace Anagram
{
    internal class GraphNode
    {
        public string Word { get; private set; }
        public string ParentPhrase { get; private set; }
        public int WordNumber { get; private set; }
        public Dictionary<string, GraphNode> AdjacencyList { get; private set; }
        public GraphNode ParentGraphNode { get; private set; }
        private bool foundSecretPhrase = false;

        public GraphNode(string word, GraphNode parentGraphNode, int wordNumber, string parentPhrase)
        {
            Word = word;
            AdjacencyList = new Dictionary<string, GraphNode>();
            ParentGraphNode = parentGraphNode;
            WordNumber = wordNumber;
            ParentPhrase = parentPhrase;
        }

        public void AddAdjacentNode(string word, Exclude excluder)
        {
            if (!foundSecretPhrase && !AdjacencyList.ContainsKey(word))
            {
                var wordNumber = WordNumber + 1;
                //var stringBuilder = new StringBuilder(excluder.HintPhrase.Length);
                var parentPhrase = GetParentPhrase(this);//, stringBuilder);

                if (!excluder.ExcludeByNumWords(string.Format("{0} {1}", parentPhrase, word), wordNumber, out foundSecretPhrase))
                {
                        var newNode = new GraphNode(word, this, wordNumber, parentPhrase);

                        AdjacencyList.Add(newNode.Word, newNode);
                }
            }
        }

        public string GetParentPhrase(GraphNode graphNode)//, StringBuilder stringBuilder)
        {
            if (graphNode.ParentGraphNode == null || graphNode.ParentGraphNode.Word == null)
            {
                //stringBuilder.Clear().Append(graphNode.Word);
                return graphNode.Word;
            }
            //stringBuilder.Clear().Append(GetParentPhrase(graphNode.ParentGraphNode, stringBuilder)).Append(" ").Append(graphNode.Word);
            return string.Format("{0} {1}", GetParentPhrase(graphNode.ParentGraphNode/*, stringBuilder*/), graphNode.Word);
        }
    }
}
