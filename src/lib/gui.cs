using Eto.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Components;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Rhino.UI.Controls;

namespace lib;

public class Gui
{

  public static Gui Load()
  {
    throw new NotImplementedException("woops");
  }

#region Eto Creation

  public static Control? CreateRow(List<IGH_DocumentObject> rowItems)
  {
    DynamicLayout layout = new();
    layout.BeginHorizontal();
    foreach(var rowItem in rowItems)
    {
      var rowControl = ConvertGrasshopperObject(rowItem);
      layout.Add(rowControl, true, false);
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
      GH_Group group => CreateGroup(group),
      GH_NumberSlider slider => CreateSlider(slider),

      _ => null
    };

  private static Control? CreateSlider(GH_NumberSlider slider)
  {
    var upDown = new NumericUpDownWithUnitParsing(true);
    upDown.Value = (double)slider.CurrentValue;
    // TODO : Add Min/Max

    return upDown;
  }

  private static Control? CreateGroup(GH_Group group)
  {
    var etoGroup = new Eto.Forms.Expander();
    var layout = new DynamicLayout();
    layout.BeginVertical();
    
    foreach(var groupChild in group.Objects())
    {
      var etoChild = ConvertGrasshopperObject(groupChild);
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