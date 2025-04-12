using Eto.Forms;

using Rhino.UI.Controls;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Components;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using lib.DTO;
using Eto.Drawing;

namespace lib;

internal class Gui : Form
{

  private GViewModel? Model => DataContext as GViewModel;

  private Gui(GViewModel viewModel)
  {
    DataContext = viewModel;

    var run = new Button() { Text = "Run!" };
    run.Click += (s,e) => Model?.Run();

    var child = CreateRow(viewModel.Sorted);

    var layout = new DynamicLayout();
    layout.BeginHorizontal();
    layout.Add(child, true, false);
    layout.AddSpace();
    layout.Add(run, true, false);
    layout.EndHorizontal();

    Content = layout;
  }

  public static Gui Load(GH_Document doc)
  {
    var viewModel = new GViewModel(doc);
    var sorted = viewModel.GetSortedRows();
    var gui = new Gui(viewModel);

    return gui;
  }

  #region Eto Creation

  public static Control? CreateRow(RowGroup row)
  {
    DynamicLayout layout = new();
    layout.BeginHorizontal();

    foreach(var rowItem in row.Children)
    {
      var rowControl = ConvertRowGroup(rowItem);
      layout.Add(rowControl, true, false);
    }
    layout.EndHorizontal();
    return layout;
  }

  private static Control? ConvertRowGroup(IRowElement rowItem)
    => rowItem switch
    {
      RowLeaf leaf => GetMultipleRow(leaf.Components),
      RowGroup group => CreateGroup(group),

      _ => null
    };

  private static Control? GetMultipleRow(List<IGH_DocumentObject> components)
  {
    DynamicLayout layout = new();
    layout.BeginHorizontal();
    foreach(var component in components)
    {
      var etoControl = ConvertGrasshopperObject(component);
    layout.Add(etoControl, true, false);
    }
    layout.EndHorizontal();
    return layout;
  }

  #endregion

  #region GH to Eto

  public static Control? ConvertGrasshopperObject(IGH_DocumentObject aObject)
    => aObject switch
    {
      IGH_Component component => ConvertComponent(component),
      IGH_Param param => ConvertParam(param),

      _ => null
    };

  private static Control? ConvertParam(IGH_Param param)
  {
    var control = param switch
    {
      Param_Integer integer => GetNumberHandlerFromInteger(integer),

      _ => null
    };

    if (control is not null)
    {
      control.ToolTip = param.Description;
    }

    return control;
  }

  private static Control? GetNumberHandlerFromInteger(Param_Integer integer)
  {
    var box = new TextBox();
    return box;
  }

  private static Control? ConvertComponent(IGH_Component component)
    => component switch
    {
      // GH_Group group => CreateGroup(group),
      GH_NumberSlider slider => CreateSlider(slider),
      GH_Scribble scribble => CreateTitle(scribble),
      _ => null
    };

    private static Control? CreateTitle(GH_Scribble scribble)
  {
    var title = new Eto.Forms.Label
    {
      Text = scribble.Text,
      Font = new Font(SystemFont.Default),
      TextColor = Colors.Black,
      VerticalAlignment = VerticalAlignment.Center,
      TextAlignment = TextAlignment.Left
    };
    return title;
  }

  private static Control? CreateSlider(GH_NumberSlider slider)
  {
    var upDown = new NumericUpDownWithUnitParsing(true);
    upDown.Value = (double)slider.CurrentValue;
    // TODO : Add Min/Max

    upDown.ValueChanged += (s, e) => {
      slider.SetSliderValue((decimal)upDown.Value);
    };

    return upDown;
  }

  private static Control? CreateGroup(RowGroup group)
  {
    var etoGroup = new Eto.Forms.Expander();
    var layout = new DynamicLayout();
    layout.BeginVertical();
    
    foreach(var groupChild in group.Children)
    {
      var etoChild = ConvertRowGroup(groupChild);
      layout.Add(etoChild, true, false);
    }

    layout.EndVertical();

    return etoGroup;
  }

  public Dialog GetDialog()
  {
    return null;
  }

#endregion

}