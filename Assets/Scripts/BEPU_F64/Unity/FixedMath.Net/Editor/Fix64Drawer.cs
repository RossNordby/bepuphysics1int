using FixMath.NET;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Fix64FloatAttribute))]
public class Fix64FloatDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float floatValue = (float)Fix64.BuildFromRawLong(property.longValue);

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        
        float lineHeight = EditorGUIUtility.singleLineHeight;
        floatValue = EditorGUI.FloatField(new Rect(position.xMin, position.yMin, position.width, lineHeight), "Float Value", floatValue);
        EditorGUI.LongField(new Rect(position.xMin, position.yMin + lineHeight, position.width, lineHeight), "Raw Value", property.longValue);

        if (EditorGUI.EndChangeCheck())
            property.longValue = ((Fix64)floatValue).RawValue;

        EditorGUI.EndProperty();
    }
}