using UnityEngine.ResourceManagement.AsyncOperations;

namespace Sleepy.Loading
{
    internal class AddressableSceneLoadObject
    {
        public string SceneName;
        public bool IsCanceled = false;
        public AsyncOperationHandle Handle;
    }
}