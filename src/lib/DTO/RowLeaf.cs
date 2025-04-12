using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;

namespace lib.DTO
{
    public class RowLeaf : IRowElement
    {
        public string Name => "Leaf";
        public List<IGH_DocumentObject> Components { get; set; } = new List<IGH_DocumentObject>();
    }
}
