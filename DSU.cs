using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTemplate
{
    internal class DSU
    {
        public int Size;
        public int numof_sets;
        int[] size;
        int[] parent;
        public double[] Internal_Diff;
        public DSU(int Size)
        {
            this.numof_sets = Size;
            this.Size = Size;
            parent = new int[Size];
            size = new int[Size];
            Internal_Diff = new double[Size];
            for (int i = 0; i < Size; i++)
            {
                size[i] = 1;
                parent[i] = i;
                Internal_Diff[i] = 0;
            }
        }
        public int get_size(int v)
        {
            return size[find_set(v)];
        }
        public int find_set(int v)
        {
            if (v == parent[v])
                return v;
            return parent[v] = find_set(parent[v]);
        }
        public void union_sets(int a, int b, double new_internal_diff)
        {
            a = find_set(a);
            b = find_set(b);
            if (a != b)
            {
                if (size[a] < size[b])
                    (a, b) = (b, a);
                parent[b] = a;
                size[a] += size[b];
                Internal_Diff[a] = Math.Max(new_internal_diff, Math.Max(Internal_Diff[a], Internal_Diff[b]));
                numof_sets--;
            }
        }
    }
}
