#if UNITY_EDITOR       

using System;
using UnityEditor;
using UnityEngine;

namespace Sleepy
{
    /// <summary>
    /// 用于条件性显示属性的自定义属性绘制器。<br/>
    /// Custom property drawer for conditionally displaying properties.
    /// </summary>
    [CustomPropertyDrawer(typeof(ConditionalDisplayAttribute))]
    public class ConditionalDisplayAttributeEditor : PropertyDrawer
    {
        /// <summary>
        /// 获取属性的高度，如果条件满足，则为属性本身的高度，否则为0。<br/>
        /// Gets the property height, which is either the property's own height if the condition is met, or 0.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsConditionMet(property))
            {
                return EditorGUI.GetPropertyHeight(property);
            }
            return 0;
        }

        /// <summary>
        /// 在GUI中绘制属性，如果条件满足。<br/>
        /// Draws the property in the GUI if the condition is met.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsConditionMet(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        // 检查属性显示的条件是否满足 / Checks if the condition to display the property is met
        private bool IsConditionMet(SerializedProperty property)
        {
            ConditionalDisplayAttribute condAttr = attribute as ConditionalDisplayAttribute;
            string pathToCondProperty = GetPropertyPath(property, condAttr.CondPropertyName);
            SerializedProperty condProperty = property.serializedObject.FindProperty(pathToCondProperty);

            if (condProperty == null)
            {
                Debug.LogWarning($"ConditionalDisplayAttribute: Could not find the property {condAttr.CondPropertyName} at path {pathToCondProperty}");
                return false;
            }

            return CheckCondition(condProperty, condAttr.CondValue);
        }

        /// <summary>
        /// 通过找到属性路径中最后一个"."，然后在这个位置之前的路径上附加新的属性名来构建新的属性路径。<br/>
        /// Constructs a new property path by finding the last "." in the property path, then appending the new property name to the path before this point.
        /// </summary>
        /// <param name="property">要处理的序列化属性。<br/>The serialized property to process.</param>
        /// <param name="propertyName">要附加的新属性名。<br/>The new property name to append.</param>
        /// <returns>构建的新属性路径。<br/>The constructed new property path.</returns>
        private string GetPropertyPath(SerializedProperty property, string propertyName)
        {
            string propertyPath = property.propertyPath;
            int lastDot = propertyPath.LastIndexOf('.');
            if (lastDot == -1)
            {
                return propertyName;
            }

            string pathWithoutProperty = propertyPath.Substring(0, lastDot);
            return $"{pathWithoutProperty}.{propertyName}";
        }

        // 根据属性类型检查条件是否满足 / Checks if the condition is met based on the property type
        private bool CheckCondition(SerializedProperty condProperty, object condValue)
        {
            switch (condProperty.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return condProperty.boolValue.Equals(condValue);
                case SerializedPropertyType.Enum:
                    // 支持单一枚举值或枚举值数组
                    if (condValue is Array condValueArray)
                    {
                        foreach (var val in condValueArray)
                        {
                            if (condProperty.enumValueIndex.Equals(Convert.ToInt32(val)))
                                return true;
                        }
                        return false;
                    }
                    else
                    {
                        return condProperty.enumValueIndex.Equals(Convert.ToInt32(condValue));
                    }
                case SerializedPropertyType.Integer:
                    return condProperty.intValue.Equals(Convert.ToInt32(condValue));
                case SerializedPropertyType.String:
                    return condProperty.stringValue.Equals(condValue.ToString());
                // 在这里添加其他类型的检查 / Add checks for other types here
                default:
                    Dev.Warning($"Unsupported property type {condProperty.propertyType} in ConditionalDisplayAttribute.");
                    return false; // 不支持的类型默认返回 false / Unsupported types default to false
            }
        }
    }
}

#endif
