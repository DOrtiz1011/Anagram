using System;

namespace TrustPilotAnagram
{
    class Program
    {
        static void Main(string[] args)
        {
            var trustPilotAnagram = new TrustPilotAnagram();

            // test with original word list, takes about 13 seconds
            var secretPhrase = trustPilotAnagram.FindSecretPhrase("poultry outwits ants", "4624d200580677270a54ccff86b9610e", "wordlist_original.txt");

            // test worst case takes about 2 minutes to seach all the posible phrases
            //var secretPhrase = trustPilotAnagram.FindSecretPhrase("poultry outwits ants", "4624d200580677270a54ccff86b9610e", "wordlist_fail.txt");

            if (!string.IsNullOrEmpty(secretPhrase))
            {
                Console.WriteLine("\nThe secret phrase is:\t" + secretPhrase);
            }
            else
            {
                Console.WriteLine("\nThe secret phrase was not found.");
            }
        }
    }
}
