using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Microsoft.Win32;

namespace NetEaseController
{
    class VBOPs
    {
        
        public void Initialize()
        {
            var HKLMRoot = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            var UnistallKey = HKLMRoot.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Windows")
                .OpenSubKey("CurrentVersion").OpenSubKey("Uninstall").OpenSubKey("VB:Voicemeeter {17359A74-1236-5467}");
            FileInfo VBUninstFileInfo = new FileInfo((string) UnistallKey.GetValue("DisplayIcon"));
            string dll_name = "";
            if (Environment.Is64BitProcess)
            {
                dll_name = VBUninstFileInfo.Directory.FullName + @"\VoicemeeterRemote64.dll";
            }
            else
            {
                dll_name = VBUninstFileInfo.Directory.FullName + @"\VoicemeeterRemote.dll";
            }

            var result = NativeMethods.LoadLibrary(dll_name);
            if (result == 0)
            {
                throw new DllNotFoundException($@"E: Can't load VRM DLL: {dll_name}");
            }

            Login = GetFuncDeleg<NoArgRetLong>(result, "VBVMR_Login");
            Logout = GetFuncDeleg<NoArgRetLong>(result, "VBVMR_Logout");
            RunVoicemeeter = GetFuncDeleg<LongArgRetLong>(result, "VBVMR_RunVoicemeeter");
            GetVoicemeeterType = GetFuncDeleg<LongOutArgRetLong>(result, "VBVMR_GetVoicemeeterType");
            GetVoicemeeterVersion = GetFuncDeleg<LongOutArgRetLong>(result, "VBVMR_GetVoicemeeterVersion");
            IsParametersDirty = GetFuncDeleg<NoArgRetLong>(result, "VBVMR_IsParametersDirty");


            GetLevel = GetFuncDeleg<LongLongFloatRef>(result, "VBVMR_GetLevel");

        }

        private TDelegType GetFuncDeleg<TDelegType>(int handle, string func_name) where TDelegType: class
        {
            var ptr = NativeMethods.GetProcAddress(handle, func_name);
            Delegate func_instance = Marshal.GetDelegateForFunctionPointer(ptr, typeof(TDelegType));
            return func_instance as TDelegType;
        }

        /// <summary>
        /// Open Communication Pipe With Voicemeeter (typically called on software startup).
        /// </summary>
        /// <returns>0: OK
        ///  1: OK but Voicemeeter Application not launched.
        /// -1: cannot get client (unexpected)
        /// -2: unexpected login (logout was expected before).
        /// </returns>
        public NoArgRetLong Login;

        /// <summary>
        /// Close Communication Pipe With Voicemeeter (typically called on software end).
        /// </summary>
        /// <returns>0 if ok.</returns>
        public NoArgRetLong Logout;

        public LongArgRetLong RunVoicemeeter;
        public LongOutArgRetLong GetVoicemeeterType;
        public LongOutArgRetLong GetVoicemeeterVersion;
        public NoArgRetLong IsParametersDirty;



        public LongLongFloatRef GetLevel;
        

        public delegate long NoArgRetLong();

        public delegate long LongArgRetLong(long arg);

        public delegate long LongOutArgRetLong(ref long arg);

        public delegate long StrRefFltRef(ref string name, ref float value);

        public delegate long StrRefStrRef(ref string name, ref string value);

        public delegate long StrRefUShortRef(ref string name, ref ushort[] value);

        public delegate long LongLongFloatRef(long arg1, long arg2, ref float value);

    }
}
