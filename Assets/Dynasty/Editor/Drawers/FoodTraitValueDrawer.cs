using System;
using Dynasty.Food;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoodTraitValue))]
public class FoodTraitValueDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var database = FoodTraitDatabase.Singleton;
        var selectionProperty = property.Find("_selection");
        var selectedHashProperty = selectionProperty.Find("_hash");
        
        var rect = position;
        rect.height = EditorGUI.GetPropertyHeight(selectionProperty);
        EditorGUI.PropertyField(rect, selectionProperty, new GUIContent("Trait"), true);
        
        var isValid = database.TryGetEntry(selectedHashProperty.intValue, out var selectedEntry);
        if (isValid && selectedEntry.Type != FoodTraitType.Tag) {
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += rect.height + 3;

            switch (selectedEntry.Type) {
                case FoodTraitType.Int:
                    EditorGUI.PropertyField(rect, property.Find("_intValue"), new GUIContent("Value")); break;
                case FoodTraitType.Float:
                    EditorGUI.PropertyField(rect, property.Find("_floatValue"), new GUIContent("Value")); break;
                case FoodTraitType.Bool:
                    EditorGUI.PropertyField(rect, property.Find("_boolValue"), new GUIContent("Value")); break;
                case FoodTraitType.Modifier:
                    EditorGUI.PropertyField(rect, property.Find("_modifierValue"), new GUIContent("Value"), true); break;
                case FoodTraitType.Tag:
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var database = FoodTraitDatabase.Singleton;
        var selectionProperty = property.Find("_selection");
        var selectedHashProperty = selectionProperty.Find("_hash");
        
        var height = EditorGUI.GetPropertyHeight(selectionProperty);

        var isValid = database.TryGetEntry(selectedHashProperty.intValue, out var selectedEntry);
        if (!isValid) return height;
        
        if (selectedEntry.Type == FoodTraitType.Modifier) {
            height += EditorGUI.GetPropertyHeight(property.Find("_modifierValue")) + 3;
        } else if (selectedEntry.Type != FoodTraitType.Tag) {
            height += EditorGUIUtility.singleLineHeight + 3;
        }

        return height;
    }
}