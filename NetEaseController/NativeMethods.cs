using System;
using System.Text;
using System.Runtime.InteropServices;
using static NetEaseController.KeyboardAndMouseHooksAndMessages;

namespace NetEaseController
{
    class NativeMethods
    {

        public delegate IntPtr HookProc(int nCode, Int32 wParam, IntPtr lParam);

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        /*

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

       private static Point CurrentMousePoint;
       */
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

    }

}