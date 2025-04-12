using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

using lib;

namespace gh;

public class ComponentGrouperGH : GH_Component
{
    public ComponentGrouperGH()
      : base("Component Grouper", "ComponentGrouper",
        "Groups components by rows",
        "Category", "Subcategory")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
        pManager.AddTextParameter("Group name", "group name", "group name", GH_ParamAccess.item, "UI");
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Ordered Components", "OrderedComponents", "Components ordered by rows", GH_ParamAccess.tree);
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

        DataTree<string> componentNames = new DataTree<string>();

        if (ScriptScanner.TryGetGroup(OnPingDocument(), groupName, out var group)) {
            var rows = ComponentGrouper.FormRows(group.Objects());
            for (int i = 0; i < rows.Count; i++) {
                var row = rows[i];
                var path = new GH_Path(i);
                componentNames.AddRange(row.Select(x => x.Name), path);
            }
            DA.SetDataTree(0, componentNames);
        }

        else {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Group not found");
        }

    }

    /// <summary>
    /// Provides an Icon for every component that will be visible in the User Interface.
    /// Icons need to be 24x24 pixels.
    /// You can add image files to your project resources and access them like this:
    /// return Resources.IconForThisComponent;
    /// </summary>
    protected override System.Drawing.Bitmap Icon => null;

    /// <summary>
    /// Each component must have a unique Guid to identify it. 
    /// It is vital this Guid doesn't change otherwise old ghx files 
    /// that use the old ID will partially fail during loading.
    /// </summary>
    public override Guid ComponentGuid => new Guid("3db4bc36-0d98-410e-bbfc-175b0697ca75");
}
