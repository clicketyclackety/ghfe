using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;

namespace lib.DTO
{
    public class RowGroup : IRowElement
    {
        public string Name { get; set; } = "Row group.";
        public List<IRowElement> Children { get; set; } = new List<IRowElement>();

        public RowGroup(string name)
        {
            Name = name;
        }
    }
}
