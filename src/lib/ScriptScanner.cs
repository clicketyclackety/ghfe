using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

using lib.DTO;

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

        public static bool TryGetGroup(GH_Document ghDoc, string name, out GH_Group group)
        {
            var activeObjects = new List<GH_ActiveObject>();
            group = null;
            foreach (var obj in ghDoc.Objects) {
                if (obj is GH_Group) {
                    if (obj.NickName == name) {
                        group = obj as GH_Group;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a list of components that are (visually) fully contained within a group.
        /// </summary>
        /// <param name="ghDoc"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static List<IGH_DocumentObject> GetComponentsVisuallyContainedInGroup(GH_Document ghDoc, GH_Group group)
        {
            var componentsIn = new List<IGH_DocumentObject>();
            var docObjects = new List<IGH_DocumentObject>();
            var groupBounds = group.Attributes.Bounds;
            foreach (var obj in ghDoc.Objects) {
                if (obj.InstanceGuid == group.InstanceGuid) { // Skip the group itself
                    continue;
                }
                var objectBounds = obj.Attributes.Bounds;
                if (groupBounds.Contains(objectBounds)) {
                    componentsIn.Add(obj);
                }
            }
            return componentsIn;
        }

        public static bool TryGetUIRowGroup(GH_Document ghDoc, out RowGroup rowGroup)
        {
            bool groupExists = TryGetGroup(ghDoc, UIGroupName, out var group);
            rowGroup = new RowGroup(); // root
            if (groupExists) {
                var rows = ComponentGrouper.FormRows(GetComponentsVisuallyContainedInGroup(ghDoc, group));
                for (int i = 0; i < rows.Count; i++) {
                    RowLeaf row = new RowLeaf();
                    bool isValidRow = false;
                    for (int j = 0; j < rows[i].Count; j++) {
                        if (rows[i][j] is not GH_Group) {
                            isValidRow = true;
                            row.Components.Add(rows[i][j]);
                        }
                    }
                    if (isValidRow) {
                        rowGroup.Children.Add(row);
                    }
                }
                if (rowGroup.Children.Count > 0) {
                    return true;
                }
            }
            return false;
        }
    }
}
