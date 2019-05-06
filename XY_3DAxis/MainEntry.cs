using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XY_3DAxis
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class MainEntry : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public Result OnStartup(UIControlledApplication application)
        {
            var curDllLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string productMenuName = "TXL";
            application.CreateRibbonTab(productMenuName);
            var mainRibbonPanel = application.CreateRibbonPanel(productMenuName, "工具");
            var itemData = new PushButtonData("SWZW", "三维轴网", curDllLocation, "XY_3DAxis.XY_3DAxisExternalCommand");
            var pushButton = mainRibbonPanel.AddItem(itemData) as PushButton;
            return Result.Succeeded;
        }
    }
}
