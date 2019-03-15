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
    /// Container class representing the substitution of one character for another (as occurs in 'adjacent' words that differ by one character).
    /// </summary>
    public class CharacterSubstitution : IEquatable<CharacterSubstitution>
    {
        protected const Int32 prime1 = 7;
        protected const Int32 prime2 = 11;

        /// <summary>The 'from' character in the substitution.</summary>
        protected Char fromCharacter;
        /// <summary>The 'to' character in the substitution.</summary>
        protected Char toCharacter;

        /// <summary>
        /// The 'from' character in the substitution.
        /// </summary>
        public Char FromCharacter
        {
            get { return fromCharacter; }
        }

        /// <summary>
        /// The 'to' character in the substitution.
        /// </summary>
        public Char ToCharacter
        {
            get { return toCharacter; }
        }

        /// <summary>
        /// Initialises a new instance of the Algorithms.DataStructureUtilities+CharacterSubstitution class.
        /// </summary>
        /// <param name="fromCharacter">The 'from' character in the substitution.</param>
        /// <param name="toCharacter">The 'to' character in the substitution.</param>
        public CharacterSubstitution(Char fromCharacter, Char toCharacter)
        {
            this.fromCharacter = fromCharacter;
            this.toCharacter = toCharacter;
        }

        public Boolean Equals(CharacterSubstitution other)
        {
            return fromCharacter == other.fromCharacter && toCharacter == other.toCharacter;
        }

        public override Int32 GetHashCode()
        {
            return fromCharacter * prime1 + toCharacter * prime2;
        }
    }
}
