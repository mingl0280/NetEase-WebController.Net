using System;

namespace NetEaseController
{
    [Flags]
    public enum GWL
    {
        GWL_EXSTYLE = -20,
        GWL_STYLE = -16,
        GWL_WNDPROC = -4,
        GWLP_WNDPROC = -4,
        GWL_HINSTANCE = -6,
        GWLP_HINSTANCE = -6,
        GWL_HWNDPARENT = -8,
        GWLP_HWNDPARENT = -8,
        GWL_ID = -12,
        GWLP_ID = -12,
        GWL_USERDATA = -21,
        GWLP_USERDATA = -21
    }

    [Flags]
    public enum WS:uint
    {
        WS_EX_ACCEPTFILES = 16,
        WS_EX_APPWINDOW = 0x40000,
        WS_EX_CLIENTEDGE = 512,
        WS_EX_COMPOSITED = 0x2000000,
        WS_EX_CONTEXTHELP = 0x400,
        WS_EX_CONTROLPARENT = 0x10000,
        WS_EX_DLGMODALFRAME = 1,
        WS_EX_LAYERED = 0x80000,
        WS_EX_LAYOUTRTL = 0x400000,
        WS_EX_LEFT = 0,
        WS_EX_LEFTSCROLLBAR = 0x4000,
        WS_EX_LTRREADING = 0,
        WS_EX_MDICHILD = 64,
        WS_EX_NOACTIVATE = 0x8000000,
        WS_EX_NOINHERITLAYOUT = 0x100000,
        WS_EX_NOPARENTNOTIFY = 4,
        WS_EX_OVERLAPPEDWINDOW = 0x300,
        WS_EX_PALETTEWINDOW = 0x188,
        WS_EX_RIGHT = 0x1000,
        WS_EX_RIGHTSCROLLBAR = 0,
        WS_EX_RTLREADING = 0x2000,
        WS_EX_STATICEDGE = 0x20000,
        WS_EX_TOOLWINDOW = 128,
        WS_EX_TOPMOST = 8,
        WS_EX_TRANSPARENT = 32,
        WS_EX_WINDOWEDGE = 256,
        WS_BORDER = 0x800000,
        WS_CAPTION = 0xc00000,
        WS_CHILD = 0x40000000,
        WS_CHILDWINDOW = 0x40000000,
        WS_CLIPCHILDREN = 0x2000000,
        WS_CLIPSIBLINGS = 0x4000000,
        WS_DISABLED = 0x8000000,
        WS_DLGFRAME = 0x400000,
        WS_GROUP = 0x20000,
        WS_HSCROLL = 0x100000,
        WS_ICONIC = 0x20000000,
        WS_MAXIMIZE = 0x1000000,
        WS_MAXIMIZEBOX = 0x10000,
        WS_MINIMIZE = 0x20000000,
        WS_MINIMIZEBOX = 0x20000,
        WS_OVERLAPPED = 0,
        WS_OVERLAPPEDWINDOW = 0xcf0000,
        WS_POPUP = 0x80000000,
        WS_POPUPWINDOW = 0x80880000,
        WS_SIZEBOX = 0x40000,
        WS_SYSMENU = 0x80000,
        WS_TABSTOP = 0x10000,
        WS_THICKFRAME = 0x40000,
        WS_TILED = 0,
        WS_TILEDWINDOW = 0xcf0000,
        WS_VISIBLE = 0x10000000,
        WS_VSCROLL = 0x200000
    }
}
