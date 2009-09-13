/* 
 * FreeSWITCH Modular Media Switching Software Library / Soft-Switch Application - mod_managed
 * Copyright (C) 2008, Michael Giagnocavo <mgg@giagnocavo.net>
 *
 * Version: MPL 1.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is FreeSWITCH Modular Media Switching Software Library / Soft-Switch Application - mod_managed
 *
 * The Initial Developer of the Original Code is
 * Michael Giagnocavo <mgg@giagnocavo.net>
 * Portions created by the Initial Developer are Copyright (C)
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 * 
 * Michael Giagnocavo <mgg@giagnocavo.net>
 * David Brazier <David.Brazier@360crm.co.uk>
 * Jeff Lenk <jeff@jefflenk.com>
 * 
 * Loader.cs -- mod_managed loader
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FreeSWITCH
{
    internal static class Loader
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
        private static readonly ExecuteDelegate execute = (cmd, streamH, eventH) => Loader.Execute(cmd, streamH, eventH);
        private static readonly ExecuteBackgroundDelegate executeBackground = (cmd) => Loader.ExecuteBackground(cmd);
        private static readonly RunDelegate run = (cmd, session) => Loader.Run(cmd, session);
        private static readonly ReloadDelegate reload = (cmd) => Loader.Reload(cmd);

        // Public delegates. These are the core extension points to allow managed code to be executed from FreeSWITCH.
        public static ExecuteDelegate Execute { get; set; }
        public static ExecuteBackgroundDelegate ExecuteBackground { get; set; }
        public static RunDelegate Run { get; set; }
        public static ReloadDelegate Reload { get; set; }

        // Extern InitManagedDelegates - executes function fo the same name in mod_managed.cpp.
        //SWITCH_MOD_DECLARE_NONSTD(void) InitManagedDelegates(runFunction run, executeFunction execute, executeBackgroundFunction executeBackground, reloadFunction reload)
        [DllImport("mod_managed", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void InitManagedDelegates(RunDelegate run, ExecuteDelegate execute, ExecuteBackgroundDelegate executeBackground, ReloadDelegate reload);

        // Load() method is found by reflection and run from mod_managed.cpp on module load. The only purpose of this method
        // is to pass pointers to the internal delegates to the unmanaged code and to instantiate a managed class to handle
        // calls to those delegates.
        public static bool Load()
        {
            Loader.InitManagedDelegates(Loader.run, Loader.execute, Loader.executeBackground, Loader.reload);
            return FreeSWITCH.Managed.Loader_Internal.Load();
        }
    }
}
