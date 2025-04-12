using Eto.Forms;
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

      Content = new TableLayout
      {
        Rows =
        {
          scroller,
          button,
          null
        }
      };
    }

  }
}
