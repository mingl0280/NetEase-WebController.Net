using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;
using NetEaseController;

namespace NetEaseController
{

    /// <summary>
    /// Keyboard and mouse hooks, and send inputs.
    /// </summary>
    public class KeyboardAndMouseHooksAndMessages
    {
        #region "Send Inputs"

        /// <summary>
        /// a single INPUT structure. See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646270.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Input
        {
            public Int32 type;
            public InputUnion data;
        }

        /// <summary>
        /// Use this struct to prevent incompatible between x86 and x64 system.
        /// In the INPUT structure, there is a union:
        /// <code>
        /// union {
        ///     MOUSEINPUT mi;
        ///     KEYBDINPUT ki;
        ///     HARDWAREINPUT hi;
        /// };
        /// </code>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public HardwareInput Hardware;
            [FieldOffset(0)]
            public KeyboardInput Keyboard;
            [FieldOffset(0)]
            public MouseInput Mouse;
        }

        /// <summary>
        /// Hardware input structure. See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646269.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public uint Msg;
            public ushort ParamL;
            public ushort ParamH;
        }

        /// <summary>
        /// Keyboard input structure. See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646271.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort Vk;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        /// <summary>
        /// Mouse input structure. See:https://msdn.microsoft.com/en-us/library/windows/desktop/ms646273.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }


        /// <summary>
        /// enum version of MOUSEEVENTF_* in C
        /// </summary>
        private enum MouseEventF
        {
            ABSOLUTE = 0x8000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            MOVE = 0x0001,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100,
            HWHEEL = 0x01000
        }

        // XButtons are defined in MOUSEINPUT structure's document.
        private const int XBUTTON1 = 0x0001;
        private const int XBUTTON2 = 0x0002;
        // INPUT consts are defined in SendInput functions' document.
        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;

        /// <summary>
        /// Mouse Buttons
        /// </summary>
        [Flags]
        public enum MouseButtons
        {
            Left = 1,
            Right = 2,
            Medium = 4,
            XB1 = 8,
            XB2 = 16
        }

        /// <summary>
        /// Process Mouse Move
        /// </summary>
        /// <param name="x">Offset X</param>
        /// <param name="y">Offset Y</param>
        public static void MouseMove(int x, int y, bool absolute = false)
        {
            Input i = new Input { type = INPUT_MOUSE };
            i.data.Mouse = new MouseInput { dx = x, dy = y };
            if (absolute)
                i.data.Mouse.dwFlags = i.data.Mouse.dwFlags | (int)MouseEventF.ABSOLUTE;
            Input[] inputs = new Input[] { i };
            NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        /// <summary>
        /// Process Mouse Click
        /// </summary>
        /// <param name="btns">Mouse Buttons, instance of <c>MouseButtons</param>
        public static void MouseClick(MouseButtons btns)
        {
            List<Input> inpList = new List<Input>();
            if ((btns & MouseButtons.Left) == MouseButtons.Left)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.LEFTDOWN | (int)MouseEventF.LEFTUP };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.Right) == MouseButtons.Right)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.RIGHTDOWN | (int)MouseEventF.RIGHTUP };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.Medium) == MouseButtons.Medium)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.MIDDLEDOWN | (int)MouseEventF.MIDDLEUP };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.XB1) == MouseButtons.XB1)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.XDOWN | (int)MouseEventF.XUP, mouseData = XBUTTON1 };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.XB2) == MouseButtons.XB2)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.XDOWN | (int)MouseEventF.XUP, mouseData = XBUTTON2 };
                inpList.Add(i);
            }
            Input[] iarr = inpList.ToArray();
            uint ret = NativeMethods.SendInput((uint)iarr.Length, iarr, Marshal.SizeOf(new Input()));
            if (ret == 0)
            {
                uint Lerr = (uint)Marshal.GetLastWin32Error();
                Debug.WriteLine(Lerr);
                StringBuilder sbuilder = new StringBuilder(2048);
                NativeMethods.FormatMessage(0x1000, IntPtr.Zero, Lerr, 0x0C00, sbuilder, 2048, IntPtr.Zero);
                Debug.WriteLine(sbuilder.ToString());
            }
        }

        /// <summary>
        /// Process Mouse Down
        /// </summary>
        /// <param name="btns">Mouse Buttons, instance of <c>MouseButtons</c></param>
        public static void MouseDown(MouseButtons btns)
        {
            List<Input> inpList = new List<Input>();
            if ((btns & MouseButtons.Left) == MouseButtons.Left)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.LEFTDOWN };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.Right) == MouseButtons.Right)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.RIGHTDOWN };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.Medium) == MouseButtons.Medium)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.MIDDLEDOWN };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.XB1) == MouseButtons.XB1)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.XDOWN, mouseData = XBUTTON1 };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.XB2) == MouseButtons.XB2)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.XDOWN, mouseData = XBUTTON2 };
                inpList.Add(i);
            }
            Input[] iarr = inpList.ToArray();
            uint ret = NativeMethods.SendInput((uint)iarr.Length, iarr, Marshal.SizeOf(new Input()));
            if (ret == 0)
            {
                uint Lerr = (uint)Marshal.GetLastWin32Error();
                Debug.WriteLine(Lerr);
                StringBuilder sbuilder = new StringBuilder(2048);
                NativeMethods.FormatMessage(0x1000, IntPtr.Zero, Lerr, 0x0C00, sbuilder, 2048, IntPtr.Zero);
                Debug.WriteLine(sbuilder.ToString());
            }
        }

        /// <summary>
        /// Process Mouse Up
        /// </summary>
        /// <param name="btns">Mouse Buttons, instance of <c>MouseButtons</c></param>
        public static void MouseUp(MouseButtons btns)
        {
            List<Input> inpList = new List<Input>();
            if ((btns & MouseButtons.Left) == MouseButtons.Left)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.LEFTUP };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.Right) == MouseButtons.Right)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.RIGHTUP };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.Medium) == MouseButtons.Medium)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.MIDDLEUP };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.XB1) == MouseButtons.XB1)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.XUP, mouseData = XBUTTON1 };
                inpList.Add(i);
            }
            if ((btns & MouseButtons.XB2) == MouseButtons.XB2)
            {
                Input i = new Input { type = INPUT_MOUSE };
                i.data.Mouse = new MouseInput { dwFlags = (int)MouseEventF.XUP, mouseData = XBUTTON2 };
                inpList.Add(i);
            }
            Input[] iarr = inpList.ToArray();
            uint ret = NativeMethods.SendInput((uint)iarr.Length, iarr, Marshal.SizeOf(new Input()));
            if (ret == 0)
            {
                uint Lerr = (uint)Marshal.GetLastWin32Error();
                Debug.WriteLine(Lerr);
                StringBuilder sbuilder = new StringBuilder(2048);
                NativeMethods.FormatMessage(0x1000, IntPtr.Zero, Lerr, 0x0C00, sbuilder, 2048, IntPtr.Zero);
                Debug.WriteLine(sbuilder.ToString());
            }
        }

        public static void SimulateKeyDown(VirtualKeyCode vk)
        {
            try
            {
                Input i = new Input { type = INPUT_KEYBOARD };
            i.data.Keyboard = new KeyboardInput { Vk = (ushort)vk, Scan = 0, Flags = 0 };
            Input[] inputs = new Input[] { i };
            
                NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(new Input()));
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public static void SimulateKeyUp(VirtualKeyCode vk)
        {
            try
            {
                Input i = new Input { type = INPUT_KEYBOARD };
            i.data.Keyboard = new KeyboardInput { Vk = (ushort)vk, Scan = 0, Flags = (uint)KeyboardFlag.KeyUp };
            Input[] inputs = new Input[] { i };
            
                NativeMethods.SendInput((uint) inputs.Length, inputs, Marshal.SizeOf(new Input()));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void SimulateKeyPress(VirtualKeyCode vk)
        {
            SimulateKeyDown(vk);
            SimulateKeyUp(vk);
        }
        #endregion
    }

}