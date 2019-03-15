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
using System.Text;
using System.IO;
using MoreComplexDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMock2;
using NMock2.Monitoring;

namespace Algorithms.UnitTests
{
    /// <summary>
    /// Unit tests for the Algorithms.DataStructureUtilities class.
    /// </summary>
    [TestClass]
    public class DataStructureUtilitiesTests
    {
        private Mockery mockery;
        private DataStructureUtilities testDataStructureUtilities;
        private Dictionary<Char, TrieNode<Char>> trieRoot;

        [TestInitialize]
        public void SetUp()
        {
            mockery = new Mockery();
            testDataStructureUtilities = new DataStructureUtilities();
            trieRoot = new Dictionary<Char, TrieNode<Char>>();
        }

        /// <summary>
        /// Success tests for the LoadWordsFromFile() method.
        /// </summary>
        [TestMethod]
        public void LoadWordsFromFile()
        {
            IStreamReader mockStreamReader = mockery.NewMock<IStreamReader>();
            ICharacterTrieBuilder mockTrieBuilder = mockery.NewMock<ICharacterTrieBuilder>();
            List<String> testWords = new List<String>() { "time", "cat", "tile", "birds", "take" };
            Func<String, Boolean> wordFilterFunction = new Func<String, Boolean>((inputString) =>
            {
                if (inputString.Length == 4)
                    return true;
                else
                    return false;
            });

            using (mockery.Ordered)
            {
                foreach (String currentTestWord in testWords)
                {
                    Expect.Once.On(mockStreamReader).GetProperty("EndOfStream").Will(Return.Value(false));
                    Expect.Once.On(mockStreamReader).Method("ReadLine").WithNoArguments().Will(Return.Value(currentTestWord));
                    if (currentTestWord.Length == 4)
                        Expect.Once.On(mockTrieBuilder).Method("AddWord").With(trieRoot, currentTestWord, true);
                }
                Expect.Once.On(mockStreamReader).GetProperty("EndOfStream").Will(Return.Value(true));
                Expect.Once.On(mockStreamReader).Method("Dispose").WithNoArguments();
            }

            testDataStructureUtilities.LoadWordsFromFile(trieRoot, mockStreamReader, mockTrieBuilder, wordFilterFunction);
        }

        /// <summary>
        /// Tests that the Dispose() method is called on the IStreamReader instance if an exception occurs when the LoadWordsFromFile() method is called.
        /// </summary>
        [TestMethod]
        public void LoadWordsFromFile_DisposeCalledOnException()
        {
            IStreamReader mockStreamReader = mockery.NewMock<IStreamReader>();
            ICharacterTrieBuilder mockTrieBuilder = mockery.NewMock<ICharacterTrieBuilder>();
            Func<String, Boolean> wordFilterFunction = new Func<String, Boolean>((inputString) => { return true; });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockStreamReader).GetProperty("EndOfStream").Will(Return.Value(false));
                Expect.Once.On(mockStreamReader).Method("ReadLine").WithNoArguments().Will(Throw.Exception(new Exception("Test Exception")));
                Expect.Once.On(mockStreamReader).Method("Dispose").WithNoArguments();
            }

            Exception e = Assert.ThrowsException<Exception>(() =>
            {
                testDataStructureUtilities.LoadWordsFromFile(trieRoot, mockStreamReader, mockTrieBuilder, wordFilterFunction);
            });
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Success tests for the PopulateAdjacentWordDataStructures() method.
        /// </summary>
        [TestMethod]
        public void PopulateAdjacentWordDataStructures()
        {
            IStreamReader mockStreamReader = mockery.NewMock<IStreamReader>();
            ICharacterTrieBuilder mockTrieBuilder = mockery.NewMock<ICharacterTrieBuilder>();
            HashSet<String> allWords = new HashSet<String>();
            FrequencyTable<Char> fromCharacterFrequencies = new FrequencyTable<Char>();
            FrequencyTable<CharacterSubstitution> characterSubstitutionFrequencies = new FrequencyTable<CharacterSubstitution>();
            List<String> testWords = new List<String>()
            {
                "read",
                "bead",
                "fail",
                "dead",
                "road",
                "reed",
                "calm",
                "real",
                "rear"
            };
            Func<String, Boolean> wordFilterFunction = new Func<String, Boolean>((inputString) =>
            {
                if (inputString.Length == 4)
                    return true;
                else
                    return false;
            });

            using (mockery.Ordered)
            {
                foreach (String currentTestWord in testWords)
                {
                    Expect.Once.On(mockStreamReader).GetProperty("EndOfStream").Will(Return.Value(false));
                    Expect.Once.On(mockStreamReader).Method("ReadLine").WithNoArguments().Will(Return.Value(currentTestWord));
                    // Invoking of action AddWordToCharacterTrieActionAction adds the test word to the trie (since this will not be done by the 'mockTrieBuilder')
                    Expect.Once.On(mockTrieBuilder).Method("AddWord").With(trieRoot, currentTestWord, true).Will(new AddWordToCharacterTrieActionAction(new CharacterTrieBuilder(), trieRoot, currentTestWord));
                }
                Expect.Once.On(mockStreamReader).GetProperty("EndOfStream").Will(Return.Value(true));
                Expect.Once.On(mockStreamReader).Method("Dispose").WithNoArguments();
            }

            testDataStructureUtilities.PopulateAdjacentWordDataStructures(mockStreamReader, mockTrieBuilder, wordFilterFunction, trieRoot, allWords, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('a', 'e')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('b', 'd')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('b', 'r')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('d', 'b')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('d', 'l')));
            Assert.AreEqual(2, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('d', 'r')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('e', 'a')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('e', 'o')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('l', 'd')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('l', 'r')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('o', 'e')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('r', 'b')));
            Assert.AreEqual(2, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('r', 'd')));
            Assert.AreEqual(1, characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('r', 'l')));
            Assert.AreEqual(14, characterSubstitutionFrequencies.ItemCount);
            Assert.AreEqual(1, fromCharacterFrequencies.GetFrequency('a'));
            Assert.AreEqual(2, fromCharacterFrequencies.GetFrequency('b'));
            Assert.AreEqual(4, fromCharacterFrequencies.GetFrequency('d'));
            Assert.AreEqual(2, fromCharacterFrequencies.GetFrequency('e'));
            Assert.AreEqual(2, fromCharacterFrequencies.GetFrequency('l'));
            Assert.AreEqual(1, fromCharacterFrequencies.GetFrequency('o'));
            Assert.AreEqual(4, fromCharacterFrequencies.GetFrequency('r'));
            Assert.AreEqual(7, fromCharacterFrequencies.ItemCount);
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that the Dispose() method is called on the IStreamReader instance if an exception occurs when the PopulateAdjacentWordDataStructures() method is called.
        /// </summary>
        [TestMethod]
        public void PopulateAdjacentWordDataStructures_DisposeCalledOnException()
        {
            IStreamReader mockStreamReader = mockery.NewMock<IStreamReader>();
            ICharacterTrieBuilder mockTrieBuilder = mockery.NewMock<ICharacterTrieBuilder>();
            HashSet<String> allWords = new HashSet<String>();
            FrequencyTable<Char> fromCharacterFrequencies = new FrequencyTable<Char>();
            FrequencyTable<CharacterSubstitution> characterSubstitutionFrequencies = new FrequencyTable<CharacterSubstitution>();
            Func<String, Boolean> wordFilterFunction = new Func<String, Boolean>((inputString) => { return true; });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockStreamReader).GetProperty("EndOfStream").Will(Return.Value(false));
                Expect.Once.On(mockStreamReader).Method("ReadLine").WithNoArguments().Will(Throw.Exception(new Exception("Test Exception")));
                Expect.Once.On(mockStreamReader).Method("Dispose").WithNoArguments();
            }

            Exception e = Assert.ThrowsException<Exception>(() =>
            {
                testDataStructureUtilities.PopulateAdjacentWordDataStructures(mockStreamReader, mockTrieBuilder, wordFilterFunction, trieRoot, allWords, fromCharacterFrequencies, characterSubstitutionFrequencies);
            });
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        #region Protected / Private Classes

        /// <summary>
        /// Implementation of IAction which calls adds a word to a character trie using an implementation of ICharacterTrieBuilder.
        /// </summary>
        protected class AddWordToCharacterTrieActionAction : IAction
        {
            protected ICharacterTrieBuilder characterTrieBuilder;
            protected Dictionary<Char, TrieNode<Char>> trieRoot;
            protected String word;

            /// <summary>
            /// Initialises a new instance of the Algorithms.UnitTests.DataStructureUtilitiesTests+AddWordToCharacterTrieActionAction class.
            /// </summary>
            /// <param name="characterTrieBuilder">An implementation of ICharacterTrieBuilder to use to add the word to the trie.</param>
            /// <param name="trieRoot">The root of the trie.</param>
            /// <param name="word">The word to add to the trie.</param>
            public AddWordToCharacterTrieActionAction(ICharacterTrieBuilder characterTrieBuilder, Dictionary<Char, TrieNode<Char>> trieRoot, String word)
            {
                this.characterTrieBuilder = characterTrieBuilder;
                this.trieRoot = trieRoot;
                this.word = word;
            }

            public void DescribeTo(TextWriter writer)
            {
                writer.Write("Adding word '" + word + "' to the character trie.");
            }

            public void Invoke(Invocation invocation)
            {
                characterTrieBuilder.AddWord(trieRoot, word, true);
            }
        }

        #endregion
    }
}
