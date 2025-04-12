using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using lib;
using System.IO;

namespace rn.viewmodels
{
  public class FileViewModel : Rhino.UI.ViewModel
  {
    public FileViewModel(FileInfo info)
    {
      Location = info;
    }

    private FileInfo _location;
    public FileInfo Location
    {
      get => _location;
      set
      {
        _location = value;
        RaisePropertyChanged(nameof(Location));
      }
    }

    public override string ToString()
    {
      return Location?.Name ?? base.ToString();
    }

    RelayCommand _run;
    public ICommand Run => _run ?? (_run = new RelayCommand(OnRunCommand, CanExecute));

    private bool CanExecute() => true;
    private void OnRunCommand ()
    {
      if (null == Location) return;
      if (!Location.Exists) return;

      GrasshopperLoader.LoadGrasshopperFromFile(Location.FullName);
    }
  }
}
