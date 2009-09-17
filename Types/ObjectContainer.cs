using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;
using System.Text;

namespace FreeSWITCH
{
    public delegate object Creator(ObjectContainer container);
    public class ObjectContainer : IObjectContainer
    {
        private readonly SynchronizedDictionary<string, object> configuration =
                      new SynchronizedDictionary<string, object>();
        private readonly SynchronizedDictionary<Type, Creator> typeToCreator =
                      new SynchronizedDictionary<Type, Creator>();
        private readonly SynchronizedDictionary<Type, object> singletons =
            new SynchronizedDictionary<Type, object>();

        public SynchronizedDictionary<string, object> Configuration
        {
            get { return configuration; }
        }

        public void DeclareSingleton(Type type)
        {
            singletons[type]=this.Create(type);
        }

        public void Register<T>(Creator creator)
        {
            typeToCreator[typeof(T)]=creator;
        }

        public void RegisterSingleton<T>(Creator creator)
        {
            typeToCreator[typeof(T)]=creator;
            singletons[typeof(T)]=this.Create<T>();
        }

        public T Create<T>()
        {
            try
            {
                return (T)singletons[typeof(T)];
            }
            catch { }
            try
            {
                if (typeToCreator.Keys.Contains(typeof(T)))
                {
                    return (T)typeToCreator[typeof(T)](this);
                }
                else
                {
                    return (T)Create(typeof(T));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Type {0} not registered. Exception: {1}{2}", typeof(T), ex, this.ToString()));
            }
        }
        internal object Create(Type type)
        {
            try
            {
                return singletons[type];
            }
            catch { }
            if (typeToCreator.Keys.Contains(type))
            {
                return (typeToCreator[type](this));
            }
            ConstructorInfo constructor;
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                try
                {
                    constructor = type.GetConstructors()[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Type {0} not registered. Exception: {1}{2}", type, ex, this.ToString()));
                }
            }
            ParameterInfo[] constructorParameters = constructor.GetParameters();
            if (constructorParameters.Length == 0) return Activator.CreateInstance(type);
            List<object> parameters = new List<object>(constructorParameters.Length);
            foreach (ParameterInfo parameterInfo in constructorParameters)
            {
                Type parameterType = parameterInfo.ParameterType;
                parameters.Add(this.Create(parameterType));
            }
            return constructor.Invoke(parameters.ToArray());
        }


        public T GetConfiguration<T>(string name)
        {
            return (T)configuration[name];
        }

        public override string ToString()
        {
            StringBuilder description = new StringBuilder();
            description.AppendLine("Registered Types in Container:");
            this.typeToCreator.Keys.ForEach(key =>
            {
                description.AppendLine(string.Format("{0}:{1}", key, this.typeToCreator[key]));
            });
            description.AppendLine("Singletons in Container:");
            this.singletons.Keys.ForEach(key =>
            {
                description.AppendLine(string.Format("{0}:{1}", key, this.singletons[key]));
            });
            description.AppendLine("Configuration objects in Container:");
            this.configuration.Keys.ForEach(key =>
            {
                description.AppendLine(string.Format("{0}:{1}", key, this.configuration[key]));
            });
            description.AppendLine(string.Empty);
            return description.ToString();
        }

        public void Reset()
        {
            this.configuration.Clear();
            this.singletons.Clear();
            this.typeToCreator.Clear();
        }
    }
}
