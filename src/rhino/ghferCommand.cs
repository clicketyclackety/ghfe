using Rhino;
using Rhino.Commands;
using Rhino.UI;
using rn.ui;

namespace rn;

public class ghferCommand : Command
{
  public ghferCommand()
  {
    // Rhino only creates one instance of each command class defined in a
    // plug-in, so it is safe to store a refence in a static property.
    Instance = this;
  }

  ///<summary>The only instance of this command.</summary>
  public static ghferCommand Instance { get; private set; }

  ///<returns>The command name as it appears on the Rhino command line.</returns>
  public override string EnglishName => "ghferCommand";

  protected override Result RunCommand(RhinoDoc doc, RunMode mode)
  {
    //var panel = new ghfePanel();
    //panel.Show();// (RhinoEtoApp.MainWindow);
    //RhinoApp.WriteLine("The {0} command is under construction.", EnglishName);
    return Result.Success;
  }
}
