using Autodesk.Revit.UI;
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
            System.Windows.Forms.MessageBox.Show("ViewProcess");
        }
    }
}
