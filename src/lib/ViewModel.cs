using Eto.Forms;

using Rhino.UI.Controls;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Components;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;

using System.ComponentModel;
using lib.DTO;

namespace lib;

internal class GViewModel : INotifyPropertyChanged
{

  private GH_Document Doc { get; }

  public RowGroup Sorted { get; set; }

  public GViewModel(GH_Document doc)
  {
    Doc = doc;
    Sorted = GetSortedRows();
  }

  public event PropertyChangedEventHandler PropertyChanged;

  internal void Run()
  {
    Doc.NewSolution(true);
  }

  internal RowGroup GetSortedRows()
  {
    ScriptScanner.TryGetUIRowGroup(Doc, out RowGroup group);
    return group ?? new RowGroup();
  }

}