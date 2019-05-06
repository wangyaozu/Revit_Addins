using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainEntry.Axis3D
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Axis3DExternalCommand : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            AxisManager.Instance().Run(revit.Application);
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
    public class Axis3DExternalEventHandler : IExternalEventHandler
    {
        public static ExternalEvent Axis3DExternalEvent = ExternalEvent.Create(new Axis3DExternalEventHandler());

        public void Execute(UIApplication app)
        {
            AxisManager.Instance().Run(app);
        }

        public string GetName()
        {
            return "Axis3DExternalEventHandler";
        }
    }
}
