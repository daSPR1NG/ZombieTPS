using UnityEngine;
using UnityEditor;

namespace Khynan_Coding
{
    [CustomPropertyDrawer(typeof(MaxAttribute))]
    public class MaxAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var maxAttribute = (MaxAttribute)attribute;
            var propertyType = property.propertyType;

            if (propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.BeginChangeCheck();

                float inputValue = EditorGUI.FloatField(position, label, property.floatValue);

                if (inputValue > maxAttribute.MaxValue) { inputValue = maxAttribute.MaxValue; }

                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = inputValue;
                }

                EditorGUI.EndChangeCheck();
            }
        }
    }
}