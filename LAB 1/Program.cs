using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LAB_1
{
    internal class Edge: IEquatable<Edge>
    {
        public int FirstNode { get; }
        public int SecondNode { get; }
        public int Cost { get; }

        public Edge(int firstNode, int secondNode, int cost)
        {
            FirstNode = firstNode;
            SecondNode = secondNode;
            Cost = cost;
        }

        public bool Equals(Edge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (FirstNode == other.FirstNode || FirstNode == other.SecondNode) && (SecondNode == other.SecondNode || SecondNode == other.FirstNode) && Cost == other.Cost;
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
    }

    internal class SolveLab
    {
        private List<Edge> finalListEdges;
        private List<int> nodes;
        private int amountNodes;
        
        private IEnumerable<Edge> GetInput()
        {
            var fileInput = new StreamReader("in.txt");
            var n = int.Parse(fileInput.ReadLine());
            IEnumerable<int> arrayChanged = Array.Empty<int>();
            while (fileInput.ReadLine() is var nextString)
            {
                arrayChanged = arrayChanged.Concat(nextString.Split().Select(int.Parse));
                if (nextString.Contains("32767"))
                {
                    break;
                }
            }

            var array = arrayChanged.ToArray();
            
            var listEdges = new List<Edge>();

            amountNodes = array[0] - 2;
            for (var i = 0; i < array[0] - 2; i++)
            {
                var amount = array[i + 1];
                var node = 0;
                var isNode = true;
                for (var j = array[i]; j < amount; j++)
                {
                    if (isNode)
                    {
                        node = array[j - 1];
                        isNode = false;
                    }
                    else
                    {
                        var cost = array[j - 1];
                        var edge = new Edge(i + 1, node, cost);
                        if (!listEdges.Contains(edge))
                            listEdges.Add(new Edge(i + 1, node, cost));
                        isNode = true;
                    }
                    
                }
            }

            return listEdges;
        }

        private bool DFS(IEnumerable<Edge> listEdges, Edge edge)
        {
            var newListEdge = listEdges.Concat(new List<Edge>{edge}).ToList();
            var stack = new Stack<int>();
            stack.Push(edge.FirstNode);
            var visitedNodes = new HashSet<int>();

            while (stack.Count != 0)
            {
                var node = stack.Pop();
                if (visitedNodes.Contains(node)) return false;
                visitedNodes.Add(node);

                foreach (var nextEdge in newListEdge.Where(x => x.Contains(node) && !visitedNodes.Contains(x.GetAnotherNode(node))))
                {
                    stack.Push(nextEdge.GetAnotherNode(node));
                }
            }

            return true;
        }

        private void BuildMinOstov(IEnumerable<Edge> listEdges)
        {
            var sortedListEdges = listEdges.OrderBy(x => x.Cost).ToList();
            var setNodes = new HashSet<int>();
            var listFinalEdges = new List<Edge>();

            foreach (var edge in sortedListEdges.Where(edge => DFS(listFinalEdges, edge)))
            {
                setNodes.Add(edge.FirstNode);
                setNodes.Add(edge.SecondNode);
                listFinalEdges.Add(edge);
            }

            finalListEdges = listFinalEdges;
            nodes = setNodes.OrderBy(x => x).ToList();
        }

        private void SetOutput()
        {
            using var fileOutput = new StreamWriter("out.txt");
            foreach (var nextListNodes in nodes.Select(node => (from edge in finalListEdges where edge.Contains(node) select edge.GetAnotherNode(node)).ToList()))
            {
                fileOutput.WriteLine(string.Join(" ", nextListNodes));
            }

            fileOutput.WriteLine(finalListEdges.Aggregate(0, (i, edge) => i + edge.Cost));
        }

        public void Start()
        {
            var listEdges = GetInput();
            BuildMinOstov(listEdges);
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