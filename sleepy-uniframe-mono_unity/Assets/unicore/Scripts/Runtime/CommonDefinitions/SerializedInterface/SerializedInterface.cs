using UnityEngine;

namespace Sleepy
{
    /// <summary>
    /// 用来实现Inspector拖拽Interface的功能。<br/>
    /// Used to implement the functionality of dragging interfaces in the Inspector.
    /// </summary>
    /// <typeparam name="T">需要实现的接口类型。/ The type of the interface that must be implemented.</typeparam>
    [System.Serializable]
    public class SerializedInterface<T> where T : class
    {
        [SerializeField] MonoBehaviour _object;

        public T Instance
        {
            get
            {
                if (_object is T interfaceInstance)
                {
                    return interfaceInstance;
                }
                else
                {
                    Dev.Error($"{_object.name} does not implement {typeof(T).Name} interface.");
                    return null;
                }
            }
        }
    }
}