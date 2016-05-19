using System.Collections.Generic;
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
            var newNode = default(Node);
            var wordNumber = WordNumber + 1;
            var phrase = string.Format("{0} {1}", GetParentPhrase(), word).Trim();

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
            var stack = new Stack<Node>();
            var tempNode = ParentNode;

            stack.Push(this);

            while (tempNode != null && tempNode.WordNumber != 0)
            {
                stack.Push(tempNode);
                tempNode = tempNode.ParentNode;
            }

            while (stack.Count > 0)
            {
                var word = stack.Pop().Word;
                stringBuilder.Append(word).Append(" ");
            }

            return stringBuilder.ToString().Trim();
        }
    }
}
