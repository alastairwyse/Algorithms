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
using System.Threading.Tasks;
using MoreComplexDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorithms.UnitTests
{
    /// <summary>
    /// Unit tests for the Algorithms.AdjacentWordGraphPathFinderTests class.
    /// </summary>
    [TestClass]
    public class AdjacentWordGraphPathFinderTests
    {
        private AdjacentWordGraphPathFinder testAdjacentWordGraphPathFinder;

        [TestInitialize]
        public void SetUp()
        {
            CandidateWordPriorityCalculator priorityCalculator = new CandidateWordPriorityCalculator(20, 1, 1, 1, 1, new Dictionary<Char, TrieNode<Char>>(), new FrequencyTable<Char>(), new FrequencyTable<CharacterSubstitution>());
            testAdjacentWordGraphPathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, new CharacterTrieUtilities(), new Dictionary<Char, TrieNode<Char>>());
        }

        /// <summary>
        /// Tests that an exception is thrown if the FindPathViaAStar() method is called with a 'sourceWord' and 'destinationWord' which are the same.
        /// </summary>
        [TestMethod]
        public void FindPathViaAStar_SourceWordAndDestinationWordsAreTheSame()
        {
            Int32 numberOfEdgesExplored = 0;

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testAdjacentWordGraphPathFinder.FindPathViaAStar("test", "test", ref numberOfEdgesExplored);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'destinationWord' cannot be the same as parameter 'sourceWord'.");
            Assert.AreEqual("destinationWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the FindPathViaAStar() method is called with a 'sourceWord' parameter whose length is less than 1.
        /// </summary>
        [TestMethod]
        public void FindPathViaAStar_SourceWordLengthLessThan1()
        {
            Int32 numberOfEdgesExplored = 0;

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testAdjacentWordGraphPathFinder.FindPathViaAStar("", "test", ref numberOfEdgesExplored);
            });

            StringAssert.StartsWith(e.Message, "The length of parameter 'sourceWord' must be greater than or equal to 1.");
            Assert.AreEqual("sourceWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the FindPathViaAStar() method is called with 'sourceWord' and 'destinationWord' parameters which have different length.
        /// </summary>
        [TestMethod]
        public void FindPathViaAStar_SourceWordAndDestinationWordsDifferentLength()
        {
            Int32 numberOfEdgesExplored = 0;

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testAdjacentWordGraphPathFinder.FindPathViaAStar("abc", "test", ref numberOfEdgesExplored);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'sourceWord' must have the same length as parameter 'destinationWord'.");
            Assert.AreEqual("destinationWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the FindPathViaBidirectionalBreadthFirstSearch() method is called with a 'sourceWord' and 'destinationWord' which are the same.
        /// </summary>
        [TestMethod]
        public void FindPathViaBidirectionalBreadthFirstSearch_SourceWordAndDestinationWordsAreTheSame()
        {
            Int32 numberOfEdgesExplored = 0;

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testAdjacentWordGraphPathFinder.FindPathViaBidirectionalBreadthFirstSearch("test", "test", ref numberOfEdgesExplored);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'destinationWord' cannot be the same as parameter 'sourceWord'.");
            Assert.AreEqual("destinationWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the FindPathViaBidirectionalBreadthFirstSearch() method is called with a 'sourceWord' parameter whose length is less than 1.
        /// </summary>
        [TestMethod]
        public void FindPathViaBidirectionalBreadthFirstSearch_SourceWordLengthLessThan1()
        {
            Int32 numberOfEdgesExplored = 0;

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testAdjacentWordGraphPathFinder.FindPathViaBidirectionalBreadthFirstSearch("", "test", ref numberOfEdgesExplored);
            });

            StringAssert.StartsWith(e.Message, "The length of parameter 'sourceWord' must be greater than or equal to 1.");
            Assert.AreEqual("sourceWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the FindPathViaBidirectionalBreadthFirstSearch() method is called with 'sourceWord' and 'destinationWord' parameters which have different length.
        /// </summary>
        [TestMethod]
        public void FindPathViaBidirectionalBreadthFirstSearch_SourceWordAndDestinationWordsDifferentLength()
        {
            Int32 numberOfEdgesExplored = 0;

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testAdjacentWordGraphPathFinder.FindPathViaBidirectionalBreadthFirstSearch("abc", "test", ref numberOfEdgesExplored);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'sourceWord' must have the same length as parameter 'destinationWord'.");
            Assert.AreEqual("destinationWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the FindLongestPathFrom() method is called with a 'sourceWord' parameter which is an empty string.
        /// </summary>
        [TestMethod]
        public void FindLongestPathFrom_SourceWordEmptyString()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testAdjacentWordGraphPathFinder.FindLongestPathFrom("");
            });

            StringAssert.StartsWith(e.Message, "Parameter 'sourceWord' cannot be an empty string.");
            Assert.AreEqual("sourceWord", e.ParamName);
        }
    }
}
