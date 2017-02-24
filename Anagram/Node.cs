using System.Collections.Generic;
using System.Text;

namespace Anagram
{
    internal sealed class Node
    {
        private string Word { get; }
        public int WordNumber { get; }
        private Node ParentNode { get; }

        public Node(string word, Node parentNode, int wordNumber)
        {
            Word = word;
            ParentNode = parentNode;
            WordNumber = wordNumber;
        }

        public string GetFullPhrase(int phraseLength)
        {
            string fullPhrase;
            var stack = new Stack<string>();
            var node = ParentNode;

            while (node != null && node.WordNumber != 0)
            {
                stack.Push(node.Word);
                node = node.ParentNode;
            }

            if (stack.Count > 0)
            {
                var stringBuilder = new StringBuilder(phraseLength);

                while (stack.Count > 0)
                {
                    stringBuilder.Append(stack.Pop()).Append(" ");
                }

                fullPhrase = stringBuilder.Append(Word).ToString();
            }
            else
            {
                fullPhrase = Word;
            }

            return fullPhrase;
        }
    }
}
