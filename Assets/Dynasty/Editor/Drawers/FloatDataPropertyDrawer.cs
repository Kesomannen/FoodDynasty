using Dynasty.Machines;
using UnityEditor;
using UnityEngine;

namespace Dynasty.Editor.Drawers {

//[CustomPropertyDrawer(typeof(FloatDataProperty))]
public class FloatDataPropertyDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        EditorGUI.PropertyField(position, property.FindPropertyRelative("_dataObject"), label);
        
        EditorGUI.EndProperty();
    }
}

}