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

namespace Algorithms
{
    /// <summary>
    /// Utility methods for tries of characters.
    /// </summary>
    public class CharacterTrieUtilities
    {
        /// <summary>
        /// Initialises a new instance of the Algorithms.CharacterTrieUtilities class.
        /// </summary>
        public CharacterTrieUtilities()
        {
        }

        /// <summary>
        /// Returns all words which are adjacent to the inputted word.
        /// </summary>
        /// <param name="trieRoot">The root of the trie.</param>
        /// <param name="inputWord">The word to find the adjacent words of.</param>
        /// <returns>The adjacent words.</returns>
        /// <remarks>Adjacent words are words which have the same length as the inputted word, but differ by one character, e.g. 'time' and 'tame'.</remarks>
        public IEnumerable<String> FindAdjacentWords(Dictionary<Char, TrieNode<Char>> trieRoot, String inputWord)
        {
            if (inputWord.Length == 0)
                throw new ArgumentException("Parameter 'inputWord' cannot be an empty string.", "word");

            LinkedList<Char> currentPathInTrie = new LinkedList<Char>();
            for (Int32 currentWildcardIndex = 0; currentWildcardIndex < inputWord.Length; currentWildcardIndex++)
            {
                if (currentWildcardIndex == 0)
                {
                    foreach (TrieNode<Char> currentNode in trieRoot.Values)
                    {
                        currentPathInTrie.AddLast(currentNode.Item);
                        foreach (String currentResult in FindAdjacentWordsRecurse(inputWord, 1, currentWildcardIndex, currentPathInTrie, currentNode))
                        {
                            yield return currentResult;
                        }
                        currentPathInTrie.RemoveLast();
                    }
                }
                else
                {
                    if (trieRoot.ContainsKey(inputWord[0]))
                    {
                        currentPathInTrie.AddLast(inputWord[0]);
                        foreach (String currentResult in FindAdjacentWordsRecurse(inputWord, 1, currentWildcardIndex, currentPathInTrie, trieRoot[inputWord[0]]))
                        {
                            yield return currentResult;
                        }
                        currentPathInTrie.RemoveLast();
                    }
                }
            }
        }

        #region Private/Protected Methods

        /// <summary>
        /// Recurses through a trie to find all words which are adjacent to the inputted word.
        /// </summary>
        /// <param name="inputWord">The word to find the adjacent words of.</param>
        /// <param name="nextIndex">The next index within the inputted word to handle.</param>
        /// <param name="wildcardIndex">The index within the word to use as a 'wildcard' (i.e. to allow substitution of).</param>
        /// <param name="currentPathInTrie">The character already added to the current adjacent word.</param>
        /// <param name="currentNode">The current node within the trie.</param>
        /// <returns>The adjacent words.</returns>
        protected IEnumerable<String> FindAdjacentWordsRecurse(String inputWord, Int32 nextIndex, Int32 wildcardIndex, LinkedList<Char> currentPathInTrie, TrieNode<Char> currentNode)
        {
            // Handle the case that the current index is the last in the input word
            if (nextIndex == inputWord.Length)
            {
                if (currentNode is EndMarkerTrieNode<Char>)
                {
                    String newWord = String.Concat(currentPathInTrie);
                    newWord.Reverse();
                    if (inputWord.Equals(newWord) == false)
                        yield return newWord;
                }
            }
            else
            {
                if (nextIndex == wildcardIndex)
                {
                    foreach (KeyValuePair<Char, TrieNode<Char>> currentChild in currentNode.Children)
                    {
                        currentPathInTrie.AddLast(currentChild.Value.Item);
                        foreach (String currentResult in FindAdjacentWordsRecurse(inputWord, nextIndex + 1, wildcardIndex, currentPathInTrie, currentChild.Value))
                        {
                            yield return currentResult;
                        }
                        currentPathInTrie.RemoveLast();
                    }
                }
                else
                {
                    if (currentNode.ChildExists(inputWord[nextIndex]))
                    {
                        currentPathInTrie.AddLast(inputWord[nextIndex]);
                        TrieNode<Char> nextNode = currentNode.GetChildNode(inputWord[nextIndex]);
                        foreach (String currentResult in FindAdjacentWordsRecurse(inputWord, nextIndex + 1, wildcardIndex, currentPathInTrie, nextNode))
                        {
                            yield return currentResult;
                        }
                        currentPathInTrie.RemoveLast();
                    }
                }
            }
        }

        #endregion
    }
}
