using UnityEngine;

namespace Sleepy
{
    /// <summary>
    /// 定义了单例模式行为的通用基类。<br/>
    /// Defines a generic base class for singleton pattern behavior.
    /// </summary>
    /// <typeparam name="T">单例类型。<br/>Singleton type.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // 定义单例实例 / Defines the singleton instance
        private static T _instance;
        // 用于锁定的对象，以确保线程安全 / Object used for locking to ensure thread safety
        private static object _lock = new object();
        // 标记应用是否正在退出，避免在应用退出时创建实例 / Flag to mark if the application is quitting to avoid creating instances on application quit
        private static bool _applicationIsQuitting = false;

        // 通过序列化字段来设置是否不在加载新场景时销毁对象 / Serialize field to set whether or not to destroy the object when loading a new scene
        [SerializeField] bool _dontDestroyOnLoad = false;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                // 如果尝试创建第二个单例实例，则警告并销毁对象 / Warn and destroy the object if attempting to create a second instance of a singleton
                Dev.Warning("[Singleton] Trying to instantiate a second instance of a singleton class.");
                Destroy(gameObject);
            }

            if (_dontDestroyOnLoad) // 仅适用于场景中的实例 / This is for the instances in the scene only
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    // 如果应用正在退出，返回null / Return null if the application is quitting
                    Dev.Warning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            // 如果存在多于一个的单例实例，输出错误 / Output an error if there are more than one singleton instances
                            Dev.Error("[Singleton] There should never be more than 1 singleton! ");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            // 如果未找到实例，则创建一个新的GameObject并添加单例组件 / Create a new GameObject and add the singleton component if an instance is not found
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = typeof(T).ToString() + " (Singleton)";

                            DontDestroyOnLoad(singleton);

                            Dev.Log("[Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                    }

                    return _instance;
                }
            }
        }

        protected void OnDestroy()
        {
            _applicationIsQuitting = true;
        }

        public static T CreateInstance()
        {
            return Instance;
        }
    }
}
