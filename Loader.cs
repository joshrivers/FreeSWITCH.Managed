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
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public static class Loader
    {
        // Load() method is found by reflection and run from mod_managed.cpp on module load. The only purpose of this method
        // is to pass pointers to the internal delegates to the unmanaged code and to instantiate a managed class to handle
        // calls to those delegates.
        public static bool Load()
        {
            //Log.WriteLine(LogLevel.Alert, "Starting Loader");
            CoreDelegates.InitializeCoreDelegates();
            return DefaultLoader.Loader.Load();
        }
    }
}
