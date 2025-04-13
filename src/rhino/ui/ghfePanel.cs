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
  [Guid("ADF7FDF3-7E53-4F8D-92E8-552B58915D29")]
  public class ghfePanel : Eto.Forms.Panel
  {
    public ghfePanel()
    {
      var vm = PanelViewModel.Instance;


      DataContext = vm;

      // allow for disabling the UI while a GH file is open
      this.BindDataContext((Panel p) => p.Enabled, (PanelViewModel pvm) => pvm.Enabled);

      ListBox fileList = new ListBox();
      fileList.DataContext = vm;
      fileList.DataStore = vm.Files;
      fileList.SelectedValueBinding.BindDataContext(nameof(PanelViewModel.SelectedFile));
      fileList.MouseDoubleClick += (o, a) =>
      {
        if (vm.SelectedFile.Run.CanExecute(null))
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
