namespace Anagram
{
    internal class Node
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
            var newNode      = default(Node);
            var wordNumber   = WordNumber + 1;
            var parentPhrase = GetParentPhrase(this);
            var phrase       = string.Format("{0} {1}", parentPhrase, word).Trim();

            if (!anagram.ExcludeByNumWords(phrase, wordNumber))
            {
                newNode = new Node(word, this, wordNumber);
                anagram.NumNodes++;
            }

            return newNode;
        }

        public string GetParentPhrase(Node node)
        {
            if (node.ParentNode == null || node.ParentNode.Word == null)
            {
                return node.Word;
            }

            return string.Format("{0} {1}", GetParentPhrase(node.ParentNode), node.Word);
        }
    }
}
