using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace webBro
{
    static class Program
    {
        static webBro form;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(MouseEvent dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public enum MouseEvent
        {
            MOUSEEVENTF_LEFTDOWN = 0x00000002,
            MOUSEEVENTF_LEFTUP = 0x00000004,
            MOUSEEVENTF_RIGHTDOWN = 0x00000008,
            MOUSEEVENTF_RIGHTUP = 0x00000016,
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (form.keyCheck.Checked)
                {
                    Keys current = (Keys)vkCode;
                    Point cp = Cursor.Position;
                    if (current == form.leftKey)
                    {
                        mouse_event(MouseEvent.MOUSEEVENTF_LEFTDOWN, cp.X, cp.Y, 0, 0);
                        mouse_event(MouseEvent.MOUSEEVENTF_LEFTUP, cp.X, cp.Y, 0, 0);
                    }
                    if (current == form.rightKey)
                    {
                        mouse_event(MouseEvent.MOUSEEVENTF_RIGHTDOWN, cp.X, cp.Y, 0, 0);
                        mouse_event(MouseEvent.MOUSEEVENTF_RIGHTUP, cp.X, cp.Y, 0, 0);
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [STAThread]
        static void Main()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, 0);
            _hookID = SetHook(_proc);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new webBro();
            Application.Run(form);
            UnhookWindowsHookEx(_hookID);
        }
    }
}
