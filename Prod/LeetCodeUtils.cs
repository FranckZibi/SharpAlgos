using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SharpAlgos
{
    public static class LeetCodeUtils
    {
        public static int[] TwoSum(int[] nums, int target)
        {
            if ((nums == null) || (nums.Length < 2))
                return new int[] {};

            var dicoToLastIndex = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; ++i)
                dicoToLastIndex[nums[i]] = i;
            for (int i = 0; i < nums.Length; ++i)
            {
                var missing = target - nums[i];
                int indexFound;
                if (dicoToLastIndex.TryGetValue(missing, out indexFound) && (indexFound != i))
                    return new[] {i, indexFound};
            }
            return new int[] {};
        }

        public static int LengthOfLongestSubstring(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            int currentMax = 0;
            int start = 0;
            int[] previousIndexMaxContent = new int[256];
            //for (int i = 0; i < previousIndexMaxContent.Length; ++i) previousIndexMaxContent[i] = -1;
            for (int end = 0; end < s.Length; ++end)
            {
                int currentChar = s[end];
                int previousIndex = previousIndexMaxContent[currentChar] - 1;
                if (previousIndex < start)
                    currentMax = Math.Max(currentMax, end - start + 1);
                else
                    start = previousIndex + 1;
                previousIndexMaxContent[currentChar] = end + 1;
            }
            return currentMax;
        }

       /*
        public class TreeNode
        {
            public int val;
            public TreeNode left;
            public TreeNode right;
            public TreeNode(int x) { val = x; }
        }
        */


        public static bool IsUgly(int num)
        {
            if (num <= 0) return false;
            while ((num%5) == 0)
                num /= 5;
            while ((num%3) == 0)
                num /= 3;
            while ((num%2) == 0)
                num /= 2;
            return num == 1;
        }

        public static int NthUglyNumber(int n)
        {
            int[] primes = {2, 3, 5};
            var allUglies = new List<int> {1};
            int[] prevIndex = new int[primes.Length];
            for (int i = 1; i < n; ++i)
            {
                int minNext = int.MaxValue;
                for (int primeIndex = 0; primeIndex < primes.Length; ++primeIndex)
                    minNext = Math.Min(minNext, allUglies[prevIndex[primeIndex]] * primes[primeIndex]);
                allUglies.Add(minNext);
                for (int iPrime = 0; iPrime < primes.Length; ++iPrime)
                    if (minNext == allUglies[prevIndex[iPrime]] * primes[iPrime])
                        ++prevIndex[iPrime];
            }
            return allUglies.Last();
        }

        public static int LastRemaining(int n)
        {
            if (n <= 1) return n;
            return n/2 + 1 + 2*LastRemaining(n/2);
        }
      
        public static List<int> AllMaximumBorderLength(string w)
        {
            var L = Enumerable.Repeat(0, 1 + w.Length).ToList();
            for (int l = 1; l < w.Length; ++l)
            {
                int k = L[l];
                while ((w[k] != w[l]) && (k > 0))
                    k = L[k];
                L[l + 1] = (w[k] == w[l]) ? (k + 1) : 0;
            }
            return L;
        }

        public static int FindNumberOfLIS(int[] nums)
        {
            int[] maxLength = new int[nums.Length];
            int[] counts = new int[nums.Length];

            maxLength[0] = counts[0] = 1;

            for (int i = 1; i < nums.Length; ++i)
            {
                maxLength[i] = 1;
                for (int j = 0; j < i; ++j)
                    if (nums[i] > nums[j])
                        maxLength[i] = Math.Max(maxLength[i], 1 + maxLength[j]);

                for (int j = 0; j < i; ++j)
                    if ((nums[i] > nums[j])&&(maxLength[i] ==  1 + maxLength[j]))
                        ++counts[i];
                counts[i] = Math.Max(counts[i], 1);
            }
            int maxLength0 = maxLength.Max();
            int result = 0;
            for (int i = 0; i < maxLength.Length; ++i)
                if (maxLength[i] == maxLength0)
                    result += counts[i];
            return result;
        }

        public static int DetectWordMinimumPeriodLength(string t)
        {
            if (t.Length <= 1) return t.Length;
            var maxBorderLength = AllMaximumBorderLength(t).Last();
            int remaining = t.Length - maxBorderLength;
            if (t.Length%remaining == 0)
                return remaining;
            return t.Length;
        }

        public static int ClimbStairs(int n)
        {
            if (n <= 0)
                return 0;
            if (n == 1)
                return 1;
            if (n == 2)
                return 2;
            int prevPrev = 1;
            int prev = 2;
            for (int i = 3; i <= n; ++i)
            {
                int tmp = prev + prevPrev;
                prevPrev = prev;
                prev = tmp;
            }
            return prev;
        }

        public static int HammingWeight(uint n)
        {
            int result = 0;
            while (n != 0)
            {
                if ((n & 1) != 0)
                    ++result;
                n = n/2;
            }
            return result;
        }

        public static int RomanToInt(string num)
        {
            var data = new Dictionary<char, int> {{ 'I', 1}, { 'V',5}, { 'X',10}, { 'L',50}, {'C',100}, {'D',500}, { 'M',1000 }};
            var numbers = num.Select(x => data[x]).ToList();
            for (int i = 1; i < numbers.Count; ++i)
            {
                if (numbers[i - 1] < numbers[i])
                {
                    numbers[i - 1] = numbers[i] - numbers[i - 1];
                    numbers.RemoveAt(i--);
                }
            }
            return numbers.Sum();
        }

        public static string IntToRoman(int num)
        {
            var result = "";
            int multiplier = 1;
            while (num != 0)
            {
                result = IntToRomanHelper(multiplier*(num%10)) + result;
                num /= 10;
                multiplier *= 10;
            }
            return result;
        }

        private static string IntToRomanHelper(int num)
        {
            var data = new[] {'I', 'V', 'X', 'L', 'C', 'D', 'M'}; //1 5 10 50 100 500 1000
            int idx = 0;
            while (num >= 10)
            {
                num /= 10;
                idx += 2;
            }
            if (num <= 3) return new string(data[idx], num);
            if (num <= 5) return new string(data[idx], 5 - num) + data[idx + 1];
            if (num <= 8) return data[idx + 1] + new string(data[idx], num - 5);
            return data[idx] + data[idx+2].ToString();
        }

        public static IList<int> GrayCode(int n)
        {
            List<int> result = new List<int>();
            result.Add(0);

            for (int i = 0; i < n; ++i)
            {
                int add = 1 << i;
                for (int j = result.Count - 1; j >= 0; --j)
                {
                    result.Add(result[j] + add);
                }
            }
            return result;
        }

        public static IList<IList<int>> FindSubsequences(int[] nums)
        {
            var allSubsequencesEndingAt0 = new List<IList<int>>[nums.Length];
            for (int i = 0; i < allSubsequencesEndingAt0.Length; ++i)
                allSubsequencesEndingAt0[i] = new List<IList<int>>();

            int[] maxLengthEndingAt = new int[nums.Length];
            maxLengthEndingAt[0] = 1;

            for (int i = 0; i < nums.Length; ++i)
            {
                allSubsequencesEndingAt0[i].Add(new List<int> {nums[i]});
                int prevMaxLength = 0;
                for (int j = 0; j < i; ++j)
                {
                    if (nums[j] <= nums[i])
                    {
                        prevMaxLength = Math.Max(prevMaxLength, maxLengthEndingAt[j]);
                        foreach (var l in allSubsequencesEndingAt0[j])
                        {
                            var t = new List<int>(l);
                            t.Add(nums[i]);
                            allSubsequencesEndingAt0[i].Add(t);
                        }
                    }
                }
                maxLengthEndingAt[i] = prevMaxLength;
            }

            var result = allSubsequencesEndingAt0.SelectMany(x => x).ToList();
            result.RemoveAll(x => x.Count <= 1);
            result.Sort((x, y) => x.Count - y.Count);
            for (int i = 0; i < result.Count; ++i)
                for (int j = i + 1; j < result.Count; ++j)
                {
                    if (result[j].Count != result[i].Count)
                        break;
                    if (result[i].SequenceEqual(result[j]))
                        result.RemoveAt(j--);
                }
            return result;
        }

        public static IList<string> FindWords(char[,] board, string[] words)
        {
            var hashWords = new HashSet<string>(words);
            var allSubWords = new HashSet<string>();
            foreach (var w in words)
                for (int l = 1; l <= w.Length; ++l)
                    allSubWords.Add(w.Substring(0, l));
            var result = new List<string>();
            foreach (var p in Utils.AllPoints(board))
                FindWordsHelper(p, board, hashWords, allSubWords, "", result);
            return new HashSet<string>(result).ToList();
        }

        public static void FindWordsHelper(Point p, char[,] board, HashSet<string> words, HashSet<string> allSubWords, string subWord, IList<string> result)
        {
            var backup = board[p.X,p.Y];
            if (backup == '#') return;
            subWord += backup;
            if (!allSubWords.Contains(subWord))
                return;
            if (words.Contains(subWord))
                result.Add(subWord);
            board[p.X,p.Y] = '#';
            foreach (var around in Utils.AllPointsHorizontalVertical(board, p.X, p.Y))
                FindWordsHelper(around, board, words, allSubWords, subWord, result);
            board[p.X,p.Y] = backup;
        }

        public static int TotalHammingDistance(int[] nums)
        {
            int result = 0;
            for (;;)
            {
                int nbOnes = 0;
                bool hasNonZero = false;
                for (int i = 0; i < nums.Length; ++i)
                {
                    if (nums[i] != 0)
                    {
                        hasNonZero = true;
                        if ((nums[i] & 1) != 0)
                            ++nbOnes;
                        nums[i] /= 2;
                    }
                }
                if (!hasNonZero)
                    return result;
                result += nbOnes*(nums.Length - nbOnes);
            }

        }

        public class TreeNode
        {

            public TreeNode left;
            public TreeNode right;
            public readonly int val;

            public TreeNode(int val)
            {
                this.val = val;
            }
        }

        public static string[] CreateHuffmanCode(int[] frequencies)
        {
            var frequenciesToIndexes = new Dictionary<int, List<int>>();
            for (int i = 0; i < frequencies.Length; ++i)
            {
                if (!frequenciesToIndexes.ContainsKey(frequencies[i]))
                    frequenciesToIndexes.Add(frequencies[i], new List<int>());
                frequenciesToIndexes[frequencies[i]].Add(i);
            }

            var p = new PriorityQueue<TreeNode>(true);

            foreach (var f in frequencies)
                p.Enqueue(new TreeNode(f), f);

            while (p.Count >= 2)
            {
                TreeNode p1 = p.Dequeue();
                TreeNode p2 = p.Dequeue();
                TreeNode p3 = new TreeNode(p1.val + p2.val);
                p3.left = p1;
                p3.right = p2;
                p.Enqueue(p3, p3.val);
            }

            var frequencyWithCode = new List<KeyValuePair<int, string>>();
            CreateHuffmanCode_Helper(p.Dequeue(), "", frequencyWithCode);
            string[] result = new string[frequencies.Length];
            foreach (var freq in frequencyWithCode)
            {
                var indexes = frequenciesToIndexes[freq.Key];
                result[indexes.Last()] = freq.Value;
                indexes.RemoveAt(indexes.Count - 1);
            }
            return result;
        }

        private static void CreateHuffmanCode_Helper(TreeNode root, string currentPrefix, List<KeyValuePair<int, string>> frequencyWithCode)
        {
            if ((root.left == null) && (root.right == null))
            {
                frequencyWithCode.Add(new KeyValuePair<int, string>(root.val, currentPrefix));
                return;
            }
            if (root.right != null)
                CreateHuffmanCode_Helper(root.right, currentPrefix + "1", frequencyWithCode);
            if (root.left != null)
                CreateHuffmanCode_Helper(root.left, currentPrefix + "0", frequencyWithCode);
        }
    }
}
