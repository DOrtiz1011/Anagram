using System.Text;

namespace Anagram
{
    public class Node
    {
        public string Word { get; private set; }
        public int WordNumber { get; private set; }
        public Node ParentNode { get; private set; }
        
        public Node(string word, Node parentNode, int wordNumber)
        {
            Word = word;
            ParentNode = parentNode;
            WordNumber = wordNumber;
        }

        public Node AddAdjacentNode(string word, Anagram anagram)
        {
            var newNode       = default(Node);
            var wordNumber    = WordNumber + 1;
            var stringBuilder = new StringBuilder(anagram.HintPhrase.Length);

            GetParentPhrase(this, stringBuilder);
            var phrase = stringBuilder.Append(word).ToString().Trim();

            if (!anagram.ExcludeByNumWords(phrase, wordNumber))
            {
                anagram.NumNodes++;
                newNode = new Node(word, this, wordNumber);
            }

            return newNode;
        }

        public string GetParentPhrase()
        {
            var stringBuilder = new StringBuilder();
            GetParentPhrase(this, stringBuilder);
            return stringBuilder.ToString().Trim();
        }

        private void GetParentPhrase(Node node, StringBuilder stringBuilder)
        {
            if (node.ParentNode == null || string.IsNullOrEmpty(node.ParentNode.Word))
            {
                stringBuilder.Append(node.Word).Append(" ");
            }
            else
            {
                GetParentPhrase(node.ParentNode, stringBuilder.Append(node.Word).Append(" "));
            }
        }
    }
}
