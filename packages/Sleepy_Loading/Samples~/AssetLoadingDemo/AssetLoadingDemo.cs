using Sleepy.Loading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sleepy.Demo.Loading
{
    /**
     * 这个测试脚本用于测试Addressable的读取功能，我们的项目中使用了一些Addressable的功能，
     * 测试这个功能时请根据README所给指示把对应的Asset加入Addressable后再开始测试。
     * This test script is used to test the reading functionality of Addressable. Our project uses some Addressable features. 
     * When testing this feature, please follow the instructions given in the README to add the corresponding Asset to Addressable before starting the test.
     */

    internal class AssetLoadingDemo : MonoBehaviour
    {
        [SerializeField] Transform _cubeParent;
        [SerializeField] Transform _gridContainer;
        [SerializeField] Transform _redImageParent;
        [SerializeField] AssetReference _redImageRef;

        const string CUBE_KEY = "SpecialCube(Demo).prefab";
        readonly string[] MULTI_KEYS = new string[]
         {
                "Text1(Demo).prefab",
                "Text2(Demo).prefab",
                "Text3(Demo).prefab",
                "Text4(Demo).prefab"
         };

        public async void LoadCubePrefab()
        {
            GameObject cubePrefab = await AssetLoader.LoadAddressableAsset<GameObject>(CUBE_KEY);
            Instantiate(cubePrefab, _cubeParent);
        }

        /// <summary>
        /// We deal with the handles by ourselves
        /// </summary>
        public async void LoadMultiplePrefabs()
        {

            if (await AssetLoader.WaitForMultipleAddressableAssetHandles(AssetLoader.CreateMultipleAddressableAssetHandle<GameObject>(MULTI_KEYS)))
            {
                foreach (var key in MULTI_KEYS)
                {
                    Instantiate(AssetLoader.GetAddressableAsset<GameObject>(key), _gridContainer);
                }
            }
        }

        public async void LoadPrefabWithReference()
        {
            GameObject redImagePrefab = await AssetLoader.LoadAddressableAsset<GameObject>(_redImageRef);
            Instantiate(redImagePrefab, _redImageParent);
        }

        private void OnDestroy()
        {
            // Maybe release here or not
            Dev.Log("Release resources");
            AssetLoader.ReleaseMultipleAddressableAsset(MULTI_KEYS);
            AssetLoader.ReleaseAddressableHandle(CUBE_KEY);
            AssetLoader.ReleaseAddressableHandle(_redImageRef);
        }

    }
}