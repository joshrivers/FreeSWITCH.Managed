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

namespace FreeSWITCH {

    public abstract class ModuleProxy : MarshalByRefObject {
        public override object InitializeLifetimeService() {
            return null;
        }
        //public ILogger logger;
        public List<ApiPluginExecutor> ApiExecutors { get { return _apiExecutors; } }
        readonly List<ApiPluginExecutor> _apiExecutors = new List<ApiPluginExecutor>();

        public List<AppPluginExecutor> AppExecutors { get { return _appExecutors; }  }

        readonly List<AppPluginExecutor> _appExecutors = new List<AppPluginExecutor>();

        bool isLoaded = false;

        public bool Load(string fileName) {
            DefaultLoader.Loader.PluginOverSeer.LoadForAllAppDomains();
            DefaultLoader.Loader.PluginOverSeer.Logger.Notice(string.Format("ModuleProxy.Load " + fileName.GetLoweredFileExtension()));
            Console.WriteLine("Loading {0} from domain {1}", fileName, AppDomain.CurrentDomain.FriendlyName);
            if (isLoaded) throw new InvalidOperationException("PluginManager has already been loaded.");
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("file cannot be null or empty.");
            if (AppDomain.CurrentDomain.IsDefaultAppDomain()) throw new InvalidOperationException("PluginManager must load in its own AppDomain.");
            bool result; 
            try
            {
                result = LoadInternal(fileName);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Info, "Couldn't load {0}: {1}", fileName, ex.Message);
                result = false;
            } 
            isLoaded = true;

            if (result) {
                result = (AppExecutors.Count > 0 || ApiExecutors.Count > 0);
                if (!result) Log.WriteLine(LogLevel.Info, "No App or Api plugins found in {0}.", fileName);
            }
            return result;
        }

        protected abstract bool LoadInternal(string fileName);

        protected void RunLoadNotify(Type[] allTypes) {
            // Run Load on all the load plugins
            var ty = typeof(ILoadNotificationPlugin);
            var pluginTypes = allTypes.Where(x => ty.IsAssignableFrom(x) && !x.IsAbstract).ToList();
            if (pluginTypes.Count == 0) return;
            foreach (var pt in pluginTypes) {
                var load = ((ILoadNotificationPlugin)Activator.CreateInstance(pt, true));
                if (!load.Load()) {
                    throw new RunNotifyException(string.Format("Type {0} requested no loading. Assembly will not be loaded.", pt.FullName));
                }
            }
            return;
        }

        protected void LoadPlugins(Assembly asm, string fileName)
        {
            // Ensure it's a plugin assembly
            this.EnsureFreswitchManagedDllReference(asm);

            // See if it wants to be loaded
            var allTypes = asm.GetExportedTypes();
            RunLoadNotify(allTypes);

            var opts = GetOptions(allTypes);

            AddApiPlugins(allTypes, opts);
            AddAppPlugins(allTypes, opts);

            // Add the script executors
            var name = Path.GetFileName(fileName);
            var aliases = new List<string> { name };
            this.ApiExecutors.Add(new ApiPluginExecutor(name, aliases, () => new ScriptApiWrapper(asm.EntryPoint.GetEntryDelegate()), opts));
            this.AppExecutors.Add(new AppPluginExecutor(name, aliases, () => new ScriptAppWrapper(asm.EntryPoint.GetEntryDelegate()), opts));
        }

        protected PluginOptions GetOptions(Type[] allTypes) {
            var ty = typeof(IPluginOptionsProvider);
            var pluginTypes = allTypes.Where(x => ty.IsAssignableFrom(x) && !x.IsAbstract).ToList();
            return pluginTypes.Aggregate(PluginOptions.None, (opts, t) => {
                var x = ((IPluginOptionsProvider)Activator.CreateInstance(t, true));
                return opts | x.GetOptions();
            });
        }

        protected void AddApiPlugins(Type[] allTypes, PluginOptions pluginOptions) {
            var iApiTy = typeof(IApiPlugin);
            foreach (var ty in allTypes.Where(x => iApiTy.IsAssignableFrom(x) && !x.IsAbstract)) {
                var del = CreateConstructorDelegate<IApiPlugin>(ty);
                var exec = new ApiPluginExecutor(ty.FullName, new List<string> { ty.FullName, ty.Name }, del, pluginOptions);
                this.ApiExecutors.Add(exec);
            }
        }

        protected void AddAppPlugins(Type[] allTypes, PluginOptions pluginOptions) {
            var iAppTy = typeof(IAppPlugin);
            foreach (var ty in allTypes.Where(x => iAppTy.IsAssignableFrom(x) && !x.IsAbstract)) {
                var del = CreateConstructorDelegate<IAppPlugin>(ty);
                var exec = new AppPluginExecutor(ty.FullName, new List<string> { ty.FullName, ty.Name }, del, pluginOptions);
                this.AppExecutors.Add(exec);
            }
        }

        #region Unload

        bool isUnloading = false;
        int unloadCount;
        System.Threading.ManualResetEvent unloadSignal = new System.Threading.ManualResetEvent(false);
        void decreaseUnloadCount() {
            if (System.Threading.Interlocked.Decrement(ref unloadCount) == 0) {
                unloadSignal.Set();
            }
        }

        public void BlockUntilUnloadIsSafe() {
            if (isUnloading) throw new InvalidOperationException("PluginManager is already unloading.");
            isUnloading = true;
            unloadCount = ApiExecutors.Count + AppExecutors.Count;
            ApiExecutors.ForEach(x => x.SetZeroUseNotification(decreaseUnloadCount));
            AppExecutors.ForEach(x => x.SetZeroUseNotification(decreaseUnloadCount));
            unloadSignal.WaitOne();
        }

        #endregion

        public static Func<T> CreateConstructorDelegate<T>(Type ty) {
            var destTy = typeof(T);
            if (!destTy.IsAssignableFrom(ty)) throw new ArgumentException(string.Format("Type {0} is not assignable from {1}.", destTy.FullName, ty.FullName));
            var con = ty.GetConstructor(Type.EmptyTypes);
            if (con == null) throw new ArgumentException(string.Format("Type {0} doesn't have an accessible parameterless constructor.", ty.FullName));

            var rand = Guid.NewGuid().ToString().Replace("-", "");
            var dm = new DynamicMethod("CREATE_" + ty.FullName.Replace('.', '_') + rand, ty, null, true);
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Newobj, ty.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Ret);

            return (Func<T>)dm.CreateDelegate(typeof(Func<T>));
        }

        protected void EnsureFreswitchManagedDllReference(Assembly asm)
        {
            var ourName = Assembly.GetExecutingAssembly().GetName().Name;
            if (!asm.GetReferencedAssemblies().Any(n => n.Name == ourName))
            {
                throw new
                    ModuleDoesNotReferenceFreeswitchManagedDllException("Assembly {0} doesn't reference FreeSWITCH.Managed, not loading.");
            }
        }
    }

}
