using System.Collections.Generic;
using System.Text;

namespace Anagram
{
    public class Node
    {
        public string Word { get; private set; }
        public int WordNumber { get; private set; }
        public int FullPhraseLength { get; private set; }
        public Node ParentNode { get; private set; }

        public Node(string word, Node parentNode, int wordNumber, int fullPhraseLength)
        {
            Word = word;
            ParentNode = parentNode;
            WordNumber = wordNumber;
            FullPhraseLength = fullPhraseLength;
        }

        public Node AddAdjacentNode(string newWord, Anagram anagram)
        {
            var newNode = default(Node);
            var newWordNumber = WordNumber + 1;
            var phrase = string.Format("{0} {1}", GetFullPhrase(), newWord).Trim();

            if (!anagram.ExcludeByNumWords(phrase, newWordNumber))
            {
                anagram.NumNodes++;
                newNode = new Node(newWord, this, newWordNumber, phrase.Length);
            }

            return newNode;
        }

        private string GetFullPhrase()
        {
            var stringBuilder = new StringBuilder();
            var stack = new Stack<string>();
            var tempNode = ParentNode;

            while (tempNode != null && tempNode.WordNumber != 0)
            {
                stack.Push(tempNode.Word);
                tempNode = tempNode.ParentNode;
            }

            while (stack.Count > 0)
            {
                stringBuilder.Append(stack.Pop()).Append(" ");
            }

            stringBuilder.Append(Word);

            return stringBuilder.ToString().Trim();
        }
    }
}
