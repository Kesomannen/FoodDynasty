using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Optional<>))]
public class OptionalDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var valueProperty = property.FindPropertyRelative("_value");
        return EditorGUI.GetPropertyHeight(valueProperty);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var enabledProperty = property.FindPropertyRelative("_enabled");
        var valueProperty = property.FindPropertyRelative("_value");

        position.width -= 24;
        EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
        EditorGUI.PropertyField(position, valueProperty, label, true);
        EditorGUI.EndDisabledGroup();
        
        position.x += position.width + 24;
        position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
        position.x -= position.width;
        EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
        
        EditorGUI.EndProperty();
    }
}