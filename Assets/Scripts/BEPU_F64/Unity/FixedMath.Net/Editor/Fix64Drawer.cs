using FixMath.NET;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Fix64))]
public class Fix64FloatDrawer : PropertyDrawer
{
	private static bool viewRaw = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		int buttonWidth = 20;

		var r = position;
		r.width -= buttonWidth + 20;
		if (viewRaw) {
			EditorGUI.PropertyField(r, property.FindPropertyRelative("RawValue"), label);
		}
		else {
			EditorGUI.BeginChangeCheck();
			float floatValue = (float) Fix64.FromRaw(property.FindPropertyRelative("RawValue").intValue);
			floatValue = EditorGUI.FloatField(r, label, floatValue);
			if (EditorGUI.EndChangeCheck())
				property.FindPropertyRelative("RawValue").intValue = ((Fix64) floatValue).ToRaw();
		}

		r.width = buttonWidth;
		r.x = position.width - buttonWidth;
		if (GUI.Button(r, viewRaw ? new GUIContent("R", "Currently in Raw view") : new GUIContent("F", "Currently in Float view"))) {
			viewRaw = !viewRaw;
			GUIUtility.keyboardControl = 0;
		}
    }
}