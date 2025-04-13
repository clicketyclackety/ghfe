using Eto.Forms;
using Rhino.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

    public void UpdateDirectory()
    {
      if (LastSelectedDirectory?.Exists ?? false)
      {
        // enumerate the *.gh files in the directory
        Files.Clear();
        FilesTree.Clear();
        TreeGridItem root = new TreeGridItem(_lastSelectedDirectory.Name);
        PopulateFromDirectory(root, _lastSelectedDirectory);
        root.Expanded = true;
        FilesTree.Add(root);
      }

    }

    public TreeGridItemCollection FilesTree { get; } = new TreeGridItemCollection();

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
        UpdateDirectory();

        RaisePropertyChanged(nameof(LastSelectedDirectory));
      }
    }

    private void PopulateFromDirectory(TreeGridItem parent, DirectoryInfo di)
    {
      // issue: this enumerates other files too somehow
      var files = di.EnumerateFiles("*.gh");
      foreach (var file in files)
      {
        FileViewModel fvm = new FileViewModel(this, file);
        TreeGridItem fileItem = new TreeGridItem(fvm);
        parent.Children.Add(fileItem);
      }

      var subDirs = di.EnumerateDirectories();
      if (subDirs.Any())
      {
        foreach (var subDir in subDirs)
        {
          TreeGridItem subItem = new TreeGridItem(subDir.Name) { Expanded = true };

          parent.Children.Add(subItem);
          PopulateFromDirectory(subItem, subDir);
        }
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
