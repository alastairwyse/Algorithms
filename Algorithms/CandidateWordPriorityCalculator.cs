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

namespace Algorithms
{
    /// <summary>
    /// Calculates the priority assigned to a candidate word for path-finding algorithms applied to a graph of adjacent words (i.e. words which differ by a single character).
    /// </summary>
    public class CandidateWordPriorityCalculator
    {
        // Functions used to calculate the overall priority
        /// <summary>Functions used to calculate components of the overall priority.</summary>
        protected List<Func<String, String, String, Int32, Double>> priorityFunctions;
        /// <summary>Function which calculates the priority based on the distance from the source word to the candidate word (the 'g(n)' score in the A* algorithm).</summary>
        protected Func<String, String, String, Int32, Double> sourceWordToCandidateWordDistanceCalculationFunction;
        /// <summary>Function which calculates the priority based on the number of matching characters between the candidate word and the destination word.</summary>
        protected Func<String, String, String, Int32, Double> numberOfCharactersMatchingDestinationCalculationFunction;
        /// <summary>Function which calculates the priority based on the frequency that the character changed to in the candidate word is the 'changed from' character in a substitution between adjacent words.</summary>
        protected Func<String, String, String, Int32, Double> popularityOfChangeToCharacterCalculationFunction;
        /// <summary>Function which calculates the priority based on the frequency/popularity of the character substitution (i.e. character changed from current word to candidate word).</summary>
        protected Func<String, String, String, Int32, Double> numberOfCharactersMatchingDestinationFunction;

        // Weights applied to each of the priority calculation functions
        /// <summary>The weighting of each function in the 'priorityFunctions' member.</summary>
        protected List<Int32> priorityFunctionWeights;

        // Data structures used to calculate the priority
        /// <summary>The root of a character trie containing all the words in the graph.</summary>
        protected Dictionary<Char, TrieNode<Char>> allWordsTrieRoot;
        /// <summary>A FrequencyTable containing the number of times each character is the 'from' character in a substitution between adjacent words (i.e. represented by an edge of the graph).</summary>
        protected FrequencyTable<Char> fromCharacterFrequencies;
        /// <summary>A FrequencyTable containing the number of times each pair of characters in a substitution between adjacent words (i.e. represented by an edge of the graph) occurs.</summary>
        protected FrequencyTable<CharacterSubstitution> characterSubstitutionFrequencies;

        /// <summary>The maximum possible distance between any two words in the graph.</summary>
        protected Int32 maximumSourceWordToCandidateWordDistance;
        /// <summary>The total of all the function weights.</summary>
        protected Int64 functionWeightsTotal;
        /// <summary>The maximum 'from' character frequency in member 'fromCharacterFrequencies'.</summary>
        protected Int32 maximumFromCharacterFrequency;
        /// <summary>The maximum character substitution frequency in member 'characterSubstitutionFrequencies'.</summary>
        protected Int32 maximumCharacterSubstitutionFrequency;
        /// <summary>Instance of the WordUtilities class.</summary>
        protected WordUtilities wordUtilities;

        /// <summary>
        /// Initialises a new instance of the Algorithms.CandidateWordPriorityCalculator class.
        /// </summary>
        /// <param name="maximumSourceWordToCandidateWordDistance">The maximum possible distance between any two words in the graph.</param>
        /// <param name="sourceWordToCandidateWordDistanceWeight">The weight which should be applied when calculating the overall priority, to the distance from the source word to the candidate word (the 'g(n)' score in the A* algorithm).</param>
        /// <param name="numberOfCharactersMatchingDestinationWeight">The weight which should be applied when calculating the overall priority, to the number of matching characters between the candidate word and the destination word.</param>
        /// <param name="popularityOfChangeToCharacterWeight">The weight which should be applied when calculating the overall priority, to the frequency that the character changed to in the candidate word is the 'changed from' character in a substitution between adjacent words.</param>
        /// <param name="popularityOfCharacterChangeWeight">The weight which should be applied when calculating the overall priority, to the frequency/popularity of the character substitution (i.e. character changed from current word to candidate word).</param>
        /// <param name="allWordsTrieRoot">The root of a character trie containing all the words in the graph.</param>
        /// <param name="fromCharacterFrequencies">A FrequencyTable containing the number of times each character is the 'from' character in a substitution between adjacent words (i.e. represented by an edge of the graph).</param>
        /// <param name="characterSubstitutionFrequencies">A FrequencyTable containing the number of times each pair of characters in a substitution between adjacent words (i.e. represented by an edge of the graph) occurs.</param>
        /// <exception cref="System.ArgumentException">Parameter 'maximumSourceWordToCandidateWordDistance' is less than 0.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'sourceWordToCandidateWordDistanceWeight' is less than 0.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'numberOfCharactersMatchingDestinationWeight' is less than 0.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'popularityOfChangeToCharacterWeight' is less than 0.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'popularityOfCharacterChangeWeight' is less than 0.</exception>
        /// <exception cref="System.ArgumentException">At least one of parameters 'sourceWordToCandidateWordDistanceWeight', 'numberOfCharactersMatchingDestinationWeight', 'popularityOfChangeToCharacterWeight', and 'popularityOfCharacterChangeWeight' must be greater than 0.</exception>
        public CandidateWordPriorityCalculator(Int32 maximumSourceWordToCandidateWordDistance, Int32 sourceWordToCandidateWordDistanceWeight, Int32 numberOfCharactersMatchingDestinationWeight, Int32 popularityOfChangeToCharacterWeight, Int32 popularityOfCharacterChangeWeight, Dictionary<Char, TrieNode<Char>> allWordsTrieRoot, FrequencyTable<Char> fromCharacterFrequencies, FrequencyTable<CharacterSubstitution> characterSubstitutionFrequencies)
        {
            if (maximumSourceWordToCandidateWordDistance < 1)
                throw new ArgumentException("Parameter 'maximumSourceWordToCandidateWordDistance' must be greater than or equal to 1.", "maximumSourceWordToCandidateWordDistance");
            if (sourceWordToCandidateWordDistanceWeight < 0)
                throw new ArgumentException("Parameter 'sourceWordToCandidateWordDistanceWeight' must be greater than or equal to 0.", "sourceWordToCandidateWordDistanceWeight");
            if (numberOfCharactersMatchingDestinationWeight < 0)
                throw new ArgumentException("Parameter 'numberOfCharactersMatchingDestinationWeight' must be greater than or equal to 0.", "numberOfCharactersMatchingDestinationWeight");
            if (popularityOfChangeToCharacterWeight < 0)
                throw new ArgumentException("Parameter 'popularityOfChangeToCharacterWeight' must be greater than or equal to 0.", "popularityOfChangeToCharacterWeight");
            if (popularityOfCharacterChangeWeight < 0)
                throw new ArgumentException("Parameter 'popularityOfCharacterChangeWeight' must be greater than or equal to 0.", "popularityOfCharacterChangeWeight");
            if (sourceWordToCandidateWordDistanceWeight == 0 && numberOfCharactersMatchingDestinationWeight == 0 && popularityOfChangeToCharacterWeight == 0 && popularityOfCharacterChangeWeight == 0)
                throw new ArgumentException("At least one of parameters 'sourceWordToCandidateWordDistanceWeight', 'numberOfCharactersMatchingDestinationWeight', 'popularityOfChangeToCharacterWeight', and 'popularityOfCharacterChangeWeight' must be greater than 0.");

            // Initialize priority functions and weights
            priorityFunctions = new List<Func<String, String, String, Int32, Double>>();
            priorityFunctionWeights = new List<Int32>();
            functionWeightsTotal = 0;
            priorityFunctionWeights.Add(sourceWordToCandidateWordDistanceWeight);
            priorityFunctions.Add(CalculateSourceWordToCandidateWordDistancePriority);
            functionWeightsTotal += Convert.ToInt64(sourceWordToCandidateWordDistanceWeight);
            priorityFunctionWeights.Add(numberOfCharactersMatchingDestinationWeight);
            priorityFunctions.Add(CalculateNumberOfCharactersMatchingDestinationPriority);
            functionWeightsTotal += Convert.ToInt64(numberOfCharactersMatchingDestinationWeight);
            priorityFunctionWeights.Add(popularityOfChangeToCharacterWeight);
            priorityFunctions.Add(CalculatePopularityOfChangeToCharacterPriority);
            functionWeightsTotal += Convert.ToInt64(popularityOfChangeToCharacterWeight);
            priorityFunctionWeights.Add(popularityOfCharacterChangeWeight);
            priorityFunctions.Add(CalculatePopularityOfCharacterChangePriority);
            functionWeightsTotal += Convert.ToInt64(popularityOfCharacterChangeWeight);

            this.maximumSourceWordToCandidateWordDistance = maximumSourceWordToCandidateWordDistance;
            this.allWordsTrieRoot = allWordsTrieRoot;
            this.fromCharacterFrequencies = fromCharacterFrequencies;
            this.characterSubstitutionFrequencies = characterSubstitutionFrequencies;
            wordUtilities = new WordUtilities();

            PopulateMaximumFrequencyMembers();
        }

        /// <summary>
        /// Calculates the priority of traversing to the specified candidate word.
        /// </summary>
        /// <param name="currentWord">The current word being traversed from.</param>
        /// <param name="candidateWord">The candidate word to calculate the priority for.</param>
        /// <param name="destinationWord">The destination word.</param>
        /// <param name="distanceFromSourceToCandidateWord">The distance (in graph edges) from the source word, to the candidate word.</param>
        /// <returns>A number between 0.0 and 1.0 (inclusive) representing the priority to traverse to the candidate word.  A value of 0.0 represents highest priority, and 1.0 lowest priority.</returns>
        /// <exception cref="System.ArgumentException">The length of parameter 'currentWord' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">The length of parameter 'candidateWord' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">The length of parameter 'destinationWord' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">Parameters 'currentWord' and 'candidateWord' have differing lengths.</exception>
        /// <exception cref="System.ArgumentException">Parameters 'candidateWord' and 'destinationWord' have differing lengths.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'distanceFromSourceToCandidateWord' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'distanceFromSourceToCandidateWord' is greater than member 'maximumSourceWordToCandidateWordDistance'.</exception>
        public Double CalculatePriority(String currentWord, String candidateWord, String destinationWord, Int32 distanceFromSourceToCandidateWord)
        {
            if (currentWord.Length < 1)
                throw new ArgumentException("Parameter 'currentWord' must be greater than or equal to 1 character in length.", "currentWord");
            if (candidateWord.Length < 1)
                throw new ArgumentException("Parameter 'candidateWord' must be greater than or equal to 1 character in length.", "candidateWord");
            if (destinationWord.Length < 1)
                throw new ArgumentException("Parameter 'destinationWord' must be greater than or equal to 1 character in length.", "destinationWord");
            if (currentWord.Length != candidateWord.Length)
                throw new ArgumentException("Parameters 'currentWord' and 'candidateWord' must have the same length.", "candidateWord");
            if (candidateWord.Length != destinationWord.Length)
                throw new ArgumentException("Parameters 'candidateWord' and 'destinationWord' must have the same length.", "destinationWord");
            if (distanceFromSourceToCandidateWord < 1)
                throw new ArgumentException("Parameter 'distanceFromSourceToCandidateWord' must be greater than or equal to 1.", "distanceFromSourceToCandidateWord");

            Double priority = 0.0;
            for (Int32 currentPriorityFunctionIndex = 0; currentPriorityFunctionIndex < priorityFunctions.Count; currentPriorityFunctionIndex++)
            {
                Double currentFunctionBasePriority = priorityFunctions[currentPriorityFunctionIndex].Invoke(currentWord, candidateWord, destinationWord, distanceFromSourceToCandidateWord);
                Double currentFunctionWeightedPriority = currentFunctionBasePriority * (Convert.ToDouble(priorityFunctionWeights[currentPriorityFunctionIndex]) / Convert.ToDouble(functionWeightsTotal));
                priority += currentFunctionWeightedPriority;
            }

            return priority;
        }

        #region Private/Protected Methods

        /// <summary>
        /// Calculates the priority based on the distance from the source word to the candidate word.
        /// </summary>
        /// <param name="currentWord">The current word being traversed from.</param>
        /// <param name="candidateWord">The candidate word to calculate the priority for.</param>
        /// <param name="destinationWord">The destination word.</param>
        /// <param name="distanceFromSourceToCandidateWord">The distance (in graph edges) from the source word, to the candidate word.</param>
        /// <returns>The priority.</returns>
        /// <exception cref="System.ArgumentException">Parameter 'distanceFromSourceToCandidateWord' is greater than member 'maximumSourceWordToCandidateWordDistance'.</exception>
        protected Double CalculateSourceWordToCandidateWordDistancePriority(String currentWord, String candidateWord, String destinationWord, Int32 distanceFromSourceToCandidateWord)
        {
            if (distanceFromSourceToCandidateWord > maximumSourceWordToCandidateWordDistance)
                throw new ArgumentException("Parameter 'distanceFromSourceToCandidateWord' cannot be greater than member 'maximumSourceWordToCandidateWordDistance'.", "distanceFromSourceToCandidateWord");

            return Convert.ToDouble(distanceFromSourceToCandidateWord) / Convert.ToDouble(maximumSourceWordToCandidateWordDistance);
        }

        /// <summary>
        /// Calculates the priority based on the number of matching characters between the candidate word and the destination word.
        /// </summary>
        /// <param name="currentWord">The current word being traversed from.</param>
        /// <param name="candidateWord">The candidate word to calculate the priority for.</param>
        /// <param name="destinationWord">The destination word.</param>
        /// <param name="distanceFromSourceToCandidateWord">The distance (in graph edges) from the source word, to the candidate word.</param>
        /// <returns>The priority.</returns>
        protected Double CalculateNumberOfCharactersMatchingDestinationPriority(String currentWord, String candidateWord, String destinationWord, Int32 distanceFromSourceToCandidateWord)
        {
            Int32 matchingCharacterCount = 0;
            for (Int32 currentCharacterIndex = 0; currentCharacterIndex < candidateWord.Length; currentCharacterIndex++)
            {
                if (candidateWord[currentCharacterIndex] == destinationWord[currentCharacterIndex])
                    matchingCharacterCount++;
            }

            return 1.0 - (Convert.ToDouble(matchingCharacterCount) / Convert.ToDouble(candidateWord.Length));
        }

        /// <summary>
        /// Calculates the priority based on the frequency that the character changed to in the candidate word is the 'changed from' character in a substitution between adjacent words.
        /// </summary>
        /// <param name="currentWord">The current word being traversed from.</param>
        /// <param name="candidateWord">The candidate word to calculate the priority for.</param>
        /// <param name="destinationWord">The destination word.</param>
        /// <param name="distanceFromSourceToCandidateWord">The distance (in graph edges) from the source word, to the candidate word.</param>
        /// <returns>The priority.</returns>
        protected Double CalculatePopularityOfChangeToCharacterPriority(String currentWord, String candidateWord, String destinationWord, Int32 distanceFromSourceToCandidateWord)
        {
            Char changeToCharacter = wordUtilities.FindDifferingCharacters(currentWord, candidateWord).Item2;
            Int32 fromCharacterFrequency = fromCharacterFrequencies.GetFrequency(changeToCharacter);

            return 1.0 - (Convert.ToDouble(fromCharacterFrequency) / Convert.ToDouble(maximumFromCharacterFrequency));
        }

        /// <summary>
        /// Calculates the priority based on the frequency/popularity of the character substitution (i.e. character changed from current word to candidate word).
        /// </summary>
        /// <param name="currentWord">The current word being traversed from.</param>
        /// <param name="candidateWord">The candidate word to calculate the priority for.</param>
        /// <param name="destinationWord">The destination word.</param>
        /// <param name="distanceFromSourceToCandidateWord">The distance (in graph edges) from the source word, to the candidate word.</param>
        /// <returns>The priority.</returns>
        protected Double CalculatePopularityOfCharacterChangePriority(String currentWord, String candidateWord, String destinationWord, Int32 distanceFromSourceToCandidateWord)
        {
            Tuple<Char, Char> changedCharacters = wordUtilities.FindDifferingCharacters(currentWord, candidateWord);
            Char changedFromCharacter = changedCharacters.Item1;
            Char changedToCharacter = changedCharacters.Item2;
            Int32 characterSubstitutionFrequency = characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution(changedFromCharacter, changedToCharacter));

            return 1.0 - (Convert.ToInt64(characterSubstitutionFrequency) / Convert.ToDouble(maximumCharacterSubstitutionFrequency));
        }

        /// <summary>
        /// Populates members 'maximumFromCharacterFrequency' and 'maximumCharacterSubstitutionFrequency' from the frequency table data structures.
        /// </summary>
        protected void PopulateMaximumFrequencyMembers()
        {
            maximumFromCharacterFrequency = 0;
            foreach(KeyValuePair<Char, Int32> currentKeyValuePair in fromCharacterFrequencies)
            {
                if (currentKeyValuePair.Value > maximumFromCharacterFrequency)
                    maximumFromCharacterFrequency = currentKeyValuePair.Value;
            } 
            maximumCharacterSubstitutionFrequency = 0;
            foreach (KeyValuePair<CharacterSubstitution, Int32> currentKeyValuePair in characterSubstitutionFrequencies)
            {
                if (currentKeyValuePair.Value > maximumCharacterSubstitutionFrequency)
                    maximumCharacterSubstitutionFrequency = currentKeyValuePair.Value;
            }
        }

        #endregion
    }
}
