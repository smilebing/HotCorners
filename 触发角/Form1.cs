using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace 触发角
{
    public partial class Form1 : Form
    {
        private int Width;
        private int Height;
        private bool canShowLeftTop = true;
        private bool canShowRightBottom = true;
        private ContextMenu notifyiconMnu;

        private string[] _menuStrings = new string[]
        {
            "无",
            "显示桌面",
            "mission control"
        };

        //private delegate void LeftTopHandle(int type);

        //private delegate void LeftBottomHandle(int type);

        //private delegate void RightTopHandle(int type);

        //private delegate void RightBottonHandle(int type);

        //private delegate void LocationHandel(int type);

        //private event LocationHandel locationEvent;

        //private event LeftTopHandle leftTopEvent;
        //private event LeftBottomHandle leftBottomEvent;
        //private event RightTopHandle rightTopEvent;
        //private event RightBottonHandle rightBottomEvent;


        private int _leftTop = 0;
        private int _leftBottom = 0;
        private int _rightTop = 0;
        private int _rightBottom = 0;

        enum FunctionType
        {
            None = 0,
            WinD = 1,
            WinTab = 2
        }
        enum MouseLoc
        {
            LeftTop = 0,
            LeftBottom = 1,
            RightTop = 2,
            RightBottom = 3
        }


        public Form1()
        {
            InitializeComponent();
            //定义一个MenuItem数组，并把此数组同时赋值给ContextMenu对象 
            MenuItem[] mnuItms = new MenuItem[3];
            mnuItms[0] = new MenuItem();
            mnuItms[0].Text = "显示窗口";
            mnuItms[0].Click += new System.EventHandler(this.notifyIcon1_showfrom);

            mnuItms[1] = new MenuItem("-");

            mnuItms[2] = new MenuItem();
            mnuItms[2].Text = "退出系统";
            mnuItms[2].Click += new System.EventHandler(this.ExitSelect);
            mnuItms[2].DefaultItem = true;

            notifyiconMnu = new ContextMenu(mnuItms);
            notifyIcon1.ContextMenu = notifyiconMnu;
            //为托盘程序加入设定好的ContextMenu对象 

            //添加菜单
            var a = _menuStrings.Clone();
            var b = _menuStrings.Clone();
            var c = _menuStrings.Clone();
            var d = _menuStrings.Clone();

            comboBox1.DataSource = a;
            comboBox2.DataSource = b;
            comboBox3.DataSource = c;
            comboBox4.DataSource = d;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //  2.当前的屏幕包括任务栏的工作域大小
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;
            richTextBox1.Text += Width;
            richTextBox1.Text += Height;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var p = Cursor.Position;
            label1.Text = p.X.ToString();
            label2.Text = p.Y.ToString();

            //判断坐标
            doShow(p.X, p.Y);
        }

        void doShow(int x, int y)
        {

            if (y < 20 && x < 20)
            {
                if (canShowLeftTop)
                {
                    canShowLeftTop = false;
                    DoAction(_leftTop);
                }
            }
            else if (y > Height - 10 && x > Width - 10)
            {
                if (canShowRightBottom)
                {
                    canShowRightBottom = false;
                    DoAction(_rightBottom);
                }
            }
            else
            {
                canShowRightBottom = true;
                canShowLeftTop = true;
            }
        }


        #region 发送按键

        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        private static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="functionType"></param>
        private void DoAction(int functionType)
        {
            switch (functionType)
            {
                case (int)FunctionType.None:
                    break;
                case (int)FunctionType.WinD:
                    showWinD();
                    break;
                case (int)FunctionType.WinTab:
                    showWinTab();
                    break;
                default:
                    break;
            }
        }

        private void showWinTab()
        {
            //触发操作
            //符号键模拟 Control+Shift+Z 调取搜狗输入法符号页面
            keybd_event(Keys.LWin, 0, 0, 0);
            keybd_event(Keys.Tab, 0, 0, 0);

            keybd_event(Keys.LWin, 0, 0x02, 0);
            keybd_event(Keys.Tab, 0, 0x02, 0);
        }

        private void showWinD()
        {
            //触发操作
            //符号键模拟 Control+Shift+Z 调取搜狗输入法符号页面
            keybd_event(Keys.LWin, 0, 0, 0);
            keybd_event(Keys.D, 0, 0, 0);

            keybd_event(Keys.LWin, 0, 0x02, 0);
            keybd_event(Keys.D, 0, 0x02, 0);
        }

        #endregion

        #region 窗体事件

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }

        public void notifyIcon1_showfrom(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;

        }

        public void ExitSelect(object sender, EventArgs e)
        {
            //隐藏托盘程序中的图标 
            notifyIcon1.Visible = false;
            //关闭系统 
            Close();
            Dispose(true);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                //托盘区图标隐藏
                notifyIcon1.Visible = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(500, "提示", "触发角程序后台运行.", ToolTipIcon.Info);
            Hide();
            e.Cancel = true;
        }
        #endregion

        #region 更改下拉菜单

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _leftTop = comboBox1.SelectedIndex;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            _rightTop = comboBox2.SelectedIndex;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            _leftBottom = comboBox3.SelectedIndex;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            _rightBottom = comboBox4.SelectedIndex;
        }

        #endregion

    }
}
