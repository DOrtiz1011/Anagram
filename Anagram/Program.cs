using System;

namespace Anagram
{
    class Program
    {
        static void Main(string[] args)
        {
            const string inputFile = "wordlist.txt";
            
            Console.WriteLine("*** Test started {0} ***\n", DateTime.Now);

            new Anagram("poultry outwits ants", "4624d200580677270a54ccff86b9610e", inputFile);      // pastils turnout towy
            new Anagram("poultry outwits ants", "4a9f51db2c7eba0c724499f749d3176a", inputFile);      // trustpilot wants you
            new Anagram("academicians outwits ac", "6c98e6e707c180420e358075e5d93b81", inputFile);   // pastils turnout towy
            new Anagram("trustpilot", "f5ad76ee64505e3ae887b634e05f30be", inputFile);                // trustpilot
            new Anagram("a", "0cc175b9c0f1b6a831c399e269772661", inputFile);                         // a
            new Anagram("a b c", "fa5124f324ffee2030948de7d4bb78ad", inputFile);                     // c b a
            new Anagram("a b c d", "a74caae2896653023ae0013f88249f91", inputFile);                   // d c b a
            new Anagram("a b c d a b c d", "430530dae271684171f86612d00935ad", inputFile);           // d c b a d c b a            
            new Anagram("a b c d e f g h", "a878170edc8cad2b11adef4c872be6be", inputFile);           // a b c d e f g h

            Console.ReadLine();
        }
    }
}
