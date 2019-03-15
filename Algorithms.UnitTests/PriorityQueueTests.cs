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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorithms.UnitTests
{
    /// <summary>
    /// Unit tests for the Algorithms.PriorityQueue class.
    /// </summary>
    [TestClass]
    public class PriorityQueueTests
    {
        private PriorityQueue<String> testPriorityQueue;

        [TestInitialize]
        public void SetUp()
        {
            testPriorityQueue = new PriorityQueue<String>();
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetPriorityForItem() method is called on an item which doesn't exist in the queue.
        /// </summary>
        [TestMethod]
        public void GetPriorityForItem_ItemDoesntExist()
        {
            testPriorityQueue.Enqueue("A", 0.1);
            testPriorityQueue.Enqueue("B", 0.9);

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testPriorityQueue.GetPriorityForItem("C");
            });

            StringAssert.StartsWith(e.Message, "The item 'C' does not exist in the queue.");
            Assert.AreEqual("item", e.ParamName);
        }

        /// <summary>
        /// Success tests for the GetPriorityForItem() method.
        /// </summary>
        [TestMethod]
        public void GetPriorityForItem()
        {
            testPriorityQueue.Enqueue("A", 0.1);
            testPriorityQueue.Enqueue("B", 0.5);
            testPriorityQueue.Enqueue("C", 0.9);

            Double result = testPriorityQueue.GetPriorityForItem("B");

            Assert.AreEqual(0.5, result);
        }

        /// <summary>
        /// Success tests for the Count property.
        /// </summary>
        [TestMethod]
        public void Count()
        {
            Assert.AreEqual(0, testPriorityQueue.Count);


            testPriorityQueue.Enqueue("A", 0.1);
            testPriorityQueue.Enqueue("B", 0.5);
            testPriorityQueue.Enqueue("C", 0.9);

            Assert.AreEqual(3, testPriorityQueue.Count);


            testPriorityQueue.Remove("A");
            testPriorityQueue.Remove("C");

            Assert.AreEqual(1, testPriorityQueue.Count);


            testPriorityQueue.Remove("B");

            Assert.AreEqual(0, testPriorityQueue.Count);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Enqueue() method is called for an item which already exists in the queue.
        /// </summary>
        [TestMethod]
        public void Enqueue_ItemAlreadyExists()
        {
            // Test where the same item is enqueued with a different priority
            testPriorityQueue.Enqueue("A", 0.1);
            testPriorityQueue.Enqueue("B", 0.5);
            testPriorityQueue.Enqueue("C", 0.9);

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testPriorityQueue.Enqueue("B", 0.6);
            });

            StringAssert.StartsWith(e.Message, "The item 'B' already exists in the queue.");
            Assert.AreEqual("item", e.ParamName);


            // Test where the same item is enqueued with the same priority
            e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testPriorityQueue.Enqueue("A", 0.1);
            });

            StringAssert.StartsWith(e.Message, "The item 'A' already exists in the queue.");
            Assert.AreEqual("item", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Enqueue() method is called with a priority that is greater than 1.0 and less than 0.0.
        /// </summary>
        [TestMethod]
        public void Enqueue_PriorityOutOfRange()
        {
            ArgumentOutOfRangeException e = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                testPriorityQueue.Enqueue("A", -0.01);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'priority' must be between 0.0 and 1.0 inclusive.");
            Assert.AreEqual("priority", e.ParamName);


            e = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                testPriorityQueue.Enqueue("B", 1.01);
            });

            StringAssert.StartsWith(e.Message, "Parameter 'priority' must be between 0.0 and 1.0 inclusive.");
            Assert.AreEqual("priority", e.ParamName);
        }

        /// <summary>
        /// Success tests for the Enqueue() and Contains() methods.
        /// </summary>
        [TestMethod]
        public void Enqueue_Contains()
        {
            Assert.IsFalse(testPriorityQueue.Contains("A"));
            Assert.IsFalse(testPriorityQueue.Contains("B"));
            Assert.IsFalse(testPriorityQueue.Contains("C"));
            Assert.IsFalse(testPriorityQueue.Contains("D"));

            testPriorityQueue.Enqueue("B", 0.5);
            testPriorityQueue.Enqueue("A", 0.0);
            testPriorityQueue.Enqueue("C", 1.0);

            Assert.IsTrue(testPriorityQueue.Contains("A"));
            Assert.IsTrue(testPriorityQueue.Contains("B"));
            Assert.IsTrue(testPriorityQueue.Contains("C"));
            Assert.IsFalse(testPriorityQueue.Contains("D"));
        }

        /// <summary>
        /// Tests that an exception is thrown if the DequeueMaximum() method is called when the queue is empty.
        /// </summary>
        [TestMethod]
        public void DequeueMaximum_QueueIsEmpty()
        {
            Exception e = Assert.ThrowsException<Exception>(() =>
            {
                testPriorityQueue.DequeueMaximum();
            });

            StringAssert.StartsWith(e.Message, "The queue is empty.");
        }

        /// <summary>
        /// Tests that an exception is thrown if the DequeueMinimum() method is called when the queue is empty.
        /// </summary>
        [TestMethod]
        public void DequeueMinimum_QueueIsEmpty()
        {
            Exception e = Assert.ThrowsException<Exception>(() =>
            {
                testPriorityQueue.DequeueMinimum();
            });

            StringAssert.StartsWith(e.Message, "The queue is empty.");
        }

        /// <summary>
        /// Success tests for the DequeueMaximum() method.
        /// </summary>
        [TestMethod]
        public void DequeueMaximum()
        {
            testPriorityQueue.Enqueue("D", 0.75);
            testPriorityQueue.Enqueue("C", 0.5);
            testPriorityQueue.Enqueue("E", 1.0);
            testPriorityQueue.Enqueue("B", 0.25);
            testPriorityQueue.Enqueue("A", 0.0);

            String result = testPriorityQueue.DequeueMaximum();

            Assert.AreEqual("E", result);
            Assert.AreEqual(4, testPriorityQueue.Count);
            Assert.IsFalse(testPriorityQueue.Contains("E"));
        }

        /// <summary>
        /// Success tests for the DequeueMinimum() method.
        /// </summary>
        [TestMethod]
        public void DequeueMinimum()
        {
            testPriorityQueue.Enqueue("D", 0.75);
            testPriorityQueue.Enqueue("A", 0.0);
            testPriorityQueue.Enqueue("C", 0.5);
            testPriorityQueue.Enqueue("E", 1.0);
            testPriorityQueue.Enqueue("B", 0.25);

            String result = testPriorityQueue.DequeueMinimum();

            Assert.AreEqual("A", result);
            Assert.AreEqual(4, testPriorityQueue.Count);
            Assert.IsFalse(testPriorityQueue.Contains("A"));
        }

        /// <summary>
        /// Tests that an exception is thrown if the Remove() method is called for an item which doesn't exist in the queue.
        /// </summary>
        [TestMethod]
        public void Remove_ItemDoesntExist()
        {
            testPriorityQueue.Enqueue("A", 0.1);
            testPriorityQueue.Enqueue("B", 0.9);

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testPriorityQueue.Remove("C");
            });

            StringAssert.StartsWith(e.Message, "The item 'C' does not exist in the queue.");
            Assert.AreEqual("item", e.ParamName);
        }

        /// <summary>
        /// Success tests for the Remove() method.
        /// </summary>
        [TestMethod]
        public void Remove()
        {
            testPriorityQueue.Enqueue("D", 0.75);
            testPriorityQueue.Enqueue("A", 0.0);
            testPriorityQueue.Enqueue("C", 0.5);
            testPriorityQueue.Enqueue("E", 1.0);
            testPriorityQueue.Enqueue("B", 0.25);

            testPriorityQueue.Remove("C");

            Assert.AreEqual(4, testPriorityQueue.Count);
            Assert.IsFalse(testPriorityQueue.Contains("C"));
        }
    }
}
