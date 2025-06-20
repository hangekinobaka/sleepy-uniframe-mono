#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Sleepy.Uniframe
{
    [CustomPropertyDrawer(typeof(SerializedInterface<>))]
    public class SerializedInterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 获取 _object 字段 / Retrieve the _object field
            SerializedProperty objectProperty = property.FindPropertyRelative("_object");

            // 只绘制 _object 字段而不是整个类 / Only draw the _object field instead of the entire class
            EditorGUI.PropertyField(position, objectProperty, label);

            // 获取当前对象 / Get the current object
            MonoBehaviour targetObject = objectProperty.objectReferenceValue as MonoBehaviour;

            // 获取泛型 T 的类型 / Get the type of the generic parameter T
            Type targetType = fieldInfo.FieldType.GetGenericArguments()[0];

            // 如果有对象被拖入，检查是否实现了 T 接口
            // If an object is dragged into the field, check if it implements the interface T
            if (targetObject != null)
            {
                if (!targetType.IsAssignableFrom(targetObject.GetType()))
                {
                    // 对象没有实现接口，输出错误信息并将对象设置为 null
                    // If the object does not implement the interface, log an error and set the object to null
                    Dev.Error($"{targetObject.name} does not implement {targetType.Name} interface.");
                    objectProperty.objectReferenceValue = null; // 阻止赋值 / Prevent the assignment
                }
            }

            // 应用修改 / Apply the changes
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif