using System;

namespace NetEaseController
{
    [Flags]
    public enum SWPFlags
    {
        SWP_ASYNCWINDOWPOS = 0x4000,
        SWP_DEFERERASE = 0x2000,
        SWP_DRAWFRAME = 0x20,
        SWP_FRAMECHANGED = 0x20,
        SWP_HIDEWINDOW = 0x80,
        SWP_NOACTIVATE = 0x10,
        SWP_NOCOPYBITS = 0x100,
        SWP_NOMOVE = 0x2,
        SWP_NOOWNERZORDER = 0x200,
        SWP_NOREDRAW = 0x8,
        SWP_NOREPOSITION = 0x200,
        SWP_NOSENDCHANGING = 0x400,
        SWP_NOSIZE = 0x1,
        SWP_NOZORDER = 0x4,
        SWP_SHOWWINDOW = 0x0040
    }
}
