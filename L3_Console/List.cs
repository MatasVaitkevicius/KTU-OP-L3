using System;
using System.Collections;
using System.Collections.Generic;

namespace L3_Console
{
    public class List<Type> : IEnumerable<Type> where Type : IComparable<Type>, IEquatable<Type>
    {
        //========================
        public sealed class Node
        {
            public Node Left { get; set; }
            public Node Right { get; set; }
            public Type Data { get; set; }

            public Node(Type value, Node left, Node right)
            {
                Data = value;
                Left = left;
                Right = right;
            }
        }
        //========================

        private Node Start;
        private Node End;
        private Node ListInterface;

        public List()
        {
            Start = End = ListInterface = null;
        }

        public void AddData(Type newOne)
        {
            var newNode = new Node(newOne, null, null);

            if (Start == null)
            {
                Start = End = new Node(newOne, null, null);
            }
            else
            {
                End.Right = new Node(newOne, Start, null);
                End = End.Right;
            }
        }

        public void StartingNode()
        {
            ListInterface = Start;
        }

        public void LeftNode()
        {
            ListInterface = ListInterface.Left;
        }
        public void RightNode()
        {
            ListInterface = ListInterface.Right;
        }

        public bool Contains()
        {
            return ListInterface != null;
        }

        public Type GetData()
        {
            return ListInterface.Data;
        }

        public Node GetNode()
        {
            return ListInterface;
        }

        public void RemoveNode(Node dd)
        {
            if (dd == Start) Start = Start.Right;
            if (dd == End) End = End.Left;
            if (dd.Left != null)
                dd.Left.Right = dd.Right;
            if (dd.Right != null)
                dd.Right.Left = dd.Left;
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

                    var data = firstNode.Data;
                    firstNode.Data = min.Data;
                    min.Data = data;
                }
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
