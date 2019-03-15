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
    /// Compares the A* graph traversal algorithm with breadth-first search and Dijkstra's algorithm, and allows experimenting with different A* heuristic functions.
    /// </summary>
    public class AStarExperiment
    {
        /// <summary>
        /// Initialises a new instance of the Algorithms.AStarExperiment class.
        /// </summary>
        public AStarExperiment()
        {
        }

        /// <summary>
        /// Runs the graph traversal comparison.
        /// </summary>
        public void Run()
        {
            // The path to a file containing a dictionary of words
            const String dictionaryFilePath = @"C:\Temp\words2.txt";
            // The assumed maximum distance from a source word to a candidate word (used in weighting of g(n) and h(n) scores)
            const Int32 maximumSourceWordToCandidateWordDistance = 30;

            // Setup the word dictionary tree and other supporting data structures
            Dictionary<Char, TrieNode<Char>> allWordsTrieRoot = new Dictionary<Char, TrieNode<Char>>();
            HashSet<String> allWords = new HashSet<String>();
            FrequencyTable<Char> fromCharacterFrequencies = new FrequencyTable<Char>();
            FrequencyTable<CharacterSubstitution> characterSubstitutionFrequencies = new FrequencyTable<CharacterSubstitution>();

            // Populate the word dictionary tree and other supporting data structures
            System.IO.StreamReader underlyingReader = new System.IO.StreamReader(dictionaryFilePath);
            Algorithms.StreamReader reader = new Algorithms.StreamReader(underlyingReader);
            CharacterTrieBuilder trieBuilder = new CharacterTrieBuilder();
            Func<String, Boolean> wordFilterFunction = new Func<String, Boolean>((inputString) =>
            {
                foreach (Char currentCharacter in inputString)
                {
                    if (Char.IsLetter(currentCharacter) == false)
                        return false;
                }
                if (inputString.Length == 4)
                    return true;
                else
                    return false;
            });
            DataStructureUtilities dataStructureUtils = new DataStructureUtilities();
            dataStructureUtils.PopulateAdjacentWordDataStructures(reader, trieBuilder, wordFilterFunction, allWordsTrieRoot, allWords, fromCharacterFrequencies, characterSubstitutionFrequencies);
            CharacterTrieUtilities trieUtilities = new CharacterTrieUtilities();

            // Setup the test data (word pairs to find paths between)
            List<Tuple<String, String>> testData = new List<Tuple<String, String>>()
            {
                new Tuple<String, String>("role", "band"),
                new Tuple<String, String>("pack", "sill"),
                new Tuple<String, String>("debt", "tyre"),
                new Tuple<String, String>("duct", "grid")
            };

            // Find paths
            foreach (Tuple<String, String> currentWordPair in testData)
            {
                // Setup priority calculator and graph path finder
                Int32 sourceWordToCandidateWordDistanceWeight = 1;
                Int32 numberOfCharactersMatchingDestinationWeight = 0;
                Int32 popularityOfChangeToCharacterWeight = 0;
                Int32 popularityOfCharacterChangeWeight = 0;
                CandidateWordPriorityCalculator priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                AdjacentWordGraphPathFinder pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);

                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("  Finding paths for strings '{0}' and '{1}'", currentWordPair.Item1, currentWordPair.Item2);
                Console.WriteLine("-----------------------------------------------");

                // Find a path using breadth-first search
                Console.WriteLine("  Using breadth-first search...");
                Int32 numberOfEdgesExplored = 0;
                LinkedList<String> path = pathFinder.FindPathViaAStar(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine();

                // Find the shortest path using Dijkstras algorithm
                Console.WriteLine("  Using Dijkstra's algorithm...");
                numberOfEdgesExplored = 0;
                path = pathFinder.FindShortestPathViaDijkstrasAlgorithm(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine();

                // Find the shortest path using bidirectional breadth-first search
                Console.WriteLine("  Using bidirectional breadth-first search...");
                numberOfEdgesExplored = 0; 
                path = pathFinder.FindPathViaBidirectionalBreadthFirstSearch(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine();

                // Find the shortest path using A* ( 50% g(n) and 50% h(n) )
                Console.WriteLine("  Using A* ( 50% g(n) and 50% h(n) )...");
                sourceWordToCandidateWordDistanceWeight = 3;
                numberOfCharactersMatchingDestinationWeight = 1;
                popularityOfChangeToCharacterWeight = 1;
                popularityOfCharacterChangeWeight = 1;
                priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);
                numberOfEdgesExplored = 0;
                path = pathFinder.FindPathViaAStar(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine();

                // Find the shortest path using A* ( 0% g(n) and 100% h(n) )
                Console.WriteLine("  Using A* ( 0% g(n) and 100% h(n) )...");
                sourceWordToCandidateWordDistanceWeight = 0;
                numberOfCharactersMatchingDestinationWeight = 1;
                popularityOfChangeToCharacterWeight = 1;
                popularityOfCharacterChangeWeight = 1;
                priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);
                numberOfEdgesExplored = 0;
                path = pathFinder.FindPathViaAStar(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine();

                // Find the shortest path using A* ( 25% g(n) and 75% h(n) )
                Console.WriteLine("  Using A* ( 25% g(n) and 75% h(n) )...");
                sourceWordToCandidateWordDistanceWeight = 1;
                numberOfCharactersMatchingDestinationWeight = 1;
                popularityOfChangeToCharacterWeight = 1;
                popularityOfCharacterChangeWeight = 1;
                priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);
                numberOfEdgesExplored = 0;
                path = pathFinder.FindPathViaAStar(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine();

                // Find the shortest path using A* ( 25% g(n) and 75% h(n) with custom h(n) weighting )
                Console.WriteLine("  Using A* ( 25% g(n) and 75% h(n) with custom h(n) weighting )...");
                sourceWordToCandidateWordDistanceWeight = 1;
                numberOfCharactersMatchingDestinationWeight = 2;
                popularityOfChangeToCharacterWeight = 1;
                popularityOfCharacterChangeWeight = 0;
                priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);
                numberOfEdgesExplored = 0;
                path = pathFinder.FindPathViaAStar(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine();

                Console.WriteLine();
            }

            return;

            #region Test Code and Utility Routines

            // Read a word from the console and find its adjacent words
            String readWord = "";
            while (true)
            {
                Console.Write("Type a source word: ");
                readWord = Console.ReadLine();
                if (readWord.Equals("q"))
                    break;
                foreach (String currAdjacent in trieUtilities.FindAdjacentWords(allWordsTrieRoot, readWord))
                {
                    Console.WriteLine("  " + currAdjacent);
                }
            }

            // Find the total number of edges and vertices in the graph
            Int32 totalEdges = 0;
            foreach (String currWord in allWords)
            {
                foreach (String currAdj in trieUtilities.FindAdjacentWords(allWordsTrieRoot, currWord))
                    totalEdges++;
            }
            Console.WriteLine("Total edges: " + totalEdges / 2);
            Console.WriteLine("Total vertices: " + allWords.Count);

            // Show contents of frequency tables
            foreach (KeyValuePair<Char, Int32> currKVP in fromCharacterFrequencies)
            {
                Console.WriteLine(currKVP.Key + ": " + currKVP.Value);
            }
            Console.WriteLine("t > b: " + characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('t', 'b')));
            Console.WriteLine("n > f: " + characterSubstitutionFrequencies.GetFrequency(new CharacterSubstitution('n', 'f')));
            foreach (KeyValuePair<CharacterSubstitution, Int32> currKVP in characterSubstitutionFrequencies)
            {
                if (currKVP.Key.ToCharacter == 'y')
                    Console.WriteLine(currKVP.Key.FromCharacter + " > " + currKVP.Key.ToCharacter + ": " + currKVP.Value);
            }
            foreach (String currWord in allWords)
            {
                if (currWord[1] == 'y')
                    Console.WriteLine(currWord);
            }

            // Compare each heuristic function in isolation
            List<Tuple<String, String>> testData2 = new List<Tuple<String, String>>()
            {
                new Tuple<String, String>("role", "band"),
                new Tuple<String, String>("pack", "sill"),
                new Tuple<String, String>("debt", "tyre"),
                new Tuple<String, String>("duct", "grid")
            };

            foreach (Tuple<String, String> currentWordPair in testData2)
            {
                // Setup priority calculator and graph path finder
                Int32 sourceWordToCandidateWordDistanceWeight = 1;
                Int32 numberOfCharactersMatchingDestinationWeight = 0;
                Int32 popularityOfChangeToCharacterWeight = 0;
                Int32 popularityOfCharacterChangeWeight = 0;
                CandidateWordPriorityCalculator priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                AdjacentWordGraphPathFinder pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);
                Int32 numberOfEdgesExplored = 0;
                Int32 maximumSourceWordToCandidateWordDistance2 = 4000;

                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("  Finding paths for strings '{0}' and '{1}'", currentWordPair.Item1, currentWordPair.Item2);
                Console.WriteLine("-----------------------------------------------");

                // Find the shortest path using A* ( 50% g(n) and 50% h(n) )
                Console.WriteLine("  1 0 0...");
                sourceWordToCandidateWordDistanceWeight = 0;
                numberOfCharactersMatchingDestinationWeight = 1;
                popularityOfChangeToCharacterWeight = 0;
                popularityOfCharacterChangeWeight = 0;
                priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance2, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);
                numberOfEdgesExplored = 0;
                LinkedList<String> path = pathFinder.FindPathViaAStar(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine("    Length of path:  {0}", path.Count);
                Console.WriteLine();

                // Find the shortest path using A* ( 0% g(n) and 100% h(n) )
                Console.WriteLine("  0 1 0...");
                sourceWordToCandidateWordDistanceWeight = 0;
                numberOfCharactersMatchingDestinationWeight = 0;
                popularityOfChangeToCharacterWeight = 1;
                popularityOfCharacterChangeWeight = 0;
                priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance2, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);
                numberOfEdgesExplored = 0;
                path = pathFinder.FindPathViaAStar(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine("    Length of path:  {0}", path.Count);
                Console.WriteLine();

                // Find the shortest path using A* ( 25% g(n) and 75% h(n) )
                Console.WriteLine("  0 0 1...");
                sourceWordToCandidateWordDistanceWeight = 0;
                numberOfCharactersMatchingDestinationWeight = 0;
                popularityOfChangeToCharacterWeight = 0;
                popularityOfCharacterChangeWeight = 1;
                priorityCalculator = new CandidateWordPriorityCalculator(maximumSourceWordToCandidateWordDistance2, sourceWordToCandidateWordDistanceWeight, numberOfCharactersMatchingDestinationWeight, popularityOfChangeToCharacterWeight, popularityOfCharacterChangeWeight, allWordsTrieRoot, fromCharacterFrequencies, characterSubstitutionFrequencies);
                pathFinder = new AdjacentWordGraphPathFinder(priorityCalculator, trieUtilities, allWordsTrieRoot);
                numberOfEdgesExplored = 0;
                path = pathFinder.FindPathViaAStar(currentWordPair.Item1, currentWordPair.Item2, ref numberOfEdgesExplored);
                Console.Write("    Path:  ");
                WritePathToConsole(path);
                Console.WriteLine("    Edges explored:  {0}", numberOfEdgesExplored);
                Console.WriteLine("    Length of path:  {0}", path.Count);
                Console.WriteLine();

                Console.WriteLine();
            }

            #endregion
        }

        #region Private/Protected Methods

        protected void WritePathToConsole(LinkedList<String> path)
        {
            if (path.Count == 0)
                throw new ArgumentException("Parameter path is an empty list.", "path");

            LinkedListNode<String> currentNode = path.First;
            while (currentNode.Next != null)
            {
                Console.Write(currentNode.Value + " > ");
                currentNode = currentNode.Next;
            }
            Console.WriteLine(currentNode.Value);
        }

        #endregion
    }
}
