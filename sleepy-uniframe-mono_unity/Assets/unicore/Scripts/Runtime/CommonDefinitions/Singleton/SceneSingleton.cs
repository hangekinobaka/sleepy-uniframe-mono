using System;
using UnityEngine;

namespace Sleepy
{
    /// <summary>
    /// Singleton Class
    /// </summary>
    /// <typeparam name="T">Class to be extended from MonoBehaviour</typeparam>
    public abstract class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {

            get
            {

                if (_instance == null)
                {
                    Type t = typeof(T);

                    _instance = (T)FindObjectOfType(t);
                    if (_instance == null)
                    {
                        Dev.Error(t + ":Null");
                    }
                }

                return _instance;
            }

        }

        virtual protected void Awake()
        {
            CheckInstance();
        }

        protected bool CheckInstance()
        {
            if (_instance == null)
            {
                _instance = this as T;
                return true;
            }
            else if (Instance == this)
            {
                return true;
            }

            Destroy(this);
            return false;
        }
        public static bool IsNull()
        {
            return _instance == null;
        }
    }
}