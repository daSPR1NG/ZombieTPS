using UnityEngine;
using UnityEditor;

namespace Khynan_Coding
{
    [CustomPropertyDrawer(typeof(Stat))]
    public class StatPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _attribute, _needsToMatchBaseValueAtStart, 
            _baseValue, _maxValue, _currentValue, _minLimit, _maxLimit, _criticalThresholdValue;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            #region Properties Fetch
            // Fill properties
            _attribute = property.FindPropertyRelative("_attribute");

            _needsToMatchBaseValueAtStart = property.FindPropertyRelative("_needsToMatchBaseValueAtStart");

            _baseValue = property.FindPropertyRelative("_baseValue");
            _maxValue = property.FindPropertyRelative("_maxValue");
            _currentValue = property.FindPropertyRelative("_currentValue");

            _minLimit = property.FindPropertyRelative("_minLimit");
            _maxLimit = property.FindPropertyRelative("_maxLimit");

            _criticalThresholdValue = property.FindPropertyRelative("_criticalThresholdValue");
            #endregion

            Rect foldOutBox = new (position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldOutBox, property.isExpanded, label);

            int indentLevel = EditorGUI.indentLevel;

            if (property.isExpanded)
            {
                DrawAttributeProperty(_attribute, position);
                DrawMatchValueAtStartProperty(_needsToMatchBaseValueAtStart, position);

                DrawBaseValueProperty(_baseValue, position);
                DrawMaxValueProperty(_maxValue, position);
                DrawCurrentValueProperty(_currentValue, position);

                DrawMinLimitValueProperty(_minLimit, position);
                DrawMaxLimitValueProperty(_maxLimit, position);

                DrawCriticalThresholdValueProperty(_criticalThresholdValue, position);
            }

            EditorGUI.indentLevel = indentLevel;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return base.GetPropertyHeight(property, label) * 6.25f;
            }

            return EditorGUIUtility.singleLineHeight * 1;
        }

        private void DrawAttributeProperty(SerializedProperty property, Rect position)
        {
            GUIContent content = new GUIContent(
                "Type",
                "This value represents the stat identity, we use this to find and manipulate the stat of this attribute type.");

            SetLabelWidth(content.text, 5);

            float widthOffset = 6;

            float xPos = position.x + widthOffset;
            float yPos = position.y + YOffset(1, 3);
            float width = position.width * .5f - widthOffset;
            float height = EditorGUIUtility.singleLineHeight;

            Rect drawArea = new(xPos, yPos, width, height);
            EditorGUI.PropertyField(drawArea, property, content);
        }

        private void DrawMatchValueAtStartProperty(SerializedProperty property, Rect position)
        {
            GUIContent content = new GUIContent(
                "Match Value", 
                "Forces current and max values to be equal to this value.");

            SetLabelWidth(content.text, 5);

            float widthOffset = 6;

            float xPos = position.x + position.width * .5f + widthOffset;
            float yPos = position.y + YOffset(1, 3);
            float width = position.width - widthOffset;
            float height = EditorGUIUtility.singleLineHeight;

            Rect drawArea = new(xPos, yPos, width, height);
            EditorGUI.PropertyField(drawArea, property, content);
        }

        #region Draw Base, Current, Max Values
        private void DrawBaseValueProperty(SerializedProperty property, Rect position)
        {
            GUIContent content = new GUIContent(
                "Base Value",
                "The value set & used at start." + '\n' +
                "If -match value at start- is true, current and max values are equal to this value.");

            SetLabelWidth(content.text, 5);

            float widthOffset = 6;

            float xPos = position.min.x + widthOffset;
            float yPos = position.min.y + YOffset(2, 7);
            float width = position.size.x * .35f - widthOffset;
            float height = EditorGUIUtility.singleLineHeight;

            Rect drawArea = new(xPos, yPos, width, height);
            EditorGUI.PropertyField(drawArea, property, content);
        }

        private void DrawMaxValueProperty(SerializedProperty property, Rect position)
        {
            GUIContent content = new GUIContent(
                "Max Value", 
                "The current value cannot exceed this value.");

            SetLabelWidth(content.text, 5);

            float widthOffset = 6;

            float xPos = position.min.x + position.width * .35f + widthOffset;
            float yPos = position.min.y + YOffset(2, 7);
            float width = position.size.x * .3f - widthOffset;
            float height = EditorGUIUtility.singleLineHeight;

            Rect drawArea = new(xPos, yPos, width, height);
            EditorGUI.PropertyField(drawArea, property, content);
        }

        private void DrawCurrentValueProperty(SerializedProperty property, Rect position)
        {
            GUIContent content = new GUIContent(
                "Current Value", 
                "The actual value of this attribute.");

            SetLabelWidth(content.text, 5);

            float widthOffset = 6;

            float xPos = position.min.x + position.width * .65f + widthOffset;
            float yPos = position.min.y + YOffset(2, 7);
            float width = position.size.x * .35f - widthOffset;
            float height = EditorGUIUtility.singleLineHeight;

            Rect drawArea = new(xPos, yPos, width, height);
            EditorGUI.PropertyField(drawArea, property, content);
        }
        #endregion

        #region Draw Min & Max Limits
        private void DrawMinLimitValueProperty(SerializedProperty property, Rect position)
        {
            GUIContent content = new GUIContent(
                "Min Limit Value", 
                "Current & max values cannot be lower than this value.");

            SetLabelWidth(content.text, 5);

            float widthOffset = 6;

            float xPos = position.min.x + widthOffset;
            float yPos = position.min.y + YOffset(3, 11);
            float width = position.size.x * .5f - widthOffset;
            float height = EditorGUIUtility.singleLineHeight;

            Rect drawArea = new(xPos, yPos, width, height);
            EditorGUI.PropertyField(drawArea, property, content);
        }

        private void DrawMaxLimitValueProperty(SerializedProperty property, Rect position)
        {
            GUIContent content = new GUIContent(
                "Max Limit Value",
                "Current & max values cannot be greater than this value.");

            SetLabelWidth(content.text, 5);

            float widthOffset = 6;

            float xPos = position.min.x + position.width * .5f + widthOffset;
            float yPos = position.min.y + YOffset(3, 11);
            float width = position.size.x * .5f - widthOffset;
            float height = EditorGUIUtility.singleLineHeight;

            Rect drawArea = new(xPos, yPos, width, height);
            EditorGUI.PropertyField(drawArea, property, content);
        }
        #endregion

        private void DrawCriticalThresholdValueProperty(SerializedProperty property, Rect position)
        {
            GUIContent content = new GUIContent(
                "Critical Threshold Value",
                "We use this to call action(s) when the stat reaches this percentage value.");

            SetLabelWidth(content.text, 7);

            float widthOffset = 6;

            float xPos = position.min.x + widthOffset + position.width * .2f;
            float yPos = position.min.y + YOffset(4, 15);
            float width = position.size.x + widthOffset - position.width * .5f;
            float height = EditorGUIUtility.singleLineHeight;

            Rect drawArea = new(xPos, yPos, width, height);
            EditorGUI.PropertyField(drawArea, property, content);
        }

        private float YOffset(float step, float offset)
        {
            return (EditorGUIUtility.singleLineHeight * step) + offset;
        }

        private void SetLabelWidth(string text, float offset)
        {
            GUIContent content = new(text);

            GUIStyle style = new(GUIStyle.none);
            Vector2 size = style.CalcSize(content);

            EditorGUIUtility.labelWidth = size.x + offset;
        }
    }
}