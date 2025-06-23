#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Sleepy.Routing
{
    [CustomEditor(typeof(Page), true)]
    public class PageEditor : Editor
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
                padding = new RectOffset(10, 10, 10, 10), // 增加一些内边距 / Adds some padding
                normal = { textColor = new Color(0.4f, 0.6f, 0.9f) } // 设置文字颜色 / Set the text color 
            };

            // 确保我们有足够的空间来显示整个标签
            // Ensures we have enough space to display the entire label
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Page", bigLabelStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 绘制默认的inspector内容
            // Draws the default inspector content
            base.OnInspectorGUI();
        }
    }
}

#endif