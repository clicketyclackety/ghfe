using Eto.Forms;

using Rhino.UI.Controls;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Components;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;

using System.ComponentModel;
using lib.DTO;

namespace lib;

public class GViewModel : INotifyPropertyChanged
{

  public GH_Document Doc { get; }

  public RowGroup Sorted { get; set; }

  public GViewModel(GH_Document doc)
  {
    Doc = doc;
    Sorted = GetSortedRows();
  }

  public event PropertyChangedEventHandler PropertyChanged;

  internal void Run()
  {
    foreach(var obj in Doc.ActiveObjects())
    {
      obj.CollectData();
      obj.ComputeData();
    }

    Doc.NewSolution(true, GH_SolutionMode.CommandLine);
    Doc.ExpirePreview(true);
    Doc.ForcePreview(true);
    Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
  }

  internal RowGroup GetSortedRows()
  {
    ScriptScanner.TryGetUIRowGroup(Doc, out RowGroup group);
    return group ?? new RowGroup("Err");
  }

}