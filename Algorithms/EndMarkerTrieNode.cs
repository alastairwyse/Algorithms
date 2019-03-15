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

namespace Algorithms
{
    /// <summary>
    /// A node of a trie which signifies the end of a sequence in that trie.
    /// </summary>
    /// <typeparam name="T">The type of data held in the node.</typeparam>
    public class EndMarkerTrieNode<T> : TrieNode<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Initialises a new instance of the Algorithms.EndMarkerTrieNode class.
        /// </summary>
        /// <param name="item">The item held by the end marker node.</param>
        public EndMarkerTrieNode(T item)
            : base(item)
        {
        }
    }
}
