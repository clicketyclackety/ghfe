using Eto.Forms;

using Rhino.UI.Controls;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Components;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using lib.DTO;
using Eto.Drawing;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace lib;

public class Gui : Form
{

  private GViewModel? Model => DataContext as GViewModel;

  private Gui(GViewModel viewModel)
  {
    DataContext = viewModel;

    var run = new Button() { Text = "Run!" };
    run.Click += (s,e) => Model?.Run();

    var child = CreateRow(viewModel.Sorted);

    var layout = new DynamicLayout();
    layout.BeginVertical();
    layout.Add(child, true, false);
    layout.AddSpace();
    layout.Add(run, true, false);
    layout.EndVertical();

    Content = layout;
  }

  public static Gui Load(GH_Document doc)
  {
    var viewModel = new GViewModel(doc);
    var gui = new Gui(viewModel);

    return gui;
  }

  #region Eto Creation

  public static Control? CreateRow(RowGroup row)
  {
    DynamicLayout layout = new();
    layout.BeginVertical();

    foreach(var rowItem in row.Children)
    {
      var rowControl = ConvertRowGroup(rowItem);
      layout.Add(rowControl, true, false);
    }
    layout.EndVertical();
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
      Param_String str => CreateTextInput(str),
      GH_NumberSlider slider => CreateSlider(slider),

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
      GH_Scribble scribble => CreateTitle(scribble),

      _ => null
    };

  private static Control? WithLabel(string title, Control? control)
  {
    if (string.IsNullOrEmpty(title)) return control;

    var label = new Label()
    {
      Text = title,
      Width = 80,
      Wrap = WrapMode.None,
    };
    
    DynamicLayout layout = new();
    layout.BeginHorizontal();
    layout.Add(label, false, true);
    layout.Add(control, true, true);
    layout.EndHorizontal();

    return layout;
  }

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

    return WithLabel(slider.NickName, upDown);
  }

  private static Control? CreateTextInput(Param_String text_param)
  {
    var text_input = new Eto.Forms.TextBox
    {
      PlaceholderText = text_param.NickName,
      Text = "",
      ReadOnly = false
    };
    return text_input;
  }

  private static Control? CreateIntegerInput(Param_Integer integer)
  {
    var box = new NumericStepper()
    {
      Value = integer.VolatileData.AllData(true).Where(d => d.IsValid).OfType<GH_Integer>().Select(d => d.Value).FirstOrDefault(),
    };

    return box;
  }

  // private static TValue GetSingleValue<TValue>(IGH_Param param) where TValue : IGH_Goo
  // {
  //   var data = param.VolatileData.AllData(true);
  //   var valid = data.Where(d => d.IsValid);
  //   var type = data.OfType<TValue>();
  //   var values = type.Select(d => d.Value)
  // }

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