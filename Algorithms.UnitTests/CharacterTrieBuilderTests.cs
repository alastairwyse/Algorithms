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
    /// Unit tests for the Algorithms.CharacterTrieBuilder class.
    /// </summary>
    [TestClass]
    public class CharacterTrieBuilderTests
    {
        private CharacterTrieBuilder testCharacterTrieBuilder;
        private Dictionary<Char, TrieNode<Char>> trieRoot;

        [TestInitialize]
        public void SetUp()
        {
            testCharacterTrieBuilder = new CharacterTrieBuilder();
            trieRoot = new Dictionary<Char, TrieNode<Char>>();
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddWord() method is called with a 'word' parameter which is an empty string.
        /// </summary>
        [TestMethod]
        public void AddWord_WordParameterEmptyString()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCharacterTrieBuilder.AddWord(trieRoot, "", true);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'word' cannot be an empty string.");
            Assert.AreEqual("word", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddWord() method is called with a single character 'word' parameter which already exists in the trie.
        /// </summary>
        [TestMethod]
        public void AddWord_SingleCharacterWordAlreadyExists()
        {
            testCharacterTrieBuilder.AddWord(trieRoot, "arm", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "bit", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "a", true);

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCharacterTrieBuilder.AddWord(trieRoot, "a", true);
            });

            StringAssert.StartsWith(e.Message, "The word 'a' in parameter 'word' already exists in the trie.");
            Assert.AreEqual("word", e.ParamName);
        }

        /// <summary>
        /// Success tests for the AddWord() method adding a single character word.
        /// </summary>
        [TestMethod]
        public void AddWord_SingleCharacterWord()
        {
            // Test adding the word where it already exists, but setting parameter 'throwExceptionIfWordAlreadyExists' to false
            testCharacterTrieBuilder.AddWord(trieRoot, "arm", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "bit", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "a", true);

            testCharacterTrieBuilder.AddWord(trieRoot, "a", false);

            Assert.IsTrue(trieRoot.ContainsKey('a'));
            Assert.IsInstanceOfType(trieRoot['a'], typeof(EndMarkerTrieNode<Char>));
            Assert.AreEqual('r', trieRoot['a'].GetChildNode('r').Item);


            // Test adding where the word doesn't already exist
            trieRoot.Clear();
            testCharacterTrieBuilder.AddWord(trieRoot, "bit", true);

            testCharacterTrieBuilder.AddWord(trieRoot, "a", true);

            Assert.IsTrue(trieRoot.ContainsKey('a'));
            Assert.IsInstanceOfType(trieRoot['a'], typeof(EndMarkerTrieNode<Char>));
            Assert.AreEqual(0, trieRoot['a'].Children.Count());
        }

        /// <summary>
        /// Success tests for the AddWord() method adding a multi-character word.
        /// </summary>
        [TestMethod]
        public void AddWord_MultiCharacterWord()
        {
            // Test adding the word where it already exists, but setting parameter 'throwExceptionIfWordAlreadyExists' to false
            testCharacterTrieBuilder.AddWord(trieRoot, "arm", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "bit", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "ate", true);

            testCharacterTrieBuilder.AddWord(trieRoot, "arm", false);

            Assert.IsTrue(trieRoot.ContainsKey('a'));
            Assert.AreEqual('r', trieRoot['a'].GetChildNode('r').Item);
            Assert.AreEqual('m', trieRoot['a'].GetChildNode('r').GetChildNode('m').Item);
            Assert.IsInstanceOfType(trieRoot['a'].GetChildNode('r').GetChildNode('m'), typeof(EndMarkerTrieNode<Char>));
            Assert.AreEqual(0, trieRoot['a'].GetChildNode('r').GetChildNode('m').Children.Count());

            // Test adding where the word doesn't already exist
            trieRoot.Clear();
            testCharacterTrieBuilder.AddWord(trieRoot, "bit", true);

            testCharacterTrieBuilder.AddWord(trieRoot, "arm", true);

            Assert.IsTrue(trieRoot.ContainsKey('a'));
            Assert.AreEqual('r', trieRoot['a'].GetChildNode('r').Item);
            Assert.AreEqual('m', trieRoot['a'].GetChildNode('r').GetChildNode('m').Item);
            Assert.IsInstanceOfType(trieRoot['a'].GetChildNode('r').GetChildNode('m'), typeof(EndMarkerTrieNode<Char>));
            Assert.AreEqual(0, trieRoot['a'].GetChildNode('r').GetChildNode('m').Children.Count());

            // Test adding where the added word is a multi-character word which is a substring of an existing word
            trieRoot.Clear();
            testCharacterTrieBuilder.AddWord(trieRoot, "bit", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "army", true);

            testCharacterTrieBuilder.AddWord(trieRoot, "arm", true);

            Assert.IsTrue(trieRoot.ContainsKey('a'));
            Assert.AreEqual('r', trieRoot['a'].GetChildNode('r').Item);
            Assert.AreEqual('m', trieRoot['a'].GetChildNode('r').GetChildNode('m').Item);
            Assert.IsInstanceOfType(trieRoot['a'].GetChildNode('r').GetChildNode('m'), typeof(EndMarkerTrieNode<Char>));
            Assert.AreEqual('y', trieRoot['a'].GetChildNode('r').GetChildNode('m').GetChildNode('y').Item);
            Assert.IsInstanceOfType(trieRoot['a'].GetChildNode('r').GetChildNode('m').GetChildNode('y'), typeof(EndMarkerTrieNode<Char>));
            Assert.AreEqual(0, trieRoot['a'].GetChildNode('r').GetChildNode('m').GetChildNode('y').Children.Count());
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddWord() method is called with a multi character 'word' parameter which already exists in the trie.
        /// </summary>
        [TestMethod]
        public void AddWord_MultiCharacterWordAlreadyExists()
        {
            testCharacterTrieBuilder.AddWord(trieRoot, "arm", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "bit", true);
            testCharacterTrieBuilder.AddWord(trieRoot, "ate", true);

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCharacterTrieBuilder.AddWord(trieRoot, "ate", true);
            });

            StringAssert.StartsWith(e.Message, "The word 'ate' in parameter 'word' already exists in the trie.");
            Assert.AreEqual("word", e.ParamName);
        }
    }
}
