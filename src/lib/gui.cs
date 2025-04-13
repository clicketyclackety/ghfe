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
using System.ComponentModel;

namespace lib;

public class Gui : FloatingForm
{

  public GViewModel Model => DataContext as GViewModel;

  private Gui(GViewModel viewModel)
  {
    DataContext = viewModel;

    var run = new Button() { Text = "Run" };
    run.Click += (s,e) => Model?.Run();

    var automatic = new CheckBox() { Checked = false, Text = "Auto" };
    automatic.CheckedChanged += (s, e) => {
      Model.Automatic = true;
    };

    var bottomLayout = new DynamicLayout()
    {
      Spacing = new Size(8, 0),
    };
    bottomLayout.BeginHorizontal();
    bottomLayout.Add(run, true, true);
    bottomLayout.Add(automatic, false, false);
    bottomLayout.EndHorizontal();

    // TODO : Cancel Button

    var child = CreateRow(viewModel.Sorted);

    var layout = new DynamicLayout();
    layout.BeginVertical(new Padding(2), new Size(8, 4), true, true);
    layout.Add(child, true, false);
    layout.AddSpace(true, true);
    layout.Add(bottomLayout, true, false);
    layout.EndVertical();

    Content = layout;
    this.Padding = 4;
    this.Resizable = true;
    this.Title = viewModel.Sorted.Name;
    this.MinimumSize = new Size(200, 80);
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
      
      // Param_Curve curve => PickCurve(GetGeometryButton(curve), curve),
      // Param_Point point => PickPoint(GetGeometryButton(point), point),
      // Param_Geometry geom => GetGeometryButton<GeometryBase>(geom),

      // TODO : Boolean Toggle / CheckBox
      GH_BooleanToggle toggle => CreateToggle(toggle),

      // TODO : Buttons

      _ => Unsupported(param)
    };

    if (control is not null)
    {
      control.ToolTip = param.Description;
    }

    return control;
  }

  private Control? CreateToggle(GH_BooleanToggle toggle)
  {
    CheckBox checkBox = new CheckBox()
    {
      Checked = toggle.Value,
      Text = toggle.NickName
    };

    checkBox.CheckedChanged += (s, e) => {
      toggle.ClearData();
      toggle.Value = checkBox.Checked.GetValueOrDefault(false);
      toggle.CollectData();
      toggle.ComputeData();

      if (Model.Automatic)
        Model?.Run();
    };

    return checkBox;
  }

  private static Control? Unsupported(IGH_ActiveObject param)
  {
    var label = new Label()
    {
      TextColor = Colors.Red
    };

    if (param is null)
    {
      label.Text = "Null Element";
    }
    else
    {
      label.Text = $"{param.NickName} is unsupported";
    }

    return label;
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

        if (Model.Automatic)
          Model?.Run();
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

        if (Model.Automatic)
          Model?.Run();
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

  private Control? GetNumberHandlerFromInteger(Param_Integer integer)
  {
    var box = new NumericUpDownWithUnitParsing()
    {
      Value = integer.PersistentData.Select(d => d.Value).FirstOrDefault()
    };

    box.ValueChanged += (s, e) => {
      integer.ClearData();
      integer.SetPersistentData(box.Value);

      if (Model.Automatic)
        Model?.Run();
    };

    return WithLabel(integer.NickName, box);
  }

  private static Control? ConvertComponent(IGH_Component component)
    => component switch
    {
      // GH_Group group => CreateGroup(group),
      GH_Scribble scribble => CreateTitle(scribble),

      _ => Unsupported(component)
    };

  private static Control? WithLabel(string title, Control? control)
  {
    if (string.IsNullOrEmpty(title)) return control;
    if (control is null) return new Label { Text = "Err" };

    var label = new Label()
    {
      Text = title,
      Wrap = WrapMode.None,
    };

    control.Width = 120;
    
    DynamicLayout layout = new()
    {
      Spacing = new Size(12, 0),
    };
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
      VerticalAlignment = VerticalAlignment.Center,
      TextAlignment = TextAlignment.Left
    };
    return title;
  }

  private Control? CreateSlider(GH_NumberSlider slider)
  {
    var upDown = new NumericUpDownWithUnitParsing(true)
    {
      Width = 60,
      Value = (double)slider.CurrentValue,
      MinValue = (double)slider.Slider.Minimum,
      MaxValue = (double)slider.Slider.Maximum,

      // TODO : Verify this works
      // upDown.Increment = (double)slider.Slider.TickFrequency;
    };
    
    upDown.ValueChanged += (s, e) => {
      
      decimal newValue = (decimal)upDown.Value;
      if (newValue > slider.Slider.Maximum)
      {
        newValue = slider.Slider.Maximum;
      }
      else if (newValue < slider.Slider.Minimum)
      {
        newValue = slider.Slider.Minimum;
      }

      slider.SetSliderValue(newValue);

      if (Model.Automatic)
        Model?.Run();
    };

    return WithLabel(slider.NickName, upDown);
  }

  private Control? CreateTextInput(Param_String text_param)
  {
    var text_input = new Eto.Forms.TextBox
    {
      PlaceholderText = text_param.NickName,
      Text = "",
      ReadOnly = false
    };

    text_input.TextChanged += (s, e) => {
      text_param.ClearData();
      text_param.SetPersistentData(text_input.Text);

      if (Model.Automatic)
        Model?.Run();
    };

    return text_input;
  }

  private Control? CreateIntegerInput(Param_Integer integer)
  {
    var box = new NumericStepper()
    {
      Value = integer.VolatileData.AllData(true).Where(d => d.IsValid).OfType<GH_Integer>().Select(d => d.Value).FirstOrDefault(),
    };

    box.ValueChanged += (s, e) => {
      integer.ClearData();
      integer.SetPersistentData(box.Value);

      if (Model.Automatic)
        Model?.Run();
    };

    return box;
  }

  private Control? CreateGroup(RowGroup group)
  {
    try
    {
      var layout = new DynamicLayout();
      layout.BeginVertical(new Padding(2), new Size(8, 4), true, true);
      
      foreach(var groupChild in group.Children)
      {
        var etoChild = ConvertRowGroup(groupChild);
        layout.Add(etoChild, true, false);
      }

      layout.EndVertical();

      var etoGroup = new Eto.Forms.Expander()
      {
        Expanded = true,
        Header = group.Name,
        Content = layout,
      };

      return etoGroup;
    }
    catch
    {
      return new DynamicLayout();
    }
  }

  #endregion

  protected override void OnClosing(CancelEventArgs e)
  {
    try
    {
      Model?.Doc?.DestroyPreviewCaches();
      Model?.Doc?.DestroyPreviewMeshes();
      Model?.Doc?.Dispose();
      Rhino.RhinoDoc.ActiveDoc?.Views?.Redraw();
    } catch {}
    base.OnClosing(e);
  }

}