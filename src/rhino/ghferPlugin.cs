using System;
using Eto.Forms;
using Rhino;
using Rhino.PlugIns;
using rn.ui;
using rn.viewmodels;

namespace rn;

///<summary>
/// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
/// class. DO NOT create instances of this class yourself. It is the
/// responsibility of Rhino to create an instance of this class.</para>
/// <para>To complete plug-in information, please also see all PlugInDescription
/// attributes in AssemblyInfo.cs (you might need to click "Project" ->
/// "Show All Files" to see it in the "Solution Explorer" window).</para>
///</summary>
public class ghferPlugin : Rhino.PlugIns.PlugIn
{
  public ghferPlugin()
  {
    Instance = this;
  }

  public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;

  protected override LoadReturnCode OnLoad(ref string errorMessage)
  {
    var icon = Rhino.UI.DrawingUtilities.IconFromResource("rn.Properties.Resources.ghfe-icon", typeof(ghferCommand).Assembly);
    Rhino.UI.Panels.RegisterPanel(Instance, typeof(ghfePanel), "GHFE Files", icon);

    if (Settings.TryGetString(SELECTED_FOLDER_NAME, out string value))
    {
      var dir = new System.IO.DirectoryInfo(value);
      if (dir.Exists)
        PanelViewModel.Instance.LastSelectedDirectory = dir;
    }
    return base.OnLoad(ref errorMessage);
  }

  ///<summary>Gets the only instance of the ghferPlugin plug-in.</summary>
  public static ghferPlugin Instance { get; private set; }

  // You can override methods here to change the plug-in behavior on
  // loading and shut down, add options pages to the Rhino _Option command
  // and maintain plug-in wide options in a document.

  const string SELECTED_FOLDER_NAME = nameof(SELECTED_FOLDER_NAME);
  protected override void OnShutdown()
  {
    if (null != PanelViewModel.Instance.LastSelectedDirectory)
      Settings.SetString(SELECTED_FOLDER_NAME, PanelViewModel.Instance.LastSelectedDirectory.FullName);
    base.OnShutdown();
  }
}