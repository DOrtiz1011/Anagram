using System.Collections.Generic;
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
                var currentNode   = stack.Pop();
                var newWordNumber = currentNode.WordNumber + 1;

                currentPhrase.Clear().Append(currentNode.GetFullPhrase(anagram.HintPhrase.Length));

                if (currentPhrase.Length > 0 && newWordNumber < anagram.NumNodes)
                {
                    currentPhrase.Append(" ");
                }

                var endIndexOfCurrentPhrase = currentPhrase.Length;

                foreach (var (lengthKey, lengthValue) in anagram.WordHash.MainHash)
                {
                    if (anagram.CheckLength(lengthKey, newWordNumber, endIndexOfCurrentPhrase))
                    {
                        foreach (var (wordKey, wordValue) in lengthValue)
                        {
                            if (newWordNumber == 1)
                            {
                                // No need to verify first word because all words were individually filtered
                                foreach (var word in wordValue)
                                {
                                    stack.Push(new Node(word, currentNode, newWordNumber));
                                    anagram.NumNodes++;
                                }
                            }
                            else
                            {
                                currentPhrase.Append(wordKey);
                                var lengthToRemove = currentPhrase.Length - endIndexOfCurrentPhrase;

                                if (newWordNumber > 1 && newWordNumber < anagram.NumWords)
                                {
                                    // not the first word and not the last word so test with the hash key, if it passes that all words in its list are valid
                                    if (anagram.IsSubPhraseValid(currentPhrase.ToString()))
                                    {
                                        foreach (var word in wordValue)
                                        {
                                            stack.Push(new Node(word, currentNode, newWordNumber));
                                            anagram.NumNodes++;
                                        }
                                    }

                                    currentPhrase.Remove(endIndexOfCurrentPhrase, lengthToRemove);
                                }
                                else if (newWordNumber == anagram.NumWords)
                                {
                                    // last word
                                    if (anagram.IsPhraseAnagram(currentPhrase.ToString()))
                                    {
                                        currentPhrase.Remove(endIndexOfCurrentPhrase, lengthToRemove);

                                        foreach (var newWord in wordValue)
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
            }
        }
    }
}
