using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public class DefaultLoader
    {
        // Thread-safe singleton implementation from: http://www.yoda.arachsys.com/csharp/singleton.html
        static DefaultLoader() { DefaultLoader.Loader = new DefaultLoader(); }

        public static DefaultLoader Loader { get; private set; }

        public bool Load()
        {
            CoreDelegates.Run = DefaultServiceLocator.Container.Create<RunCommandDirector>().Run;
            CoreDelegates.Execute = DefaultServiceLocator.Container.Create<ExecuteCommandDirector>().Execute;
            CoreDelegates.ExecuteBackground = DefaultServiceLocator.Container.Create<ExecuteBackgroundCommandOnCollection>().ExecuteBackground;
            CoreDelegates.Reload = DefaultServiceLocator.Container.Create<ReloadCommandDirector>().Reload;
            var pluginOverSeer = DefaultServiceLocator.Container.Create<DefaultPluginOverseer>();
            return pluginOverSeer.Load();
        }
    }
}
