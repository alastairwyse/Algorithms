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
    /// Utility methods used to create tries of characters.
    /// </summary>
    public class CharacterTrieBuilder : ICharacterTrieBuilder
    {
        /// <summary>
        /// Initialises a new instance of the Algorithms.CharacterTrieBuilder class.
        /// </summary>
        public CharacterTrieBuilder()
        {
        }

        /// <summary>
        /// Adds a word to the trie with the specified root.
        /// </summary>
        /// <param name="trieRoot">The root of the trie.</param>
        /// <param name="word">The word to add.</param>
        /// <param name="throwExceptionIfWordAlreadyExists">Specifies whether an exception should be thrown if the word already exists in the trie.</param>
        /// <exception cref="System.ArgumentException">Parameter 'word' is an empty string.</exception>
        /// <exception cref="System.ArgumentException">The specified word already exists in the trie.</exception>
        public void AddWord(Dictionary<Char, TrieNode<Char>> trieRoot, String word, Boolean throwExceptionIfWordAlreadyExists)
        {
            if (word.Length == 0)
                throw new ArgumentException("Parameter 'word' cannot be an empty string.", "word");

            TrieNode<Char> currentNode = null;

            // Handle the first character
            if (trieRoot.ContainsKey(word[0]))
            {
                if (word.Length == 1)
                {
                    if (trieRoot[word[0]] is EndMarkerTrieNode<Char>)
                    {
                        if (throwExceptionIfWordAlreadyExists == true)
                            ThrowWordAlreadyExistsException(word);
                    }
                    else
                    {
                        // 'Upgrade' the node to an end marker node
                        EndMarkerTrieNode<Char> replacementNode = new EndMarkerTrieNode<Char>(word[0]);
                        foreach (KeyValuePair<Char, TrieNode<Char>> currChildNode in trieRoot[word[0]].Children)
                        {
                            replacementNode.AddChild(currChildNode.Value);
                        }
                        trieRoot.Remove(word[0]);
                        trieRoot.Add(word[0], replacementNode);
                    }
                }
            }
            else
            {
                TrieNode<Char> newNode = null;
                if (word.Length == 1)
                    newNode = new EndMarkerTrieNode<Char>(word[0]);
                else
                    newNode = new TrieNode<Char>(word[0]);
                trieRoot.Add(newNode.Item, newNode);
            }
            currentNode = trieRoot[word[0]];

            // Handle the remaining characters
            Int32 currentCharIndex = 1;
            while (currentCharIndex < word.Length)
            {
                if (currentNode.ChildExists(word[currentCharIndex]))
                {
                    if (currentCharIndex == word.Length - 1)
                    {
                        if (currentNode.GetChildNode(word[currentCharIndex]) is EndMarkerTrieNode<Char>)
                        {
                            if (throwExceptionIfWordAlreadyExists == true)
                                ThrowWordAlreadyExistsException(word);
                        }
                        else
                        {
                            // 'Upgrade' the node to an end marker node
                            TrieNode<Char> lastCharNode = currentNode.GetChildNode(word[currentCharIndex]);
                            EndMarkerTrieNode<Char> replacementNode = new EndMarkerTrieNode<Char>(lastCharNode.Item);
                            foreach (KeyValuePair<Char, TrieNode<Char>> currChildNode in lastCharNode.Children)
                            {
                                replacementNode.AddChild(currChildNode.Value);
                            }
                            currentNode.RemoveChildForItem(lastCharNode.Item);
                            currentNode.AddChild(replacementNode);
                        }
                    }
                    else
                    {
                        currentNode = currentNode.GetChildNode(word[currentCharIndex]);
                    }
                }
                else
                {
                    // Add a node
                    TrieNode<Char> newNode = null;
                    if (currentCharIndex == word.Length - 1)
                    {
                        newNode = new EndMarkerTrieNode<char>(word[currentCharIndex]);
                    }
                    else
                    {
                        newNode = new TrieNode<char>(word[currentCharIndex]);
                    }
                    currentNode.AddChild(newNode);
                    currentNode = currentNode.GetChildNode(word[currentCharIndex]);
                }

                currentCharIndex++;
            }
        }

        #region Private/Protected Methods

        protected void ThrowWordAlreadyExistsException(String word)
        {
            throw new ArgumentException("The word '" + word + "' in parameter 'word' already exists in the trie.", "word");
        }

        #endregion
    }
}
