using Eto.Drawing;
using Eto.Forms;
using Rhino.UI;
using rn.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace rn.ui
{
  class FileCell : CustomCell
  {
    protected override Control OnCreateCell(CellEventArgs args)
    {
      var label = new Label() { TextAlignment = TextAlignment.Left };
      label.BindDataContext(l => l.Text, (FileViewModel f) => f.Name);
      return label;
    }

    protected override void OnConfigureCell(CellEventArgs args, Control control)
    {
      if (control is not Label text) return;
      if (args.Item is not TreeGridItem item) return;
      if (item.Values?.FirstOrDefault() is FileViewModel fvm) 
        text.Text = fvm.Name;

      // subdirectory does not have FileViewModel, give it the item string
      if (item.Values.Length > 0)
        text.Text = item.Values[0].ToString();
    }
  }


  class FileCanRunCell : CustomCell
  {
    protected override Control OnCreateCell(CellEventArgs args)
    {
      // a subdirectory does not contain a FileViewModel. In that case return an empty label
      if (args.Item is not TreeGridItem item) return new Label();
      if (item.Values?.FirstOrDefault() is not FileViewModel) return new Label();

      var cb = new CheckBox();
      cb.BindDataContext((CheckBox a) => a.Checked, (FileViewModel vm) => vm.CanRun);
      return cb;
    }

    protected override void OnConfigureCell(CellEventArgs args, Control control)
    {
      if (control is not CheckBox cb) return;
      if (args.Item is not TreeGridItem item) return;
      if (item.Values?.FirstOrDefault() is not FileViewModel fvm) return;

      // set the value
      cb.Checked = fvm.CanRun;
    }
  }



  [Guid("ADF7FDF3-7E53-4F8D-92E8-552B58915D29")]
  public class ghfePanel : Eto.Forms.Panel
  {
    public ghfePanel()
    {
      var vm = PanelViewModel.Instance;


      DataContext = vm;

      // allow for disabling the UI while a GH file is open
      this.BindDataContext((Panel p) => p.Enabled, (PanelViewModel pvm) => pvm.Enabled);

      TreeGridView fileList = new TreeGridView();
      fileList.DataContext = vm;
      var column1 = new GridColumn();
      column1.HeaderText = "File name";
      column1.Editable = false;
      column1.DataCell = new FileCell();
      
      fileList.Columns.Add(column1);

      var column2 = new GridColumn();
      column2.HeaderText = "Can run";
      column2.Editable = false;
      column2.DataCell = new FileCanRunCell();
      fileList.Columns.Add(column2);

      fileList.DataStore = vm.FilesTree;
      vm.PropertyChanged += (o, a) =>
      {
        if (a.PropertyName == nameof(PanelViewModel.LastSelectedDirectory))
        {
          fileList?.ReloadData();
        }
      };

      this.Shown += (o, a) =>
      {
        vm?.UpdateDirectory(); 
        fileList?.ReloadData();
      };

      // this binding does not work:
      //fileList.SelectedItemBinding.BindDataContext(nameof(PanelViewModel.SelectedFile));

      // so do this instead.
      fileList.SelectedItemChanged += (o, a) =>
      {
        if (o is TreeGridView tgv)
        {
          if (tgv.SelectedItem is TreeGridItem tgi)
          {
            vm.SelectedFile = tgi.GetValue(0) as FileViewModel;
          }
          else
          {
            vm.SelectedFile = null;
          }
        }
      };

      fileList.MouseDoubleClick += (o, a) =>
      {
        if (vm.SelectedFile?.Run.CanExecute(null)??false)
        {
          vm.SelectedFile.Run.Execute(null);
        }
      };

      Scrollable scroller = new Scrollable
      {
        MinimumSize = new Eto.Drawing.Size(50, 200),
        Content = fileList
      };

      Button button = new Button { Text = "Select path..." };
      button.Click += (o, a) => vm.Browse();

      var dl = new DynamicLayout();
      dl.BeginVertical(new Padding(5), new Size(2, 8));
      dl.Add(scroller,true, true);
      dl.Add(button,true, false);
      dl.EndVertical();


      Content = dl;

#if NETCOREAPP
      this.UseRhinoStyle();
      fileList.UseRhinoStyle();
      button.UseRhinoStyle();
#endif
    }

  }
}
