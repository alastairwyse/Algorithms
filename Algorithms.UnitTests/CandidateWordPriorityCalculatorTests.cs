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
using NMock2;

namespace Algorithms.UnitTests
{
    /// <summary>
    /// Unit tests for the Algorithms.CandidateWordPriorityCalculator class.
    /// </summary>
    [TestClass]
    public class CandidateWordPriorityCalculatorTests
    {
        private Mockery mockery;
        private IStreamReader mockStreamReader;
        private Dictionary<Char, TrieNode<Char>> allWordsTrieRoot;
        private HashSet<String> allWords;
        private FrequencyTable<Char> fromCharacterFrequencies;
        private FrequencyTable<CharacterSubstitution> characterSubstitutionFrequencies;
        private CandidateWordPriorityCalculator testCandidateWordPriorityCalculator;
        private DataStructureUtilities dataStructureUtilities = new DataStructureUtilities();

        [TestInitialize]
        public void SetUp()
        {
            mockery = new Mockery();
            mockStreamReader = mockery.NewMock<IStreamReader>();
            allWordsTrieRoot = new Dictionary<Char, TrieNode<Char>>();
            allWords = new HashSet<String>();
            fromCharacterFrequencies = new FrequencyTable<Char>();
            characterSubstitutionFrequencies = new FrequencyTable<CharacterSubstitution>();
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
            Func<String, Boolean> wordFilterFunction = new Func<String, Boolean>((inputString) => { return true; });
            CharacterTrieBuilder characterTrieBuilder = new CharacterTrieBuilder();
            using (mockery.Ordered)
            {
                foreach (String currentTestWord in testWords)
                {
                    Expect.Once.On(mockStreamReader).GetProperty("EndOfStream").Will(Return.Value(false));
                    Expect.Once.On(mockStreamReader).Method("ReadLine").WithNoArguments().Will(Return.Value(currentTestWord));
                }
                Expect.Once.On(mockStreamReader).GetProperty("EndOfStream").Will(Return.Value(true));
                Expect.Once.On(mockStreamReader).Method("Dispose").WithNoArguments();
            }
            dataStructureUtilities.PopulateAdjacentWordDataStructures(mockStreamReader, characterTrieBuilder, wordFilterFunction, allWordsTrieRoot, allWords, fromCharacterFrequencies, characterSubstitutionFrequencies);
            mockery.ClearExpectation(mockStreamReader);
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 1, 1, 1, 1, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with a 'maximumSourceWordToCandidateWordDistance' parameter less than 1;
        /// </summary>
        [TestMethod]
        public void Constructor_MaximumSourceWordToCandidateWordDistanceLessThan1()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(0, 1, 1, 1, 1, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'maximumSourceWordToCandidateWordDistance' must be greater than or equal to 1.");
            Assert.AreEqual("maximumSourceWordToCandidateWordDistance", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with a 'sourceWordToCandidateWordDistanceWeight' parameter less than 0;
        /// </summary>
        [TestMethod]
        public void Constructor_SourceWordToCandidateWordDistanceWeightLessThan0()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, -1, 1, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'sourceWordToCandidateWordDistanceWeight' must be greater than or equal to 0.");
            Assert.AreEqual("sourceWordToCandidateWordDistanceWeight", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with a 'numberOfCharactersMatchingDestinationWeight' parameter less than 0;
        /// </summary>
        [TestMethod]
        public void Constructor_NumberOfCharactersMatchingDestinationWeightLessThan0()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 1, -1, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'numberOfCharactersMatchingDestinationWeight' must be greater than or equal to 0.");
            Assert.AreEqual("numberOfCharactersMatchingDestinationWeight", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with a 'popularityOfChangeToCharacterWeight' parameter less than 0;
        /// </summary>
        [TestMethod]
        public void Constructor_PopularityOfChangeToCharacterWeightLessThan0()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 1, 0, -1, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'popularityOfChangeToCharacterWeight' must be greater than or equal to 0.");
            Assert.AreEqual("popularityOfChangeToCharacterWeight", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with a 'popularityOfCharacterChangeWeight' parameter less than 0;
        /// </summary>
        [TestMethod]
        public void Constructor_PopularityOfCharacterChangeWeightLessThan0()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 1, 0, 0, -1, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'popularityOfCharacterChangeWeight' must be greater than or equal to 0.");
            Assert.AreEqual("popularityOfCharacterChangeWeight", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with all 'weight' parameters equal to 0;
        /// </summary>
        [TestMethod]
        public void Constructor_AllWeights0()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 0, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
            });

            StringAssert.StartsWith(e.Message, "At least one of parameters 'sourceWordToCandidateWordDistanceWeight', 'numberOfCharactersMatchingDestinationWeight', 'popularityOfChangeToCharacterWeight', and 'popularityOfCharacterChangeWeight' must be greater than 0.");
        }

        /// <summary>
        /// Tests that an exception is thrown when the CalculatePriority() method is called with a 'currentWord' parameter which is 0 length.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CurrentWord0Length()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator.CalculatePriority("", "test", "mall", 1);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'currentWord' must be greater than or equal to 1 character in length.");
            Assert.AreEqual("currentWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the CalculatePriority() method is called with a 'candidateWord' parameter which is 0 length.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CandidateWord0Length()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator.CalculatePriority("test", "", "mall", 1);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'candidateWord' must be greater than or equal to 1 character in length.");
            Assert.AreEqual("candidateWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the CalculatePriority() method is called with a 'destinationWord' parameter which is 0 length.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_DestinationWord0Length()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator.CalculatePriority("test", "word", "", 1);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'destinationWord' must be greater than or equal to 1 character in length.");
            Assert.AreEqual("destinationWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the CalculatePriority() method is called with 'currentWord' and 'candidateWord' parameters which differ in length.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CurrentWordLengthDifferentToCandidateWord()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator.CalculatePriority("test", "tes", "mall", 1);
            });

            StringAssert.StartsWith(e.Message, "Parameters 'currentWord' and 'candidateWord' must have the same length.");
            Assert.AreEqual("candidateWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the CalculatePriority() method is called with 'candidateWord' and 'destinationWord' parameters which differ in length.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CandidateWordLengthDifferentToDestinationWord()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator.CalculatePriority("test", "mall", "mal", 1);
            });

            StringAssert.StartsWith(e.Message, "Parameters 'candidateWord' and 'destinationWord' must have the same length.");
            Assert.AreEqual("destinationWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the CalculatePriority() method is called with a 'distanceFromSourceToCandidateWord' parameter less than 1.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_DistanceFromSourceToCandidateWordLessThan1()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator.CalculatePriority("test", "best", "mall", 0);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'distanceFromSourceToCandidateWord' must be greater than or equal to 1.");
            Assert.AreEqual("distanceFromSourceToCandidateWord", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when the CalculatePriority() method is called with a 'distanceFromSourceToCandidateWord' parameter greater than member 'maximumSourceWordToCandidateWordDistance'.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_DistanceFromSourceToCandidateWordGreaterThanMaximumSourceWordToCandidateWordDistance()
        {
            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testCandidateWordPriorityCalculator.CalculatePriority("test", "best", "mall", 21);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'distanceFromSourceToCandidateWord' cannot be greater than member 'maximumSourceWordToCandidateWordDistance'.");
            Assert.AreEqual("distanceFromSourceToCandidateWord", e.ParamName);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the source word to candidate word distance is a minimum.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_SourceToCandidateDistanceMinimum()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 1, 0, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("test", "best", "mall", 20);

            Assert.AreEqual(1.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the source word to candidate word distance is a maximum.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_SourceToCandidateDistanceMaximum()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 1, 0, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("test", "best", "mall", 1);

            Assert.AreEqual(0.05, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the candidate word and destination word number of matching characters is a minimum.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CandidateToDestinationMatchingCharactersMinimum()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 1, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("test", "best", "mall", 20);

            Assert.AreEqual(1.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the candidate word and destination word number of matching characters is a maximum.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CandidateToDestinationMatchingCharactersMaximum()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 1, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("malt", "mall", "mall", 20);

            Assert.AreEqual(0.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the candidate word and destination word number of matching characters is an intermediate value.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CandidateToDestinationMatchingCharactersIntermediate()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 1, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("malt", "mall", "mate", 20);

            Assert.AreEqual(0.5, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the candidate word and destination word have the same characters in different postitions.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CandidateToDestinationMatchingCharactersDifferingPositions()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 1, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("read", "lead", "dlea", 20);

            Assert.AreEqual(1.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the candidate word and destination word have matching characters, but different counts of characters.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CandidateToDestinationMatchingCharactersDifferingCounts()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 1, 0, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("read", "lead", "toll", 20);

            Assert.AreEqual(1.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the frequency that the character changed to is a 'changed from' character is a minimum.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_ChangedToCharacterFrequencyMinimum()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 0, 1, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("mall", "malt", "mate", 20);

            Assert.AreEqual(1.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the frequency that the character changed to is a 'changed from' character is a maximum.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_ChangedToCharacterFrequencyMaximum()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 0, 1, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("real", "read", "mate", 20);

            Assert.AreEqual(0.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the frequency that the character changed to is a 'changed from' character is an intermediate value.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_ChangedToCharacterFrequencyIntermediate()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 0, 1, 0, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("malt", "mall", "mate", 20);

            Assert.AreEqual(0.5, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the frequecy of the character substitution (i.e. the character changed between the current and candidate words) is a minimum.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CharacterSubstitutionFrequencyMinimum()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 0, 0, 1, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("mall", "malt", "mate", 20);

            Assert.AreEqual(1.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the frequecy of the character substitution (i.e. the character changed between the current and candidate words) is a maximum.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CharacterSubstitutionFrequencyMaximum()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 0, 0, 1, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("read", "dead", "mate", 20);

            Assert.AreEqual(0.0, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the frequecy of the character substitution (i.e. the character changed between the current and candidate words) is an intermediate value.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_CharacterSubstitutionFrequencyIntermediate()
        {
            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 0, 0, 0, 1, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("read", "real", "mate", 20);

            Assert.AreEqual(0.5, result);
        }

        /// <summary>
        /// Success test for the CalculatePriority() method where the weightings of the underlying priorities are mixed.
        /// </summary>
        [TestMethod]
        public void CalculatePriority_MixedPriorityWeights()
        {
            // Overall priority should be calculated as follows (see DataStructureUtilitiesTests.PopulateAdjacentWordDataStructures() test for a list of the expected contents of 'fromCharacterFrequencies' and 'characterSubstitutionFrequencies')...
            // CalculateSourceWordToCandidateWordDistancePriority()
            //   distance = 5/20 = 0.25,  weighting = 1/10 = 0.1, weighted priority = 0.025
            // CalculateNumberOfCharactersMatchingDestinationPriority()
            //   matching characters ('bead' and 'beat') = 3/4 = 0.75, weighting = 2/10 = 0.2, weighted priority = (1 - 0.75) * 0.2 = 0.05
            // CalculatePopularityOfChangeToCharacterPriority()
            //   changeTo character = 'b', popularity = 2/4 = 0.5, weighting = 3/10 = 0.3, weighted priority = (1 - 0.5) * 0.3 = 0.15
            // CalculatePopularityOfCharacterChangePriority()
            //   character substitution = 'r' > 'b', popularity = 1/2 = 0.5, weighting = 4/10 = 0.4, weighted priority = (1 - 0.5) * 0.4 = 0.2
            // Total weighting = 0.025 + 0.05 + 0.15 + 0.2 = 0.425

            testCandidateWordPriorityCalculator = new CandidateWordPriorityCalculator(20, 1, 2, 3, 4, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);

            Double result = testCandidateWordPriorityCalculator.CalculatePriority("read", "bead", "beat", 5);

            Assert.AreEqual(0.425D, Math.Round(result, 4));
        }
    }
}
