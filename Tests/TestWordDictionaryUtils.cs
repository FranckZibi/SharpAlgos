using System;
using System.Collections.Generic;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {
        [Test]
        public void TestTrieNode()
        {
            var root = new TrieNode();
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(0, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            root.Add("");
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(1, root.WordCount);
            Assert.IsTrue(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            root.Add("ab");
            Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(2, root.WordCount);
            Assert.IsTrue(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            Assert.IsFalse(root.Contains("ba"));
            Assert.IsTrue(root.Contains("ab"));
            root.Add("ac");
            Assert.AreEqual(4, root.NodeCount);
            Assert.AreEqual(3, root.WordCount);
            Assert.IsTrue(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            Assert.IsTrue(root.Contains("ab"));
            Assert.IsTrue(root.Contains("ac"));
            root.Delete("ab");
            Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(2, root.WordCount);
            Assert.IsTrue(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            Assert.IsFalse(root.Contains("ab"));
            Assert.IsTrue(root.Contains("ac"));
            root.Delete("ac");
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(1, root.WordCount);
            root.Delete("");
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(0, root.WordCount);
        }

        [Test]
        public void TestTernarySearchTreeNode()
        {
            var root = new TernarySearchTreeNode();
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(0, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            root.Add("a");
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(1, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsTrue(root.Contains("a"));
            root.Add("ab");
            Assert.AreEqual(2, root.NodeCount);
            Assert.AreEqual(2, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsTrue(root.Contains("a"));
            Assert.IsFalse(root.Contains("ba"));
            Assert.IsTrue(root.Contains("ab"));
            root.Add("ac");
            Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(3, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsTrue(root.Contains("a"));
            Assert.IsTrue(root.Contains("ab"));
            Assert.IsTrue(root.Contains("ac"));
            root.Add("fgh");
            Assert.AreEqual(6, root.NodeCount);
            Assert.AreEqual(4, root.WordCount);
            Assert.IsTrue(root.Contains("a"));
            Assert.IsTrue(root.Contains("ab"));
            Assert.IsTrue(root.Contains("fgh"));
            root.Delete("fgh");
            Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(3, root.WordCount);
            root.Delete("ab");
            //Assert.AreEqual(2, root.NodeCount);
            Assert.AreEqual(2, root.WordCount);
            root.Delete("a");
            //Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(1, root.WordCount);
        }

        [Test]
        public void TestTrieNodeFirstWord()
        {
            var trie = new TrieNode();
            Assert.AreEqual("", trie.FirstWord());
            trie.Add("table");
            trie.Add("air");
            trie.Add("link");
            Assert.AreEqual("air", trie.FirstWord());
            trie.Delete("air");
            Assert.AreEqual("link", trie.FirstWord());
            trie.Delete("link");
            Assert.AreEqual("table", trie.FirstWord());
            trie.Delete("table");
            Assert.AreEqual("", trie.FirstWord());
        }

        [Test]
        public void TestTrieNodeSearchWordAtExactDistance()
        {
            var trie = new TrieNode();
            trie.Add("table");
            trie.Add("air");
            trie.Add("link");

            Assert.AreEqual(Tuple.Create("link", 1), trie.NearestWord("links"));

            Assert.AreEqual(Tuple.Create("table", 0), trie.NearestWord("table"));
            Assert.AreEqual(Tuple.Create("air", 0), trie.NearestWord("air"));
            Assert.AreEqual(Tuple.Create("link", 0), trie.NearestWord("link"));
            Assert.AreEqual(Tuple.Create("link", 1), trie.NearestWord("links"));
            Assert.AreEqual(Tuple.Create("link", 1), trie.NearestWord("alink"));
            Assert.AreEqual(Tuple.Create("link", 2), trie.NearestWord("alinka"));
            Assert.AreEqual(Tuple.Create("link", 1), trie.NearestWord("linpk"));
            Assert.AreEqual(Tuple.Create("link", 1), trie.NearestWord("liok"));
            Assert.AreEqual(Tuple.Create("link", 2), trie.NearestWord("liokp"));
            Assert.AreEqual(Tuple.Create("air", 3), trie.NearestWord(""));
            Assert.AreEqual(Tuple.Create("link", 1), trie.NearestWord("lnik"));
            Assert.AreEqual(Tuple.Create("link", 1), trie.NearestWord("likn"));
            Assert.AreEqual(Tuple.Create("link", 1), trie.NearestWord("ilnk"));
            Assert.AreEqual(Tuple.Create("link", 2), trie.NearestWord("ilnka"));
            Assert.AreEqual(Tuple.Create("link", 0), trie.NearestWord("likn",10,10,10,0));
            Assert.AreEqual(Tuple.Create("link", 5), trie.NearestWord("likno",10,3,10,2));
            Assert.AreEqual(Tuple.Create("table", 1), trie.NearestWord("tabler"));
            Assert.AreEqual(Tuple.Create("table", 17+22), trie.NearestWord("taler",17,22,2222,2222));
        }



        [Test]
        public void TestT9strings_to_MostWeightedWordsInDico()
        {
            var wordCharToT9Char = Get_WordChar_to_T9Char();
            var dicoWordToWeight = new Dictionary<string, int>();
            dicoWordToWeight["aab"] = 10; //222
            dicoWordToWeight["aad"] = 5; //223
            dicoWordToWeight["abc"] = 20; //222
            dicoWordToWeight["aba"] = 20; //222
            dicoWordToWeight["g"] = -100; //4

            var result = Utils.EncodedString_to_MostWeightedWordsInDico(wordCharToT9Char, dicoWordToWeight);
            Assert.AreEqual(5, result.Count);
            var aList = result[Utils.EncodeWord("a", wordCharToT9Char)];
            Assert.AreEqual(2, aList.Count);
            Assert.IsTrue(aList.Contains("abc"));
            Assert.IsTrue(aList.Contains("aba"));
            var abcdT9Strings = Utils.EncodeWord("abcd", wordCharToT9Char);
            Assert.IsFalse(result.ContainsKey(abcdT9Strings));
            var gList = result[Utils.EncodeWord("g", wordCharToT9Char)];
            Assert.AreEqual(1, gList.Count);
            Assert.IsTrue(gList.Contains("g"));
            var aadList = result[Utils.EncodeWord("aad", wordCharToT9Char)];
            Assert.AreEqual(1, aadList.Count);
            Assert.IsTrue(aadList.Contains("aad"));
        }

        private static IDictionary<char, char> Get_WordChar_to_T9Char()
        {
            var result = new Dictionary<char, char>();
            result['a'] = result['b'] = result['c'] = '2';
            result['d'] = result['e'] = result['f'] = '3';
            result['g'] = result['g'] = result['i'] = '4';
            result['j'] = result['k'] = result['l'] = '5';
            result['m'] = result['n'] = result['o'] = '6';
            result['p'] = result['q'] = result['r'] = result['s'] = '7';
            result['t'] = result['u'] = result['v'] = '8';
            result['w'] = result['x'] = result['y'] = result['y'] = '9';
            return result;
        }
    }
}
