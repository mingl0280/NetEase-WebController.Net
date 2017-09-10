using NetEaseController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Web;

namespace NetEaseController
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private Dictionary<string, List<VirtualKeyCode>> CommandDict = new Dictionary<string, List<VirtualKeyCode>>();
        private static int NEPid = 0;
        private static bool ProcRunning = true;
        private static Thread MusicTitleThread, ControllerListenerThread;
        private static string MusicTitleString = "";

        private delegate void AddToTextLogD(string text, string severity);

        public Form1()
        {
            InitializeComponent();
            LoadCommandKeySeettings();
        }

        /// <summary>
        /// 载入快捷键设定
        /// </summary>
        private void LoadCommandKeySeettings()
        {
            foreach (SettingsProperty SettingProp in Properties.CommandSetting.Default.Properties)
            {
                List<VirtualKeyCode> currentVKList = new List<VirtualKeyCode>();
                string CVKString = (string)SettingProp.DefaultValue;
                textBox1.AppendText(SettingProp.Name + " = " + (string)SettingProp.DefaultValue + "\r\n");
                string[] CVKStrings = CVKString.Split('+');
                foreach (string CVK in CVKStrings)
                {
                    string VKStr = CVK.Trim().ToUpper();
                    VKStr = GetProperVKString(VKStr);
                    currentVKList.Add((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), VKStr));
                }
                CommandDict.Add(SettingProp.Name, currentVKList);
            }
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
                    return "RWIN";
                default:
                    return "VK_" + input;
            }
        }

        /// <summary>
        /// 音乐标题更新线程
        /// </summary>
        private void MusicTitleUpdater()
        {
            while (ProcRunning)
            {
                if (NEPid != 0)
                {
                    try
                    {
                        MusicTitleString = Process.GetProcessById(NEPid).MainWindowTitle;
                    }
                    catch (Exception e)
                    {
                        NEPid = 0;
                        Debug.WriteLine(e.Message);
                    }
                }
                else
                {
                    try
                    {
                        Process[] neps = Process.GetProcessesByName("cloudmusic");
                        foreach (Process proc in neps)
                        {
                            var CNText = new StringBuilder("", 2053);
                            GetClassName(proc.MainWindowHandle, CNText, 2050);
                            if (CNText.ToString().Trim() != "" && CNText.ToString() != "DesktopLyrics")
                            {
                                NEPid = proc.Id;
                            }
                        }
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
                tList.RemoveRange(CommandDict.Count - 1, 150+CommandDict.Count);
                textBox1.Lines = tList.ToArray();
            }

            textBox1.AppendText(string.Format("[{0} {1}][{2}]{3}\r\n", new string[]{
                DateTime.Now.ToLongDateString(),
                DateTime.Now.ToLongTimeString(),
                severity,
                text}));
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
                while (ProcRunning)
                {
                    HttpListenerContext hContext = hListener.GetContext();
                    string RelativeUrl = hContext.Request.RawUrl;
                    if (RelativeUrl == "/")
                    {
                        hContext.Response.StatusCode = 200;
                        hContext.Response.ContentEncoding = Encoding.UTF8;
                        BeginInvoke(new AddToTextLogD(AddToTextLog), new string[] { string.Format("{0}:{1} Query to {2} Succeed. 200.",
                            new string[] {
                                hContext.Request.RemoteEndPoint.Address.ToString(),
                                hContext.Request.RemoteEndPoint.Port.ToString(),
                                hContext.Request.RawUrl })  , "Info" });
                        SendResponse("./html/index.htm", ref hContext);
                    }
                    else
                    {
                        if (!(RelativeUrl.StartsWith(@"/Operation")))
                        {
                            string decodedPathString = "." + HttpUtility.UrlDecode(RelativeUrl).Replace(@"/", @"\");
                            if (!File.Exists(decodedPathString))
                            {
                                hContext.Response.StatusCode = 400;
                                hContext.Response.ContentEncoding = Encoding.UTF8;
                                decodedPathString = "./error/404.htm";
                                BeginInvoke(new AddToTextLogD(AddToTextLog), new string[] { "Query to " + hContext.Request.RawUrl + " Failed. 404.", "Error" });
                                SendResponse(decodedPathString, ref hContext);
                            }
                            else
                            {
                                hContext.Response.StatusCode = 200;
                                hContext.Response.ContentEncoding = Encoding.UTF8;
                                BeginInvoke(new AddToTextLogD(AddToTextLog), new string[] { "Query to " + hContext.Request.RawUrl + " Succeed. 200.", "Info" });
                                SendResponse(decodedPathString, ref hContext);
                            }
                        }
                        else
                        {
                            string opString = new StreamReader(hContext.Request.InputStream).ReadToEnd();
                            if (opString == "Status")
                            {
                                BeginInvoke(new AddToTextLogD(AddToTextLog), new string[] { "Query to " + hContext.Request.RawUrl + " Succeed. 200.", "Info" });
                                SendPlainTextResponse(@"正在播放: " + MusicTitleString, ref hContext);
                            }
                            else
                            {
                                ExecuteNECommand(opString);
                                BeginInvoke(new AddToTextLogD(AddToTextLog), new string[] { "Query to " + hContext.Request.RawUrl + " Succeed. 200.", "Info" });
                                SendPlainTextResponse("OK", ref hContext);
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
            using (StreamWriter pageWriter = new StreamWriter(hContext.Response.OutputStream))
            {
                using (StreamReader pReader = new StreamReader(RealFilePath))
                {
                    pageWriter.Write(pReader.ReadToEnd());
                }
                pageWriter.Close();
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
            MusicTitleThread = new Thread(new ThreadStart(MusicTitleUpdater));
            MusicTitleThread.Start();
            ControllerListenerThread = new Thread(new ThreadStart(ControlListener));
            ControllerListenerThread.Start();
        }

        /// <summary>
        /// 翻译命令到快捷键并执行
        /// </summary>
        /// <param name="Command">目标命令</param>
        private void ExecuteNECommand(string Command)
        {
            List<VirtualKeyCode> vks = new List<VirtualKeyCode>(CommandDict[Command]);
            PressCommandKeys(vks);
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
                VirtualKeyCode vkThis = vks.First();
                vks.RemoveAt(0);
                KeyboardAndMouseHooksAndMessages.SimulateKeyDown(vkThis);
                PressCommandKeys(vks);
                KeyboardAndMouseHooksAndMessages.SimulateKeyUp(vkThis);
            }
        }
    }
}
