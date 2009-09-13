using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FreeSWITCH
{
    public static class CoreDelegates
    {
        // Thunks - defining the delegate format for interop with unmanaged code. See global delegates in mod_managed.cpp.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool ExecuteDelegate(string cmd, IntPtr streamH, IntPtr eventH);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool ExecuteBackgroundDelegate(string cmd);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool RunDelegate(string cmd, IntPtr session);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool ReloadDelegate(string cmd);

        // Internal delegates to be references from unmanaged code. These directly pass through to the public delegates.
        // FreeSWITCH core will dump if the public delegates are set to null.
        private static readonly ExecuteDelegate execute = (cmd, streamH, eventH) => CoreDelegates.Execute(cmd, streamH, eventH);
        private static readonly ExecuteBackgroundDelegate executeBackground = (cmd) => CoreDelegates.ExecuteBackground(cmd);
        private static readonly RunDelegate run = (cmd, session) => CoreDelegates.Run(cmd, session);
        private static readonly ReloadDelegate reload = (cmd) => CoreDelegates.Reload(cmd);

        // Public delegates. These are the core extension points to allow managed code to be executed from FreeSWITCH.
        public static ExecuteDelegate Execute { get; set; }
        public static ExecuteBackgroundDelegate ExecuteBackground { get; set; }
        public static RunDelegate Run { get; set; }
        public static ReloadDelegate Reload { get; set; }

        // Extern InitManagedDelegates - executes function fo the same name in mod_managed.cpp.
        //SWITCH_MOD_DECLARE_NONSTD(void) InitManagedDelegates(runFunction run, executeFunction execute, executeBackgroundFunction executeBackground, reloadFunction reload)
        [DllImport("mod_managed", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void InitManagedDelegates(RunDelegate run, ExecuteDelegate execute, ExecuteBackgroundDelegate executeBackground, ReloadDelegate reload);

        public static void InitializeCoreDelegates()
        {
            CoreDelegates.InitManagedDelegates(CoreDelegates.run, CoreDelegates.execute, CoreDelegates.executeBackground, CoreDelegates.reload);
        }
    }
}
