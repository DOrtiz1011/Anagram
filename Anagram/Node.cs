using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anagram
{
    internal sealed class Node
    {
        private string Word { get; }
        internal int WordNumber { get; }
        private Node ParentNode { get; }

        internal Node(string word, Node parentNode, int wordNumber)
        {
            Word = word;
            ParentNode = parentNode;
            WordNumber = wordNumber;
        }

        internal string GetFullPhrase()
        {
            string fullPhrase;
            var stack = new Stack<string>();
            var node = ParentNode;

            while (node != null && node.WordNumber != 0)
            {
                stack.Push(node.Word);
                node = node.ParentNode;
            }

            if (stack.Any())
            {
                var stringBuilder = new StringBuilder();

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
