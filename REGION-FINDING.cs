using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTemplate
{
    internal class REGION_FINDING
    {
        static int[] dx = new int[] { 0, -1, 1, -1, 1, -1, 0, 1 };
        static int[] dy = new int[] { 1, 1, 1, 0, 0, -1, -1, -1 };
        static public DSU dsu;
        static public void buildTheGraph(RGBPixel[,] ImageMatrix)
        {
            Graph.n = ImageMatrix.GetLength(0);
            Graph.m = ImageMatrix.GetLength(1);
            Graph.adj = new List<List<Edge>>();
            Graph.vertices = new Vertex[Graph.n, Graph.m];
            Graph.edges = new SortedSet<Edge>();
            dsu = new DSU(Graph.n * Graph.m); 
            for (int i = 0; i < Graph.n; i++)
            {
                for (int j = 0; j < Graph.m; j++)
                {
                    Graph.vertices[i, j] = new Vertex(i, j, ImageMatrix[i, j]);
                    Graph.adj.Add(new List<Edge>());
                }
            }
            for (int i = 0; i < Graph.n; i++)
            {
                for (int j = 0; j < Graph.m; j++)
                {
                    Vertex u = Graph.vertices[i, j];
                    for (int d = 0; d < 8; d++)
                    {
                        if (i + dx[d] < 0 || j + dy[d] < 0) continue;
                        if (i + dx[d] >= Graph.n || j + dy[d] >= Graph.m) continue;
                        Vertex v = Graph.vertices[i + dx[d], j + dy[d]];
                        Edge edge = new Edge(u.node, v.node, Math.Abs(u.Intensity - v.Intensity));
                        Graph.adj[u.node].Add(edge);
                        Graph.edges.Add(edge);
                    }
                }
            }
        }
         static public void StartRegionFinding(int k)
        {
            foreach (Edge edge in Graph.edges)
            {
                int s1 = dsu.find_set(edge.sourse);
                int s2 = dsu.find_set(edge.destination);
                if (s1 == s2) continue;
                if (edge.weight > Math.Min(dsu.Internal_Diff[s1] + k / dsu.get_size(s1), dsu.Internal_Diff[s2] + k / dsu.get_size(s2))) continue;
                dsu.union_sets(s1, s2, edge.weight);
            }
            Console.WriteLine(dsu.numof_sets);
        }
        public static Bitmap VisualizeSegments(RGBPixel[,] ImageMatrix)
        {
            int n = ImageMatrix.GetLength(0);
            int m = ImageMatrix.GetLength(1);
            Bitmap output = new Bitmap(m, n);

            Random rnd = new Random();
            Dictionary<int, Color> regionColors = new Dictionary<int, Color>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    int node = Graph.get_node(i, j);
                    int root = dsu.find_set(node);
                    if (!regionColors.ContainsKey(root))
                        regionColors[root] = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    output.SetPixel(j, i, regionColors[root]);
                }
            }

            return output;
        }
        public static int[,] GetSegmentMap(RGBPixel[,] ImageMatrix)
        {
            int height = ImageMatrix.GetLength(0);
            int width = ImageMatrix.GetLength(1);
            int[,] segmentMap = new int[height, width];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    segmentMap[i, j] = dsu.find_set(i * width + j);                
                }
            }
            
            return segmentMap;
        }
    }
}
