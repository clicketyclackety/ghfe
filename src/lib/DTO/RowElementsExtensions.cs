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
    }
}
