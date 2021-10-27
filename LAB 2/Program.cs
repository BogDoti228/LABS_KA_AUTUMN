using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LAB_2
{
    internal class Node
    {
        public int Value { get; }
        public Node PrevNode { get; set; }

        public Node(int value, Node prevNode)
        {
            PrevNode = prevNode;
            Value = value;
        }
    }
    
    internal class Edge: IEquatable<Edge>
    {
        public int FirstNode { get; set; }
        public int SecondNode { get; set; }
        public int Cost { get; set; }
        public Edge Parent { get; }

        public Edge(int firstNode, int secondNode, int cost, Edge parentEdge)
        {
            FirstNode = firstNode;
            SecondNode = secondNode;
            Cost = cost;
            Parent = parentEdge;
        }
        

        public bool Equals(Edge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FirstNode == other.FirstNode  && SecondNode == other.SecondNode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Edge) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstNode, SecondNode, Cost);
        }

        public bool Contains(int node)
        {
            return FirstNode == node || SecondNode == node;
        }

        public int GetAnotherNode(int node)
        {
            return FirstNode == node ? SecondNode : FirstNode;
        }

        public void Inverse()
        {
            (FirstNode, SecondNode) = (SecondNode, FirstNode);
            Cost = Cost == 1 ? 0 : 1;
        }
    }
    
    internal class SolveLab
    {
        private int xLength;
        private int yLength;
        private List<Edge> listEdges;
        private int sNode;
        private int tNode;
        private List<Edge> maxPairsEdge;

        private void GetInput()
        {
            var fileInput = new StreamReader("in.txt");
            var firstLineArr = fileInput.ReadLine().Split(" ").Select(int.Parse).ToArray();
            xLength = firstLineArr[0];
            yLength = firstLineArr[1];
            var edges = new List<Edge>();

            for (var i = 1; i <= xLength; i++)
            {
                var nextLineArr = fileInput.ReadLine().Split(" ").Select(int.Parse).ToArray();
                for (var j = 1; j <= yLength; j++)
                {
                    if (nextLineArr[j - 1] == 1)
                    {
                        edges.Add(new Edge(i, j + xLength, 0, null));
                    }
                }
            }

            listEdges = edges;
        }

        private void BuildMaxPairs()
        {
            while (true)
            {
                var stack = new Stack<Node>();
                stack.Push(new Node(sNode, null));
                var isPathFind = false;
                var finalNode = new Node(-1, null);
                while (stack.Count != 0)
                {
                    var node = stack.Pop();
                    if (node.Value == tNode)
                    {
                        isPathFind = true;
                        finalNode = node;
                        break;
                    }

                    foreach (var edge in listEdges.Where(x => x.FirstNode == node.Value && (x.Cost == 0 || x.Cost == 1 && !x.Contains(sNode) && !x.Contains(tNode))))
                    {
                        var nextNode = new Node(edge.SecondNode, node);
                        stack.Push(nextNode);
                    }
                }

                if (isPathFind)
                {
                    var listValue = new List<int>();
                    var prev = finalNode;
                    while (prev != null)
                    {
                        var value = prev.Value;
                        listValue.Add(value);
                        prev = prev.PrevNode;
                    }

                    listValue.Reverse();
                    
                    for (var i = 0; i < listValue.Count - 1; i++)
                    {
                        foreach (var edge in listEdges.Where(edge => edge.FirstNode == listValue[i] && edge.SecondNode == listValue[i + 1]))
                        {
                            edge.Inverse();
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            maxPairsEdge = listEdges.Where(x => x.Cost == 1 && !x.Contains(sNode) && !x.Contains(tNode)).ToList();
        }

        private void SetOutput()
        {
            using var fileOutput = new StreamWriter("out.txt");
            var sortedMaxPairs = maxPairsEdge.OrderBy(x => x.SecondNode).ToDictionary(edge => edge.SecondNode, edge => edge.FirstNode);
            var finalArray = new int[xLength];
            for (var i = 0; i < xLength; i++)
            {
                if (sortedMaxPairs.ContainsKey(i + 1))
                    finalArray[i] = sortedMaxPairs[i + 1] - xLength;
                else
                {
                    finalArray[i] = 0;
                }
            }
            fileOutput.WriteLine(string.Join(" ", finalArray));
        }

        private void AddSTNodes()
        {
            sNode = 0;
            tNode = xLength + yLength + 1;
            
            for (var i = 1; i <= xLength; i++)
            {
                listEdges.Add(new Edge(sNode, i, 0, null));
            }

            for (var i = 1; i <= yLength; i++)
            {
                listEdges.Add(new Edge(i + xLength, tNode, 0, null));
            }
        }

        public void Start()
        {
            GetInput();
            AddSTNodes();
            BuildMaxPairs();
            SetOutput();
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            new SolveLab().Start();
        }
    }
}