# Trustpilot Backend Programming Challege

This is my solution to Trustpilot's backend anagram programming challege. It uses a hash within a hash to organize the words first by length then by a hash key and is the chars sorted alphabetically. Words are only placed in this hash if they meet the constrains set by the hint phrase. After the words are organized a tree is built up with word combinations that meet the constrains set by the hint phrase. This is repeated using DFS until the secret phrase is found or all the combinations that meet the constrains set by the hint phrase are tested.

The performace worked out pretty good. The last test case uses a MD5 produces a phrase that cannot be found in the word list. In this case my algorithm made the detemination in 28.8473648 seconds.

This challenge was alot of fun and I encourage everyone to try it as it will help you learn to use algorithms and data structures together. This link to the problem is below. Any suggestions to make it work even faster? Feel free to send me a pull request.

[TrustPilot Backend Challenge](http://followthewhiterabbit.trustpilot.com/cs/step3.html)

## Other things to note

- This solution can handle any number of words. Many people have come up with solutions that assume there will always be three words.
- To get the fastest possible execution time you must run this in release mode. Debug mode has some overhead that slows down the algorithm.
- The three test cases here are from the original problem. Adding more will include them to the total and average times.
- The original word list provided is in this repository. I also include a list of all words withing the English language for testing. The correct solution is found but takes much longer. The third test case takes about 13 minutes to determine that the secret phrase does not exist.
- I did not use Linq because it has overhead that causes the algorithm to slow down.

## Sample Output

```text
Secret Phrase = printout stout yawls
Total Time    = 00:00:05.5986030
Hint Phrase   = poultry outwits ants
MD5 Hash Key  = e4820b45d2277f3844eac66c903e84be
Words         = 1,659
Nodes         = 38,100
Comparisons   = 1,527

Secret Phrase = ty outlaws printouts
Total Time    = 00:00:19.5296882
Hint Phrase   = poultry outwits ants
MD5 Hash Key  = 23170acc097c24edb98fc5488ab033fe
Words         = 1,659
Nodes         = 264,761
Comparisons   = 3,487

Secret Phrase = ** NOT FOUND **
Total Time    = 00:00:28.8473648
Hint Phrase   = poultry outwits ants
MD5 Hash Key  = 665e5bcb0c20062fe8abaaf4628bb154
Words         = 1,659
Nodes         = 591,809
Comparisons   = 4,560


Min Time     = 00:00:05.5986030
Max Time     = 00:00:28.8473648
Total Time   = 00:00:53.9756560
Average Time = 00:00:17.9918853
```
