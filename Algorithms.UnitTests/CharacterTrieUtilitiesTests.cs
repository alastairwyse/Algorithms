/*
 * Copyright 2019 Alastair Wyse (https://github.com/alastairwyse/Algorithms/)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorithms.UnitTests
{
    /// <summary>
    /// Unit tests for the Algorithms.CharacterTrieUtilities class.
    /// </summary>
    [TestClass]
    public class CharacterTrieUtilitiesTests
    {
        private CharacterTrieUtilities testCharacterTrieUtilities;
        private Dictionary<Char, TrieNode<Char>> trieRoot;

        [TestInitialize]
        public void SetUp()
        {
            testCharacterTrieUtilities = new CharacterTrieUtilities();
            trieRoot = new Dictionary<Char, TrieNode<Char>>();
        }

        /// <summary>
        /// Tests that an exception is thrown if the FindAdjacentWords() method is called with an 'inputWord' parameter which is an empty string.
        /// </summary>
        [TestMethod]
        public void FindAdjacentWords_InputWordParameterEmptyString()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                IEnumerable<String> results = testCharacterTrieUtilities.FindAdjacentWords(trieRoot, "");
                results.Count();
            });

            StringAssert.StartsWith(e.Message, "Parameter 'inputWord' cannot be an empty string.");
            Assert.AreEqual("word", e.ParamName);
        }

        /// <summary>
        /// Success tests for the FindAdjacentWords() method.
        /// </summary>
        [TestMethod]
        public void FindAdjacentWords()
        {
            IEnumerable<String> results = testCharacterTrieUtilities.FindAdjacentWords(trieRoot, "time");

            Assert.AreEqual(0, results.Count());


            CharacterTrieBuilder builder = new CharacterTrieBuilder();
            builder.AddWord(trieRoot, "limb", true);
            builder.AddWord(trieRoot, "line", true);
            builder.AddWord(trieRoot, "lime", true);
            builder.AddWord(trieRoot, "time", true);
            builder.AddWord(trieRoot, "timo", true);
            builder.AddWord(trieRoot, "tame", true);
            builder.AddWord(trieRoot, "tile", true);
            builder.AddWord(trieRoot, "timer", true);
            builder.AddWord(trieRoot, "timers", true);
            builder.AddWord(trieRoot, "tamer", true);
            builder.AddWord(trieRoot, "lines", true);
            builder.AddWord(trieRoot, "liner", true);
            builder.AddWord(trieRoot, "tim", true);

            results = testCharacterTrieUtilities.FindAdjacentWords(trieRoot, "time");

            Assert.AreEqual(4, results.Count());
            Assert.IsTrue(results.Contains("lime"));
            Assert.IsTrue(results.Contains("tame"));
            Assert.IsTrue(results.Contains("tile"));
            Assert.IsTrue(results.Contains("timo"));
        }
    }
}
