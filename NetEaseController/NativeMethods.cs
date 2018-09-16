using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static NetEaseController.KeyboardAndMouseHooksAndMessages;
using static NetEaseController.WS;
using static NetEaseController.SWPFlags;
using static NetEaseController.WindowPosTypes;
using static NetEaseController.GWL;
using System.Drawing;

namespace NetEaseController
{
    public class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        public delegate IntPtr HookProc(int nCode, Int32 wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public class Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public Point pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
        

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms644990.aspx
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hInstance"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms644993.aspx
        /// </summary>
        /// <param name="idHook"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms644974.aspx
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms683183.aspx
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        /// <summary>
        /// Avoid hook failure. see https://msdn.microsoft.com/en-us/library/windows/desktop/ms683199.aspx
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true, BestFitMapping = true)]
        public static extern IntPtr GetModuleHandle(string name);

        /// <summary>
        /// See MS Documentation: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646316.aspx
        /// </summary>
        /// <param name="uVirtKey"></param>
        /// <param name="uScanCode"></param>
        /// <param name="lpbKeyState"></param>
        /// <param name="lpwTransKey"></param>
        /// <param name="fuState"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        /// <summary>
        /// Get Keyboard State. See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646299.aspx
        /// </summary>
        /// <param name="pbKeyState"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        /// <summary>
        /// See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646301.aspx
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);


        /// <summary>
        /// Send Input. See:https://msdn.microsoft.com/en-us/library/windows/desktop/ms646310.aspx
        /// </summary>
        /// <param name="nInputs"></param>
        /// <param name="pInputs"></param>
        /// <param name="cbSize"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        /// <summary>
        /// Format GetLastError data. See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms679351.aspx
        /// </summary>
        /// <param name="dwFlags"></param>
        /// <param name="lpSource"></param>
        /// <param name="dwMessageId"></param>
        /// <param name="dwLanguageId"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nSize"></param>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, BestFitMapping = true, ThrowOnUnmappableChar = true)]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, [Out] StringBuilder lpBuffer, uint nSize, IntPtr Arguments);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        //枚举窗体
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr Hwnd);


        //获取窗体标题
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpText, int nCount);
        
         
        //声明委托函数
        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
        
        
        //根据窗口获取线程句柄
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out uint pid);

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetLastError();


        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumChildWindows (IntPtr HwndParent, WNDENUMPROC lpEnumProc, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ShowWindow(IntPtr Hwnd, int nCmdShow);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr Hwnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool IsIconic(IntPtr Hwnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SwitchToThisWindow(IntPtr Hwnd, bool fUnknown);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetTopWindow(IntPtr Hwnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);


        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetClientRect(IntPtr Hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public static Dictionary<WindowPosTypes, IntPtr> WindowPosHandles = new Dictionary<WindowPosTypes, IntPtr>()
        {
            { HWND_BOTTOM, new IntPtr(1) },
            { HWND_NOTOPMOST, new IntPtr(-2) },
            { HWND_TOP, new IntPtr(0) },
            { HWND_TOPMOST, new IntPtr(-1) }
        };

        public static List<IntPtr> GetAllWindowsByPID(int processID)
        {
            List<IntPtr> Handles = new List<IntPtr>();
            EnumWindows((hwnd, lparam) => {
                int OutPid = 0;
                GetWindowThreadProcessId(hwnd, out OutPid);
                if (OutPid == processID)
                {
                    Handles.Add(hwnd);
                }
                Debug.WriteLine(GetLastError());
                return true;
            }, 0);
            return Handles;
        }

        public static int MakePoints(Points p)
        {
            return (p.y << 16) | (p.x);
        }

        public static void PutForwardAllWechatWindows()
        {
            EnumWindows((hwnd, lparam) =>
            {
                StringBuilder sb = new StringBuilder("", 2048);
                GetWindowText(hwnd, sb, 2000);
                if (sb.ToString().Trim().Contains("微信"))
                {
                    SetWindowPos(hwnd, WindowPosHandles[HWND_TOPMOST], 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE));
                }
                return true;
            }, 0);
        }
        public static void PutForwardAllNeteaseWindows()
        {
            EnumWindows((hwnd, lparam) =>
            {
                StringBuilder sb = new StringBuilder("", 2048);
                GetClassName(hwnd, sb, 2040);
                if (sb.ToString().Trim() == "OrpheusBrowserHost")
                {
                    SetWindowPos(hwnd, WindowPosHandles[HWND_TOPMOST], 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE));
                }
                return true;
            }, 0);
        }

        public static void RemoveAllTopmost(IntPtr hwnd)
        {
            var CurrentLong = GetWindowLong(hwnd, (int) GWL_EXSTYLE);
            var NewLong = CurrentLong & ~(int)WS_EX_TOPMOST;
            SetWindowLong(hwnd, (int)GWL_EXSTYLE, NewLong);
        }

        public static Color GetPixelColor(IntPtr hWnd, WndPoints p)
        {
            return GetPixelColor(hWnd, p.X, p.Y);
        }

        public static Color GetPixelColor(IntPtr hWnd, int X, int Y)
        {
            IntPtr hdc = GetDC(hWnd);
            uint pixel = GetPixel(hdc, X, Y);
            ReleaseDC(hWnd, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                            (int)(pixel & 0x0000FF00) >> 8,
                            (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        public static Bitmap GetNetEaseBitmap(IntPtr MainWindowHwnd)
        {
            Graphics g = Graphics.FromHwnd(MainWindowHwnd);
            RECT cliSize;
            GetClientRect(MainWindowHwnd, out cliSize);
            Bitmap bmp = new Bitmap(cliSize.right, cliSize.bottom, g);
            Graphics gMem = Graphics.FromImage(bmp);
            IntPtr dc = gMem.GetHdc();
            PrintWindow(MainWindowHwnd, dc, 0);
            gMem.ReleaseHdc(dc);
            g.Dispose();
            return bmp;           
        }
    }
}
