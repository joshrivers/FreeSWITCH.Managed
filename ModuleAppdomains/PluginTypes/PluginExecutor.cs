using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public abstract class PluginExecutor : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }
        private ILogger logger;

        /// <summary>Names by which this plugin may be executed.</summary>
        public List<string> Aliases { get { return aliases; } }
        readonly List<string> aliases = new List<string>();

        /// <summary>The canonical name to identify this plugin (informative).</summary>
        public string Name { get { return name; } }
        readonly string name;

        public PluginOptions PluginOptions { get { return pluginOptions; } }
        readonly PluginOptions pluginOptions;

        protected PluginExecutor(string name, List<string> aliases, PluginOptions pluginOptions)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("No name provided.");
            if (aliases == null || aliases.Count == 0) throw new ArgumentException("No aliases provided.");
            this.name = name;
            this.aliases = aliases.Distinct().ToList();
            this.pluginOptions = pluginOptions;
            // Hardwired because the container doesn't generate PluginExectors. May be a good place for a later refactoring.
            this.logger = InternalAppdomainServiceLocator.Container.Create<ILogger>();
        }

        int useCount = 0;
        protected void IncreaseUse()
        {
            System.Threading.Interlocked.Increment(ref useCount);
        }
        protected void DecreaseUse()
        {
            var count = System.Threading.Interlocked.Decrement(ref useCount);
            if (count == 0 && onZeroUse != null)
            {
                onZeroUse();
            }
        }

        Action onZeroUse;
        public void SetZeroUseNotification(Action onZeroUse)
        {
            this.onZeroUse = onZeroUse;
            if (useCount == 0) onZeroUse();
        }

        protected void LogException(string action, string moduleName, Exception ex)
        {
            this.logger.Error(string.Format("{0} exception in {1}: {2}", action, moduleName, ex.Message));
            this.logger.Debug(string.Format("{0} exception: {1}", moduleName, ex.ToString()));
        }
    }
}
