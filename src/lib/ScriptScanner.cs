using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace lib
{
    public static class ScriptScanner
    {
        public const string UIGroupName = "UI";

        public static bool TryGetUIGroup(GH_Document document, out GH_Group group)
        {
            var activeObjects = new List<GH_ActiveObject>();
            var allObjects = document.Objects;
            group = null;
            foreach (var obj in allObjects)
            {
                if (obj is GH_Group)
                {
                    if (obj.NickName == UIGroupName)
                    {
                        group = obj as GH_Group;
                        return true;
                    }
                }
                else
                {
                    //return false;
                }
            }
            return true;
        }
    }
}
