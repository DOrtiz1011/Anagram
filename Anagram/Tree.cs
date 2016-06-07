using System.Collections.Generic;
using System.Text;

namespace Anagram
{
    internal class Tree
    {
        public void AddNodes(Anagram anagram)
        {
            var queue = new Queue<Node>();
            var stringBuilder = new StringBuilder(anagram.HintPhrase.Length);

            queue.Enqueue(new Node(null, null, 0));   // add root node
            anagram.NumNodes++;

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                var currentPhrase = currentNode.GetFullPhrase();
                var newWordNumber = currentNode.WordNumber + 1;

                stringBuilder.Clear().Append(currentPhrase);
                var endIndexOfCurrentPhrase = stringBuilder.Length;

                foreach (var keyWordListPair in anagram.WordHash)
                {
                    if (!string.IsNullOrEmpty(currentPhrase))
                    {
                        stringBuilder.Append(" ");
                    }

                    stringBuilder.Append(keyWordListPair.Key);
                    var lengthToRemove = stringBuilder.Length - endIndexOfCurrentPhrase;

                    if (anagram.IsPhraseValid(stringBuilder.ToString(), newWordNumber))
                    {
                        // loop through each word that matches the key
                        foreach (var newWord in keyWordListPair.Value)
                        {
                            if (newWordNumber == anagram.NumWords)
                            {
                                stringBuilder.Remove(endIndexOfCurrentPhrase, lengthToRemove);

                                if (stringBuilder.Length > 0)
                                {
                                    stringBuilder.Append(" ");
                                }

                                stringBuilder.Append(newWord);

                                anagram.VerifyMd5Hash(stringBuilder.ToString());

                                if (anagram.SecretPhraseFound)
                                {
                                    return;
                                }
                            }
                            else if (newWordNumber < anagram.NumWords)
                            {
                                queue.Enqueue(new Node(newWord, currentNode, newWordNumber));
                                anagram.NumNodes++;
                            }
                        }
                    }

                    stringBuilder.Remove(endIndexOfCurrentPhrase, lengthToRemove);
                }
            }
        }
    }
}
