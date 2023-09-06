using System;
using Dynasty.Food.Data;
using Dynasty.Food.Filtering;
using Dynasty.Food.Modification;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoodFilter))]
public class FoodFilterDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var traitProperty = property.Find("_trait");
        var traitHashProperty = traitProperty.Find("_hash");
        
        var height = EditorGUIUtility.singleLineHeight + 3;
        height += EditorGUI.GetPropertyHeight(traitProperty);
        
        var database = FoodTraitDatabase.Singleton;
        var validTrait = database.TryGetEntry(traitHashProperty.intValue, out var selectedTrait);
        
        if (validTrait && selectedTrait.Type != FoodTraitType.Tag) {
            height += EditorGUIUtility.singleLineHeight + 3;
        }
        
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var typeProperty = property.Find("_type");
        var traitProperty = property.Find("_trait");
        var traitHashProperty = traitProperty.Find("_hash");

        var database = FoodTraitDatabase.Singleton;
        var validTrait = database.TryGetEntry(traitHashProperty.intValue, out var selectedTrait);
        
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, typeProperty);
        
        rect.y += rect.height + 3;
        rect.height = EditorGUI.GetPropertyHeight(traitProperty);
        EditorGUI.PropertyField(rect, traitProperty);

        if (validTrait) {
            rect.y += rect.height + 3;
            DrawValueField(property, selectedTrait, rect);
        }

        EditorGUI.EndProperty();
    }

    static void DrawValueField(SerializedProperty property, FoodTraitDatabase.Entry trait, Rect rect) {
        switch (trait.Type) {
            case FoodTraitType.Int:
                EditorGUI.PropertyField(rect, property.Find("_intRange"), new GUIContent("Range")); break;
            case FoodTraitType.Float:
                EditorGUI.PropertyField(rect, property.Find("_floatRange"), new GUIContent("Range")); break;
            case FoodTraitType.Bool:
                EditorGUI.PropertyField(rect, property.Find("_boolValue"), new GUIContent("Value")); break;
            case FoodTraitType.Tag: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}