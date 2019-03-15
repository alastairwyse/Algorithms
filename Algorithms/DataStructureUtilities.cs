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
    /// Utility methods for data structures which support implementation of the A* algorithm.
    /// </summary>
    public class DataStructureUtilities
    {
        /// <summary>
        /// Initialises a new instance of the Algorithms.DataStructureUtilities class.
        /// </summary>
        public DataStructureUtilities()
        {
        }

        /// <summary>
        /// Reads a set of words from a stream, and adds the words and statistics pertaining to the words to a set of data structures.
        /// </summary>
        /// <param name="reader">The stream reader to used to read the set of words (allows specifying a mocked reader for unit testing).</param>
        /// <param name="trieBuilder">The trie builder to use to add the words to the trie (allows specifying a mocked builder for unit testing).</param>
        /// <param name="wordFilterFunction">A Func to filter whether or not the specified word should be added to the trie.  Accepts the word as a parameter, and returns a bookean indicating whether that word should be added to the trie.</param>
        /// <param name="allWordsTrieRoot">The root of a character trie to populate with the words.</param>
        /// <param name="allWords">A HashSet to populate with the words.</param>
        /// <param name="fromCharacterFrequencies">A FrequencyTable to populate with the number of times each character is the 'from' character in a substitution.</param>
        /// <param name="characterSubstitutionFrequencies">A FrequencyTable to populate with the number of times each pair of characters in a substitution occur.</param>
        public void PopulateAdjacentWordDataStructures(IStreamReader reader, ICharacterTrieBuilder trieBuilder, Func<String, Boolean> wordFilterFunction, Dictionary<Char, TrieNode<Char>> allWordsTrieRoot, HashSet<String> allWords, FrequencyTable<Char> fromCharacterFrequencies, FrequencyTable<CharacterSubstitution> characterSubstitutionFrequencies)
        {
            // Read all words and add them to the HashSet and trie
            using (reader)
            {
                while (reader.EndOfStream == false)
                {
                    String currentWord = reader.ReadLine();
                    if (wordFilterFunction.Invoke(currentWord) == true)
                    {
                        if (allWords.Contains(currentWord) == false)
                        {
                            allWords.Add(currentWord);
                            trieBuilder.AddWord(allWordsTrieRoot, currentWord, true);
                        }
                    }
                }
            }

            // Populate the frequency tables
            CharacterTrieUtilities trieUtilities = new CharacterTrieUtilities();
            WordUtilities wordUtilities = new WordUtilities();
            foreach (String currentWord in allWords)
            {
                foreach (String adjacentWord in trieUtilities.FindAdjacentWords(allWordsTrieRoot, currentWord))
                {
                    // Find the character which was substitued between the word and the adjacent word
                    Tuple<Char, Char> differingCharacters = wordUtilities.FindDifferingCharacters(currentWord, adjacentWord);
                    Char fromCharacter = differingCharacters.Item1, toCharacter = differingCharacters.Item2;

                    // Increment the data structures
                    fromCharacterFrequencies.Increment(fromCharacter);
                    characterSubstitutionFrequencies.Increment(new CharacterSubstitution(fromCharacter, toCharacter));
                }
            }
        }

        /// <summary>
        /// Reads a set of words from a stream, and adds them to the trie with the specified root.
        /// </summary>
        /// <param name="trieRoot">The root of the trie.</param>
        /// <param name="reader">The stream reader to used to read the set of words (allows specifying a mocked reader for unit testing).</param>
        /// <param name="trieBuilder">The trie builder to use to add the words to the trie (allows specifying a mocked builder for unit testing).</param>
        /// <param name="wordFilterFunction">A Func to filter whether or not the specified word should be added to the trie.  Accepts the word as a parameter, and returns a bookean indicating whether that word should be added to the trie.</param>
        public void LoadWordsFromFile(Dictionary<Char, TrieNode<Char>> trieRoot, IStreamReader reader, ICharacterTrieBuilder trieBuilder, Func<String, Boolean> wordFilterFunction)
        {
            using (reader)
            {
                while (reader.EndOfStream == false)
                {
                    String currentWord = reader.ReadLine();
                    if (wordFilterFunction.Invoke(currentWord) == true)
                    {
                        trieBuilder.AddWord(trieRoot, currentWord, true);
                    }
                }
            }
        }
    }
}
