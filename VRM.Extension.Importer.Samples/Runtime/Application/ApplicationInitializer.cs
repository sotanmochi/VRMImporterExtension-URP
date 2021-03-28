using System.Collections.Generic;
using UnityEngine;

namespace VRM.Extension.Samples
{
    public static class ApplicationInitializer
    {
        /// <summary>
        /// This method is invoked before Awake method.
        /// このメソッドは、Awakeメソッドの前に呼び出されます。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeBeforeSceneLoad()
        {
            Debug.Log("<color=orange>ApplicationInitializer.InitializeBeforeSceneLoad()</color>");

            var initializables = FindObjectsOfInterface<IInitializableBeforeSceneLoad>();
            foreach (var i in initializables)
            {
                i.InitializeBeforeSceneLoad();
            }
        }

        public static T FindObjectOfInterface<T>() where T : class
        {
            foreach ( var n in GameObject.FindObjectsOfType<Component>() )
            {
                var component = n as T;
                if ( component != null )
                {
                    return component;
                }
            }
            return null;
        }

        public static List<T> FindObjectsOfInterface<T>() where T : class
        {
            List<T> list = new List<T>();
            foreach (var n in GameObject.FindObjectsOfType<Component>() )
            {
                var component = n as T;
                if (component != null )
                {
                    list.Add(component);
                }
            }
            return list;
        }
    }
}
