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

        public static void SetZeroes(int[,] matrix)
        {
            int W = matrix.GetLength(0);
            int H = matrix.GetLength(1);

            bool[] zeroX = new bool[W];
            bool[] zeroY = new bool[H];
            for (int x = 0; x < W; ++x)
                for (int y = 0; y < H; ++y)
                    if (matrix[x, y] == 0)
                    {
                        zeroX[x] = true;
                        zeroY[y] = true;
                    }

            for (int x = 0; x < W; ++x)
            {
                if (!zeroX[x]) continue;
                for (int y = 0; y < H; ++y)
                    matrix[x, y] = 0;
            }
            for (int y = 0; y < H; ++y)
            {
                if (!zeroY[y]) continue;
                for (int x = 0; x < W; ++x)
                    matrix[x, y] = 0;
            }
        }




     



     

        public static int FirstUniqChar(string s)
        {
            if (string.IsNullOrEmpty(s)) return -1;
            var dico = new Dictionary<char, int>();
            for (int i = 0; i < s.Length; ++i)
            {
                if (dico.ContainsKey(s[i]))
                    dico[s[i]] = int.MaxValue;
                else
                    dico[s[i]] = i;
            }

            int minValue = dico.Values.Min();
            if (minValue == int.MaxValue)
                return -1;
            return minValue;

        }

        public static int LongestPalindrome(string s)
        {
            var count = new int[256];
            foreach (var c in s)
                ++count[c];
            int result = 0;
            foreach (var c in count)
                result += 2*(c/2);
            if (result < s.Length)
                ++result;
            return result;
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

        
        public static double KnightProbability(int N, int K, int r, int c)
        {
            var moves = new List<int[]>
                        {
                            new[] {1, 2},
                            new[] {2, 1},
                            new[] {1, -2},
                            new[] {2, -1},
                            new[] {-1, 2},
                            new[] {-2, 1},
                            new[] {-1, -2},
                            new[] {-2, -1}
                        };
            double totalOk = KnightProbabilityCount(N, K, r, c, moves, Enumerable.Repeat(-1.0, 1 + N*N*100).ToArray());
            double total = Math.Pow(8, K);
            return totalOk/total;
        }

        public static double KnightProbabilityCount(int N, int K, int r, int c, List<int[]> moves, double[] cache)
        {
            if (Math.Min(r, c) < 0) return 0;
            if (Math.Max(r, c) >= N) return 0;
            if (K < 0) return 0;
            if (K == 0) return 1;
            var key = (N*r + c)*100 + (K - 1);
            if (cache[key] >= 0)
                return cache[key];
            double result = 0;
            foreach (var m in moves)
                result += KnightProbabilityCount(N, K - 1, r + m[0], c + m[1], moves, cache);
            cache[key] = result;
            return result;

        }


        public static bool JudgeCircle(string moves)
        {
            if ((moves.Length%2) != 0)
                return false;
            var count = new int[128];
            foreach (var m in moves)
                ++count[m];
            return (count['U'] == count['D']) && (count['L'] == count['R']);

        }


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

        public static char FindTheDifference(string s, string t)
        {
            var sDico = s.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            var tDico = t.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            foreach (var e in tDico)
            {
                if (!sDico.ContainsKey(e.Key))
                    return e.Key;
                if (e.Value != sDico[e.Key])
                    return e.Key;
            }
            return 'A';
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


        public static IList<string> LetterCombinations(string digits)
        {
            var tmp = digits.Select(x => phoneDigitsToCharDico[x - '0'].ToList()).ToList();
            return Utils.AllCombinations(tmp).Select(x => string.Join("", x.ToArray())).ToList();
        }
        private static readonly IDictionary<int, IEnumerable<string>> phoneDigitsToCharDico =
            new Dictionary<int, IEnumerable<string>>
            {
                {1, new[] {""}},
                {2, new[] {"a", "b", "c"}},
                {3, new[] {"d", "e", "f"}},
                {4, new[] {"g", "h", "i"}},
                {5, new[] {"j", "k", "l"}},
                {6, new[] {"m", "n", "o"}},
                {7, new[] {"p", "q", "r", "s"}},
                {8, new[] {"t", "u", "v"}},
                {9, new[] {"w", "x", "y", "z"}}
            };



        public static string LongestCommonPrefix(string[] strs)
        {
            if ((strs == null) || (strs.Length == 0))
                return "";
            var currentResult = strs[0];
            for (int i = 1; i < strs.Length; ++i)
            {
                int endIndex = Math.Min(currentResult.Length - 1, strs[i].Length - 1);
                int commonPrefixLength = 0;
                for (int j = 0; j <= endIndex; ++j)
                {
                    if (strs[i][j] != currentResult[j])
                        break;
                    commonPrefixLength = j + 1;
                }
                if (commonPrefixLength <= 0)
                    return "";
                currentResult = currentResult.Substring(0, commonPrefixLength);
            }
            return currentResult;
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

        public static string AddBinary(string a, string b)
        {

            List<int> a1 = a.ToCharArray().Reverse().Select(x => x - '0').ToList();
            List<int> b1 = b.ToCharArray().Reverse().Select(x => x - '0').ToList();

            int retenu = 0;
            List<int> sum = new List<int>();
            for (int i = 0; i < Math.Max(a1.Count, b1.Count); ++i)
            {
                int res = (i >= a1.Count ? 0 : a1[i]) + (i >= b1.Count ? 0 : b1[i]) + retenu;
                sum.Add(res%2);
                retenu = res/2;
            }
            if (retenu != 0)
                sum.Add(retenu);

            return string.Join("", sum.Select(x => x.ToString()).Reverse().ToArray());
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


        public static int UniquePaths(int m, int n)
        {
            var m_2_N_2_result = new Dictionary<int, IDictionary<int, int>>();

            for (int i = 0; i <= m; ++i)
                m_2_N_2_result[i] = new Dictionary<int, int>();
            return UniquePaths_Helper(m, n, m_2_N_2_result);

        }

        private static int UniquePaths_Helper(int m, int n, IDictionary<int, IDictionary<int, int>> m_2_N_2_result)
        {
            if ((m <= 0) || (n <= 0))
                return 0;
            if (m == 1)
                return 1;
            if (n == 1)
                return 1;
            int result;
            if (m_2_N_2_result[m].TryGetValue(n, out result))
                return result;
            result = UniquePaths_Helper(m - 1, n, m_2_N_2_result) + UniquePaths_Helper(m, n - 1, m_2_N_2_result);
            m_2_N_2_result[m][n] = result;
            return result;
        }

        public static IList<IList<int>> Combine(int n, int k)
        {
            var result = new List<IList<int>>();
            AllCombinationsHelper(n, k, new List<int>(), 1, result);
            return result;
        }

        private static void AllCombinationsHelper(int n, int k, List<int> currentSolutionInProgress, int firsAllowedInt, List<IList<int>> allSolutionSoFar)
        {
            if (currentSolutionInProgress.Count == k)
            {
                allSolutionSoFar.Add(currentSolutionInProgress); // a new combination has been found
                return;
            }

            int nbMissing = k - currentSolutionInProgress.Count;
            for (int number = firsAllowedInt; number <= (n - nbMissing + 1); number++)
                AllCombinationsHelper(n, k, new List<int>(currentSolutionInProgress) {number}, number + 1,
                    allSolutionSoFar);
        }



        public static int FindPeakElement(int[] nums)
        {
            for (int i = 0; i < nums.Length; ++i)
            {
                int n = nums[i];
                bool prev = i == 0 || (n > nums[i - 1]);
                bool next = i == (nums.Length - 1) || (n > nums[i + 1]);
                if (prev && next)
                    return i;
            }
            return -1;
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

        public static string IntToRomanHelper(int num)
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


        public static int NumDecodings(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            int[] scores = new int[1 + s.Length];
            scores[0] = 1;
            scores[1] = s[0] == '0' ? 0 : 1;
            for (int i = 1; i < s.Length; ++i)
            {
                int prev = 10*(s[i - 1] - '0') + s[i] - '0';
                if ((prev >= 10) && (prev <= 26))
                    scores[i + 1] += scores[i - 1];
                if (s[i] != '0')
                    scores[i + 1] += scores[i];
            }
            return scores.Last();
        }

        public static IList<IList<int>> CombinationSum(int[] candidates, int target)
        {
            IList<IList<int>> result = new List<IList<int>>();
            List<int> currentList = new List<int>();
            CombinationSum_Helper(candidates, 0, target, result, currentList);
            return result;
        }

        private static void CombinationSum_Helper(int[] candidates, int firstAllowedIndex, int remaining, IList<IList<int>> result, List<int> currentList)
        {
            if (remaining < 0)
                return;
            if (remaining == 0)
            {
                result.Add(currentList);
                return;
            }
            if (firstAllowedIndex >= candidates.Length)
                return;
            var tmpList = new List<int>(currentList);
            CombinationSum_Helper(candidates, firstAllowedIndex + 1, remaining, result, tmpList);
            for (int count = 1; true; ++count)
            {
                int nextRemaining = remaining - count*candidates[firstAllowedIndex];
                if (nextRemaining < 0)
                    return;
                tmpList.Add(candidates[firstAllowedIndex]);
                CombinationSum_Helper(candidates, firstAllowedIndex + 1, nextRemaining, result, tmpList);
            }


        }

        public static int EvalRPN(string[] tokens)
        {
            var t = new List<string>(tokens);
            int i = 2;
            while (t.Count != 1)
            {
                var s = t[i];
                if (!"+*/-".Contains(s))
                {
                    ++i;
                    continue;
                }
                int res = 0;
                if (s == "+")
                    res = (int.Parse(t[i - 2]) + int.Parse(t[i - 1]));
                else if (s == "-")
                    res = (int.Parse(t[i - 2]) - int.Parse(t[i - 1]));
                else if (s == "/")
                    res = (int.Parse(t[i - 2])/int.Parse(t[i - 1]));
                else
                    res = (int.Parse(t[i - 2])*int.Parse(t[i - 1]));
                t[i - 2] = res.ToString();
                t.RemoveAt(i - 1);
                t.RemoveAt(i - 1);
                --i;
            }

            return int.Parse(t.First());
        }

        public static IList<string> RestoreIpAddresses(string s)
        {
            List<string> current = new List<string>();
            List<string> result = new List<string>();
            RestoreIpAddressesHelper(s, 0, current, result);
            return result;
        }

        public static void RestoreIpAddressesHelper(string s, int idx, List<string> current, List<string> result)
        {
            if (idx > s.Length)
                return;
            if (idx == s.Length)
            {
                if (current.Count == 4)
                {
                    result.Add(string.Join(".", current));
                }
                return;
            }

            int nbMissing = 4 - current.Count;
            int max = 3*nbMissing;
            int min = 1*nbMissing;
            int remaining = s.Length - idx;
            if (remaining > max) return;
            if (remaining < min) return;

            for (int l = 1; l <= 3; ++l)
            {
                if ((idx + l) > s.Length)
                    break;
                if ((s[idx] == '0') && (l >= 2))
                    break;
                var part = s.Substring(idx, l);
                if (int.Parse(part) <= 255)
                {
                    current.Add(part);
                    RestoreIpAddressesHelper(s, idx + l, current, result);
                    current.RemoveAt(current.Count - 1);
                }

            }
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
            var allSubsequecnesEndingAt0 = new List<IList<int>>[nums.Length];
            for (int i = 0; i < allSubsequecnesEndingAt0.Length; ++i)
                allSubsequecnesEndingAt0[i] = new List<IList<int>>();

            int[] maxLengthEndingAt = new int[nums.Length];
            maxLengthEndingAt[0] = 1;

            for (int i = 0; i < nums.Length; ++i)
            {
                allSubsequecnesEndingAt0[i].Add(new List<int> {nums[i]});
                int prevMaxLength = 0;
                for (int j = 0; j < i; ++j)
                {
                    if (nums[j] <= nums[i])
                    {
                        prevMaxLength = Math.Max(prevMaxLength, maxLengthEndingAt[j]);
                        foreach (var l in allSubsequecnesEndingAt0[j])
                        {
                            var t = new List<int>(l);
                            t.Add(nums[i]);
                            allSubsequecnesEndingAt0[i].Add(t);
                        }
                    }
                }
                maxLengthEndingAt[i] = prevMaxLength;
            }

            var result = allSubsequecnesEndingAt0.SelectMany(x => x).ToList();
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
 




        /*
        public class Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                var other = (Point) obj;
                return X == other.X && Y == other.Y;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (X*397) ^ Y;
                }
            }
        }



    */
        public static  string LongestWord(string[] words)
        {
            var cache = new Dictionary<string, bool>();
            Array.Sort(words, (x,y)=> (x.Length!=y.Length)?y.Length.CompareTo(x.Length):string.Compare(x, y, StringComparison.Ordinal)  );
            var set = new HashSet<string>(words);

            foreach (var w in words)
            {
                if (w.Length <= 1)
                    return "";
                if (IsOk(w, set, cache))
                    return w;
            }
            return "";
        }

        private static bool IsOk(string word, HashSet<string> allWords, Dictionary<string, bool> cache)
        {
            if (cache.ContainsKey(word))
                return cache[word];
            if (!allWords.Contains(word))
            {
                cache[word] = false;
                return false;
            }
            if (word.Length <= 1)
            {
                cache[word] = true;
                return true;
            }
            return IsOk(word.Substring(0, word.Length - 1), allWords, cache);
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

            PriorityQueue<TreeNode> p = new PriorityQueue<TreeNode>(true);

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


        public static bool IsIsomorphic(string s, string t)
        {
            if (s.Length != t.Length)
                return false;

            IDictionary<char, char> dicoS2T = new Dictionary<char, char>();
            IDictionary<char, char> dicoT2S = new Dictionary<char, char>();
            var sChar = s.ToCharArray();
            var tChar = t.ToCharArray();
            for (int i = 0; i < sChar.Length; ++i)
            {
                if ((!dicoS2T.ContainsKey(sChar[i])))
                {

                    if (dicoT2S.ContainsKey(tChar[i]))
                        return false;
                    dicoS2T[sChar[i]] = tChar[i];
                    dicoT2S[tChar[i]] = sChar[i];
                    continue;
                }
                if (dicoS2T[sChar[i]] != tChar[i])
                    return false;
            }
            return true;
        }


        public static int HammingDistance(int a, int b)
        {
            int result = 0;
            while (a != b)
            {
                if ((a & 1) != (b & 1))
                    ++result;
                a /= 2;
                b /= 2;
            }
            return result;
        }


        //number of ways to place K Bishops on a NxN board
        // https://cp-algorithms.com/combinatorics/bishops-on-chessboard.html

        public static IList<IList<string>> SolveNQueens(int n)
        {
            var result = new List<IList<string>>();
            var visited = new bool[n];
            SolveNQueens_Helper(n, visited, new List<int>(), result);
            return result;
        }

        private static void SolveNQueens_Helper(int n, bool[] visited, List<int> path, List<IList<string>> result)
        {
            if (path.Count >= n)
            {
                var solution = new List<string>();
                foreach (var p in path)
                {
                    var line = Enumerable.Repeat('.', n).ToArray();
                    line[p] = 'Q';
                    solution.Add(new string(line));
                }
                result.Add(solution);
            }

            for (int i = 0; i < n; ++i)
            {
                if (visited[i])
                    continue;
                if (!SolveNQueens_Helper_IsValid(path, i))
                    continue;
                visited[i] = true;
                path.Add(i);
                SolveNQueens_Helper(n, visited, path, result);
                visited[i] = false;
                path.RemoveAt(path.Count - 1);
            }

        }

        private static bool SolveNQueens_Helper_IsValid(List<int> path, int newColumn)
        {
            for (int i = 0; i < path.Count; ++i)
                if (Math.Abs(path[i] - newColumn) == (path.Count - i))
                    return false;
            return true;
        }



       

        public static  IList<int> SelfDividingNumbers(int left, int right)
        {
            return Enumerable.Range(left, right - left + 1).Where(SelfDividingNumbersHelper).ToList();
        }

        public static bool SelfDividingNumbersHelper(int i)
        {
            if (i == 0)
                return false;
            int j = i;
            while (j != 0)
            {
                int nb = j%10;
                if (nb == 0)
                    return false;
                if ((i%nb) != 0)
                    return false;
                j /= 10;
            }
            return true;
        }



        public static int CutOffTree(IList<IList<int>> forest)
        {
            int[][] tmp = forest.Select(x=>x.ToArray()).ToArray();
            var forrest = new List<KeyValuePair<int, Point>>();
            for (int x = 0; x < tmp.Length; ++x)
                for (int y = 0; y < tmp[x].Length; ++y)
                {
                    if (tmp[x][y] > 1)
                        forrest.Add(new KeyValuePair<int,Point>(tmp[x][y], new Point(x,y)));
                }

            forrest.Sort((x,y)=>x.Key.CompareTo(y.Key));
            var allPoints = forrest.Select(x => x.Value).ToList();
            if (!allPoints.First().Equals(new Point(0,0)))
                allPoints.Insert(0, new Point(0,0));

            int result = 0;
            for(int i=1;i<allPoints.Count;++i)
            {
                var start = allPoints[i - 1];
                var to = allPoints[i];
                var pathLength = AllReachableFrom(start, to, tmp);
                if (pathLength == -1)
                    return -1;
                result += pathLength;
            }
            return result;
        }

   

        private static readonly List<int[]>  delta = new List<int[]> {new [] {1,0}, new[] { -1, 0 }, new[] { 0, 1 }, new[] { 0, -1 }};

        private static int AllReachableFrom(Point start, Point to, int[][] tmp)
        {
            int W = tmp.Length;
            int H = tmp[0].Length;
            var depth = new int[W, H];

            var toProcessX = new Queue<int>();
            var toProcessY = new Queue<int>();
            toProcessX.Enqueue(start.X);
            toProcessY.Enqueue(start.Y);
            while (toProcessX.Count != 0)
            {
                var currentX = toProcessX.Dequeue();
                var currentY = toProcessY.Dequeue();
                foreach (var d in delta)
                {
                    int newX = currentX + d[0];
                    int newY = currentY + d[1];
                    if ((newX == to.X)&&(newY ==to.Y))
                        return depth[currentX,currentY]+1;
                    if ((newX < 0) || (newX >= W) || (newY < 0) || (newY >= H) ||tmp[newX][newY]==0 || (depth[newX,newY]!= 0) )
                        continue;
                    depth[newX,newY] = depth[currentX,currentY] + 1;
                    toProcessX.Enqueue(newX);
                    toProcessY.Enqueue(newY);
                }
            }
            return -1;
        }

        public static int FindNthDigit(int n)
        {
            long min = 1;
            long max = 9;
            long digitCountBeforeMin = 0;
            long digitCountUpToMax = 9;
            long nbDigits = 1;
            for (;;)
            {

                long nbDigitsInRange = nbDigits*(max - min + 1);
                if (n <= (digitCountBeforeMin + nbDigitsInRange))
                    break;
                min *= 10;
                max = 10*max + 9;
                ++nbDigits;
                digitCountBeforeMin = digitCountUpToMax;
                digitCountUpToMax += (max - min + 1)*nbDigits;
            }
            long number = min + (n - digitCountBeforeMin - 1)/nbDigits;
            long idx = (n - digitCountBeforeMin - 1)%nbDigits;
            return int.Parse(number.ToString()[(int) idx] + "");
        }

        public static IList<int> LexicalOrder(int n)
        {
            var result = new List<int>();
            for (int i = 1; i <= 9; ++i)
                LexicalOrder_Dfs(i, n, result);
            return result;
        }

        private static void LexicalOrder_Dfs(int a, int n, List<int> result)
        {
            if (a > n)
                return;
            result.Add(a);
            for (int subDigit = 0; subDigit < 10; ++subDigit)
                LexicalOrder_Dfs(10*a+subDigit, n, result);
        }



        public static string ConvertToBase(int num, int baseValue)
        {
            bool isNegative = num < 0;
            num = Math.Abs(num);
            string result = "";
            do
            {
                var newDigitToAddAtLeft = (num% baseValue).ToString();
                result = newDigitToAddAtLeft + result;
                num /= baseValue;
            }
            while (num != 0);
            if (isNegative)
                result = "-" + result;
            return result;
        }


        public static char[,] UpdateBoardMinesweeper(char[,] board, Point start)
        {
            var visited = new HashSet<Point> { start };
            var toProcess = new Queue<Point>();
            toProcess.Enqueue(start);
            while (toProcess.Count != 0)
            {
                var current = toProcess.Dequeue();
                var valueCurrent = board[current.X,current.Y];
                if ((valueCurrent == 'M') || (valueCurrent == 'X'))
                {
                    board[current.X,current.Y] = 'X';
                    break;
                }
                var around = Utils.AllPointsAround(board, current,1).Select(x => board[x.X,x.Y]).ToList();
                int nbMines = around.Count(x=>(x=='X')||(x=='M'));
                if (nbMines == 0)
                {
                    board[current.X,current.Y] = 'B';
                    foreach (var child in Utils.AllPointsAround(board, current, 1))
                        if (!visited.Contains(child))
                        {
                            toProcess.Enqueue(child);
                            visited.Add(child);
                        }
                }
                else
                    board[current.X,current.Y] = (char)('0' + nbMines);
            }
            return board;
        }





        public static int FindMinArrowShots(int[,] points)
        {
            var data =  new List<KeyValuePair<int, int>>();
            for (int i = 0; i < points.GetLength(0); ++i)
                data.Add(new KeyValuePair<int, int>(points[1,0], points[1, 1]));
            data.Sort((x,y)=>x.Value.CompareTo(y.Value));
            int indexNextShoot = 0;
            int nbShoots = 0;

            for (;;)
            {
                if (indexNextShoot >= data.Count)
                    break;
                int shootPos = data[indexNextShoot].Value;
                ++nbShoots;
                while ((indexNextShoot < data.Count)&&data[indexNextShoot].Key<=shootPos)
                    ++indexNextShoot;
            }
            return nbShoots;
        }
        /*
                public class Point
                {
                    public int X { get; }
                    public int Y { get; }

                    public Point(int x, int y)
                    {
                        X = x;
                        Y = y;
                    }

                    public override bool Equals(object obj)
                    {
                        if (ReferenceEquals(null, obj)) return false;
                        if (ReferenceEquals(this, obj)) return true;
                        if (obj.GetType() != GetType()) return false;
                        var other = (Point)obj;
                        return X == other.X && Y == other.Y;
                    }

                    public override int GetHashCode()
                    {
                        unchecked
                        {
                            return (X * 397) ^ Y;
                        }
                    }
                }
                */

    }




}

