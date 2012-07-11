using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiningGame.Code.Managers
{
    public static class ReflectionManager
    {
        public static Assembly Assembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public static Type[] GetAllSubClassesOf<T>(bool includeBaseClass = false)
        {
            List<Type> ret = new List<Type>();
            foreach (Type t in Assembly.GetTypes())
            {
                if (t.IsSubclassOf(typeof(T)) || (t == typeof(T) && includeBaseClass))
                {
                    ret.Add(t);
                }
            }
            return ret.ToArray();
        }

        public static T CallConstructor<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public static object CallConstructor(Type T)
        {
            return Activator.CreateInstance(T);
        }
    }
}
