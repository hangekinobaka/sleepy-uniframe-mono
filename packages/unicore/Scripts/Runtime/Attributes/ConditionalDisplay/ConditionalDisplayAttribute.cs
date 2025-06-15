using System;
using UnityEngine;

namespace Sleepy
{
    /// <summary>
    /// Display the field only if a certain condition is met
    /// <remarks>
    /// Example:   
    /// [ConditionalDisplay("_hasLimit", true)]
    /// [ConditionalDisplay("_myEnumProperty", MyEnum.SomeValue)]
    /// [ConditionalDisplay("_transitionType", new[] { TransitionType.SlideIn, TransitionType.Slide })]
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ConditionalDisplayAttribute : PropertyAttribute
    {
        public string CondPropertyName { get; private set; }
        public object CondValue { get; private set; }
        public ConditionalDisplayAttribute(string condPropertyName, object condValue)
        {
            CondPropertyName = condPropertyName;
            CondValue = condValue;
        }
    }
}