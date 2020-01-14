using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXL.SoftwareRender
{
    public class ViewProcess
    {
        UIApplication m_Uiapp { get; set; }
        public ViewProcess(UIApplication uiapp)
        {
            m_Uiapp = uiapp;
        }
        public void Run()
        {
            var select = m_Uiapp.ActiveUIDocument.Selection;
            var pickedBox = select.PickBox(PickBoxStyle.Crossing, "PickBoxStyle");
            var elements = select.PickElementsByRectangle();
            System.Windows.Forms.MessageBox.Show("ViewProcess: " + elements.Count);
        }
    }
}
