using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LicensePlate
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();
        [DllImport("User32.dll ", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll ", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll ", EntryPoint = "RemoveMenu")]
        extern static int RemoveMenu(IntPtr hMenu, int nPos, int flags);

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //显示控制台窗口，观测console输出
            AllocConsole();

            #region 禁用控制台关闭按钮
            ////根据控制台标题找控制台
            //int WINDOW_HANDLER = FindWindow(null, Console.Title);
            ////找关闭按钮 禁用控制台窗口的关闭按钮（同时也会关闭程序）           
            //IntPtr CLOSE_MENU = GetSystemMenu((IntPtr)WINDOW_HANDLER, IntPtr.Zero);
            //int SC_CLOSE = 0xF060;
            ////关闭按钮禁用      
            //RemoveMenu(CLOSE_MENU, SC_CLOSE, 0x0);

            //Console.WriteLine("程序已启动");
            #endregion
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

/*
 问题：
 1.先称重还是先识别车牌
 2.一台车停留时长最大值
 3.地磅数据会不会一直输出，导致显示闪烁
     */