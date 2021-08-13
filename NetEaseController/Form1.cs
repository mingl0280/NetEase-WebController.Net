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
            foreach (SettingsProperty setting_prop in CommandSetting.Default.Properties)
            {
                List<VirtualKeyCode> current_vk_list = new List<VirtualKeyCode>();
                string cvk_string = (string)setting_prop.DefaultValue;
                textBox1.AppendText(setting_prop.Name + " = " + (string)setting_prop.DefaultValue + "\r\n");
                string[] cvk_strings = cvk_string.Split('+');
                foreach (string cvk in cvk_strings)
                {
                    string vk_str = cvk.Trim().ToUpper();
                    vk_str = GetProperVKString(vk_str);
                    current_vk_list.Add((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), vk_str));
                }
                CommandDict.Add(setting_prop.Name, current_vk_list);
            }
        }

        private VBOPs VBOps = new VBOPs();
        private readonly ComponentResourceManager _Res = new ComponentResourceManager(typeof(Form1));

        private string SetNEPath()
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
                NEPath = SetNEPath();
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
                        NEPath = SetNEPath();
                    }
                }

                if (string.IsNullOrWhiteSpace(NEPath))
                {
                    NEPath = SetNEPath();
                }
            }
            textBox1.AppendText(_Res.GetString("NEName") + _Res.GetString("PathTranslate") + _Res.GetString("CommaSymbol") + NEPath + "\r\n");
            //VBOps.Initialize();
            FunctionsMap.Add("restartnetease", () =>
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
            FunctionsMap.Add("restartme", () =>
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
            FunctionsMap.Add("AddVBVoice", () =>
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
            StringBuilder title_builder = new StringBuilder(2053);
            while (ProcRunning)
            {
                if (NEPid != IntPtr.Zero)
                {
                    try
                    {
                        title_builder.Clear();
                        NativeMethods.GetWindowText(NEPid, title_builder, 2053);
                        MusicTitleString = title_builder.ToString();
                        if (string.IsNullOrWhiteSpace(MusicTitleString))
                        {
                            NEPid = IntPtr.Zero;
                        }
                        //MusicTitleString = Process.GetProcessById(NEPid).MainWindowTitle;
                    }
                    catch (Exception e)
                    {
                        NEPid = IntPtr.Zero;
                        Debug.WriteLine(e.Message);
                    }
                }
                else
                {
                    try
                    {
                        Process[] neps = Process.GetProcessesByName("cloudmusic");
                        var pids = neps.Select(process => process.Id).ToList();
                        NativeMethods.EnumWindows((hwnd, param) =>
                        {
                            title_builder.Clear();   
                            NativeMethods.GetWindowText(hwnd, title_builder, 2053);
                            int ckpid = 0;
                            NativeMethods.GetWindowThreadProcessId(hwnd, out ckpid);
                            if (pids.Contains(ckpid))
                            {
                                if (!string.IsNullOrWhiteSpace(title_builder.ToString()))
                                {
                                    if (title_builder.ToString().Contains("-"))
                                    {
                                        NEPid = hwnd;
                                        return false;
                                    }
                                }
                            }

                            return true;
                        }, 0);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
                Thread.Sleep(2000);
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
            using (HttpListener hListener = new HttpListener())
            {
                hListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                hListener.Prefixes.Add("http://*:10180/");
                hListener.Start();
                BeginInvoke(new AddToTextLogD(AddToTextLog),
                    _Res.GetString("ServerListeningText") + _Res.GetString("CommaSymbol") + "http://*:10180/", "Info");
                while (ProcRunning)
                {
                    HttpListenerContext HContext = hListener.GetContext();
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
                            string decodedPathString = "." + HttpUtility.UrlDecode(RelativeUrl).Replace(@"/", @"\");
                            if (!File.Exists(decodedPathString))
                            {
                                HContext.Response.StatusCode = 404;
                                HContext.Response.ContentEncoding = Encoding.UTF8;
                                decodedPathString = "./error/404.htm";
                                BeginInvoke(new AddToTextLogD(AddToTextLog), "Query to " + HContext.Request.RawUrl + " Failed. 404.", "Error");
                                SendResponse(decodedPathString, ref HContext);
                            }
                            else
                            {
                                HContext.Response.StatusCode = 200;
                                HContext.Response.ContentEncoding = Encoding.UTF8;
                                //BeginInvoke(new AddToTextLogD(AddToTextLog), "Query to " + HContext.Request.RawUrl + " Succeed. 200.", "Info");
                                SendResponse(decodedPathString, ref HContext);
                            }
                        }
                        else
                        {
                            string opString = new StreamReader(HContext.Request.InputStream).ReadToEnd();
                            if (opString == "Status")
                            {
                                //BeginInvoke(new AddToTextLogD(AddToTextLog), "Query to " + HContext.Request.RawUrl + " Succeed. 200.", "Info");
                                SendPlainTextResponse(@"正在播放: " + MusicTitleString, ref HContext);
                            }
                            else
                            {
                                ExecuteNECommand(opString);
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
        /// <param name="hContext">Web请求上下文</param>
        private void SendResponse(string RealFilePath, ref HttpListenerContext hContext)
        {
            FileInfo fi = new FileInfo(RealFilePath);
            hContext.Response.ContentType = MIMEMaps.GetMimeType(fi.Extension);
            try
            {
                using (StreamWriter pageWriter = new StreamWriter(hContext.Response.OutputStream))
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

            hContext.Response.Close();
        }

        /// <summary>
        /// 发送少量文本
        /// </summary>
        /// <param name="TextContent">文本内容</param>
        /// <param name="hContext">Web请求上下文</param>
        private void SendPlainTextResponse(string TextContent, ref HttpListenerContext hContext)
        {
            hContext.Response.ContentType = "text/text";
            using (StreamWriter pageWriter = new StreamWriter(hContext.Response.OutputStream))
            {
                pageWriter.Write(TextContent);
                pageWriter.Close();
            }
            hContext.Response.Close();
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
            new Form1().ExecuteNECommand(Command);
        }

        private Dictionary<string, Func<string>> FunctionsMap = new Dictionary<string, Func<string>>();


        /// <summary>
        /// 翻译命令到快捷键并执行
        /// </summary>
        /// <param name="command">目标命令</param>
        public void ExecuteNECommand(string command)
        {
            try
            {
                string command_execute_result = "";
                if (CommandDict.ContainsKey(command))
                {
                    List<VirtualKeyCode> vks = new List<VirtualKeyCode>(CommandDict[command]);
                    PressCommandKeys(vks);
                    command_execute_result = "Succeed.";
                }
                else
                {
                    if (FunctionsMap.ContainsKey(command.ToLower()))
                    {
                        command_execute_result = FunctionsMap[command.ToLower()]();
                    }
                    else
                    {
                        command_execute_result = "Failed. Unknown Command.";
                    }
                }

                BeginInvoke(new AddToTextLogD(AddToTextLog), $"\t\tCommand {command} Execute {command_execute_result}", "Info");
            }
            catch (Exception)
            {

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
