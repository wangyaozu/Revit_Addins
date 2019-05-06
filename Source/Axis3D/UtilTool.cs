using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis3D
{
    class UtilTool
    {
        public static void ShowFormModeless(System.Windows.Forms.Form form)
        {
            if (form == null)
            {
                return;
            }

            form.Show(new RevitOwnerWindowHandler());
        }
        public static UIView GetActiveUIView(UIApplication uiapp)
        {
            if (uiapp == null || uiapp.ActiveUIDocument == null)
            {
                return null;
            }
            Autodesk.Revit.DB.View activeView = uiapp.ActiveUIDocument.ActiveView;
            if (activeView != null)
            {
                IList<UIView> allUiViews = uiapp.ActiveUIDocument.GetOpenUIViews();
                foreach (var uiview in allUiViews)
                {
                    if (uiview.ViewId == activeView.Id)
                    {
                        return uiview;
                    }
                }
            }
            return null;
        }

        public static List<Grid> GetAllGrids(UIApplication uiapp)
        {
            var retGrids = new List<Grid>();
            var collector = new FilteredElementCollector(uiapp.ActiveUIDocument.Document);
            collector.OfClass(typeof(Grid));
            retGrids.AddRange(collector.Select(a => a as Grid));
            return retGrids;
        }
        static public double mmToRevit(double value)
        {
            return UnitUtils.ConvertToInternalUnits(value, DisplayUnitType.DUT_MILLIMETERS);
        }
        public static List<LocalGrid> GetLocalGrids(List<Grid> revitGrids, View3D view3D, List<XYZ> conners, Rectangle viewRect)
        {
            var retDatas = new List<LocalGrid>();
            var transform = Autodesk.Revit.DB.Transform.Identity;
            transform.BasisX = view3D.RightDirection;
            var ori = view3D.GetOrientation();
            transform.BasisZ = ori.ForwardDirection;
            transform.BasisY = ori.ForwardDirection.CrossProduct(view3D.RightDirection);

            var newLocation = new System.Drawing.PointF(viewRect.Left, viewRect.Top);
            var width = (viewRect.Right - viewRect.Left) * 1.0;
            var height = (viewRect.Bottom - viewRect.Top) * 1.0;

            var yLine = Line.CreateUnbound(conners[0], transform.BasisY);
            var xLine = Line.CreateUnbound(conners[0], transform.BasisX);
            var revitWidth = yLine.Distance(conners[1]);
            var revitHeight = xLine.Distance(conners[1]);

            var revitConner0 = conners[0];
            var revitConner1 = revitConner0 + transform.BasisX * revitWidth;
            var revitConner2 = conners[1];
            var revitConner3 = revitConner0 - transform.BasisY * revitHeight;
            transform.Origin = revitConner3;

            var leftBottom = ProjectToPlane(transform, revitConner0);
            var rightBottom = ProjectToPlane(transform, revitConner1);
            var rightTop = ProjectToPlane(transform, revitConner2);
            var leftTop = ProjectToPlane(transform, revitConner3);

            var xRatio = width / (leftTop.DistanceTo(rightTop));
            var yRatio = height / (leftTop.DistanceTo(leftBottom));

            foreach (var grid in revitGrids)
            {
                var curve = grid.Curve;
                if (curve != null)
                {
                    var localPoints = curve
                        .Tessellate()
                        .Select(a => ProjectToPlane(transform, a))
                        .Select(a => a - leftTop)
                        .Select(a => new System.Drawing.PointF((float)(a.X * xRatio), (float)(a.Y * yRatio)))
                        .ToList();
                    retDatas.Add(new LocalGrid(grid.Name, localPoints));
                }
            }
            return retDatas;
        }
        public static List<Line> TessellateCurveToLine(Curve curve)
        {
            var points = curve.Tessellate().ToList();
            var lines = new List<Line>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                try
                {
                    lines.Add(Line.CreateBound(points[i], points[i + 1]));
                }
                catch (Exception ex)
                {
                }
            }
            return lines.ToList();
        }
        public static XYZ ProjectToPlane(Transform transform, XYZ point)
        {
            var pointTemp = transform.Inverse.OfPoint(point);
            var pointOnPlane = new XYZ(pointTemp.X, pointTemp.Y, 0);
            return pointOnPlane;
        }
    }
    public class RevitOwnerWindowHandler : System.Windows.Forms.IWin32Window
    {
        private IntPtr m_Hwnd = Autodesk.Windows.ComponentManager.ApplicationWindow;

        public IntPtr Handle
        {
            get
            {
                return this.m_Hwnd;
            }
        }
    }
}
