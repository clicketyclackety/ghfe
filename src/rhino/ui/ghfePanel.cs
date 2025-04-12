using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace rn.ui
{
  [Guid("ADF7FDF3-7E53-4F8D-92E8-552B58915D29")]
  public class ghfePanel : Eto.Forms.FloatingForm
  {
    public ghfePanel()
    {
      Content = new TableLayout
      {
        Rows =
        {
          new Button
          {
            MinimumSize = new Eto.Drawing.Size(200,100),
            Text = "click me!"

          }
        }
      };
    }
  }
}
