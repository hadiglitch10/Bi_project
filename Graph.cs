using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTemplate
{
    internal class Graph
    {
        static public int n;
        static public int m;
        static public Vertex[, ] vertices;
        static public List<List<Edge>> adj;
        static public SortedSet<Edge> edges;

        static public int get_node(int x, int y)
        {
            return x * m + y;
        }
        static public (int, int) get_cordinates(int node)
        {
            return (node / m, node - node / m * m);
        }
    }
    public struct Edge : IComparable<Edge>
    {
        public int sourse;
        public int destination;
        public double weight;
        public Edge(int sourse, int destination, double weight)
        {
            this.sourse = sourse;
            this.destination = destination;
            this.weight = weight;
        }
        public static bool operator <(Edge left, Edge right) => left.weight < right.weight;
        public static bool operator >(Edge left, Edge right) => left.weight > right.weight;
       public int CompareTo(Edge other)
 {
        if (weight != other.weight)
            return weight.CompareTo(other.weight);
        if (sourse != other.sourse)
            return sourse.CompareTo(other.sourse);
        return destination.CompareTo(other.destination);
 }
    }
    struct Vertex
    {
        public int node;
        public int x, y;
        public RGBPixel pixel;
        public double Intensity;
        public Vertex(int x, int y, RGBPixel pixel)
        {
            this.x = x;
            this.y = y;
            this.pixel = pixel;
            this.Intensity = 0.299 * pixel.red + 0.587 * pixel.green + 0.114 * pixel.blue;
            this.node = Graph.get_node(x, y);
        }
    }
}
