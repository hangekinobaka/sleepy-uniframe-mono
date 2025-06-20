#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Sleepy.Uniframe
{
    public static class AssetEditorUtil
    {
        /// <summary>
        /// 将目标资源转换为Addressable资源，并可指定键值。<br/>
        /// Converts the target asset to an Addressable asset and allows specifying a key.
        /// </summary>
        /// <param name="targetAsset">要转换的目标资源。<br/>The target asset to be converted.</param>
        /// <param name="groupName">目标资源将被分配到的Addressable资源组的名称，默认为"Default Local Group"。<br/>
        /// The name of the Addressable asset group to which the target asset will be assigned, default is "Default Local Group".</param>
        /// <param name="key">资源的键值，如果未提供，则使用资源名称作为键值。<br/>
        /// The key of the asset, if not provided, the asset's name will be used as the key.</param>
        public static void ConvertToAddressable(Object targetAsset, string groupName = "Default Local Group", string key = "")
        {
            // 获取Addressable Asset Settings
            // Get Addressable Asset Settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // 创建或获取Addressable Asset Group
            // Create or get Addressable Asset Group
            var group = settings.FindGroup(groupName) ?? settings.CreateGroup(groupName, false, false, true, null);

            // 将选中的资源添加到Addressable Asset Group
            // Add the selected asset to the Addressable Asset Group
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(targetAsset));
            var entry = settings.CreateOrMoveEntry(guid, group);

            // 并设置地址（Key）
            // Also set the address (Key)
            entry.address = string.IsNullOrEmpty(key) ? targetAsset.name : key;

            // 保存设置
            // Save the settings
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
            AssetDatabase.SaveAssets();

            // 记录转换操作的日志
            // Log the conversion operation
            Dev.Log($"{targetAsset.name} is converted to an addressable in group:{groupName}");
        }

    }
}
#endif
