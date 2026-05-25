using System.Configuration;
using System.Data;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;

namespace LUnivers_Beaute
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        private const int SW_RESTORE = 9;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "LUnivers_Beaute_App_Single_Instance_Mutex";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // App is already running, find the existing process
                Process currentProcess = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(currentProcess.ProcessName))
                {
                    if (process.Id != currentProcess.Id)
                    {
                        // Bring the existing instance to the front (even if minimized)
                        IntPtr hWnd = process.MainWindowHandle;
                        if (hWnd != IntPtr.Zero)
                        {
                            ShowWindow(hWnd, SW_RESTORE);
                            SetForegroundWindow(hWnd);
                        }
                        break;
                    }
                }
                
                // Cấm mở thêm, tự động tắt bản sao mới này
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}
