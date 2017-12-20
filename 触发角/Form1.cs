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

            doShow(p.X, p.Y);
        }

        void doShow(int x, int y)
        {

            if (y < 20 && x < 20)
            {
                if (canShowLeftTop)
                {
                    canShowLeftTop = false;
                    showWinTab();
                }
            }
            else if (y > Height - 10 && x > Width - 10)
            {
                if (canShowRightBottom)
                {
                    canShowRightBottom = false;
                    showWinD();
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

        public void notifyIcon1_showfrom(object sender, System.EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;

        }

        public void ExitSelect(object sender, System.EventArgs e)
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
    }
}
