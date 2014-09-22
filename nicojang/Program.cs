using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nicojang
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //if (!ShowPrevProcess())
            //{
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            //}
        }

        [DllImport("USER32.DLL", CharSet = CharSet.Auto)]
        private static extern int ShowWindow(System.IntPtr hWnd, int nCmdShow);

        [DllImport("USER32.DLL", CharSet = CharSet.Auto)]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);

        private const int SW_NORMAL = 1;

        public static bool ShowPrevProcess()
        {
            Process hThisProcess = Process.GetCurrentProcess();
            Process[] hProcesses = Process.GetProcessesByName(hThisProcess.ProcessName);
            int iThisProcessId = hThisProcess.Id;

            foreach (Process hProcess in hProcesses)
            {
                if (hProcess.Id != iThisProcessId)
                {
                    ShowWindow(hProcess.MainWindowHandle, SW_NORMAL);
                    SetForegroundWindow(hProcess.MainWindowHandle);
                    return true;
                }
            }

            return false;
        }
    }
}
