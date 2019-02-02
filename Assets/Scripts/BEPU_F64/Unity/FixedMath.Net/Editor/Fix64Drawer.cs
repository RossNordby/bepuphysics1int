using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Fix32))]
public class Fix32FloatDrawer : PropertyDrawer
{
	private static bool viewRaw = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		int buttonWidth = 20;

		var r = position;
		r.width = r.width - (buttonWidth + 20);
		if (viewRaw) {
			EditorGUI.PropertyField(r, property.FindPropertyRelative("RawValue"), label);
		}
		else {
			EditorGUI.BeginChangeCheck();
			float floatValue = property.FindPropertyRelative("RawValue").intValue.ToFix32().ToFloat();
			floatValue = EditorGUI.FloatField(r, label, floatValue);
			if (EditorGUI.EndChangeCheck())
				property.FindPropertyRelative("RawValue").intValue = (int) floatValue.ToFix32();
		}

		r.width = buttonWidth;
		r.x = position.width - buttonWidth;
		if (GUI.Button(r, viewRaw ? new GUIContent("R", "Currently in Raw view") : new GUIContent("F", "Currently in Float view"))) {
			viewRaw = !viewRaw;
			GUIUtility.keyboardControl = 0;
		}
    }
}