using System;

namespace Anagram
{
    class Program
    {
        static void Main(string[] args)
        {
            // test with original word list, takes about 5 seconds
            Console.WriteLine("*** Test started {0} ***\n", DateTime.Now);

            //new AnagramUtilities("poultry outwits ants", "4624d200580677270a54ccff86b9610e", "wordlist_original.txt");   // pastils turnout towy
            new AnagramUtilities("poultry outwits ants", "4a9f51db2c7eba0c724499f749d3176a", "wordlist_original.txt");   // trustpilot wants you
            //new AnagramUtilities("a", "0cc175b9c0f1b6a831c399e269772661", "wordlist_original.txt");                      // a
            //new AnagramUtilities("a b c", "fa5124f324ffee2030948de7d4bb78ad", "wordlist_original.txt");                  // c b a

            //new AnagramUtilities("a b c d", "a74caae2896653023ae0013f88249f91", "wordlist_original.txt");



            Console.ReadLine();
        }
    }
}
