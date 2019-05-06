using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXL.SoftwareRender
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class SoftwareRenderExternalCommand : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            var processer = new ViewProcess(revit.Application);
            processer.Run();
            return Autodesk.Revit.UI.Result.Succeeded;
        }
    }
    public class SoftwareRenderExternalEventHandler : IExternalEventHandler
    {
        public static ExternalEvent SoftwareRenderExternalEvent = ExternalEvent.Create(new SoftwareRenderExternalEventHandler());

        public void Execute(UIApplication app)
        {
            var processer = new ViewProcess(app);
            processer.Run();
        }

        public string GetName()
        {
            return "SoftwareRenderExternalEventHandler";
        }
    }
}
