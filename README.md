# Anagram

This is my solution to Trustpilot's backend anagram programming challege. It uses a hash within a has to organize the words first by length then by a hash key and is the chars sorted alphabetically. Words are only placed in this has it they meet the constrains set by the hint phrase. After the words are organized a tree is built up with word combinations that meet the constrains set by the hint phrase. This is repeated using DFS until the secret phrase is found or all the combinations that meet the constrains set by the hint phrase are tested.

The performace worked out pretty good. The last test case uses a MD5 produces a phrase that cannot be found in the word list. In this case my algorithm made the detemination in 28.8473648 seconds.

This challenge is alot of fun and I encourage everyone to try it as it will help you learn to use algorithms and datastructures together. This link to the problem is below. Any suggestions to make it work even faster? Feel free to sent me a pull request.

http://followthewhiterabbit.trustpilot.com/cs/step3.html


Other things to note:
- This solution can handle any number of words. Many people have come up with solutions that assume there will always be three words.
- To get the fastest possible execution time you must run this in release mode. Debug mode has some overhead that slows down the algorithm.
- The three test cases here are from the original problem. Adding more will include them to the total and average times.
