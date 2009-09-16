using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using FreeSWITCH.Managed;

namespace FreeSWITCH
{
    public class ServiceLocator
    {
        public delegate object Creator(ServiceLocator container);

        private readonly SynchronizedDictionary<string, object> configuration =
                      new SynchronizedDictionary<string, object>();
        private readonly SynchronizedDictionary<Type, Creator> typeToCreator =
                      new SynchronizedDictionary<Type, Creator>();
        private readonly SynchronizedDictionary<Type, object> singletons =
            new SynchronizedDictionary<Type, object>();

        public Dictionary<string, object> Configuration
        {
            get { return configuration.ToDictionary(); }
        }

        public void DeclareSingleton(Type type)
        {
            singletons.Add(type, this.Create(type));
        }

        public void Register<T>(Creator creator)
        {
            typeToCreator.Add(typeof(T), creator);
        }

        public void RegisterSingleton<T>(Creator creator)
        {
            typeToCreator.Add(typeof(T), creator);
            singletons.Add(typeof(T), this.Create<T>());
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
                var config = Environment.NewLine;
                this.Configuration.ForEach(pair => config = string.Format("{0}{1}:{2}{3}", config, pair.Key, pair.Value, Environment.NewLine));
                throw new Exception(string.Format("Type {0} not registered. Exception: {1}", typeof(T), ex));
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
                    throw new Exception(string.Format("Type {0} not registered. Exception: {1}", type, ex));
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
    }
}
