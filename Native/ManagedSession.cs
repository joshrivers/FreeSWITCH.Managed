﻿/* 
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
 * 
 * ManagedSession.cs -- ManagedSession additional functions
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FreeSWITCH.Native
{
    // switch_status_t ManagedSession::run_dtmf_callback(void *input, switch_input_type_t itype)
    // But, process_callback_result is used to turn a string into a switch_status_t
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate string DtmfCallback(IntPtr input, Native.switch_input_type_t itype);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void CdeclAction();

    public partial class ManagedSession
    {
        // SWITCH_DECLARE(void) InitManagedSession(ManagedSession *session, MonoObject *dtmfDelegate, MonoObject *hangupDelegate)
        [DllImport("mod_managed.dll", CharSet = CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
        static extern void InitManagedSession(IntPtr sessionPtr, DtmfCallback dtmfDelegate, CdeclAction hangupDelegate);

        /// <summary>Initializes the native ManagedSession. Must be called after Originate.</summary>
        public void Initialize()
        {
            if (allocated == 0) {
                Log.WriteLine(LogLevel.Critical, "Cannot initialize a ManagedSession until it is allocated (originated successfully).");
            }
            // P/Invoke generated function pointers stick around until the delegate is collected
            // By sticking the delegates in fields, their lifetime won't be less than the session
            // So we don't need to worry about GCHandles and all that....
            // Info here: http://blogs.msdn.com/cbrumme/archive/2003/05/06/51385.aspx
            this._inputCallbackRef = inputCallback;
            this._hangupCallbackRef = hangupCallback;
            InitManagedSession(ManagedSession.getCPtr(this).Handle, this._inputCallbackRef, this._hangupCallbackRef);
            this._variables = new ChannelVariables(this);
        }
        DtmfCallback _inputCallbackRef;
        CdeclAction _hangupCallbackRef;

        /// <summary>Function to execute when this session hangs up.</summary>
        public Action HangupFunction { get; set; }

        void hangupCallback()
        {
            Log.WriteLine(LogLevel.Debug, "AppFunction is in hangupCallback.");
            try {
                var f = HangupFunction;
                if (f != null) f();
            }
            catch (Exception ex) {
                Log.WriteLine(LogLevel.Warning, "Exception in hangupCallback: {0}", ex.ToString());
            }
        }

        public Func<Char, TimeSpan, string> DtmfReceivedFunction { get; set; }

        public Func<Native.Event, string> EventReceivedFunction { get; set; }

        string inputCallback(IntPtr input, Native.switch_input_type_t inputType)
        {
            try {
                switch (inputType) {
                    case FreeSWITCH.Native.switch_input_type_t.SWITCH_INPUT_TYPE_DTMF:
                        using (var dtmf = new Native.switch_dtmf_t(input, false)) {
                            return dtmfCallback(dtmf);
                        }
                    case FreeSWITCH.Native.switch_input_type_t.SWITCH_INPUT_TYPE_EVENT:
                        using (var swevt = new Native.switch_event(input, false)) {
                            return eventCallback(swevt);
                        }
                    default:
                        return "";
                }
            } catch (Exception ex) {
                Log.WriteLine(LogLevel.Error, "InputCallback threw exception: " + ex.ToString());
                return "-ERR InputCallback Exception: " + ex.Message;
            }
        }

        string dtmfCallback(Native.switch_dtmf_t dtmf) {
            var f = DtmfReceivedFunction;
            return f == null ? "" 
                : f(((char)(byte)dtmf.digit), TimeSpan.FromMilliseconds(dtmf.duration));
        }

        string eventCallback(Native.switch_event swevt) {
            using (var evt = new FreeSWITCH.Native.Event(swevt, 0)) {
                var f = EventReceivedFunction;
                return f == null ? "" 
                    : f(evt);
            }
        }

        // Convenience
        public bool IsAvailable {
            get { return this.Ready(); }
        }

        public Guid Uuid {
            get {
                return new Guid(this.GetUuid());
            }
        }

        // Need to find a better place to put these - then make them public
        static readonly DateTime epoch = new DateTime(1970, 1, 1);
        static DateTime epochUsToDateTime(long us)
        {
            return us == 0 ?
                DateTime.MinValue :
                epoch.AddMilliseconds((double)((decimal)us / 1000m));
        }
        static bool strToBool(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            switch (s.ToLowerInvariant())
            {
                case "true":
                case "yes":
                case "on":
                case "enable":
                case "enabled":
                case "active":
                case "allow":
                    return true;
                default:
                    // Numbers are true
                    long tmp;
                    return long.TryParse(s, out tmp);
            }
        }
        static string boolToStr(bool b)
        {
            return b ? "true" : "false";
        }

        ChannelVariables _variables; // Set on ManagedSession init
        public ChannelVariables Variables
        {
            get
            {
                if (_variables == null)
                {
                    _variables = new ChannelVariables(this);
                }
                return _variables;
            }
        }
    }
}
