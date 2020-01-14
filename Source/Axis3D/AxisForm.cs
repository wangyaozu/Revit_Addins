using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TXL.Axis3D
{
    public partial class AxisForm : System.Windows.Forms.Form
    {
        private UIApplication Uiapp { get; set; }
        private Autodesk.Revit.DB.View3D m_View3D { get; set; }
        private List<Autodesk.Revit.DB.Grid> ValidGrid { get; set; }
        private SolidBrush m_SolidBrush { get; set; }
        private Pen m_LinePen { get; set; }
        private List<LocalGrid> ShowedGrids { get; set; }
        private Object GridLock { get; set; }
        private Object EnterLock { get; set; }
        private Tuple<Autodesk.Revit.DB.ElementId, Autodesk.Revit.DB.XYZ, Autodesk.Revit.DB.XYZ> EnterKeyData { get; set; }
        public AxisForm(UIApplication uiapp)
        {
            InitializeComponent();

            this.TopMost = true;
            this.TransparencyKey = this.BackColor;
            this.ShowInTaskbar = false;

            uint num = UtilTool.GetWindowLong(base.Handle, -20);//GWL_EXSTYLE
            num |= 32u;//WS_EX_TRANSPARENT
            num |= 524288u;//WS_EX_LAYERED
            UtilTool.SetWindowLong(base.Handle, -20, (IntPtr)((long)((ulong)num)));

            m_SolidBrush = new SolidBrush(Color.Black);
            m_LinePen = new Pen(Color.Black, 1.5f);
            GridLock = new Object();
            EnterLock = new Object();
            EnterKeyData = null;

            Uiapp = uiapp;
            ShowedGrids = new List<LocalGrid>();
            ValidGrid = new List<Autodesk.Revit.DB.Grid>();

            this.Paint += AxisForm_Paint;
        }

        public void UpdateForm()
        {
            var view3D = Uiapp.ActiveUIDocument.ActiveView as Autodesk.Revit.DB.View3D;
            var uiView = UtilTool.GetActiveUIView(Uiapp);
            if ((m_View3D == null || m_View3D.Id.IntegerValue !=view3D.Id.IntegerValue) && view3D != null)
            {
                m_View3D = view3D;
                if (view3D == null)
                {
                    ValidGrid.Clear();
                }
                else
                {
                    ValidGrid = UtilTool.GetAllGrids(Uiapp);
                }
            }
            if (uiView != null && view3D != null && ValidGrid.Count > 0)
            {
                var corners = uiView.GetZoomCorners().ToList();
                var isEnter = true;
                lock (EnterLock)
                {
                    if (EnterKeyData != null
                        && EnterKeyData.Item1 == view3D.Id
                        && EnterKeyData.Item2.DistanceTo(corners[0]) < UtilTool.mmToRevit(1)
                        && EnterKeyData.Item3.DistanceTo(corners[1]) < UtilTool.mmToRevit(1))
                    {
                        isEnter = false;
                    }
                }
                if (isEnter)
                {
                    lock (EnterLock)
                    {
                        EnterKeyData = new Tuple<Autodesk.Revit.DB.ElementId
                            , Autodesk.Revit.DB.XYZ
                            , Autodesk.Revit.DB.XYZ>(view3D.Id, corners[0], corners[1]);
                    }
                    var viewRect = uiView.GetWindowRectangle();
                    UpdateWindowsPositionAndSize(viewRect);
                    lock (GridLock)
                    {
                        ShowedGrids = UtilTool.GetLocalGrids(ValidGrid, view3D, corners, viewRect);
                    }
                    this.Refresh();
                }
                else
                {

                }
            }
            else
            {
                lock (GridLock)
                {
                    ShowedGrids.Clear();
                }
                this.Refresh();
            }
        }
        private void UpdateWindowsPositionAndSize(Autodesk.Revit.UI.Rectangle viewRect)
        {
            var newLocation = new Point(viewRect.Left, viewRect.Top);
            this.Location = newLocation;
            this.Width = viewRect.Right - viewRect.Left;
            this.Height = viewRect.Bottom - viewRect.Top;
        }

        private void AxisForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            var pointPairStrLList = new List<Tuple<List<PointF>, string>>();
            lock (GridLock)
            {
                pointPairStrLList.AddRange(ShowedGrids.Select(a => new Tuple<List<PointF>, string>(a.PointFs, a.Name)));
            }
            if (pointPairStrLList.Count > 0)
            {
                foreach (var pointPairStrList in pointPairStrLList)
                {
                    if (pointPairStrList.Item1.Count >= 2)
                    {
                        e.Graphics.DrawString(pointPairStrList.Item2, new Font("Verdana", 20), m_SolidBrush, pointPairStrList.Item1.First());
                        e.Graphics.DrawLines(m_LinePen, pointPairStrList.Item1.ToArray());
                        e.Graphics.DrawString(pointPairStrList.Item2, new Font("Verdana", 20), m_SolidBrush, pointPairStrList.Item1.Last());
                    }
                }
            }
        }
    }

    public class LocalGrid
    {
        public string Name { get; set; }
        public List<PointF> PointFs { get; set; }
        public LocalGrid(string name, List<PointF> points)
        {
            Name = name;
            PointFs = points;
        }
    }
}
