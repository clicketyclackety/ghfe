using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

using Rhino.Geometry;

namespace lib
{
    public static class ComponentGrouper
    {

        /// <summary>
        /// Forms rows of components in the Grasshopper canvas.
        /// Two components belong in the same row if Y domains of their bounding rectangles overlap.
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public static List<List<IGH_DocumentObject>> FormRows(List<IGH_DocumentObject> components)
        {
            var sortedComponents = components.OrderBy(c => c.Attributes.Bounds.Top).ToList();
            int n = sortedComponents.Count;
            if (n == 0)
                return new List<List<IGH_DocumentObject>>();


            var intervals = sortedComponents.Select(c => new Interval(c.Attributes.Bounds.Top, c.Attributes.Bounds.Bottom)).ToList();

            int[] parent = new int[n];
            for (int i = 0; i < n; i++) {
                parent[i] = i;
            }

            int Find(int i)
            {
                return parent[i] == i ? i : parent[i] = Find(parent[i]);
            }

            void Union(int i, int j)
            {
                int ri = Find(i);
                int rj = Find(j);
                if (ri != rj)
                    parent[rj] = ri;
            }

            for (int i = 0; i < n; i++) {
                for (int j = i + 1; j < n; j++) {
                    bool isGroupI = sortedComponents[i] is GH_Group;
                    bool isGroupJ = sortedComponents[j] is GH_Group;
                    if (isGroupI != isGroupJ)
                        continue; // Skip unioning a group with a non-group.
                    if (IntervalsOverlap(intervals[i], intervals[j])) {
                        Union(i, j);
                    }
                }
            }

            var rowsDict = new Dictionary<int, List<IGH_DocumentObject>>();
            for (int i = 0; i < n; i++) {
                int rep = Find(i);
                if (!rowsDict.ContainsKey(rep))
                    rowsDict[rep] = new List<IGH_DocumentObject>();

                rowsDict[rep].Add(sortedComponents[i]);
            }

            foreach (var row in rowsDict.Values) {
                row.Sort((a, b) => a.Attributes.Bounds.Left.CompareTo(b.Attributes.Bounds.Left));
            }

            return rowsDict.Values.ToList();
        }

        /// <summary>
        /// Determines whether two intervals overlap.
        /// Two intervals [a.Min, a.Max] and [b.Min, b.Max] are considered overlapping 
        /// if a.Min < b.Max and a.Max > b.Min.
        /// </summary>
        private static bool IntervalsOverlap(Interval a, Interval b)
        {
            return a.Min <= b.Max && a.Max >= b.Min;
        }
    }
}
