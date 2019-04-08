using System;
using System.Collections;
using System.Collections.Generic;

namespace L3_Web
{
    public class ListNodes<Type> : IEnumerable<Type> where Type : IComparable<Type>, IEquatable<Type>
    {
        public sealed class Node
        {
            public Node Left { get; set; }
            public Type Data { get; set; }
            public Node Right { get; set; }

            public Node(Type value, Node left, Node right)
            {
                Data = value;
                Left = left;      
                Right = right;
            }
        }

        private Node Start;
        private Node End;
        private Node ListNodesInterface;

        public ListNodes()
        {
            Start = End = ListNodesInterface = null;
        }

        public void AddData(Type newOne)
        {
            //var newNode = new Node(newOne, null, null);

            if (Start == null)
            {
                Start = End = new Node(newOne, null, null);
            }
            else
            {
                End.Right = new Node(newOne, End, null);
                End = End.Right;
            }
        }

        public void StartingNode()
        {
            ListNodesInterface = Start;
        }

        public void LeftNode()
        {
            ListNodesInterface = ListNodesInterface.Left;
        }
        public void RightNode()
        {
            ListNodesInterface = ListNodesInterface.Right;
        }

        public bool Contains()
        {
            return ListNodesInterface != null;
        }

        public bool ContainsAll()
        {
            return ListNodesInterface != null || Start != null || End != null;
        }

        public Type GetData()
        {
            return ListNodesInterface.Data;
        }

        public Node GetNodeInterface()
        {
            return ListNodesInterface;
        }

        public void RemoveNode(Node node)
        {
            if (node == Start)
            {
                Start = Start.Right;
            }
            if (node == End)
            {
                End = End.Left;
            }
            if (node.Left != null)
            {
                node.Left.Right = node.Right;
            }
            if (node.Right != null)
            {
                node.Right.Left = node.Left;
            }
            node.Right = null;
            node.Left = null;
        }

        public void Sort()
        {
            for (var firstNode = Start; firstNode != null; firstNode = firstNode.Right)
            {
                var min = firstNode;

                for (var secondNode = firstNode; secondNode != null; secondNode = secondNode.Right)
                {
                    if (secondNode.Data.CompareTo(min.Data) < 0)
                    {
                        min = secondNode;
                    }
                }
                var data = firstNode.Data;
                firstNode.Data = min.Data;
                min.Data = data;
            }
        }

        /// <summary>
        /// Kolekcijos sąsajos įgyvendinimas
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Type> GetEnumerator()
        {
            for (var newNode = Start; newNode != null; newNode = newNode.Right)
                yield return newNode.Data;
        }

        /// <summary>
        /// Būtinas metodas, nes IEnumerable<Tipas> paveldi iš IEnumerable
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
