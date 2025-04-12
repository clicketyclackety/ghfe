using GH_IO.Serialization;
using Grasshopper.Kernel;

namespace lib;

public static class GrasshopperLoader
{

  public static bool LoadGrasshopperFromFile(string path)
  {
    var io = new GH_DocumentIO();
    if (!io.Open(path)) return false;

    GH_Document doc = io.Document;
    Gui gui = Gui.Load(doc);

    return true;
  }

}
