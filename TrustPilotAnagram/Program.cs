﻿using System;

namespace TrustPilotAnagram
{
    class Program
    {
        static void Main(string[] args)
        {
            var trustPilotAnagram = new TrustPilotAnagram();

            // test with original word list, takes about 2 minutes
            var secretPhrase = trustPilotAnagram.FindSecretPhrase("poultry outwits ants", "4624d200580677270a54ccff86b9610e", "wordlist_original.txt");

            // test worst case, also takes about 2 minutes
            //var secretPhrase = trustPilotAnagram.FindSecretPhrase("poultry outwits ants", "4624d200580677270a54ccff86b9610e", "wordlist_fail.txt");

            if (!string.IsNullOrEmpty(secretPhrase))
            {
                Console.WriteLine("The secret phrase is: " + secretPhrase);
            }
            else
            {
                Console.WriteLine("The secret phrase was not found.");
            }
        }
    }
}
