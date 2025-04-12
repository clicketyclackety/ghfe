using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using EF = Eto.Forms;

namespace rn.viewmodels
{
  public class PanelViewModel : ViewModel
  {
    public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();

    private FileViewModel _selectedFile;
    public FileViewModel SelectedFile
    {
      get => _selectedFile;
      set
      {
        _selectedFile = value;
        RaisePropertyChanged(nameof(SelectedFile));
      }
    }

    public void Browse()
    {
      // show the user the folder select dialog
      EF.SelectFolderDialog dlg = new EF.SelectFolderDialog();
      EF.DialogResult result = dlg.ShowDialog(RhinoEtoApp.MainWindow);
      if (result == EF.DialogResult.Cancel) return;

      // enumerate the *.gh files in the directory
      DirectoryInfo di = new DirectoryInfo(dlg.Directory);
      if (!di.Exists) return;

      Files.Clear();
      IEnumerable<FileInfo> ghFiles = di.EnumerateFiles("*.gh");
      foreach(FileInfo ghFile in ghFiles)
      {
        FileViewModel fvm = new FileViewModel(ghFile);
        Files.Add(fvm);
      }
    }
  }
}
