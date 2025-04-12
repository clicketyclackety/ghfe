﻿using Eto.Forms;
using Rhino.UI;
using System.Windows.Input;
using lib;
using System.IO;
using Grasshopper.Kernel;

namespace rn.viewmodels
{
  public class FileViewModel : Rhino.UI.ViewModel
  {
    
    public FileViewModel(PanelViewModel parent, FileInfo info)
    {
      Parent = parent;
      Location = info;
    }

    public bool Enabled { get; set; } = true;

    private FileInfo _location;

    public PanelViewModel Parent { get; }

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

      LoadGrasshopperFromFile(Location.FullName);
    }

    public bool LoadGrasshopperFromFile(string path)
    {
      var io = new GH_DocumentIO();
      if (!io.Open(path)) return false;

      GH_Document ghDoc = io.Document;
      Gui gui = Gui.Load(ghDoc);

      var doc = Rhino.RhinoDoc.ActiveDoc;
      var parent = RhinoEtoApp.MainWindowForDocument(doc);

      gui.Closed += (s, e) => {
        Parent.Enabled = true;
      };

      // Application.Instance.InvokeAsync(() => {
      //   gui.ShowSemiModal(doc, parent);
      //   this.Enabled = false;
      // });

      gui.Show();

      return true;
    }

  }
}
