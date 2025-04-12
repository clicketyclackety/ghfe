using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

using lib;

namespace gh;

public class GetVisuallyContainedComponentsGH : GH_Component
{
    public GetVisuallyContainedComponentsGH()
      : base("Visually Contained", "VisuallyContained",
        "Groups components by rows",
        "Category", "Subcategory")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
        pManager.AddTextParameter("Group name", "group name", "group name", GH_ParamAccess.item, "UI");
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Components Within", "ComponentsWithin", "Components visually contained by the group.", GH_ParamAccess.list);
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

        if (ScriptScanner.TryGetGroup(OnPingDocument(), groupName, out var group)) {
            var componentsIn = ScriptScanner.GetComponentsVisuallyContainedInGroup(OnPingDocument(), group);
            var componentNames = componentsIn.Select(x => x.Name).ToList();
            DA.SetDataList(0, componentNames);
        }

        else {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Group not found");
        }

    }
    protected override System.Drawing.Bitmap Icon => null;

    public override Guid ComponentGuid => new Guid("439d9920-799c-49db-b9a5-147291e008ad");
}
