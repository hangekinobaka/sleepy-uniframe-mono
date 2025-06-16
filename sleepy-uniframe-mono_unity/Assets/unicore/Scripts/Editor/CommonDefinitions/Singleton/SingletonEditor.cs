#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy.Uniframe
{
    /// <summary>
    /// 为Singleton类定制的编辑器。<br/>
    /// Custom editor for the Singleton class.
    /// </summary>
    [CustomEditor(typeof(Singleton<>), true)]
    public class SingletonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            // 创建一个大号字体的样式，并设置其文本对齐方式为居中，同时允许文字自动换行，并设置适当的内边距
            // Creates a large font style with centered text alignment, enables word wrapping, and sets appropriate padding
            GUIStyle bigLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                alignment = TextAnchor.UpperCenter,
                wordWrap = true, // 允许文字自动换行 / Enables word wrapping
                padding = new RectOffset(10, 10, 10, 10) // 增加一些内边距 / Adds some padding
            };

            // 确保我们有足够的空间来显示整个标签
            // Ensures we have enough space to display the entire label
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Singleton", bigLabelStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 绘制默认的inspector内容
            // Draws the default inspector content
            base.OnInspectorGUI();
        }
    }
}

#endif