using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Rhino.Display;
using Rhino.Geometry;

namespace lib.DTO
{
    public static class RowElementsExtensions
    {
        public static string DisplayTree(this IRowElement elem, string indent = "")
        {
            string result = $"{indent}{elem.Name}:";

            if (elem is RowGroup group) {
                foreach (var child in group.Children) {
                    result += "\n";
                    result += child.DisplayTree(indent + "  ");
                }
            }

            else if (elem is RowLeaf leaf) {
                result += "  ";
                result += string.Join(",", leaf.Components.Select(c => c.NickName));
                //result += "\n";
            }
            return result;
        }

        public static double DrawHierarchy(this RowGroup uiRowGroup, Point3d origin, double totalWidth, double leafHeight, double indent, List<Polyline> outlines, List<string> labels, List<Line> margins)
        {
            double currentY = origin.Y;

            // Iterate through each row element in the list
            foreach (var element in uiRowGroup.Children) {
                if (element is RowLeaf leaf) {
                    int cellCount = (leaf.Components != null && leaf.Components.Count > 0) ? leaf.Components.Count : 1;
                    double cellWidth = totalWidth / cellCount;

                    for (int i = 0; i < cellCount; i++) {
                        // Define the bottom-left and top-right points for the cell rectangle.
                        Point3d cellOrigin = new Point3d(origin.X + i * cellWidth, currentY, 0);
                        Point3d cellLowerRight = new Point3d(origin.X + (i + 1) * cellWidth, currentY - leafHeight, 0);

                        Rectangle3d cellRect = new Rectangle3d(Plane.WorldXY, cellOrigin, cellLowerRight);
                        string leafName = leaf.Name;
                        outlines.Add(cellRect.ToPolyline());
                        labels.Add(leafName);
                    }

                    currentY -= leafHeight;
                }
                else if (element is RowGroup group) {
                    double groupStartY = currentY;

                    // Optionally, you might want to draw the group’s name (or a container rectangle) in the margin or above its contents.
                    // In this example, we continue drawing the children indented by the specified value.
                    // The available width for children is reduced by the indent.
                    currentY = DrawHierarchy(group as RowGroup, new Point3d(origin.X + indent, currentY, 0), totalWidth - indent, leafHeight, indent, outlines, labels, margins);

                    // After the recursive call, currentY marks the end of the last nested row.
                    // Next, we draw a vertical line that visually indicates the indentation “guide”.
                    // We place the line at a fixed offset from the original left edge.
                    Point3d lineStart = new Point3d(origin.X + indent * 0.5, groupStartY, 0);
                    Point3d lineEnd = new Point3d(origin.X + indent * 0.5, currentY, 0);
                    margins.Add(new Line(lineStart, lineEnd));
                }
            }
            // Return the Y position after all elements have been rendered.
            return currentY;
        }
    }
}
