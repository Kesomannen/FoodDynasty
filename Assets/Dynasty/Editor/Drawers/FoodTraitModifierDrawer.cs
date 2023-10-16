using System;
using Dynasty.Food;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoodTraitModifier))]
public class FoodTraitModifierDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var traitProperty = property.Find("_trait");
        var traitHashProperty = traitProperty.Find("_hash");
        var operationProperty = property.Find("_operation");
        
        var height = EditorGUI.GetPropertyHeight(traitProperty);
        height += EditorGUIUtility.singleLineHeight + 3;
        
        var database = FoodTraitDatabase.Singleton;
        var validTrait = database.TryGetEntry(traitHashProperty.intValue, out var selectedTrait);

        if (operationProperty.enumValueIndex == (int) FoodTraitModifier.Operation.Set) {
            if (validTrait && selectedTrait.Type is not FoodTraitType.Tag) {
                height += EditorGUIUtility.singleLineHeight + 3;
            } 
        } else {
            var modifierProperty = property.Find("_modifier");
            height += EditorGUI.GetPropertyHeight(modifierProperty) + 3;
        }
        
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        var traitProperty = property.Find("_trait");
        var traitHashProperty = traitProperty.Find("_hash");
        var operationProperty = property.Find("_operation");

        var database = FoodTraitDatabase.Singleton;
        var validTrait = database.TryGetEntry(traitHashProperty.intValue, out var selectedTrait);
        
        var rect = position;
        rect.height = EditorGUI.GetPropertyHeight(traitProperty);
        EditorGUI.PropertyField(rect, traitProperty);

        var canModify = validTrait && selectedTrait.Type is FoodTraitType.Int or FoodTraitType.Float;
        if (!canModify) operationProperty.enumValueIndex = 0;

        EditorGUI.BeginDisabledGroup(!canModify);
        
        rect.y += rect.height + 3;
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, operationProperty);
        
        EditorGUI.EndDisabledGroup();

        if (validTrait) {
            rect.y += rect.height + 3;
            var operation = (FoodTraitModifier.Operation) operationProperty.enumValueIndex;
            switch (operation) {
                case FoodTraitModifier.Operation.Set:
                    DrawValueField(property, selectedTrait, rect);
                    break;
                case FoodTraitModifier.Operation.Modify:
                    EditorGUI.PropertyField(rect, property.Find("_modifier"), new GUIContent("Modifier"), true);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        EditorGUI.EndProperty();
    }

    static void DrawValueField(SerializedProperty property, FoodTraitDatabase.Entry trait, Rect rect) {
        switch (trait.Type) {
            case FoodTraitType.Int:
                EditorGUI.PropertyField(rect, property.Find("_intValue"), new GUIContent("Value")); break;
            case FoodTraitType.Float:
                EditorGUI.PropertyField(rect, property.Find("_floatValue"), new GUIContent("Value")); break;
            case FoodTraitType.Bool:
                EditorGUI.PropertyField(rect, property.Find("_boolValue"), new GUIContent("Value")); break;
            case FoodTraitType.Tag: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}