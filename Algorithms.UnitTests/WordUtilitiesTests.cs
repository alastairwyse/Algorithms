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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorithms.UnitTests
{
    /// <summary>
    /// Unit tests for the Algorithms.WordUtilities class.
    /// </summary>
    [TestClass]
    public class WordUtilitiesTests
    {
        private WordUtilities testWordUtilities;

        [TestInitialize]
        public void SetUp()
        {
            testWordUtilities = new WordUtilities();
        }

        /// <summary>
        /// Tests that an exception is thrown when the FindDifferingCharacters() method is called with a 'word1' parameter which is 0 length.
        /// </summary>
        [TestMethod]
        public void FindDifferingCharacters_Word10Length()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testWordUtilities.FindDifferingCharacters("", "test");
            });

            StringAssert.StartsWith(e.Message, "Parameter 'word1' must be greater than or equal to 1 character in length.");
            Assert.AreEqual("word1", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the FindDifferingCharacters() method is called with a 'word2' parameter which is 0 length.
        /// </summary>
        [TestMethod]
        public void FindDifferingCharacters_Word20Length()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testWordUtilities.FindDifferingCharacters("test", "");
            });

            StringAssert.StartsWith(e.Message, "Parameter 'word2' must be greater than or equal to 1 character in length.");
            Assert.AreEqual("word2", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the FindDifferingCharacters() method is called with 'word1' and 'word2' parameters which differ in length.
        /// </summary>
        [TestMethod]
        public void FindDifferingCharacters_Word1LengthDifferentToWord2()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testWordUtilities.FindDifferingCharacters("test", "tes");
            });

            StringAssert.StartsWith(e.Message, "Parameters 'word1' and 'word2' must have the same length.");
            Assert.AreEqual("word2", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the FindDifferingCharacters() method is called with 'word1' and 'word2' parameters which differ by more than 1 character.
        /// </summary>
        [TestMethod]
        public void FindDifferingCharacters_Word1AndWord2DifferByMoreThan1Character()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testWordUtilities.FindDifferingCharacters("read", "lean");
            });

            StringAssert.StartsWith(e.Message, "Parameters 'word1' and 'word2' differ by more than 1 character.");
            Assert.AreEqual("word2", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the FindDifferingCharacters() method is called with 'word1' and 'word2' parameters which are the same.
        /// </summary>
        [TestMethod]
        public void FindDifferingCharacters_Word1AndWord2AreSame()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testWordUtilities.FindDifferingCharacters("read", "read");
            });

            StringAssert.StartsWith(e.Message, "Parameters 'word1' and 'word2' are the same.");
            Assert.AreEqual("word2", e.ParamName);
        }

        /// <summary>
        /// Success tests for the FindDifferingCharacters() method.
        /// </summary>
        [TestMethod]
        public void FindDifferingCharacters()
        {
            Tuple<Char, Char> result = testWordUtilities.FindDifferingCharacters("read", "lead");

            Assert.AreEqual('r', result.Item1);
            Assert.AreEqual('l', result.Item2);


            result = testWordUtilities.FindDifferingCharacters("read", "reap");

            Assert.AreEqual('d', result.Item1);
            Assert.AreEqual('p', result.Item2);
        }
    }
}
