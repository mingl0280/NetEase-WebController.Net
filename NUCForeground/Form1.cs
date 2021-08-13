using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetEaseController;
using static NetEaseController.WM;
using static NetEaseController.MKs;
using static NetEaseController.NativeMethods;
using static NetEaseController.SWPFlags;
using static NetEaseController.WindowPosTypes;
using static NUCForeground.PreDefMouseOps;
using static NUCForeground.NEMIconSearcher;
using static NUCForeground.Properties.Resources;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NUCForeground
{
    public partial class FrmMain : Form
    {

        IntPtr LyricHandle, WechatHandle, NetEaseMainHandle;
        List<IntPtr> AllNetEaseHandles = new List<IntPtr>();
        List<IntPtr> AllWeChatHandles = new List<IntPtr>();
        List<IntPtr> AllHandles = new List<IntPtr>();
        Dictionary<PreDefMouseOps, WndPoints> PreDefinedPoints = new Dictionary<PreDefMouseOps, WndPoints>();
        Dictionary<WndPoints, Color> KeyPointColors = new Dictionary<WndPoints, Color>();
        WndPoints[] KeyPoints = new WndPoints[5];
        bool ShowingOtherWindow = false;
        enum PlayState
        {
            Playing,
            Paused
        }
        enum RepState
        {
            ListNoLoop,
            ListLoop,
            OneLoop,
            Random,
            Unknown
        }
        enum LrcState
        {
            On,
            Off
        }

        PlayState PlayingState = PlayState.Paused;
        RepState RepeatState = RepState.ListNoLoop;
        LrcState LyricState = LrcState.Off;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in AllHandles)
            {
                SetWindowPos(item, WindowPosHandles[HWND_NOTOPMOST], 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE));
                RemoveAllTopmost(item);
            }
        }
        private void btnPrevMusic_Click(object sender, EventArgs e)
        {
            SendNetEaseLeftClick(PreDefinedPoints[MO_Prev]);
            if (PlayingState == PlayState.Paused)
            {
                PlayingState = PlayState.Playing;
                btnPlayPause.BackgroundImage = Pause;
            }
        }

        private IntPtr GetNetEaseTargetWindow()
        {
            IntPtr wwHandle = IntPtr.Zero;
            EnumChildWindows(NetEaseMainHandle, (hwnd, lparam) =>
            {
                var ClassNameStr = new StringBuilder("", 2053);
                GetClassName(hwnd, ClassNameStr, 2050);
                if (ClassNameStr.ToString().Trim().Contains("Chrome_WidgetWin"))
                {
                    wwHandle = hwnd;
                    return false;
                }
                return true;
            }, 0);
            return wwHandle;
        }

        private void SendNetEaseLeftClick(WndPoints p)
        {
            Points pts;
            pts.x = p.X;
            pts.y = p.Y;
            int PointInt = MakePoints(pts);
            IntPtr wwHandle = GetNetEaseTargetWindow();
            SendMessage(wwHandle, (int)WM_LBUTTONDOWN, (int)MK_LBUTTON, MakePoints(pts));
            Thread.Sleep(5);
            SendMessage(wwHandle, (int)WM_LBUTTONUP, (int)MK_LBUTTON, MakePoints(pts));
        }

        private void GetCloudMusicWindows()
        {
            Process[] NEPs = Process.GetProcessesByName("cloudmusic");
            foreach (Process p in NEPs)
            {

                foreach (var window in GetAllWindowsByPID(p.Id))
                {
                    var ClassNameStr = new StringBuilder("", 2053);
                    GetClassName(window, ClassNameStr, 2050);

                    if (ClassNameStr.ToString().Trim() == "DesktopLyrics")
                    {
                        LyricHandle = p.MainWindowHandle;
                        
                    }
                    else
                    {
                        if (ClassNameStr.ToString().Trim() == "OrpheusBrowserHost")
                        {
                            NetEaseMainHandle = window;
                        }
                    }
                    AllNetEaseHandles.Add(window);
                    AllHandles.Add(window);
                }
            }
        }
        private void GetWeChatWindows()
        {
            var NEPs = Process.GetProcessesByName("WeChat");
            foreach (Process p in NEPs)
            {
                foreach (var window in GetAllWindowsByPID(p.Id))
                {
                    var WindowTitleStr = new StringBuilder("", 2053);
                    GetWindowText(window, WindowTitleStr, 2050);

                    if (WindowTitleStr.ToString().Trim() == "微信")
                    {
                        WechatHandle = p.MainWindowHandle;
                    }
                    else
                    {
                        
                    }
                    AllHandles.Add(window);
                    AllWeChatHandles.Add(window);
                }
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            GetCloudMusicWindows();
            GetWeChatWindows();
            tTopMostSetup.Start();
            PreDefinedPoints.Add(MO_Prev, new WndPoints(45, 745));
            PreDefinedPoints.Add(MO_PlayPause, new WndPoints(101,743));
            PreDefinedPoints.Add(MO_Next, new WndPoints(158, 743));
            PreDefinedPoints.Add(MO_Lrc, new WndPoints(1193, 743));
            PreDefinedPoints.Add(MO_Repeat, new WndPoints(1157, 743));
            KeyPoints[0] = new WndPoints(1158, 743);
            KeyPoints[1] = new WndPoints(1149, 743);
            KeyPoints[2] = new WndPoints(1158, 746);
            KeyPoints[3] = new WndPoints(101, 743);
            KeyPoints[4] = new WndPoints(1187, 748);
            for(int i=0;i<5;i++)
            {
                KeyPointColors.Add(KeyPoints[i], Color.FromArgb(0, 0, 0));
            }
            GetNeteasePlayingState();
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            SendNetEaseLeftClick(PreDefinedPoints[MO_PlayPause]);
            if (PlayingState == PlayState.Paused)
            {
                PlayingState = PlayState.Playing;
                btnPlayPause.BackgroundImage = Pause;
            }
            else
            {
                PlayingState = PlayState.Paused;
                btnPlayPause.BackgroundImage = Play;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SendNetEaseLeftClick(PreDefinedPoints[MO_Next]);
            if (PlayingState == PlayState.Paused)
            {
                PlayingState = PlayState.Playing;
                btnPlayPause.BackgroundImage = Pause;
            }
        }

        private void btnVolDown_Click(object sender, EventArgs e)
        {
            Form1.ExecuteCmd("VolDown");
        }

        private void btnVolUp_Click(object sender, EventArgs e)
        {
            Form1.ExecuteCmd("VolUp");
        }

        private void btnNEM_Click(object sender, EventArgs e)
        {
            TopMost = false;
            ShowingOtherWindow = true;
            BringAllWindowToTop(NetEaseMainHandle);
        }

        private void FrmMain_Activated(object sender, EventArgs e)
        {
            if (GetTopWindow(IntPtr.Zero) != LyricHandle)
            {
                SetWindowPos(LyricHandle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE));
            }
            SetWindowPos(Handle, LyricHandle, 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE));
        }

        private void btnLoop_Click(object sender, EventArgs e)
        {
            switch(RepeatState)
            {
                case RepState.ListNoLoop:
                    RepeatState = RepState.ListLoop;
                    SendNetEaseLeftClick(PreDefinedPoints[MO_Repeat]);
                    btnLoop.BackgroundImage = ListLoop;
                    break;
                case RepState.ListLoop:
                    RepeatState = RepState.OneLoop;
                    SendNetEaseLeftClick(PreDefinedPoints[MO_Repeat]);
                    btnLoop.BackgroundImage = OneLoop;
                    break;
                case RepState.OneLoop:
                    RepeatState = RepState.Random;
                    SendNetEaseLeftClick(PreDefinedPoints[MO_Repeat]);
                    btnLoop.BackgroundImage = Properties.Resources.Random;
                    break;
                case RepState.Random:
                    RepeatState = RepState.ListNoLoop;
                    SendNetEaseLeftClick(PreDefinedPoints[MO_Repeat]);
                    btnLoop.BackgroundImage = ListNoLoop;
                    break;
            }
        }

        private void btnLrc_Click(object sender, EventArgs e)
        {
            switch(LyricState)
            {
                case LrcState.Off:
                    LyricState = LrcState.On;
                    btnLrc.BackgroundImage = Lyric;
                    SendNetEaseLeftClick(PreDefinedPoints[MO_Lrc]);
                    break;
                case LrcState.On:
                    LyricState = LrcState.Off;
                    SendNetEaseLeftClick(PreDefinedPoints[MO_Lrc]);
                    btnLrc.BackgroundImage = Lyric_Off;
                    break;
            }
        }

        private void btnHibernate_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
            psi.Arguments = "/k shutdown /h /f";
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            p.StartInfo = psi;
            p.Start();
        }

        private void btnShutdown_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
            psi.Arguments = "/k shutdown /p";
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            p.StartInfo = psi;
            p.Start();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            TopMost = false;
            ShowingOtherWindow = true;
            Form1.ExecuteCmd("Win");
        }

        private void BringAllWindowToTop(IntPtr ParentHwnd)
        {
            SetWindowPos(ParentHwnd, (IntPtr)HWND_TOP, 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE));
            if (IsIconic(ParentHwnd))
            {
                SwitchToThisWindow(ParentHwnd, true);
                EnumChildWindows(ParentHwnd, (hwnd, lparam) =>
                {
                    SwitchToThisWindow(hwnd, true);
                    return true;
                }, 0);
            }
            else
            {
                BringWindowToTop(ParentHwnd);
                EnumChildWindows(ParentHwnd, (hwnd, lparam) =>
                {
                    BringWindowToTop(hwnd);
                    return true;
                }, 0);
            }
        }

        private void btnWechat_Click(object sender, EventArgs e)
        {
            TopMost = false;
            ShowingOtherWindow = true;
            foreach(var wndHandle in AllWeChatHandles)
            {
                BringAllWindowToTop(wndHandle);
            }
        }

        private void tTopMostSetup_Tick(object sender, EventArgs e)
        {
            if (ShowingOtherWindow)
            {
                return;
            }
            if (GetTopWindow(IntPtr.Zero) != LyricHandle)
            {
                SetWindowPos(LyricHandle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE));
            }
            SetWindowPos(Handle, LyricHandle, 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE));
            //GetNeteasePlayingState();
        }

        private RepState DoImageCompare()
        {
            TopMost = false;
            BringAllWindowToTop(NetEaseMainHandle);
            
            Bitmap NetEaseBitmap = GetNetEaseBitmap(NetEaseMainHandle);
            Dictionary<RepState, double> Possibility = new Dictionary<RepState, double>() {
                { RepState.ListLoop, 0},
                { RepState.ListNoLoop, 0 },
                { RepState.OneLoop, 0 },
                { RepState.Random, 0 }
            };

            System.Drawing.Point p;
            p = GetImageContains(NetEaseBitmap, SAMP_ONEREP, 1);
            Possibility[RepState.OneLoop] = GetSimilarity(NetEaseBitmap, SAMP_ONEREP, p);
            p = GetImageContains(NetEaseBitmap, SAMP_NOREP, 1);
            Possibility[RepState.ListNoLoop] = GetSimilarity(NetEaseBitmap, SAMP_NOREP, p);
            p = GetImageContains(NetEaseBitmap, SAMP_ALLREP2, 1);
            Possibility[RepState.ListLoop] = GetSimilarity(NetEaseBitmap, SAMP_ALLREP2, p);
            p = GetImageContains(NetEaseBitmap, SAMP_RND, 1);
            Possibility[RepState.Random] = GetSimilarity(NetEaseBitmap, SAMP_RND, p);
            RepState LowestRepState = RepState.Unknown;
            double LowestP = 1.0;
            foreach(var elem in Possibility)
            {
                if (elem.Value < LowestP)
                {
                    LowestRepState = elem.Key;
                    LowestP = elem.Value;
                }
            }
            tTopMostSetup_Tick(new object(), new EventArgs());
            return LowestRepState;
        }

        private bool isEmptyPoint(System.Drawing.Point p)
        {
            if (p.X == -1 || p.Y == -1)
                return true;
            return false;
        }

        private double GetSimilarity(Bitmap Original, Bitmap Compare, System.Drawing.Point LeftTopPoint)
        {
            if (isEmptyPoint(LeftTopPoint))
            {
                return 999999.99;
            }
            Bitmap CroppedOrigBmp = new Bitmap(Compare.Width, Compare.Height);
            Graphics gCroppedOrig = Graphics.FromImage(CroppedOrigBmp);
            gCroppedOrig.DrawImage(Original, 0,0, new Rectangle(LeftTopPoint.X, LeftTopPoint.Y, Compare.Width, Compare.Height), GraphicsUnit.Pixel);
            //BitmapData bmdSource = CroppedOrigBmp.LockBits(new Rectangle(0, 0, Compare.Width, Compare.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //BitmapData bmdTarget = Compare.LockBits(new Rectangle(0, 0, Compare.Width, Compare.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //IntPtr SrcPtr = bmdSource.Scan0;
            //IntPtr TgtPtr = bmdTarget.Scan0;
            //int bytes = bmdTarget.Width * bmdTarget.Height * 4;
            //byte[] srcBytes = new byte[bytes];
            //byte[] cmpBytes = new byte[bytes];
            

            //Marshal.Copy(SrcPtr, srcBytes, 0, bytes);
            //Marshal.Copy(TgtPtr, cmpBytes, 0, bytes);
            double TotalDist = 0;
            for(int y=0;y< Compare.Height;y++)
            {
                for (int x=0;x< Compare.Width;x++)
                {
                    //Color SrcColor = Color.FromArgb(srcBytes[y * bmdSource.Stride + x + 3], srcBytes[y * bmdSource.Stride + x + 2], srcBytes[y * bmdSource.Stride + x+1], srcBytes[y * bmdSource.Stride + x ]);
                    //Color CmpColor = Color.FromArgb(cmpBytes[y * bmdSource.Stride + x + 3], cmpBytes[y * bmdSource.Stride + x + 2], cmpBytes[y * bmdSource.Stride + x+1], cmpBytes[y * bmdSource.Stride + x ]);
                    Color SrcColor = CroppedOrigBmp.GetPixel(x, y);
                    Color CmpColor = Compare.GetPixel(x, y);
                    int rmean = (SrcColor.R + CmpColor.R) / 2;
                    int r = Math.Abs(SrcColor.R - CmpColor.R);
                    int g = Math.Abs(SrcColor.G - CmpColor.G);
                    int b = Math.Abs(SrcColor.B - CmpColor.B);
                    var dist = Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
                    TotalDist += dist;
                }
            }
            //CroppedOrigBmp.UnlockBits(bmdSource);
            //Compare.UnlockBits(bmdTarget);
            //SrcPtr = IntPtr.Zero;
            //TgtPtr = IntPtr.Zero;

            gCroppedOrig.Dispose();
            CroppedOrigBmp.Dispose();
            
            return TotalDist;
        }

        private void GetNeteasePlayingState()
        {
            Retry:
            RepeatState = DoImageCompare();
            if (RepeatState != RepState.Unknown)
            {
                switch(RepeatState)
                {
                    case RepState.ListLoop:
                        btnLoop.BackgroundImage = ListLoop;
                        break;
                    case RepState.ListNoLoop:
                        btnLoop.BackgroundImage = ListNoLoop;
                        break;
                    case RepState.OneLoop:
                        btnLoop.BackgroundImage = OneLoop;
                        break;
                    case RepState.Random:
                        btnLoop.BackgroundImage = Properties.Resources.Random;
                        break;
                }
            }
            else
            {
                Thread.Sleep(5);
                goto Retry;
            }
            
            if (KeyPointColors[KeyPoints[3]].R == 0xff && KeyPointColors[KeyPoints[3]].G == 0xff && KeyPointColors[KeyPoints[3]].B == 0xff )
            {
                PlayingState = PlayState.Playing;
            }
            else
            {
                PlayingState = PlayState.Paused;
            }
            if (KeyPointColors[KeyPoints[4]].R == 0x66 && KeyPointColors[KeyPoints[4]].G == 0x66 && KeyPointColors[KeyPoints[4]].B == 0x66)
            {
                LyricState = LrcState.On;
                btnLrc.BackgroundImage = Lyric;
            }else
            {
                LyricState = LrcState.Off;
                btnLrc.BackgroundImage = Lyric_Off;
            }


        }
        
    }
}
