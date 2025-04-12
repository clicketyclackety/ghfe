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
        public string Name => "EXPANDER (GROUP OF ROWS)";
        public List<IRowElement> Children { get; set; } = new List<IRowElement>();
    }
}
