using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using lib;

namespace gh;

public class TestComponent : GH_Component
{
    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public TestComponent()
      : base("ghfe Component", "Nickname",
        "Description of component",
        "Category", "Subcategory")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
        pManager.AddBooleanParameter("Run", "Run", "Run", GH_ParamAccess.item, false);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Out", "Out", "Out", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
        var Run = false;
        DA.GetData(0, ref Run);

        if(Run){
            ScriptScanner.TryGetUIGroup(OnPingDocument(), out var group);
            List<string> componentNames = group.Objects().Select(o => o.Name).ToList();
            DA.SetDataList(0, componentNames);
        }

        else
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error");
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
    public override Guid ComponentGuid => new Guid("fd001e86-ba1e-4b96-bd4d-4bab804d547c");
}
