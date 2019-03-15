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
    /// Implementation of a priority queue to use in the A* algorithm.
    /// </summary>
    /// <typeparam name="T">The type of item held in the queue.</typeparam>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        /// <summary>Tree holding all the items and priorities in the queue.</summary>
        protected WeightBalancedTree<ItemAndPriority<T>> tree;
        /// <summary>A mapping from an item to its cooresponding priority in the tree.</summary>
        protected Dictionary<T, Double> itemToPriorityMap;

        /// <summary>
        /// The number of items in the queue.
        /// </summary>
        public Int32 Count
        {
            get
            {
                return tree.Count;
            }
        }

        /// <summary>
        /// Initialises a new instance of the Algorithms.PriorityQueue class.
        /// </summary>
        public PriorityQueue()
        {
            tree = new WeightBalancedTree<ItemAndPriority<T>>();
            itemToPriorityMap = new Dictionary<T, double>();
        }

        /// <summary>
        /// Checks whether the specified item exists in the queue.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>True if the item exists in the queue.  False otherwise.</returns>
        public Boolean Contains(T item)
        {
            return itemToPriorityMap.ContainsKey(item);
        }

        /// <summary>
        /// Get the priority for the specified item.
        /// </summary>
        /// <param name="item">The item to retrieve the priority for.</param>
        /// <returns>The priority for the item.</returns>
        /// <exception cref="System.ArgumentException">The item does not exist in the queue.</exception>
        public Double GetPriorityForItem(T item)
        {
            ThrowExceptionIfItemDoesntExistInQueue(item, "item");

            return itemToPriorityMap[item];
        }

        /// <summary>
        /// Adds an item to the queue
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="priority">The priority of the item.</param>
        /// <exception cref="System.ArgumentException">The item already exists in the queue.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Parameter 'priority' must be between 0.0 and 1.0 inclusive.</exception>
        public void Enqueue(T item, Double priority)
        {
            if (itemToPriorityMap.ContainsKey(item))
                throw new ArgumentException("The item '" + item.ToString() + "' already exists in the queue.", "item");
            if (priority < 0.0 || priority > 1.0)
                throw new ArgumentOutOfRangeException("priority", "Parameter 'priority' must be between 0.0 and 1.0 inclusive.");

            ItemAndPriority<T> newItem = new ItemAndPriority<T>(item, priority);
            tree.Add(newItem);
            itemToPriorityMap.Add(item, priority);
        }

        /// <summary>
        /// Removes and returns the item with the lowest priority.
        /// </summary>
        /// <returns>The item with the lowest priority.</returns>
        public T DequeueMaximum()
        {
            ThrowExceptionIfQueueIsEmpty();

            ItemAndPriority<T> highestPriorityItem = tree.GetNextLessThan(new ItemAndPriority<T>(default(T), 1.1)).Item2;
            Remove(highestPriorityItem.Item);
            itemToPriorityMap.Remove(highestPriorityItem.Item);

            return highestPriorityItem.Item;
        }

        /// <summary>
        /// Removes and returns the item with the highest priority.
        /// </summary>
        /// <returns>The item with the highest priority.</returns>
        public T DequeueMinimum()
        {
            ThrowExceptionIfQueueIsEmpty();

            ItemAndPriority<T> lowestPriorityItem = tree.GetNextGreaterThan(new ItemAndPriority<T>(default(T), -0.1)).Item2;
            Remove(lowestPriorityItem.Item);
            itemToPriorityMap.Remove(lowestPriorityItem.Item);

            return lowestPriorityItem.Item; 
        }

        /// <summary>
        /// Removes the specified item from the queue.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <exception cref="System.ArgumentException">The item does not exist in the queue.</exception>
        public void Remove(T item)
        {
            ThrowExceptionIfItemDoesntExistInQueue(item, "item");

            Double priority = itemToPriorityMap[item];
            ItemAndPriority<T> itemAndPriorityToRemove = new ItemAndPriority<T>(item, priority);
            tree.Remove(itemAndPriorityToRemove);
            itemToPriorityMap.Remove(item);
        }

        #region Private/Protected Methods

        /// <summary>
        /// Throws an ArgumentException if the specified item does not exist in the queue.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <param name="parameterName">The name of the parameter the item was passed in.</param>
        protected void ThrowExceptionIfItemDoesntExistInQueue(T item, String parameterName)
        {
            if (itemToPriorityMap.ContainsKey(item) == false)
                throw new ArgumentException("The item '" + item.ToString() + "' does not exist in the queue.", parameterName);
        }

        /// <summary>
        /// Throws an Exception if the queue is empty.
        /// </summary>
        protected void ThrowExceptionIfQueueIsEmpty()
        {
            if (tree.Count == 0)
                throw new Exception("The queue is empty.");
        }

        #endregion

        #region Protected / Private Classes

        /// <summary>
        /// Container class representing the entry held in the priority queue, containing an item and a priority between 0.0 and 1.0.
        /// </summary>
        /// <typeparam name="T">The type of item held in the queue.</typeparam>
        protected class ItemAndPriority<T> : IComparable<ItemAndPriority<T>> where T : IComparable<T>
        {
            protected T item;
            protected Double priority;

            /// <summary>The item held by the entry.</summary>
            public T Item
            {
                get { return item; }
            }

            /// <summary>The priority.</summary>
            public Double Priority
            {
                get { return priority; }
            }

            /// <summary>
            /// Initialises a new instance of the Algorithms.PriorityQueue+ItemAndPriority class.
            /// </summary>
            /// <param name="item">The item held by the entry.</param>
            /// <param name="priority">The priority (between 0.0 and 1.0 inclusive).</param>
            public ItemAndPriority(T item, Double priority)
            {
                this.item = item;
                this.priority = priority;
            }

            public Int32 CompareTo(ItemAndPriority<T> other)
            {
                if (priority != other.priority)
                    return priority.CompareTo(other.priority);
                else
                    return item.CompareTo(other.item);
            }
        }

        #endregion
    }
}
