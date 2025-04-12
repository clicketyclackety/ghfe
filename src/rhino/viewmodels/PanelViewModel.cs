using Rhino.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using EF = Eto.Forms;

namespace rn.viewmodels
{
  public class PanelViewModel : ViewModel
  {
    private PanelViewModel() { }

    private static PanelViewModel s_instance;
    public static PanelViewModel Instance
    {
      get
      {
        if (s_instance == null)
        {
          s_instance = new PanelViewModel();
        }
        return s_instance;
      }
    }

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

    bool _enabled = true;
    private DirectoryInfo _lastSelectedDirectory;

    public bool Enabled { get => _enabled;
    set
      {
        _enabled = value;
        RaisePropertyChanged(nameof(Enabled));
      }
    }

    public DirectoryInfo LastSelectedDirectory
    {
      get => _lastSelectedDirectory;
      internal set
      {
        // refuse to set to a non-existent directory
        if (!value.Exists) return;

        _lastSelectedDirectory = value;

        // enumerate the *.gh files in the directory
        Files.Clear();
        IEnumerable<FileInfo> ghFiles = LastSelectedDirectory.EnumerateFiles("*.gh");
        foreach (FileInfo ghFile in ghFiles)
        {
          FileViewModel fvm = new FileViewModel(this, ghFile);
          Files.Add(fvm);
        }

        RaisePropertyChanged(nameof(LastSelectedDirectory));
      }
    }

    public void Browse()
    {
      // show the user the folder select dialog
      EF.SelectFolderDialog dlg = new EF.SelectFolderDialog();
      EF.DialogResult result = dlg.ShowDialog(RhinoEtoApp.MainWindow);
      if (result == EF.DialogResult.Cancel) return;

      // set the directory. Setting will re-populate the files list
      LastSelectedDirectory = new DirectoryInfo(dlg.Directory);

    }
  }
}
