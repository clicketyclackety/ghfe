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
                    if (obj.NickName.StartsWith(name)) {
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
            rowGroup = new RowGroup(name: "UI"); // root
            if (groupExists) {
                var hierarchy = ScriptScanner.BuildGroupHierarchy(ghDoc, group);
                rowGroup = hierarchy.ToRowGroup();
                return true;
            }
            return false;
        }

        public class GroupHierarchy
        {
            public GH_Group Group { get; set; }
            public List<IGH_DocumentObject> DirectComponents { get; set; } = new List<IGH_DocumentObject>();
            public List<GroupHierarchy> NestedGroup { get; set; } = new List<GroupHierarchy>();

            public RowGroup ToRowGroup()
            {
                var rowGroup = new RowGroup(Group.NickName);
                var allElements = DirectComponents.ToList();
                allElements.AddRange(NestedGroup.Select(ng => ng.Group));
                var rows = ComponentGrouper.FormRows(allElements);
                for (int i = 0; i < rows.Count; i++) {
                    if (rows[i].Count == 0) {
                        continue;
                    }
                    else if (rows[i][0] is GH_Group) {
                        foreach (var nestedGroup in NestedGroup) {
                            if (rows[i][0].InstanceGuid == nestedGroup.Group.InstanceGuid) {
                                rowGroup.Children.Add(nestedGroup.ToRowGroup());
                            }
                        }
                    }
                    else {
                        RowLeaf row = new RowLeaf();
                        foreach (var obj in rows[i]) {
                            if (obj is GH_Group) {
                                continue;
                            }
                            row.Components.Add(obj);
                        }
                        rowGroup.Children.Add(row);
                    }
                }
                return rowGroup;
            }
        }

        public static GroupHierarchy BuildGroupHierarchy(GH_Document ghDoc, GH_Group group)
        {
            GroupHierarchy hierarchy = new GroupHierarchy { Group = group };

            var containedObjects = GetComponentsVisuallyContainedInGroup(ghDoc, group);

            List<IGH_DocumentObject> potentialDirectComponents = new List<IGH_DocumentObject>();
            List<GH_Group> nestedGroups = new List<GH_Group>();

            foreach (var obj in containedObjects) {
                if (obj is GH_Group nested) {
                    nestedGroups.Add(nested);
                }
                else {
                    potentialDirectComponents.Add(obj);
                }
            }

            foreach (var nested in nestedGroups) {
                var childHierarchy = BuildGroupHierarchy(ghDoc, nested);
                hierarchy.NestedGroup.Add(childHierarchy);
            }

            // Filter the potential direct components: keep only those that are not completely within a nested group's bounds.
            hierarchy.DirectComponents.AddRange(
                potentialDirectComponents.Where(comp =>
                    !nestedGroups.Any(nested => nested.Attributes.Bounds.Contains(comp.Attributes.Bounds)))
            );

            return hierarchy;
        }
    }
}
