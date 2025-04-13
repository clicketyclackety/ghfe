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
        pManager.AddPointParameter("Origin", "origin", "origin", GH_ParamAccess.item, new Point3d(0, 0, 0));
        pManager.AddNumberParameter("Width", "width", "width", GH_ParamAccess.item, 100);
        pManager.AddNumberParameter("Row Height", "Row height", "height", GH_ParamAccess.item, 100);
        pManager.AddNumberParameter("Indent", "indent", "indent", GH_ParamAccess.item, 10);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
        pManager.AddCurveParameter("Outlines", "Outlines", "Outlines", GH_ParamAccess.list);
        pManager.AddTextParameter("Labels", "Labels", "Labels", GH_ParamAccess.list);
        pManager.AddCurveParameter("Margins", "Margins", "Margins", GH_ParamAccess.list);
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
        var origin = new Point3d(0, 0, 0);
        DA.GetData(1, ref origin);
        double totalWidth = 100;
        DA.GetData(2, ref totalWidth);
        double leafHeight = 100;
        DA.GetData(3, ref leafHeight);
        double indent = 10;
        DA.GetData(4, ref indent);
        var outlines = new List<Polyline>();
        var labels = new List<string>();
        var margins = new List<Line>();

        if (ScriptScanner.TryGetGroup(ghDoc, groupName, out var group)) {
            var hierarchy = ScriptScanner.BuildGroupHierarchy(ghDoc, group);
            var rowGroup = hierarchy.ToRowGroup();
            _ = rowGroup.DrawHierarchy(origin, totalWidth, leafHeight, indent, outlines, labels, margins);
        }

        else {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Group not found");
        }

        DA.SetDataList(0, outlines);
        DA.SetDataList(1, labels);
        DA.SetDataList(2, margins);

    }
    protected override System.Drawing.Bitmap Icon => null;

    public override Guid ComponentGuid => new Guid("2b80d1a3-67b3-441d-a723-1b4350995fda");
}
