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
 * Jeff Lenk <jeff@jefflenk.com>
 * 
 * PluginManager.cs -- Plugin execution code
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class ModuleProxy : MarshalByRefObject, IModuleProxy
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }
        public IPluginHandlerOrchestrator PluginHandlerOrchestrator { get { return ModuleServiceLocator.Container.Create<IPluginHandlerOrchestrator>(); } }
        public ILogDirector LogDirector { get { return ModuleServiceLocator.Container.Create<LogDirector>() as ILogDirector; } }
        public ILogDirector Logger { get { return ModuleServiceLocator.Container.Create<ILogger>() as ILogDirector; } }
        public IModuleAssemblyLoader AssemblyLoader { get { return ModuleServiceLocator.Container.Create<IModuleAssemblyLoader>(); } }
        public string MasterAssemblyPath
        {
            get { return ModuleServiceLocator.MasterAssemblyPath; }
            set
            {
                ModuleServiceLocator.MasterAssemblyPath = value;
            }
        }

    }
}
