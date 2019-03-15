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

namespace Algorithms
{
    /// <summary>
    /// Finds a path between two words in a graph of adjacent words (i.e. words which differ by one character), using variations of breadth-first search, Dijkstra's algorithn, and A*.
    /// </summary>
    public class AdjacentWordGraphPathFinder
    {
        /// <summary>Used to calculate the priority assigned to candidate words.</summary>
        protected CandidateWordPriorityCalculator priorityCalculator;
        /// <summary>Used to find adjacent vertices in the graph of words.</summary>
        protected CharacterTrieUtilities trieUtilities;
        /// <summary>The root node of a character trie containing all the words in the graph.</summary>
        protected Dictionary<Char, TrieNode<Char>> wordDictionaryTrieRoot;

        /// <summary>
        /// Initialises a new instance of the Algorithms.AdjacentWordGraphPathFinder class.
        /// </summary>
        /// <param name="priorityCalculator">Used to calculate the priority assigned to candidate words.</param>
        /// <param name="trieUtilities">Used to find adjacent vertices in the graph of words.</param>
        /// <param name="wordDictionaryTrieRoot">The root node of a character trie containing all the words in the graph.</param>
        /// <remarks>Note that parameter 'wordDictionaryTrieRoot' should be the same character trie root that is set on the constructor of parameter 'priorityCalculator'.</remarks>
        public AdjacentWordGraphPathFinder(CandidateWordPriorityCalculator priorityCalculator, CharacterTrieUtilities trieUtilities, Dictionary<Char, TrieNode<Char>> wordDictionaryTrieRoot)
        {
            this.priorityCalculator = priorityCalculator;
            this.trieUtilities = trieUtilities;
            this.wordDictionaryTrieRoot = wordDictionaryTrieRoot;
        }

        /// <summary>
        /// Finds a path between two words in a graph of adjacent words using A*, with priorities of candidate words assigned using class member 'priorityCalculator'.
        /// </summary>
        /// <param name="sourceWord">The source word to find a path from.</param>
        /// <param name="destinationWord">The destination word to find a path to.</param>
        /// <param name="numberOfEdgesExplored">Is populated with the number of graph edges explored while finding a path.</param>
        /// <returns>A linked list containing the path from the source word to the destination word.  The list will be enpty if no path was found.</returns>
        /// <exception cref="System.ArgumentException">Parameter 'sourceWord' is the same word as parameter 'destinationWord'.</exception>
        /// <exception cref="System.ArgumentException">The length of parameter 'sourceWord' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'sourceWord' has different length to parameter 'destinationWord'.</exception>
        /// <remarks>The graph of words is not pre-built, but rather is generated dynamically, using data structures in class members 'trieUtilities' and 'wordDictionaryTrieRoot'.</remarks>
        public LinkedList<String> FindPathViaAStar(String sourceWord, String destinationWord, ref Int32 numberOfEdgesExplored)
        {
            CheckFindPathParameters(sourceWord, destinationWord);

            // Priority queue used to decide which vertices (words) to traverse to next
            PriorityQueue<String> priorityQueue = new PriorityQueue<String>();
            // The set of vertices (words) which have been completely explored
            HashSet<String> visitedVertices = new HashSet<String>();
            // For the shortest path to each vertex (represented by the key string), the vertex which is prior to the vertex in the shortest path (represented by the value string)
            Dictionary<String, String> previousVertices = new Dictionary<String, String>();
            // Holds a map of a vertex (word), and the shortest distance found so far between the source and the vertex (i.e. the g(n) score in A*)
            Dictionary<String, Int32> shortestDistancesFromSource = new Dictionary<String, Int32>();
            Boolean pathFound = false;

            priorityQueue.Enqueue(sourceWord, 0.0);
            shortestDistancesFromSource.Add(sourceWord, 0);
            numberOfEdgesExplored = 0;

            // Continue while there are still vertices in the priority queue and a path has not been found
            while (priorityQueue.Count > 0 && pathFound == false)
            {
                String currentWord = priorityQueue.DequeueMinimum();
                Int32 distanceFromSourceToCurrentVertex = shortestDistancesFromSource[currentWord];
                foreach (String currentCandidateWord in trieUtilities.FindAdjacentWords(wordDictionaryTrieRoot, currentWord))
                {
                    if (currentCandidateWord.Equals(destinationWord))
                    {
                        previousVertices.Add(currentCandidateWord, currentWord);
                        numberOfEdgesExplored++;
                        pathFound = true;
                        break;
                    }
                    else
                    {
                        if (visitedVertices.Contains(currentCandidateWord) == false)
                        {
                            Double candidateWordPriority = priorityCalculator.CalculatePriority(currentWord, currentCandidateWord, destinationWord, distanceFromSourceToCurrentVertex + 1);
                            if (priorityQueue.Contains(currentCandidateWord))
                            {
                                // If the candidate word is already in the priority queue and newly calculated priority is less than the existing priority (i.e. higher priority since maximum priority is 0.0), then update the queue and other data structures
                                if (candidateWordPriority < priorityQueue.GetPriorityForItem(currentCandidateWord))
                                {
                                    priorityQueue.Remove(currentCandidateWord);
                                    priorityQueue.Enqueue(currentCandidateWord, candidateWordPriority);
                                    previousVertices.Remove(currentCandidateWord);
                                    previousVertices.Add(currentCandidateWord, currentWord);
                                    shortestDistancesFromSource.Remove(currentCandidateWord);
                                    shortestDistancesFromSource.Add(currentCandidateWord, distanceFromSourceToCurrentVertex + 1);
                                }
                            }
                            // If the candidate word is not already enqueued, then add it to the queue and other data structures
                            else
                            {
                                priorityQueue.Enqueue(currentCandidateWord, candidateWordPriority);
                                previousVertices.Add(currentCandidateWord, currentWord);
                                shortestDistancesFromSource.Add(currentCandidateWord, distanceFromSourceToCurrentVertex + 1);
                            }
                            numberOfEdgesExplored++;
                        }
                    }
                }

                visitedVertices.Add(currentWord);
            }

            // Reconstruct the path from 'previousVertices'
            LinkedList<String> returnList = new LinkedList<String>();
            if (pathFound == true)
            {
                String currentWord = destinationWord;
                while (previousVertices.ContainsKey(currentWord))
                {
                    returnList.AddFirst(currentWord);
                    currentWord = previousVertices[currentWord];
                }
                returnList.AddFirst(currentWord);
            }

            return returnList;
        }

        /// <summary>
        /// Finds the shortest path between two words in a graph of adjacent words, using Dijkstra's algorithm.
        /// </summary>
        /// <param name="sourceWord">The source word to find a path from.</param>
        /// <param name="destinationWord">The destination word to find a path to.</param>
        /// <param name="numberOfEdgesExplored">Is populated with the number of graph edges explored while finding a path.</param>
        /// <returns>A linked list containing the shortest path from the source word to the destination word.  The list will be enpty if no path was found.</returns>
        /// <exception cref="System.ArgumentException">Parameter 'sourceWord' is the same word as parameter 'destinationWord'.</exception>
        /// <exception cref="System.ArgumentException">The length of parameter 'sourceWord' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'sourceWord' has different length to parameter 'destinationWord'.</exception>
        /// <remarks>The graph of words is not pre-built, but rather is generated dynamically, using data structures in class members 'trieUtilities' and 'wordDictionaryTrieRoot'.</remarks>
        public LinkedList<String> FindShortestPathViaDijkstrasAlgorithm(String sourceWord, String destinationWord, ref Int32 numberOfEdgesExplored)
        {
            CheckFindPathParameters(sourceWord, destinationWord);

            // Priority queue used to decide which vertices (words) to traverse to next
            PriorityQueue<String> priorityQueue = new PriorityQueue<String>();
            // The set of vertices (words) which have been completely explored
            HashSet<String> visitedVertices = new HashSet<String>();
            // For the shortest path to each vertex (represented by the key string), the vertex which is prior to the vertex in the shortest path (represented by the value string)
            Dictionary<String, String> previousVertices = new Dictionary<String, String>();
            // Holds a map of a vertex (word), and the shortest distance found so far between the source and the vertex
            Dictionary<String, Int32> shortestDistancesFromSource = new Dictionary<String, Int32>();
            // Denominator to divide the distance from source vertex by, to create a priority between 0.0 and 1.0 (using an arbitary large number still means that a distance from source of 5 will be prioritized over a distance of 6... i.e. 5/1000 is less than 6/1000, and lower numbers have higher priority in the PriorityQueue<T> class)
            const Double priorityDenominator = 1000.0;

            priorityQueue.Enqueue(sourceWord, 0.0);
            shortestDistancesFromSource.Add(sourceWord, 0);
            numberOfEdgesExplored = 0;

            // Continue while there are still vertices in the priority queue
            while (priorityQueue.Count > 0)
            {
                String currentWord = priorityQueue.DequeueMinimum();
                Int32 distanceFromSourceToCurrentVertex = shortestDistancesFromSource[currentWord];
                foreach (String currentCandidateWord in trieUtilities.FindAdjacentWords(wordDictionaryTrieRoot, currentWord))
                {
                    if (visitedVertices.Contains(currentCandidateWord) == false)
                    {
                        Int32 currentDistanceToCandidateWord = distanceFromSourceToCurrentVertex + 1;
                        Double currentCandidateWordPriority = Convert.ToDouble(currentDistanceToCandidateWord) / priorityDenominator;
                        if (priorityQueue.Contains(currentCandidateWord))
                        {
                            // If the candidate word is already in the priority queue and newly calculated distance from source is less than the existing distance from source, then update the queue and other data structures
                            if (currentCandidateWordPriority < priorityQueue.GetPriorityForItem(currentCandidateWord))
                            {
                                priorityQueue.Remove(currentCandidateWord);
                                priorityQueue.Enqueue(currentCandidateWord, currentCandidateWordPriority);
                                previousVertices.Remove(currentCandidateWord);
                                previousVertices.Add(currentCandidateWord, currentWord);
                                shortestDistancesFromSource.Remove(currentCandidateWord);
                                shortestDistancesFromSource.Add(currentCandidateWord, currentDistanceToCandidateWord);
                            }
                        }
                        // If the candidate word is not already enqueued, then add it to the queue and other data structures
                        else
                        {
                            priorityQueue.Enqueue(currentCandidateWord, currentCandidateWordPriority);
                            previousVertices.Add(currentCandidateWord, currentWord);
                            shortestDistancesFromSource.Add(currentCandidateWord, currentDistanceToCandidateWord);
                        }
                        numberOfEdgesExplored++;
                    }
                }

                visitedVertices.Add(currentWord);

                // If the current word is the destination word, the shortest path has been found
                if (currentWord.Equals(destinationWord))
                    break;
            }

            // Reconstruct the path from 'previousVertices'
            LinkedList<String> returnList = new LinkedList<String>();
            if (previousVertices.ContainsKey(destinationWord))
            {
                String currentWord = destinationWord;
                while (previousVertices.ContainsKey(currentWord))
                {
                    returnList.AddFirst(currentWord);
                    currentWord = previousVertices[currentWord];
                }
                returnList.AddFirst(currentWord);
            }

            return returnList;
        }

        /// <summary>
        /// Finds a path between two words in a graph of adjacent words, using a bidirectional breadth-first search.
        /// </summary>
        /// <param name="sourceWord">The source word to find a path from.</param>
        /// <param name="destinationWord">The destination word to find a path to.</param>
        /// <param name="numberOfEdgesExplored">Is populated with the number of graph edges explored while finding a path.</param>
        /// <returns>A linked list containing the path from the source word to the destination word.  The list will be enpty if no path was found.</returns>
        /// <exception cref="System.ArgumentException">Parameter 'sourceWord' is the same word as parameter 'destinationWord'.</exception>
        /// <exception cref="System.ArgumentException">The length of parameter 'sourceWord' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">Parameter 'sourceWord' has different length to parameter 'destinationWord'.</exception>
        /// <remarks>The graph of words is not pre-built, but rather is generated dynamically, using data structures in class members 'trieUtilities' and 'wordDictionaryTrieRoot'.</remarks>
        public LinkedList<String> FindPathViaBidirectionalBreadthFirstSearch(String sourceWord, String destinationWord, ref Int32 numberOfEdgesExplored)
        {
            CheckFindPathParameters(sourceWord, destinationWord);

            // The vertex prior to each vertex in the forward path
            Dictionary<String, String> forwardPathPreviousVertices = new Dictionary<String, String>();
            // The vertex prior to each vertex in the backward path
            Dictionary<String, String> backwardPathPreviousVertices = new Dictionary<String, String>();
            // Vertices still to process in the forward path
            Queue<String> forwardPathTraverseQueue = new Queue<String>();
            // Vertices still to process in the backward path
            Queue<String> backwardPathTraverseQueue = new Queue<String>();

            forwardPathPreviousVertices.Add(sourceWord, null);
            backwardPathPreviousVertices.Add(destinationWord, null);
            forwardPathTraverseQueue.Enqueue(sourceWord);
            backwardPathTraverseQueue.Enqueue(destinationWord);
            String currentForwardPathVertex = sourceWord;
            String currentBackwardPathVertex = destinationWord;
            String pathJoinVertex = null;
            numberOfEdgesExplored = 0;

            while ((forwardPathTraverseQueue.Count > 0 || backwardPathTraverseQueue.Count > 0) && pathJoinVertex == null)
            {
                // Step in the forward path
                if (forwardPathTraverseQueue.Count > 0)
                {
                    currentForwardPathVertex = forwardPathTraverseQueue.Dequeue();
                    foreach (String currentCandidateVertex in trieUtilities.FindAdjacentWords(wordDictionaryTrieRoot, currentForwardPathVertex))
                    {
                        if (backwardPathPreviousVertices.ContainsKey(currentCandidateVertex))
                        {
                            forwardPathPreviousVertices.Add(currentCandidateVertex, currentForwardPathVertex);
                            pathJoinVertex = currentCandidateVertex;
                            numberOfEdgesExplored++;
                            break;
                        }
                        else
                        {
                            if (forwardPathPreviousVertices.ContainsKey(currentCandidateVertex) == false)
                            {
                                forwardPathPreviousVertices.Add(currentCandidateVertex, currentForwardPathVertex);
                                forwardPathTraverseQueue.Enqueue(currentCandidateVertex);
                                numberOfEdgesExplored++;
                            }
                        }
                    }
                }
                // Step in the backward path
                if (backwardPathTraverseQueue.Count > 0)
                {
                    currentBackwardPathVertex = backwardPathTraverseQueue.Dequeue();
                    foreach (String currentCandidateVertex in trieUtilities.FindAdjacentWords(wordDictionaryTrieRoot, currentBackwardPathVertex))
                    {
                        if (forwardPathPreviousVertices.ContainsKey(currentCandidateVertex))
                        {
                            backwardPathPreviousVertices.Add(currentCandidateVertex, currentBackwardPathVertex);
                            pathJoinVertex = currentCandidateVertex;
                            numberOfEdgesExplored++;
                            break;
                        }
                        else
                        {
                            if (backwardPathPreviousVertices.ContainsKey(currentCandidateVertex) == false)
                            {
                                backwardPathPreviousVertices.Add(currentCandidateVertex, currentBackwardPathVertex);
                                backwardPathTraverseQueue.Enqueue(currentCandidateVertex);
                                numberOfEdgesExplored++;
                            }
                        }
                    }
                }
            }

            // Reconstruct the path from 'forwardPathPreviousVertices' and 'backwardPathPreviousVertices'
            LinkedList<String> forwardPath = new LinkedList<String>();
            if (pathJoinVertex != null)
            {
                // Reconstruct the forward path
                String currentWord = pathJoinVertex;
                while (currentWord != null)
                {
                    forwardPath.AddFirst(currentWord);
                    currentWord = forwardPathPreviousVertices[currentWord];
                }
                // Reconstruct the backward path
                LinkedList<String> backwardPath = new LinkedList<String>();
                currentWord = backwardPathPreviousVertices[pathJoinVertex];
                while (currentWord != null)
                {
                    backwardPath.AddLast(currentWord);
                    currentWord = backwardPathPreviousVertices[currentWord];
                }
                // Join the two paths
                foreach (String currentVertex in backwardPath)
                    forwardPath.AddLast(currentVertex);
            }

            return forwardPath;
        }

        /// <summary>
        /// Finds the longest path from a source word through a graph of adjacent words, using depth-first search.
        /// </summary>
        /// <param name="sourceWord">The source word to find a path from.</param>
        /// <returns>A linked list containing the longest path from the source word.</returns>
        /// <remarks>The graph of words is not pre-built, but rather is generated dynamically, using data structures in class members 'trieUtilities' and 'wordDictionaryTrieRoot'.  Also, use caution calling this method on anything but small graphs, as it attempts to traverse all possible paths.</remarks>
        public LinkedList<String> FindLongestPathFrom(String sourceWord)
        {
            if (sourceWord.Length == 0)
                throw new ArgumentException("Parameter 'sourceWord' cannot be an empty string.", "sourceWord");

            LinkedList<String> currentPath = new LinkedList<String>();
            HashSet<String> currentPathMembers = new HashSet<String>();
            LinkedList<String> longestPath = new LinkedList<String>();
            currentPath.AddLast(sourceWord);
            currentPathMembers.Add(sourceWord);
            FindLongestPathFromRecurse(sourceWord, currentPath, currentPathMembers, longestPath);

            return longestPath;
        }

        #region Private/Protected Methods

        /// <summary>
        /// Checks the 'sourceWord' and 'destinationWord' parameters for methods which find a path in the graph.
        /// </summary>
        /// <param name="sourceWord">The 'sourceWord' parameter in the FindPath*() method.</param>
        /// <param name="destinationWord">The 'destinationWord' parameter in the FindPath*() method.</param>
        protected void CheckFindPathParameters(String sourceWord, String destinationWord)
        {
            if (sourceWord.Equals(destinationWord))
                throw new ArgumentException("Parameter 'destinationWord' cannot be the same as parameter 'sourceWord'.", "destinationWord");
            if (sourceWord.Length < 1)
                throw new ArgumentException("The length of parameter 'sourceWord' must be greater than or equal to 1.", "sourceWord");
            if (sourceWord.Length != destinationWord.Length)
                throw new ArgumentException("Parameter 'sourceWord' must have the same length as parameter 'destinationWord'.", "destinationWord");
        }

        /// <summary>
        /// Recurses through a graph of adjacent words and finds the longest path in the graph.
        /// </summary>
        /// <param name="currentWord">The current word recursed to.</param>
        /// <param name="currentPath">The words in the current path from the source (including 'currentWord').</param>
        /// <param name="currentPathMembers">The words in the current path from the source (including 'currentWord').</param>
        /// <param name="longestPath">The longest path found so far.</param>
        protected void FindLongestPathFromRecurse(String currentWord, LinkedList<String> currentPath, HashSet<String> currentPathMembers, LinkedList<String> longestPath)
        {
            if (currentPath.Count > longestPath.Count)
            {
                longestPath.Clear();
                foreach (String currentPathWord in currentPath)
                    longestPath.AddLast(currentPathWord);
            }

            foreach (String nextWord in trieUtilities.FindAdjacentWords(wordDictionaryTrieRoot, currentWord))
            {
                if (currentPathMembers.Contains(nextWord) == false)
                {
                    currentPath.AddLast(nextWord);
                    currentPathMembers.Add(nextWord);
                    FindLongestPathFromRecurse(nextWord, currentPath, currentPathMembers, longestPath);
                    currentPathMembers.Remove(nextWord);
                    currentPath.RemoveLast();
                }
            }
        }

        #endregion
    }
}
