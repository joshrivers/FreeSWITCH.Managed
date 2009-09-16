using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class DefaultPluginHandler : IPluginHandler
    {
        private PluginHandlerOrchestrator orchestrator;
        public DefaultPluginHandler(PluginHandlerOrchestrator orchestrator)
        {
            this.orchestrator = orchestrator;
        }
        public List<ApiPluginExecutor> ApiExecutors { get { return _apiExecutors; } }
        readonly List<ApiPluginExecutor> _apiExecutors = new List<ApiPluginExecutor>();

        public List<AppPluginExecutor> AppExecutors { get { return _appExecutors; } }
        readonly List<AppPluginExecutor> _appExecutors = new List<AppPluginExecutor>();

        public bool Execute(string command, IntPtr streamHandle, IntPtr eventHandle)
        {
            try
            {
                System.Diagnostics.Debug.Assert(streamHandle != IntPtr.Zero, "streamHandle is null.");
                var parsed = command.FreeSWITCHCommandParse();
                if (parsed == null) return false;
                var fullName = parsed[0];
                var args = parsed[1];
                ApiPluginExecutor exec = ApiExecutors.Where(app => app.Aliases.Contains(fullName)).FirstOrDefault();
                if (exec == null) { return false; }

                //var execs = moduleListContainer.ModuleList.apiExecs.ToDictionary();
                //ApiPluginExecutor exec;
                //if (!execs.TryGetValue(fullName, out exec))
                //{
                //    Log.WriteLine(LogLevel.Error, "API plugin {0} not found.", fullName);
                //    return false;
                //}
                var res = exec.ExecuteApi(args, streamHandle, eventHandle);
                return res;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Exception in Execute({0}): {1}", command, ex.ToString());
                return false;
            }
        }

        public bool ExecuteBackground(string command)
        {
            try
            {
                var parsed = command.FreeSWITCHCommandParse();
                if (parsed == null) return false;
                var fullName = parsed[0];
                var args = parsed[1];
                ApiPluginExecutor exec = ApiExecutors.Where(app => app.Aliases.Contains(fullName)).FirstOrDefault();
                if (exec == null) { return false; }
                //var execs = moduleListContainer.ModuleList.apiExecs.ToDictionary();
                //ApiPluginExecutor exec;
                //if (!execs.TryGetValue(fullName, out exec))
                //{
                //    Log.WriteLine(LogLevel.Error, "API plugin {0} not found.", fullName);
                //    return false;
                //}
                return exec.ExecuteApiBackground(args);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Exception in ExecuteBackground({0}): {1}", command, ex.ToString());
                return false;
            }
        }

        public void LoadPlugins(Assembly assembly)
        {
            var allTypes = assembly.GetExportedTypes();
            var opts = GetOptions(allTypes);
            if ((opts & PluginOptions.NoAutoReload) == PluginOptions.NoAutoReload)
            {
                orchestrator.NoReload = true;
            }
            AddApiPlugins(allTypes, opts);
            AddAppPlugins(allTypes, opts);

            // Add the script executors
            var name = Path.GetFileName(orchestrator.FileName);
            var aliases = new List<string> { name };
            this.ApiExecutors.Add(new ApiPluginExecutor(name, aliases, () => new ScriptApiWrapper(assembly.EntryPoint.GetEntryDelegate()), opts));
            this.AppExecutors.Add(new AppPluginExecutor(name, aliases, () => new ScriptAppWrapper(assembly.EntryPoint.GetEntryDelegate()), opts));
        }


        protected PluginOptions GetOptions(Type[] allTypes)
        {
            var ty = typeof(IPluginOptionsProvider);
            var pluginTypes = allTypes.Where(x => ty.IsAssignableFrom(x) && !x.IsAbstract).ToList();
            return pluginTypes.Aggregate(PluginOptions.None, (opts, t) =>
            {
                var x = ((IPluginOptionsProvider)Activator.CreateInstance(t, true));
                return opts | x.GetOptions();
            });
        }

        protected void AddApiPlugins(Type[] allTypes, PluginOptions pluginOptions)
        {
            var iApiTy = typeof(IApiPlugin);
            foreach (var ty in allTypes.Where(x => iApiTy.IsAssignableFrom(x) && !x.IsAbstract))
            {
                var del = CreateConstructorDelegate<IApiPlugin>(ty);
                var exec = new ApiPluginExecutor(ty.FullName, new List<string> { ty.FullName, ty.Name }, del, pluginOptions);
                this.ApiExecutors.Add(exec);
            }
        }

        protected void AddAppPlugins(Type[] allTypes, PluginOptions pluginOptions)
        {
            var iAppTy = typeof(IAppPlugin);
            foreach (var ty in allTypes.Where(x => iAppTy.IsAssignableFrom(x) && !x.IsAbstract))
            {
                var del = CreateConstructorDelegate<IAppPlugin>(ty);
                var exec = new AppPluginExecutor(ty.FullName, new List<string> { ty.FullName, ty.Name }, del, pluginOptions);
                this.AppExecutors.Add(exec);
            }
        }

        public static Func<T> CreateConstructorDelegate<T>(Type ty)
        {
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
        public bool Run(string command, IntPtr sessionHandle)
        {
            try
            {
                Log.WriteLine(LogLevel.Debug, "FreeSWITCH.Managed: attempting to run application '{0}'.", command);
                System.Diagnostics.Debug.Assert(sessionHandle != IntPtr.Zero, "sessionHandle is null.");
                var parsed = command.FreeSWITCHCommandParse();
                if (parsed == null) return false;
                var fullName = parsed[0];
                var args = parsed[1];

                AppPluginExecutor exec = AppExecutors.Where(app => app.Aliases.Contains(fullName)).FirstOrDefault();
                //var execs = moduleListContainer.ModuleList.appExecs.ToDictionary();
                //if (!execs.TryGetValue(fullName, out exec))
                //{
                //    Log.WriteLine(LogLevel.Error, "App plugin {0} not found.", fullName);
                //    return false;
                //}
                if (exec == null) { return false; }
                return exec.Execute(args, sessionHandle);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Exception in Run({0}): {1}", command, ex.ToString());
                return false;
            }
        }
        public void Unload()
        {
            this.BlockUntilUnloadIsSafe();
        }

        bool isUnloading = false;
        int unloadCount;
        System.Threading.ManualResetEvent unloadSignal = new System.Threading.ManualResetEvent(false);
        void decreaseUnloadCount()
        {
            if (System.Threading.Interlocked.Decrement(ref unloadCount) == 0)
            {
                unloadSignal.Set();
            }
        }

        public void BlockUntilUnloadIsSafe()
        {
            if (isUnloading) throw new InvalidOperationException("PluginManager is already unloading.");
            isUnloading = true;
            unloadCount = ApiExecutors.Count + AppExecutors.Count;
            ApiExecutors.ForEach(x => x.SetZeroUseNotification(decreaseUnloadCount));
            AppExecutors.ForEach(x => x.SetZeroUseNotification(decreaseUnloadCount));
            unloadSignal.WaitOne();
        }

    }
}
