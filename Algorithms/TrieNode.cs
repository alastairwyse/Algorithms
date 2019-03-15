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

namespace Algorithms
{
    /// <summary>
    /// A node of a trie.
    /// </summary>
    /// <typeparam name="T">The type of data held in the node.</typeparam>
    public class TrieNode<T> where T : IEquatable<T>
    {
        /// <summary>The child nodes of this node.</summary>
        protected Dictionary<T, TrieNode<T>> children;

        /// <summary>
        /// The item held by the node.
        /// </summary>
        public T Item { get; }

        /// <summary>
        /// The child nodes of this node.
        /// </summary>
        public IEnumerable<KeyValuePair<T, TrieNode<T>>> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Initialises a new instance of the Algorithms.TrieNode class.
        /// </summary>
        /// <param name="item">The item held by the node.</param>
        public TrieNode(T item)
        {
            children = new Dictionary<T, TrieNode<T>> ();
            Item = item;
        }

        /// <summary>
        /// Checks whether a child node for the specified item exists.
        /// </summary>
        /// <param name="item">The item of the child node.</param>
        /// <returns>True if a child node for the specified item exists.  False otherwise.</returns>
        public Boolean ChildExists(T item)
        {
            return children.ContainsKey(item);
        }

        /// <summary>
        /// Returns the child node (of this node) for the specified item.
        /// </summary>
        /// <param name="childItem">The item of the child node.</param>
        /// <returns>The specified child node.</returns>
        /// <exception cref="System.ArgumentException">If the node does not contain a child for the item specified.</exception>
        public TrieNode<T> GetChildNode(T childItem)
        {
            ThrowExceptionIfChildNodeDoesntExist(childItem, "childItem");

            return children[childItem];
        }

        /// <summary>
        /// Adds a child node (to this node) for the specified item.
        /// </summary>
        /// <param name="childNode">The child node.</param>
        /// <exception cref="System.ArgumentException">If a child node for the specified item already exists.</exception>
        public void AddChild(TrieNode<T> childNode)
        {
            if (children.ContainsKey(childNode.Item))
                throw new ArgumentException("A child node for item '" + childNode.Item.ToString() + "' already exists.", "childNode");

            children.Add(childNode.Item, childNode);
        }

        /// <summary>
        /// Removes a child node (from this node)
        /// </summary>
        /// <param name="childItem">The item of the child node to remove.</param>
        /// <exception cref="System.ArgumentException">If the node does not contain a child for the item specified.</exception>
        public void RemoveChildForItem(T childItem)
        {
            ThrowExceptionIfChildNodeDoesntExist(childItem, "childItem");

            children.Remove(childItem);
        }

        #region Private/Protected Methods

        /// <summary>
        /// Throws an ArgumentException if a child node for the specified item doesn't exist.
        /// </summary>
        /// <param name="childNodeItem">The item of the child node.</param>
        /// <param name="argumentName">The name of the argument the child item is contained in.</param>
        protected void ThrowExceptionIfChildNodeDoesntExist(T childNodeItem, String argumentName)
        {
            if (children.ContainsKey(childNodeItem) == false)
                throw new ArgumentException("The node does not contain a child for item '" + childNodeItem.ToString() + "'.", argumentName);
        }

        #endregion 
    }
}
