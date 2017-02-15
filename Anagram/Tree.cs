using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anagram
{
    internal static class Tree
    {
        public static void AddNodes(Anagram anagram)
        {
            var stack = new Stack<Node>();
            var currentPhrase = new StringBuilder(anagram.HintPhrase.Length);

            stack.Push(new Node(null, null, 0));   // add root node
            anagram.NumNodes++;

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                var newWordNumber = currentNode.WordNumber + 1;
                currentPhrase.Clear().Append(currentNode.GetFullPhrase());

                if (currentPhrase.Length > 0 && newWordNumber < anagram.NumNodes)
                {
                    currentPhrase.Append(" ");
                }

                var endIndexOfCurrentPhrase = currentPhrase.Length;

                foreach (var wordKeyKeyValuePair in anagram.WordHash.MainHash.Where(wordLengthKeyValuePair => anagram.CheckLength(wordLengthKeyValuePair.Key, newWordNumber, endIndexOfCurrentPhrase))
                                                                             .SelectMany(wordLengthKeyValuePair => wordLengthKeyValuePair.Value))
                {
                    if (newWordNumber <= 1)
                    {
                        // No need to verify first word because all words were indivdually filtered
                        AddWords(anagram, wordKeyKeyValuePair.Value, stack, currentNode, newWordNumber);
                    }
                    else
                    {
                        currentPhrase.Append(wordKeyKeyValuePair.Key);
                        var lengthToRemove = currentPhrase.Length - endIndexOfCurrentPhrase;

                        if (newWordNumber > 1 && newWordNumber < anagram.NumWords)
                        {
                            // not the first word and not the last word so test with the hash key, if it passes that all words in its list are valid
                            if (anagram.IsSubPhraseValid(currentPhrase.ToString()))
                            {
                                AddWords(anagram, wordKeyKeyValuePair.Value, stack, currentNode, newWordNumber);
                            }

                            currentPhrase.Remove(endIndexOfCurrentPhrase, lengthToRemove);
                        }
                        else if (newWordNumber == anagram.NumWords)
                        {
                            // last word
                            if (anagram.IsPhraseAnagram(currentPhrase.ToString()))
                            {
                                currentPhrase.Remove(endIndexOfCurrentPhrase, lengthToRemove);

                                foreach (var newWord in wordKeyKeyValuePair.Value)
                                {
                                    currentPhrase.Append(newWord);
                                    anagram.VerifyMd5Hash(currentPhrase.ToString());

                                    if (anagram.SecretPhraseFound)
                                    {
                                        return;
                                    }

                                    currentPhrase.Remove(endIndexOfCurrentPhrase, lengthToRemove);
                                }
                            }
                            else
                            {
                                currentPhrase.Remove(endIndexOfCurrentPhrase, lengthToRemove);
                            }
                        }
                    }
                }
            }
        }

        private static void AddWords(Anagram anagram, IEnumerable<string> words, Stack<Node> stack, Node parentNode, int wordNumber)
        {
            foreach (var word in words)
            {
                stack.Push(new Node(word, parentNode, wordNumber));
                anagram.NumNodes++;
            }
        }
    }
}

