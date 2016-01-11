using System;

namespace TrustPilotAnagram
{
    class Program
    {
        static void Main(string[] args)
        {
            var trustPilotAnagram = new TrustPilotAnagram();

            // test with original word list, takes about 5 seconds
            Console.WriteLine("*************************************************************");
            Console.WriteLine("* Test 1: Positive test case with original inputs and file. *");
            Console.WriteLine("*************************************************************");

            trustPilotAnagram.FindSecretPhrase("poultry outwits ants", "4624d200580677270a54ccff86b9610e", "wordlist_original.txt");

            // test worst case takes about 45 seconds to seach all the posible phrases
            Console.WriteLine("********************************************************************************************");
            Console.WriteLine("* Test 2: Negative test case with original inputs and file intentionally modified to fail. *");
            Console.WriteLine("********************************************************************************************");

            trustPilotAnagram.FindSecretPhrase("poultry outwits ants", "4624d200580677270a54ccff86b9610e", "wordlist_fail.txt");
        }
    }
}
