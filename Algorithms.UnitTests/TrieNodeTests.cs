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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorithms.UnitTests
{
    /// <summary>
    /// Unit tests for the Algorithms.TrieNode class.
    /// </summary>
    [TestClass]
    public class TrieNodeTests
    {
        private TrieNode<Char> testTrieNode;

        [TestInitialize]
        public void SetUp()
        {
            testTrieNode = new TrieNode<Char>('A');
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetChildNode() method is called with an item for which a child doesn't exist.
        /// </summary>
        [TestMethod]
        public void GetChildNode_ChildForSpecifiedItemDoesntExist()
        {
            TrieNode<Char> childNode1 = new TrieNode<Char>('B');
            testTrieNode.AddChild(childNode1);

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testTrieNode.GetChildNode('C');
            });

            StringAssert.StartsWith(e.Message, "The node does not contain a child for item 'C'.");
            Assert.AreEqual("childItem", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the RemoveChildForItem() method is called with an item for which a child doesn't exist.
        /// </summary>
        [TestMethod]
        public void RemoveChildForItem_ChildForSpecifiedItemDoesntExist()
        {
            TrieNode<Char> childNode1 = new TrieNode<Char>('B');
            testTrieNode.AddChild(childNode1);

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testTrieNode.RemoveChildForItem('C');
            });

            StringAssert.StartsWith(e.Message, "The node does not contain a child for item 'C'.");
            Assert.AreEqual("childItem", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddChild() method is called with an item for which a child already exists.
        /// </summary>
        [TestMethod]
        public void AddChild_ChildForSpecifiedItemAlreadyExists()
        {
            TrieNode<Char> childNode1 = new TrieNode<Char>('B');
            TrieNode<Char> childNode2 = new TrieNode<Char>('B');
            testTrieNode.AddChild(childNode1);

            ArgumentException e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testTrieNode.AddChild(childNode1);
            });

            StringAssert.StartsWith(e.Message, "A child node for item 'B' already exists.");
            Assert.AreEqual("childNode", e.ParamName);

            e = Assert.ThrowsException<ArgumentException>(() =>
            {
                testTrieNode.AddChild(childNode2);
            });

            StringAssert.StartsWith(e.Message, "A child node for item 'B' already exists.");
            Assert.AreEqual("childNode", e.ParamName);
        }
    }
}
