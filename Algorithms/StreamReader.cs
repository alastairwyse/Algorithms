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
    /// Default implementation of the Algorithms.IStreamReader interface.
    /// </summary>
    public class StreamReader : IStreamReader
    {
        protected System.IO.StreamReader underlyingReader;

        /// <summary>
        /// Initialises a new instance of the Algorithms.StreamReader class.
        /// </summary>
        /// <param name="underlyingReader">The System.IO.StreamReader underlying this reader.</param>
        public StreamReader(System.IO.StreamReader underlyingReader)
        {
            this.underlyingReader = underlyingReader;
        }

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end of the stream. 
        /// </summary>
        public Boolean EndOfStream
        {
            get { return underlyingReader.EndOfStream; }
        }

        /// <summary>
        /// Reads a line of characters from the current stream and returns the data as a string.
        /// </summary>
        /// <returns>The next line from the input stream, or null if the end of the input stream is reached.</returns>
        public string ReadLine()
        {
            return underlyingReader.ReadLine();
        }

        #region IDisposable Support

        private Boolean disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(Boolean disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    underlyingReader.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override a finalizer below.
                // Set large fields to null.

                disposedValue = true;
            }
        }

        // Override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StreamReader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // Uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
