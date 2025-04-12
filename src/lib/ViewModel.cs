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
  }

  public event PropertyChangedEventHandler PropertyChanged;

  internal void Run()
  {
    Doc.NewSolution(false);
  }

  internal RowGroup GetSortedRows()
  {
    return Sorted = null;
  }
}