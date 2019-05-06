using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis3D
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class XY_3DAxisExternalCommand : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            AxisManager.Instance().Run(revit.Application);
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
    public class XY_3DAxisExternalEventHandler : IExternalEventHandler
    {
        public static ExternalEvent XY_3DAxisExternalEvent = ExternalEvent.Create(new XY_3DAxisExternalEventHandler());

        public void Execute(UIApplication app)
        {
            AxisManager.Instance().Run(app);
        }

        public string GetName()
        {
            return "XY_3DAxisExternalEventHandler";
        }
    }
}
