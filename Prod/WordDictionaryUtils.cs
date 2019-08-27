using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    #region Ternary search tree
    /// <summary>
    /// search/add/remove a word in a dictionary of 'n' words in o(n log(n) ) time
    /// </summary>
    public class TernarySearchTreeNode
    {
        private char C { get; set; }
        private bool IsLeaf { get; set; }
        private readonly TernarySearchTreeNode[] Children = new TernarySearchTreeNode[3];
        //Children[0]: Left / Children[1]: Center / Children[2]: Right

        public TernarySearchTreeNode(char c = (char)0) { C = c; }
        public void Add(string key)
        {
            if (C == 0 && key.Length > 0)
            {
                C = key[0];
            }
            var current = this;
            for (var index = 0; index < key.Length;)
            {
                var childIndex = current.ChildrenIndex(key[index]);
                if ((childIndex == 1) && (++index == key.Length))
                {
                    break;
                }
                if (current.Children[childIndex] == null)
                {
                    current.Children[childIndex] = new TernarySearchTreeNode(key[index]);
                }
                current = current.Children[childIndex];
            }
            current.IsLeaf = true;
        }
        public bool Contains(string key)
        {
            var node = GetNode(key);
            return node != null && node.IsLeaf;
        }
        public void Delete(string key) { DeleteHelper(key, 0); }
        public int NodeCount => 1 + Children.Sum(x => x == null ? 0 : x.NodeCount);
        public int WordCount => (IsLeaf ? 1 : 0) + Children.Sum(x => x == null ? 0 : x.WordCount);

        //return true if we can safely delete the node (it has no interesting info)
        private bool DeleteHelper(string key, int index)
        {
            var childIndex = ChildrenIndex(key[index]);
            if (childIndex == 1 && ++index >= key.Length) // Base case
            {
                IsLeaf = false; // Unmark leaf node
            }
            else // Recursive case
            {
                if (Children[childIndex].DeleteHelper(key, index))
                {
                    Children[childIndex] = null;
                }
            }
            return !IsLeaf && Children.All(x => x == null);
        }
        private TernarySearchTreeNode GetNode(string key)
        {
            var current = this;
            var index = 0;
            while ((current != null) && index < key.Length)
            {
                var childIndex = current.ChildrenIndex(key[index]);
                if ((childIndex == 1) && (++index == key.Length))
                {
                    return current; //found
                }
                current = current.Children[childIndex];
            }
            return null; //not found
        }
        private int ChildrenIndex(char c) { return c < C ? 0 : (c > C ? 2 : 1); }
    }
    #endregion

    #region TRIE Structure
    /// <summary>
    /// search/add/remove a word in a dictionary of 'n' words in o(W) time (W = length of the longest word in the dictionary)
    /// </summary>
    public class TrieNode
    {
        private const int FIRST_CHAR = 0; private const int LAST_CHAR = 255;
        //private const int FIRST_CHAR = 'a';private const int LAST_CHAR = 'z';
        private TrieNode[] Children;
        /// <summary>
        /// true if a word is ending at this node
        /// </summary>
        private bool IsLeaf { get; set; }

        private int GetChildrenIndex(int c) { return c - FIRST_CHAR; }
        private TrieNode GetChildren(int c) { return Children?[GetChildrenIndex(c)]; }
        public void Add(string word)
        {
            var current = this;
            foreach (var c in word)
            {
                var idx = GetChildrenIndex(c);
                if (current.Children == null)
                {
                    current.Children = new TrieNode[LAST_CHAR-FIRST_CHAR+1];
                }
                if (current.Children[idx] == null)
                {
                    current.Children[idx] = new TrieNode();
                }
                current = current.Children[idx];
            }
            current.IsLeaf = true;
        }
        public bool Contains(string word)
        {
            var node = GetNode(word);
            return node != null && node.IsLeaf;
        }
        public int NodeCount => 1 + (Children?.Sum(x => x?.NodeCount ?? 0) ?? 0);
        public int WordCount => (IsLeaf ? 1 : 0) + (Children?.Sum(x => x?.WordCount ?? 0) ?? 0);
        public void Delete(string word) { DeleteHelper(word, 0); }
        //return true if we can safely delete the node (it has no interesting info)
        private bool DeleteHelper(string word, int depth)
        {
            if (depth == word.Length) // Base case
            {
                IsLeaf = false; // Unmark leaf node
            }
            else // Recursive case
            {
                var index = GetChildrenIndex(word[depth]);
                if (Children[index].DeleteHelper(word, depth + 1))
                {
                    Children[index] = null;
                }
            }
            return !IsLeaf && (Children == null || Children.All(x => x == null));
        }

        public string FirstWord()
        {
            if (IsLeaf || Children == null)
            {
                return "";
            }
            for (int c = FIRST_CHAR; c <= LAST_CHAR; ++c)
            {
                if (GetChildren(c) != null)
                {
                    return ((char)c) + GetChildren(c).FirstWord();
                }
            }
            return "";
        }

        /// <summary>
        /// Find the nearest word in the dictionary from 'word' in o( log(W) * W) time
        /// </summary>
        /// <param name="word"></param>
        /// <param name="insertionCost"></param>
        /// <param name="deletionCost"></param>
        /// <param name="substitutionCost"></param>
        /// <param name="transpositionCost"></param>
        /// <returns>
        /// Item1 the nearest word
        /// Item the distance from this word
        /// </returns>
        public Tuple<string,int>  NearestWord(string word, int insertionCost = 1, int deletionCost = 1, int substitutionCost = 1, int transpositionCost = 1)
        {
            int minLength = 0;
            int maximumCost = word.Length * deletionCost + FirstWord().Length * insertionCost+1;
            int maxLength = maximumCost;
            while (minLength < maxLength)
            {
                var middle = (minLength + maxLength + 1) / 2;
                if (SearchWordAtMostAtDistance(this, maximumCost-middle, word, 0, new List<char>(),insertionCost, deletionCost, substitutionCost, transpositionCost))
                {
                    minLength = middle;
                }
                else
                {
                    maxLength = middle - 1;
                }
            }
            var result = new List<char>();
            var minimumCost = maximumCost - minLength;
            SearchWordAtMostAtDistance(this, minimumCost, word, 0, result,insertionCost, deletionCost, substitutionCost, transpositionCost);
            return Tuple.Create(new string(result.ToArray()), minimumCost);
        }
        private static bool SearchWordAtMostAtDistance(TrieNode T, int maxAllowedEditDistance, string word, int indexInWord, List<char> currentSolution, int insertionCost, int deletionCost, int substitutionCost, int transpositionCost)
        {
            if (T == null || maxAllowedEditDistance<0 || indexInWord > word.Length)
            {
                return false;
            }
            if (indexInWord == word.Length && T.IsLeaf)
            {
                return true;
            }

            //we remove the character at index 'i' (deletion)
            if (SearchWordAtMostAtDistance(T, maxAllowedEditDistance - deletionCost, word, indexInWord + 1, currentSolution, insertionCost, deletionCost, substitutionCost, transpositionCost))
            {
                return true;
            }
            if (T.Children == null)
            {
                return false;
            }
            //same character at index 'i'
            if (indexInWord < word.Length)
            {
                currentSolution.Add(word[indexInWord]);
                if (SearchWordAtMostAtDistance(T.GetChildren(word[indexInWord]), maxAllowedEditDistance, word, indexInWord + 1, currentSolution, insertionCost, deletionCost, substitutionCost, transpositionCost))
                {
                    return true;
                }
                currentSolution.RemoveAt(currentSolution.Count-1);
            }
            for (int c = FIRST_CHAR; c <= LAST_CHAR; ++c)
            {
                //we insert at index 'i' the character 'c' (insertion)
                currentSolution.Add((char)c);
                if (SearchWordAtMostAtDistance(T.GetChildren(c), maxAllowedEditDistance - insertionCost, word, indexInWord, currentSolution, insertionCost, deletionCost, substitutionCost, transpositionCost))
                {
                    return true;
                }
                //we set at index 'i' the character 'c' (substitution)
                if (SearchWordAtMostAtDistance(T.GetChildren(c), maxAllowedEditDistance - substitutionCost, word, indexInWord + 1, currentSolution, insertionCost, deletionCost, substitutionCost, transpositionCost))
                {
                    return true;
                }
                currentSolution.RemoveAt(currentSolution.Count - 1);
            }

            //transposition
            if (indexInWord <= (word.Length - 2))
            {
                currentSolution.Add(word[indexInWord+1]);
                currentSolution.Add(word[indexInWord]);
                if (SearchWordAtMostAtDistance(T.GetChildren(word[indexInWord + 1])?.GetChildren(word[indexInWord]),maxAllowedEditDistance - transpositionCost, word, indexInWord + 2, currentSolution, insertionCost,deletionCost, substitutionCost, transpositionCost))
                {
                    return true;
                }
                currentSolution.RemoveAt(currentSolution.Count - 1);
                currentSolution.RemoveAt(currentSolution.Count - 1);
            }
            return false;
        }


        private TrieNode GetNode(string wordPrefix)
        {
            var current = this;
            foreach (var c in wordPrefix)
            {
                var child = current.Children?[c - FIRST_CHAR];
                if (child == null)
                {
                    return null;
                }
                current = child;
            }
            return current;
        }
    }
    #endregion

    public static partial class Utils
    {
        /// <summary>
        /// Given a special encoding for each char in the dictionary,
        /// for each encoding string:
        ///     return all word in dictionary starting with this encoding and with the most weight
        /// </summary>
        /// <param name="encoding">mapping word dictionary char => encoding char</param>
        /// <param name="weights">mapping word => associated weight</param>
        /// <returns>
        /// Keys : encoded strings
        /// Values : all words starting with this encoding and with the most weights
        /// </returns>
        public static IDictionary<string, List<string>> EncodedString_to_MostWeightedWordsInDico(
            IDictionary<char, char> encoding,
            IDictionary<string, int> weights)
        {
            var prefix_to_MaxHeight = new Dictionary<string, int>();
            var result = new Dictionary<string, List<string>>();

            foreach (var wordAndWeight in weights)
            {
                var word = wordAndWeight.Key;
                var wordWeight = wordAndWeight.Value;
                for (int l = 1; l <= word.Length; ++l)
                {
                    var prefix = word.Substring(0, l);
                    int currentWeight;
                    if (!prefix_to_MaxHeight.TryGetValue(prefix, out currentWeight) || wordWeight>currentWeight)
                    {
                        prefix_to_MaxHeight[prefix] = wordWeight;
                        result[EncodeWord(prefix, encoding)] = new List<string> { word };
                    }
                    else if (currentWeight == wordWeight)
                    {
                        result[EncodeWord(prefix, encoding)].Add(word);
                    }
                }
            }
            return result;
        }
        public static string EncodeWord(string word, IDictionary<char, char> encoding)
        {
            return new string(word.Select(x => encoding[x]).ToArray());
        }

    }
}