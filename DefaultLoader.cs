using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed
{
    public interface IDefaultServiceLocator
    {
        LogDirector LogDirector { get; }
        RunCommandDirector RunCommandDirector { get; }
        ExecuteCommandDirector ExecuteCommandDirector { get; }
        ExecuteBackgroundCommandDirector ExecuteBackgroundCommandDirector { get; }
        ReloadCommandDirector ReloadCommandDirector { get; }
        DefaultPluginOverseer PluginOverSeer { get; }
    }

    public class DefaultLoader : IDefaultServiceLocator
    {
        // Thread-safe singleton implementation from: http://www.yoda.arachsys.com/csharp/singleton.html
        private static readonly DefaultLoader loader = new DefaultLoader();

        static DefaultLoader() { }

        public static DefaultLoader Loader
        {
            get { return loader; }
        }

        public LogDirector LogDirector { get; private set; }
        public RunCommandDirector RunCommandDirector { get; private set; }
        public ExecuteCommandDirector ExecuteCommandDirector { get; private set; }
        public ExecuteBackgroundCommandDirector ExecuteBackgroundCommandDirector { get; private set; }
        public ReloadCommandDirector ReloadCommandDirector { get; private set; }
        public DefaultPluginOverseer PluginOverSeer { get; private set; }

        private DefaultLoader()
        {
            this.LogDirector = new LogDirector();
            this.RunCommandDirector = new RunCommandDirector(LogDirector);
            this.ExecuteCommandDirector = new ExecuteCommandDirector(LogDirector);
            this.ExecuteBackgroundCommandDirector = new ExecuteBackgroundCommandDirector(LogDirector);
            this.ReloadCommandDirector = new ReloadCommandDirector(LogDirector);
            DefaultDirectoryController dc = new DefaultDirectoryController();
            this.PluginOverSeer = new DefaultPluginOverseer(this, dc, this.LogDirector);
        }

        public bool Load()
        {
            CoreDelegates.Run = this.RunCommandDirector.Run;
            CoreDelegates.Execute = this.ExecuteCommandDirector.Execute;
            CoreDelegates.ExecuteBackground = this.ExecuteBackgroundCommandDirector.ExecuteBackground;
            CoreDelegates.Reload = this.ReloadCommandDirector.Reload;
            return this.PluginOverSeer.Load();
        }
    }
}
