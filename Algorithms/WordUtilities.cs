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
    /// Utility methods for comparing words.
    /// </summary>
    public class WordUtilities
    {
        /// <summary>
        /// Initialises a new instance of the Algorithms.WordUtilities class.
        /// </summary>
        public WordUtilities()
        {
        }

        /// <summary>
        /// Finds and returns the character that differs between two adjacent words.
        /// </summary>
        /// <param name="word1">The first word.</param>
        /// <param name="word2">The second word.</param>
        /// <returns>A Tuple containing: the differing character in parameter 'word1', and the differing character in parameter 'word2'.</returns>
        /// <exception cref="System.ArgumentException">The length of parameter 'word1' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">The length of parameter 'word2' is less than 1.</exception>
        /// <exception cref="System.ArgumentException">Parameters 'word1' and 'word2' have differing lengths.</exception>
        /// <exception cref="System.ArgumentException">Parameters 'word1' and 'word2' differ by more than 1 character.</exception>
        /// <exception cref="System.ArgumentException">Parameters 'word1' and 'word2' are the same.</exception>
        public Tuple<Char, Char> FindDifferingCharacters(String word1, String word2)
        {
            if (word1.Length < 1)
                throw new ArgumentException("Parameter 'word1' must be greater than or equal to 1 character in length.", "word1");
            if (word2.Length < 1)
                throw new ArgumentException("Parameter 'word2' must be greater than or equal to 1 character in length.", "word2");
            if (word1.Length != word2.Length)
                throw new ArgumentException("Parameters 'word1' and 'word2' must have the same length.", "word2");

            Boolean differingCharacterFound = false;
            Tuple<Char, Char> returnCharacters = null;
            for (Int32 characterIndex = 0; characterIndex < word1.Length; characterIndex++)
            {
                if (differingCharacterFound == true)
                {
                    if (word1[characterIndex] != word2[characterIndex])
                    {
                        throw new ArgumentException("Parameters 'word1' and 'word2' differ by more than 1 character.", "word2");
                    }
                }
                else
                {
                    if (word1[characterIndex] != word2[characterIndex])
                    {
                        differingCharacterFound = true;
                        returnCharacters = new Tuple<Char, Char>(word1[characterIndex], word2[characterIndex]);
                    }
                }
            }
            if (differingCharacterFound == false)
                throw new ArgumentException("Parameters 'word1' and 'word2' are the same.", "word2");

            return returnCharacters;
        }
    }
}
