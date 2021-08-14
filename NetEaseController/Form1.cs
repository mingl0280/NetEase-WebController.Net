using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using NetEaseController.Properties;

namespace NetEaseController
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        /*
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hwnd, );
        */

        private Dictionary<string, List<VirtualKeyCode>> CommandDict = new Dictionary<string, List<VirtualKeyCode>>();
        private static IntPtr NEPid = IntPtr.Zero;
        private static bool ProcRunning = true;
        private static Thread MusicTitleThread, ControllerListenerThread;
        private static string MusicTitleString = "";
        private readonly Process CurrentProcess = Process.GetCurrentProcess();

        private delegate void AddToTextLogD(string text, string severity);

        public Form1()
        {
            InitializeComponent();
            LoadCommandKeySeettings();
            LoadActionsMap();
        }

        /// <summary>
        /// 载入快捷键设定
        /// </summary>
        private void LoadCommandKeySeettings()
        {
            foreach (SettingsProperty SettingProp in CommandSetting.Default.Properties)
            {
                List<VirtualKeyCode> CurrentVkList = new List<VirtualKeyCode>();
                string CvkString = (string)SettingProp.DefaultValue;
                textBox1.AppendText(SettingProp.Name + " = " + (string)SettingProp.DefaultValue + "\r\n");
                string[] CvkStrings = CvkString.Split('+');
                foreach (string Cvk in CvkStrings)
                {
                    string VkStr = Cvk.Trim().ToUpper();
                    VkStr = GetProperVKString(VkStr);
                    CurrentVkList.Add((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), VkStr));
                }
                CommandDict.Add(SettingProp.Name, CurrentVkList);
            }
        }

        private VBOPs _VbOps = new VBOPs();
        private readonly ComponentResourceManager _Res = new ComponentResourceManager(typeof(Form1));

        private string SetNePath()
        {
           

            string NEPath;
            var CfgFile = File.CreateText("config.txt");
            while (true)
            {
                OpenFileDialog Ofd = new OpenFileDialog
                {
                    FileName = "CloudMusic.exe",
                    CheckFileExists = true,
                    Multiselect = false,
                    Filter = _Res.GetString("NEFilterStr"),
                };
                Ofd.ShowDialog();
                NEPath = Ofd.FileName;
                if (!string.IsNullOrWhiteSpace(NEPath))
                {
                    CfgFile.WriteLine($"Exec={NEPath}");
                    CfgFile.Flush();
                    CfgFile.Close();
                    break;
                }
            }
            return NEPath;
        }

        /// <summary>
        /// 载入动作列表
        /// </summary>
        private void LoadActionsMap()
        {
            string NEPath = "";
            if (!File.Exists("config.txt"))
            {
                NEPath = SetNePath();
            }
            else
            {
                using (StreamReader Sr = new StreamReader("config.txt"))
                {
                    var LineText = Sr.ReadLine();
                    string Arg = "", Value = "";
                    bool ValueRange = false;
                    if (LineText != null)
                    {
                        foreach (var T in LineText)
                        {
                            if (T == '=' && !ValueRange)
                            {
                                ValueRange = true;
                                continue;
                            }

                            if (!ValueRange)
                            {
                                Arg += T;
                            }
                            else
                            {
                                Value += T;
                            }
                        }

                        if (Arg == "Exec")
                        {
                            NEPath = Value;
                        }
                    }
                    else
                    {
                        NEPath = SetNePath();
                    }
                }

                if (string.IsNullOrWhiteSpace(NEPath))
                {
                    NEPath = SetNePath();
                }
            }
            textBox1.AppendText(_Res.GetString("NEName") + _Res.GetString("PathTranslate") + _Res.GetString("CommaSymbol") + NEPath + "\r\n");
            //VBOps.Initialize();
            _FunctionsMap.Add("restartnetease", () =>
            {
                try
                {
                    Process[] neps = Process.GetProcessesByName("cloudmusic");
                    foreach (Process nep in neps)
                    {
                        try
                        {
                            nep.Kill();
                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() != (new NotSupportedException()).GetType())
                            {
                                continue;
                            }

                            throw ex;
                        }
                    }

                    Process NewProc = new Process
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            Arguments = "",
                            FileName = NEPath,
                            CreateNoWindow = false,
                            WorkingDirectory = new FileInfo(NEPath).DirectoryName ?? string.Empty,
                        }
                    };
                    NewProc.Start();
                    return "Succeed.";
                }
                catch (Exception e)
                {
                    return "Failed. " + e.Message + "\r\n\r\n" + e.StackTrace;
                }
            });
            _FunctionsMap.Add("restartme", () =>
            {
                BeginInvoke(new AddToTextLogD(AddToTextLog), "Restart Command Received. Restart.", "Info");
                Thread.Sleep(500);
                Process NewProc = new Process()
                {
                    StartInfo = new ProcessStartInfo(Application.ExecutablePath)
                };
                NewProc.Start();
                Environment.Exit(0);
                return "OK";
            });
            _FunctionsMap.Add("AddVBVoice", () =>
            {
                return "";
            });
        }

        /// <summary>
        /// 修正虚拟键映射问题
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetProperVKString(string input)
        {
            if (input.StartsWith("F") && (input.Length == 2))
                return input;
            if (input.StartsWith("VOLUME") || input.StartsWith("MEDIA"))
                return input;
            switch (input)
            {
                case "CTRL":
                    return "CONTROL";
                case "ALT":
                    return "MENU";
                case "WIN":
                    return "LWIN";
                default:
                    return "VK_" + input;
            }
        }

        /// <summary>
        /// 音乐标题更新线程
        /// </summary>
        private void MusicTitleUpdater()
        {
            StringBuilder TitleBuilder = new StringBuilder(2053);
            while (ProcRunning)
            {
                if (NEPid != IntPtr.Zero)
                {
                    try
                    {
                        TitleBuilder.Clear();
                        NativeMethods.GetWindowText(NEPid, TitleBuilder, 2053);
                        MusicTitleString = TitleBuilder.ToString();
                        if (string.IsNullOrWhiteSpace(MusicTitleString))
                        {
                            NEPid = IntPtr.Zero;
                        }
                        //MusicTitleString = Process.GetProcessById(NEPid).MainWindowTitle;
                    }
                    catch (Exception E)
                    {
                        NEPid = IntPtr.Zero;
                        Debug.WriteLine(E.Message);
                    }
                }
                else
                {
                    try
                    {
                        Process[] Neps = Process.GetProcessesByName("cloudmusic");
                        var Pids = Neps.Select(Process => Process.Id).ToList();
                        NativeMethods.EnumWindows((Hwnd, Param) =>
                        {
                            TitleBuilder.Clear();   
                            NativeMethods.GetWindowText(Hwnd, TitleBuilder, 2053);
                            NativeMethods.GetWindowThreadProcessId(Hwnd, out int CkPid);
                            if (!Pids.Contains(CkPid) || string.IsNullOrWhiteSpace(TitleBuilder.ToString()) ||
                                !TitleBuilder.ToString().Contains("-")) return true;
                            NEPid = Hwnd;
                            return false;

                        }, 0);
                    }
                    catch (Exception E)
                    {
                        Debug.WriteLine(E.Message);
                    }
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="text"></param>
        /// <param name="severity"></param>
        private void AddToTextLog(string text, string severity)
        {
            if (textBox1.Lines.Length > 200)
            {
                var tList = textBox1.Lines.ToList();
                tList.RemoveRange(CommandDict.Count - 1, 150 + CommandDict.Count);
                textBox1.Lines = tList.ToArray();
            }

            textBox1.AppendText(
                $"[{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}][{severity}]{text}\r\n");
        }

        /// <summary>
        /// Web服务器线程
        /// </summary>
        private void ControlListener()
        {
            using (HttpListener HListener = new HttpListener())
            {
                HListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                HListener.Prefixes.Add(Settings.Default.ServerAddress);
                HListener.Start();
                BeginInvoke(new AddToTextLogD(AddToTextLog),
                    _Res.GetString("ServerListeningText") + _Res.GetString("CommaSymbol") + Settings.Default.ServerAddress, "Info");
                while (ProcRunning)
                {
                    HttpListenerContext HContext = HListener.GetContext();
                    string RelativeUrl = HContext.Request.RawUrl;
                    if (RelativeUrl == "/")
                    {
                        HContext.Response.StatusCode = 200;
                        HContext.Response.ContentEncoding = Encoding.UTF8;
                        BeginInvoke(new AddToTextLogD(AddToTextLog),
                            $"{HContext.Request.RemoteEndPoint?.Address}:{HContext.Request.RemoteEndPoint?.Port.ToString()} Query to {HContext.Request.RawUrl} Succeed. 200.", "Info");
                        SendResponse("./html/index.htm", ref HContext);
                    }
                    else
                    {
                        if (!(RelativeUrl.StartsWith(@"/Operation")))
                        {
                            string DecodedPathString = "." + HttpUtility.UrlDecode(RelativeUrl).Replace(@"/", @"\");
                            if (!File.Exists(DecodedPathString))
                            {
                                HContext.Response.StatusCode = 404;
                                HContext.Response.ContentEncoding = Encoding.UTF8;
                                DecodedPathString = "./error/404.htm";
                                BeginInvoke(new AddToTextLogD(AddToTextLog), "Query to " + HContext.Request.RawUrl + " Failed. 404.", "Error");
                                SendResponse(DecodedPathString, ref HContext);
                            }
                            else
                            {
                                HContext.Response.StatusCode = 200;
                                HContext.Response.ContentEncoding = Encoding.UTF8;
                                //BeginInvoke(new AddToTextLogD(AddToTextLog), "Query to " + HContext.Request.RawUrl + " Succeed. 200.", "Info");
                                SendResponse(DecodedPathString, ref HContext);
                            }
                        }
                        else
                        {
                            string OpString = new StreamReader(HContext.Request.InputStream).ReadToEnd();
                            if (OpString == "Status")
                            {
                                //BeginInvoke(new AddToTextLogD(AddToTextLog), "Query to " + HContext.Request.RawUrl + " Succeed. 200.", "Info");
                                SendPlainTextResponse(@"正在播放: " + MusicTitleString, ref HContext);
                            }
                            else
                            {
                                ExecuteKeyboardCmd(OpString);
                                //BeginInvoke(new AddToTextLogD(AddToTextLog), "Query to " + HContext.Request.RawUrl + " Succeed. 200.", "Info");
                                SendPlainTextResponse("OK", ref HContext);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="RealFilePath">文件目录</param>
        /// <param name="HContext">Web请求上下文</param>
        private void SendResponse(string RealFilePath, ref HttpListenerContext HContext)
        {
            FileInfo fi = new FileInfo(RealFilePath);
            HContext.Response.ContentType = MIMEMaps.GetMimeType(fi.Extension);
            try
            {
                using (StreamWriter pageWriter = new StreamWriter(HContext.Response.OutputStream))
                {
                    using (StreamReader pReader = new StreamReader(RealFilePath))
                    {
                        pageWriter.Write(pReader.ReadToEnd());
                    }

                    pageWriter.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            HContext.Response.Close();
        }

        /// <summary>
        /// 发送少量文本
        /// </summary>
        /// <param name="TextContent">文本内容</param>
        /// <param name="HContext">Web请求上下文</param>
        private void SendPlainTextResponse(string TextContent, ref HttpListenerContext HContext)
        {
            HContext.Response.ContentType = "text/text";
            using (StreamWriter pageWriter = new StreamWriter(HContext.Response.OutputStream))
            {
                pageWriter.Write(TextContent);
                pageWriter.Close();
            }
            HContext.Response.Close();
        }

        /// <summary>
        /// 线程初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            MusicTitleThread = new Thread(MusicTitleUpdater);
            MusicTitleThread.Start();
            ControllerListenerThread = new Thread(ControlListener);
            ControllerListenerThread.Start();
        }

        /// <summary>
        /// 翻译命令到快捷键并执行（静态方法）
        /// </summary>
        /// <param name="Command">目标命令</param>
        public static void ExecuteCmd(string Command)
        {
            new Form1().ExecuteKeyboardCmd(Command);
        }

        private Dictionary<string, Func<string>> _FunctionsMap = new Dictionary<string, Func<string>>();


        /// <summary>
        /// 翻译命令到快捷键并执行
        /// </summary>
        /// <param name="Command">目标命令</param>
        public void ExecuteKeyboardCmd(string Command)
        {
            try
            {
                string CommandExecuteResult = "";
                if (CommandDict.ContainsKey(Command))
                {
                    List<VirtualKeyCode> vks = new List<VirtualKeyCode>(CommandDict[Command]);
                    PressCommandKeys(vks);
                    CommandExecuteResult = "Succeed.";
                }
                else
                {
                    if (_FunctionsMap.ContainsKey(Command.ToLower()))
                    {
                        CommandExecuteResult = _FunctionsMap[Command.ToLower()]();
                    }
                    else
                    {
                        CommandExecuteResult = "Failed. Unknown Command.";
                    }
                }

                BeginInvoke(new AddToTextLogD(AddToTextLog), $"\t\tCommand {Command} Execute {CommandExecuteResult}", "Info");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 线程销毁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ProcRunning = false;
            MusicTitleThread.Abort();
            ControllerListenerThread.Abort();
        }

        /// <summary>
        /// 执行快捷键
        /// </summary>
        /// <param name="vks"></param>
        private void PressCommandKeys(List<VirtualKeyCode> vks)
        {
            if (vks.Count == 1)
            {
                KeyboardAndMouseHooksAndMessages.SimulateKeyPress(vks.First());
            }
            else
            {
                VirtualKeyCode vk_this = vks.First();
                vks.RemoveAt(0);
                KeyboardAndMouseHooksAndMessages.SimulateKeyDown(vk_this);
                PressCommandKeys(vks);
                KeyboardAndMouseHooksAndMessages.SimulateKeyUp(vk_this);
            }
        }
    }
}
