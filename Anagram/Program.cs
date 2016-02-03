using System;

namespace Anagram
{
    class Program
    {
        static void Main(string[] args)
        {
            const string inputFile = "wordlist.txt";
            
            Console.WriteLine("*** Test started {0} ***\n", DateTime.Now);

            new Anagram("poultry outwits ants", "4624d200580677270a54ccff86b9610e", inputFile);   // pastils turnout towy
            //new AnagramUtilities("poultry outwits ants", "4a9f51db2c7eba0c724499f749d3176a", inputFile);   // trustpilot wants you
            //new AnagramUtilities("a", "0cc175b9c0f1b6a831c399e269772661", inputFile);                      // a
            //new AnagramUtilities("a b c", "fa5124f324ffee2030948de7d4bb78ad", inputFile);                  // c b a
            //new AnagramUtilities("a b c d", "a74caae2896653023ae0013f88249f91", inputFile);                // d c b a
            //new AnagramUtilities("a b c d a b c d", "430530dae271684171f86612d00935ad", inputFile);        // d c b a d c b a

            //new AnagramUtilities("trustpilot outwits ants turnout", "ff771224323d73673f24b50192ae2e5c", inputFile);                // trustpilot outwits ants a c d turnout



            Console.ReadLine();
        }
    }
}
