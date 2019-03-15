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
    /// Defines methods for a set of utility methods used to create tries of characters.
    /// </summary>
    public interface ICharacterTrieBuilder
    {
        /// <summary>
        /// Adds a word to the trie with the specified root.
        /// </summary>
        /// <param name="trieRoot">The root of the trie.</param>
        /// <param name="word">The word to add.</param>
        /// <param name="throwExceptionIfWordAlreadyExists">Specifies whether an exception should be thrown if the word already exists in the trie.</param>
        void AddWord(Dictionary<Char, TrieNode<Char>> trieRoot, String word, Boolean throwExceptionIfWordAlreadyExists);
    }
}
