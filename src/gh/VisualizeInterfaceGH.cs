using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

using lib;
using lib.DTO;

using Rhino.Geometry;

namespace gh;

public class VisualizeInterfaceGH : GH_Component
{
    public VisualizeInterfaceGH()
      : base("Visualize Hierarchy", "VisualizeHierarchy",
        "Visualizes hierarchy of the group",
        "Category", "Subcategory")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
        pManager.AddTextParameter("Group name", "group name", "group name", GH_ParamAccess.item, "UI");
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
        //pManager.AddTextParameter("Hierarchy", "Hierarchy", "Hierarchy of the group.", GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        string groupName = string.Empty;
        DA.GetData(0, ref groupName);

        var ghDoc = OnPingDocument();

        if (ScriptScanner.TryGetGroup(ghDoc, groupName, out var group)) {
            var hierarchy = ScriptScanner.BuildGroupHierarchy(ghDoc, group);
            var rowGroup = hierarchy.ToRowGroup();
            var description = rowGroup.DisplayTree();
            //DA.SetData(0, description);
        }

        else {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Group not found");
        }

    }
    protected override System.Drawing.Bitmap Icon => null;

    public override Guid ComponentGuid => new Guid("2b80d1a3-67b3-441d-a723-1b4350995fda");
}
