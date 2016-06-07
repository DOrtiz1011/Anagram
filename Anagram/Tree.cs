using System.Collections.Generic;
using System.Text;

namespace Anagram
{
    internal class Tree
    {
        public void TreeSearch(Anagram anagram)
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

                foreach (var keyWordListPair in anagram.WordHash)
                {
                    var currentPhrasePlusNextKey = stringBuilder.Clear().Append(currentPhrase).Append(" ").Append(keyWordListPair.Key).ToString().Trim();

                    if (anagram.IsPhraseValid(currentPhrasePlusNextKey, newWordNumber))
                    {
                        // loop through each word that matches the key
                        foreach (var newWord in keyWordListPair.Value)
                        {
                            if (newWordNumber == anagram.NumWords)
                            {
                                anagram.VerifyMd5Hash(string.Format("{0} {1}", currentPhrase, newWord).Trim());

                                if (anagram.SecretPhraseFound)
                                {
                                    return;
                                }
                            }
                            else if (newWordNumber < anagram.NumWords)
                            {
                                anagram.NumNodes++;
                                queue.Enqueue(currentNode.AddAdjacentNode(newWord));
                            }
                        }
                    }
                }
            }
        }
    }
}
