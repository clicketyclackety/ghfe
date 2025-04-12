using Eto.Forms;
using Eto.Drawing;

using Rhino.UI.Controls;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Components;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using lib.DTO;
using Rhino.Geometry;
using Rhino.UI;
using Rhino.Input;

namespace lib;

public class Gui : Dialog
{

  private GViewModel? Model => DataContext as GViewModel;

  private Gui(GViewModel viewModel)
  {
    DataContext = viewModel;

    var run = new Button() { Text = "Run!" };
    run.Click += (s,e) => Model?.Run();

    var child = CreateRow(viewModel.Sorted);

    var layout = new DynamicLayout();
    layout.BeginVertical(new Padding(2), new Size(8, 4), true, true);
    layout.Add(child, true, false);
    layout.AddSpace(true, true);
    layout.Add(run, true, false);
    layout.EndVertical();

    Content = layout;
    this.Padding = 4;
    this.Resizable = true;

    DefaultButton = run;
    AbortButton = new();
  }

  public static Gui Load(GH_Document doc)
  {
    var viewModel = new GViewModel(doc);
    var gui = new Gui(viewModel);
    return gui;
  }

  #region Eto Creation

  private Control? CreateRow(RowGroup row)
  {
    DynamicLayout layout = new();
    layout.BeginVertical(new Padding(2), new Size(8, 4), true, true);

    foreach(var rowItem in row.Children)
    {
      var rowControl = ConvertRowGroup(rowItem);
      layout.Add(rowControl, true, false);
    }
    layout.EndVertical();
    return layout;
  }

  private Control? ConvertRowGroup(IRowElement rowItem)
    => rowItem switch
    {
      RowLeaf leaf => GetMultipleRow(leaf.Components),
      RowGroup group => CreateGroup(group),

      _ => null
    };

  private Control? GetMultipleRow(List<IGH_DocumentObject> components)
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

  public Control? ConvertGrasshopperObject(IGH_DocumentObject aObject)
    => aObject switch
    {
      IGH_Component component => ConvertComponent(component),
      IGH_Param param => ConvertParam(param),

      _ => null
    };

  private Control? ConvertParam(IGH_Param param)
  {
    var control = param switch
    {
      Param_Integer integer => GetNumberHandlerFromInteger(integer),
      Param_String str => CreateTextInput(str),
      GH_NumberSlider slider => CreateSlider(slider),
      GH_Panel panel => CreateLabel(panel),
      
      Param_Curve curve => PickCurve(GetGeometryButton(curve), curve),
      Param_Point point => PickPoint(GetGeometryButton(point), point),
      // Param_Geometry geom => GetGeometryButton<GeometryBase>(geom),

      _ => null
    };

    if (control is not null)
    {
      control.ToolTip = param.Description;
    }

    return control;
  }

  private Button? PickPoint(Button button, Param_Point param)
  {
    button.Click += (s, e) => {
      this.PushPickButton((sender, args) => {

        if (RhinoGet.GetPoint("Point", true, out var point) == Rhino.Commands.Result.Success)
        {
          // TODO : 
          // param.CollectData();
        }
      });
    };

    return button;
  }

  private Button? PickCurve(Button button, Param_Curve param)
  {
    button.Click += (s, e) => {
      this.PushPickButton((sender, args) => {

        if (RhinoGet.GetPolyline(out var polyline) == Rhino.Commands.Result.Success)
        {
          // TODO : 
          // param.CollectData();
        }
      });
    };

    return button;
  }

  private Button? GetGeometryButton(IGH_Param param)
  {
    var button = new Button()
    {
      Text = $"Choose {param.NickName}",
    };

    return button;
  }

  private static Control? CreateLabel(GH_Panel panel)
  {
    return new Label()
    {
      Text = panel.UserText,
      Height = 20,
      TextAlignment = TextAlignment.Center,
      TextColor = Colors.Gray,
    };
  }

  private static Control? GetNumberHandlerFromInteger(Param_Integer integer)
  {
    var box = new NumericUpDownWithUnitParsing()
    {
      // Value = integer.Value
    };
    return WithLabel(integer.NickName, box);
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
      Width = 120,
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

  private Control? CreateGroup(RowGroup group)
  {
    var etoGroup = new Eto.Forms.Expander();
    var layout = new DynamicLayout();
    layout.BeginVertical(new Padding(2), new Size(8, 4), true, true);
    
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