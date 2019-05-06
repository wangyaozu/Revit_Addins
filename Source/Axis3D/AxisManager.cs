using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using System.IO;

namespace TXL.Axis3D
{
    public class AxisManager
    {
        public bool IsShow { get; set; }
        static private AxisManager m_Instance { get; set; }
        static public AxisManager Instance()
        {
            if (m_Instance == null)
            {
                m_Instance = new AxisManager();
            }
            return m_Instance;
        }
        private AxisManager()
        {
            IsShow = false;
        }
        System.Windows.Forms.Timer RefreshTimer { get; set; }
        AxisForm AxisDialog { get; set; }

        public AxisForm GetAixsForm()
        {
            return AxisDialog;
        }
        public void Run(UIApplication uiapp)
        {
            if (IsShow)
            {
                IsShow = false;
                CloseFloatAxisForm();
            }
            else
            {
                IsShow = true;
                InitFloatAxisForm(uiapp);
            }
        }
        //初始化
        private void InitFloatAxisForm(UIApplication uiapp)
        {
            if (AxisDialog == null)
            {
                AxisDialog = new AxisForm(uiapp);
                UtilTool.ShowFormModeless(AxisDialog);
            }
            //启动定时刷新
            InitRefreshTimer();
        }

        //关闭
        private void CloseFloatAxisForm()
        {
            //释放计时器
            ReleaseTimer();
            if (AxisDialog != null)
            {
                AxisDialog.Close();
                AxisDialog.Dispose();
                AxisDialog = null;
            }
        }

        //初始化计时器
        private void InitRefreshTimer()
        {
            ReleaseTimer();
            RefreshTimer = new System.Windows.Forms.Timer();
            RefreshTimer.Interval = 10;
            RefreshTimer.Tick += RefreshTimer_Tick;
            RefreshTimer.Start();
        }

        //计时器计时完毕，刷新
        void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshAxisForm();
        }


        //释放计时器
        private void ReleaseTimer()
        {
            if (RefreshTimer != null)
            {
                RefreshTimer.Stop();
                RefreshTimer.Enabled = false;
                RefreshTimer.Dispose();
                RefreshTimer = null;
            }
        }

        //刷新
        private void RefreshAxisForm()
        {
            if (AxisDialog != null)
            {
                AxisDialog.UpdateForm();
            }
        }
    }
}
