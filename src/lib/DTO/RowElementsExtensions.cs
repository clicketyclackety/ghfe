using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace lib.DTO
{
    public static class RowElementsExtensions
    {
        public static string DisplayTree(this IRowElement elem, string indent = "")
        {
            // Start with the current element name.
            string result = $"{indent}{elem.Name}:";

            // If this is a RowGroup, recursively build strings for its children.
            if (elem is RowGroup group) {
                foreach (var child in group.Children) {
                    result += "\n";
                    result += child.DisplayTree(indent + "  ");
                }
            }
            // If this is a RowLeaf, optionally display its components.
            else if (elem is RowLeaf leaf) {
                result += "  ";
                result += string.Join(",", leaf.Components.Select(c => c.NickName));
                //result += "\n";
            }
            return result;
        }
    }
}
